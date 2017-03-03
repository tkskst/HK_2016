using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AForge.Video.DirectShow;

namespace SKYNST_CharaRecog
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        //Form2が呼び出されたとき必ず行われる処理
        private void Form2_Load(object sender, EventArgs e)
        {
            //シャッターボタンは無効に
            button_chapture.Enabled = false;
        }

        //カメラ起動ボタンを押下
        private void Start_Camera_Click(object sender, EventArgs e)
        {
            Camera_Format();
            var form = new VideoCaptureDeviceForm();
            // 選択ダイアログを開く
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                // 選択されたデバイスをVideoSourcePlayerのソースに設定
                videoSourcePlayer1.VideoSource = form.VideoDevice;
                //Webカメラ起動でシャッターボタンを有効に
                button_chapture.Enabled = true;
                // ビデオキャプチャのスタート
                videoSourcePlayer1.Start();
            }
        }

        //シャッターボタンを押下
        private void button_chapture_Click(object sender, EventArgs e)
        {
            //シャッターボタンを押すと画像をキャプチャ
            var bmp = videoSourcePlayer1.GetCurrentVideoFrame();
            //グローバル変数に代入
            Form1.image = bmp;
            //ウィンドウをクローズ
            this.Close();
        }

        //シャッターボタンを押下
        private void button_cansel_Click(object sender, EventArgs e)
        {
            //ウィンドウクローズ
            this.Close();
        }

        //ウィンドウを閉じたときに呼び出される処理
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Camera_Format();
        }

        private void Camera_Format() 
        {
            //Webカメラ初期化
            if (videoSourcePlayer1.VideoSource != null && videoSourcePlayer1.VideoSource.IsRunning)
            {
                videoSourcePlayer1.VideoSource.SignalToStop();
                videoSourcePlayer1.VideoSource = null;
            }
        }

        private void videoSourcePlayer1_Click(object sender, EventArgs e) { }
    }
}
