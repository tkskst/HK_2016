using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNST_CharaRecog
{
    public partial class Form3 : Form
    {
        Point SPoint = new Point();  // ドラッグ開始点の座標
        Boolean flag = false;                 // マウスの押下判定
        public Bitmap bmp = Form1.image;  // 読み込んだ画像をコピー

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;  // ドラッグ＆ドロップを有効化
            bmp = Form1.image;
            pictureBox1.Image = bmp;  // 画像読み込み＆表示
        }

        //マウスが押された時の動作
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            flag = true;
            // ドラッグ開始位置の座標記録
            SPoint.X = e.X;
            SPoint.Y = e.Y;
        }

        //マウスが離された時の動作
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            flag = false;  // フラグを初期化
            Point EPoint = new Point();      // 離した点の座標
            EPoint.X = e.X; EPoint.Y = e.Y;  // 離した点の座標を格納
            Point resP = new Point();        // 修正後の離した点の座標
            Point LT = new Point();          // 長方形の左上の座標
            Point RB = new Point();          // 長方形の右下の座標
            int difX = -1, difY = -1;        // 2つの点のXY座標の差

            // 範囲外判定
            areajudge(EPoint, ref resP);

            // 左上・右下の点を判定し、誤差を求める
            pointjudge(SPoint, resP, ref LT, ref RB, ref difX, ref difY);
            // 例外処理
            if (difX == 0 || difY == 0)
            {
                return;
            }
            // 切り取り範囲
            Rectangle TriAreaRect = new Rectangle((int)LT.X, (int)LT.Y, (int)difX, (int)difY);
            // 画像を格納
            System.Drawing.Imaging.PixelFormat format = bmp.PixelFormat;
            Bitmap bmp2 = bmp.Clone(TriAreaRect, format);
            Form1.storeimg = bmp2;
            // 確認ウィンドウを出す
            Form4 form4 = new Form4();
            form4.Show();

            this.Close();
        }

        // カーソルを動かした時の動作
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // 描画した図形を消す
            ((Control)sender).Refresh();
            // グラフィックイベントの追加
            Graphics g = pictureBox1.CreateGraphics();
            // クリックされていない時はスルー
            if (flag == false)
            {
                return;
            }
            Point nowP = new Point();    // カーソルの位置
            nowP.X = e.X; nowP.Y = e.Y;  // カーソルの位置を格納
            Point resP = new Point();
            Point LT = new Point();      // 長方形の左上の座標
            Point RB = new Point();      // 長方形の右下の座標
            int difX = -1, difY = -1;    // 2つの点のXY座標の差

            // 範囲外判定
            areajudge(nowP, ref resP);
            // 左上・右下の点を判定し、誤差を求める
            pointjudge(SPoint, resP, ref LT, ref RB, ref difX, ref difY);
            // 描画する長方形の色を作成
            Pen pen1 = new Pen(Color.FromArgb(255, Color.Blue), 2);
            SolidBrush brush1 = new SolidBrush(Color.FromArgb(30, Color.Blue));
            // 長方形を描画
            g.DrawRectangle(pen1, new Rectangle(LT.X, LT.Y, difX, difY));
            g.FillRectangle(brush1, LT.X, LT.Y, difX, difY);

            g.Dispose();
            pen1.Dispose();
            brush1.Dispose();
        }

        // 左上・右下の点を判定し、誤差を求めるメソッド
        private void pointjudge(Point sp, Point ep, ref Point lt, ref Point rb, ref int difX, ref int difY)
        {
            lt.X = Math.Min(sp.X, ep.X);
            lt.Y = Math.Min(sp.Y, ep.Y);
            rb.X = Math.Max(sp.X, ep.X);
            rb.Y = Math.Max(sp.Y, ep.Y);
            difX = rb.X - lt.X;
            difY = rb.Y - lt.Y;
        }

        // 範囲外を判定し、座標を修正するメソッド
        private void areajudge(Point input, ref Point output)
        {
            if (input.X > bmp.Width)
            {
                // x座標が最大を超えた場合
                output.X = bmp.Width;
            }
            else if (input.X < 0)
            {
                // x座標が最小を下回った場合
                output.X = 0;
            }
            else
            {
                output.X = input.X;
            }
            if (input.Y > bmp.Height)
            {
                // y座標が最大を超えた場合
                output.Y = bmp.Height;
            }
            else if (input.Y < 0)
            {
                // y座標が最小を下回った場合
                output.Y = 0;
            }
            else
            {
                output.Y = input.Y;
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            // キャンセルボタンを押した時にウィンドウを閉じる
            this.Close();
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }
    }
}
