using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNST_CharaRecog
{
    public partial class Form1 : Form
    {
        /*================ グローバル変数 ================*/
        //15行目

        public static Bitmap image = null;// 読み込んだ画像を格納する変数
        // 保存を行ったかどうかを管理するフラグ変数
        // 画像表示後（保存不要：0
        // 解析後　　（保存前　：1
        // 保存後　　（保存後　：2
        int nowdo_flag = 0;
        int tenji_flag = -1;
        string normal_str;
        string tenji_str = "";
        [System.Runtime.InteropServices.DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr AddFontMemResourceEx(byte[] pbFont, int cbFont, IntPtr pdv, out uint pcFonts);
        System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();//PrivateFontCollectionオブジェクトを作成する

        public static Bitmap storeimg = null;

















        /*================================================*/
        //50行目


        //●コンストラクタ
        public Form1()
        {//55行目
            InitializeComponent();

            this.textBox_result.ReadOnly = true;//文字認識処理結果のテキストボックスは編集不可

            button_start.Enabled = false;//初期状態では、解析ボタンを不可にする

            button_output.Enabled = false;
            出力OToolStripMenuItem.Enabled = false;//初期状態では、保存ボタンを不可にする

            button_readout.Enabled = false;//初期状態では、読み上げボタンを不可にする

            radioButton_all.Enabled = false;
            radioButton_eng.Enabled = false;
            radioButton_jpn.Enabled = false;//初期状態では、各ラジオボタンを不可にする

            tenji_export.Enabled = false;//点字出力無効化














        }//86行目

        //●フォームを閉じる場合の処理
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result;
            // 保存をしたかどうかを判定する
            if (nowdo_flag == 1)
            {
                // →保存前ならば、ポップアップで保存確認
                result = MessageBox.Show("出力を保存しますか？", "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // →OKならば、保存フォームを開く
                    save();
                    // 終了メッセージボックスを出す
                    if (quit() == false) e.Cancel = true;//キャンセルならば、終了しない
                }
                else if (result == DialogResult.No)
                {
                    // NOならば、終了メッセージボックスを出す
                    if (quit() == false) e.Cancel = true;//キャンセルならば、終了しない
                }
                else
                {
                    // →キャンセルならば、終了しない
                    e.Cancel = true;
                }
            }
            else
            {
                // →保存後（あるいは保存不要）ならば、終了メッセージボックスを出す
                if (quit() == false) e.Cancel = true;//キャンセルならば、終了しない
                //点字フォントの後処理
                pfc.Dispose();
            }
        }//120行目

        private void Form1_Activated(object sender, EventArgs e)
        {
            pictureBox.Image = image;
        }//127行目














        

        /*================ ↓以下、各UIのイベント↓ ================*/

        //146行

        //●『カメラ起動』ボタン：クリックイベント
        private void button_webcam_Click(object sender, EventArgs e)
        {
            //ウェブカメラフォームを起動する
            webcam_open();








        }//161行目

        //●『参照』ボタン：クリックイベント
        private void button_brows_Click(object sender, EventArgs e)
        {
            //参照フォームを起動する
            brows_open();








        }//176行目     

        //●『画像表示』ボタン：クリックイベント
        private void button_show_Click(object sender, EventArgs e)
        {
            // 入力されたパスの画像をピクチャボックスに表示する
            if (!(image_show()))
            {
                return;//画像の読み込みに失敗した場合、このイベントを終了する
            }





        }//191行目

        //●『出力』ボタン：クリックイベント
        private void button_output_Click(object sender, EventArgs e)
        {
            //保存フォームを開く
            save();








        }//206行目

        //●『解析』ボタン：クリックイベント
        private void button_start_Click(object sender, EventArgs e)
        {
            //文字認識処理を開始する
            chara_recog_start(image);








        }//221行目

        //●『トリミング』ボタン：クリックイベント
        private void button_trimming_Click(object sender, EventArgs e)
        {
            trimming_start();
        }




        // ●読み上げを行うメソッド
        private void button_readout_Click(object sender, EventArgs e)
        {
            readout();
        }






        //237行目
        //●ドラッグアンドドロップの処理―――――――――――――――――
        private void textBox_pass_DragEnter(object sender, DragEventArgs e)
        {
            //ファイルがドラッグされている場合、カーソルを変更する。
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags)
                {
                    if (!System.IO.File.Exists(d))
                    {
                        // ファイル以外であればイベント・ハンドラを抜ける
                        return;
                    }
                }
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void textBox_pass_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたファイルの一覧を取得
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileName.Length <= 0)
            {
                return;
            }
            if (System.IO.File.Exists(fileName[0]) == true)
            {
                //ドロップ先がTextBoxであるかチェック
                TextBox txtTarget = sender as TextBox;
                if (txtTarget == null)
                {
                    return;
                }
                //TextBoxの内容をファイル名に変更
                txtTarget.Text = fileName[0];
                if (image_show() == false)
                {//画像以外のファイルをD&Dした場合、表記を元に戻す
                    textBox_pass.Text = "C:\\....";
                    return;
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)//D&Dを行うためのイベント（DragDropと同時使用）
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags)
                {
                    if (!System.IO.File.Exists(d))
                    {
                        // ファイル以外であればイベント・ハンドラを抜ける
                        return;
                    }
                }
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)//D&Dを行うためのイベント（DragEnterと同時使用）
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (System.IO.File.Exists(files[0]) == true)
            {

                //D&Dしてきたファイル名をテキストボックスに表示

                textBox_pass.Text = files[0];

                if (image_show() == false)
                {//画像以外のファイルをD&Dした場合、表記を元に戻す
                    textBox_pass.Text = "C:\\....";
                    return;
                }
            }
        }
        //――――――――――――――――――――――――――――――――
        //323行目


        /*================ ↓メニューバーのイベント↓ ================*/

        private void フォルダから参照BToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brows_open();
        }

        private void カメラ起動CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webcam_open();
        }

        private void 出力OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void 閉じるToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void トリミングTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trimming_start();
        }

        private void 解析AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chara_recog_start(image);
        }

        private void 読み上げRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readout();
        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            version_info();
        }


        /*================ 以下、自作のメソッド ================*/

        //371行

        //●入力されたパスの画像をピクチャボックスに表示するメソッド
        //・戻り値 true ：画像の読み込みに成功
        //         false：画像の読み込みに失敗
        private bool image_show()
        {
            if (pass_read())
            {
                //imageに読み込み完了
                pic_show();
                return true;
            }
            else
            {
                return false;
            }
        }

        //●パスから画像を読み込むメソッド
        private bool pass_read()
        {
            try
            {
                image = new Bitmap(textBox_pass.Text);// 画像をビットマップ型で読み込み
                return true;
            }
            //↓読み込めなかった場合の処理↓
            catch (System.ArgumentException)
            {
                // エラーのメッセージボックスを表示する
                MessageBox.Show("入力が無効です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //●pictureBoxに画像を表示するメソッド
        private void pic_show()
        {
            pictureBox.Image = image;// 画像をピクチャボックスに表示

            //各UIのEnabale状態を変更
            nowdo_flag = 0;
            enable_change();
        }

        //●ウェブカメラフォームを起動するメソッド
        private void webcam_open()
        {
            Form2 fo2 = new Form2();//インスタンス生成
            //Form2オープン、Form1は操作不能にする
            if (fo2.ShowDialog(this) == DialogResult.OK) { }
            else { /*オープンエラー処理が必要ならば書く*/ }

            fo2.Dispose();//リソースを開放

            if (image != null)//imageに画像が入力されていたら画像表示
            {
                pic_show();
            }
        
        }

        //●参照フォームを起動するメソッド
        private void brows_open()
        {
            //参照フォームを開く
            DialogResult dr = openFileDialog1.ShowDialog();

            //OKボタンを押下された場合
            if (dr == DialogResult.OK)
            {
                //ディレクトリパスを入力するフォームに参照したパスを入力する
                textBox_pass.Text = openFileDialog1.FileName;
            }
            else if (dr == DialogResult.Cancel)
            {
                //キャンセルが押されたらfalseを返して終了する
                return;
            }

            //画像をピクチャボックスに表示する
            if (!(image_show()))
            {
                //画像の読み込みに失敗したら終了する
                return;
            }
        }

        //●文字認識を行うメソッド
        //・引数    Bitmap img：文字認識処理対象の画像を指定する
        private void chara_recog_start(Bitmap img)
        {
            if(tenji_export.Checked == true)
            {
                textBox_result.Font = new Font("MS 明朝", 8);
            }
            //処理中であることをテキストボックスに表示する
            textBox_result.Text = "解析中...";
            this.Refresh();

            try
            {
                //『English』のチェックボックスがチェックされているかの判定
                if (radioButton_eng.Checked)
                {
                    // Englishにチェックが入っている
                    // →engの言語データで文字認識処理を行う
                    textBox_result.Text = chara_recog(img, "eng");
                }
                else
                {
                    // Englishにチェックが入っていない
                    // →jpnの言語データで文字認識処理を行う
                    textBox_result.Text = chara_recog(img, "jpn");
                }
                //処理が完了したことを知らせるメッセージボックスを表示
                MessageBox.Show("解析が終了しました！", "解析終了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                normal_str = textBox_result.Text; //解析時の文字列を保持

                //処理フラグを1（解析後）にする
                nowdo_flag = 1;
                enable_change();

                return;
            }
            catch (Exception e)
            {
                // 読み込めない画像が入力された場合、エラーを出力
                MessageBox.Show("例外が発生しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox_result.Text = "";// リザルトテキストを空にする
                return;// メソッド終了
            }
        }

        //●文字認識処理
        //・引数  Bitmap img ：文字認識処理対象の画像を指定する
        //        string lang：文字認識処理を行う言語を指定する
        //・戻り値：文字認識処理結果
        private string chara_recog(Bitmap img, string lang)
        {
            //文字認識結果を格納する変数
            string str = "";

            // OCRを行うオブジェクトの生成
            //  言語データの場所と言語名を引数で指定する
            var tesseract = new Tesseract.TesseractEngine(
                @"..\..\..\tessdata", // 言語ファイルを「C:\tessdata」に置いた場合
                lang);         // 英語なら"eng" 「○○.traineddata」の○○の部分
   
            // OCRの実行と表示
            var page = tesseract.Process(img);
            str = page.GetText();

            //文字認識結果を返す
            return str;
        }
        
        //●ファイル出力（保存）を行うメソッド
        private void save()
        {
            //ここから出力ダイアログボックスの設定//
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "新しいファイル.txt";
            sfd.InitialDirectory = @"C:\";
            sfd.Filter = "テキストファイル(*.txt;)|*.txt;*|すべてのファイル(*.*)|*.*";
            sfd.FilterIndex = 2;
            sfd.Title = "保存先のファイルを選択してください";
            sfd.RestoreDirectory = true;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            //ここまで出力ダイアログボックスの設定//

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@sfd.FileName, false, sjisEnc);
                writer.WriteLine(normal_str);//ここにtesseractから送られてきた文字をぶち込む//
                writer.Close();

                // 処理フラグを2（保存後）にする
                nowdo_flag = 2;
            }
        }

        // ●終了確認を行うメソッド
        private bool quit()
        {
            // ポップアップで終了確認
            DialogResult result = MessageBox.Show("終了しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                return false;
            }
            return true;
        }

        // ●各UIのEnable操作を行うメソッド
        private void enable_change()
        {
            // 0：読上、保存ボタンのみEnable=false
            // 1：全てEnable=true
            // 2：状態２のまま
            switch (nowdo_flag)
            {
                case 0://画像表示後
                    radioButton_all.Enabled = true;
                    radioButton_eng.Enabled = true;
                    radioButton_jpn.Enabled = true;//各ラジオボタンを可にする
                    radioButton_all.Checked = true;//『全て』ラジオボタンのみチェックを付ける

                    button_start.Enabled = true;//『解析』ボタンを可にする

                    button_readout.Enabled = false;//『読み上げ』ボタンを不可にする
                    button_output.Enabled = false;//『保存』ボタンを不可にする

                    tenji_export.Enabled = false;//『点字表記』チェックボックスを不可にする

                    textBox_result.Text = "";//リザルトのテキストをリセットする
                    break;
                case 1://解析後
                    button_readout.Enabled = true;//『読み上げ』ボタンを不可にする
                    button_output.Enabled = true;//『保存』ボタンを不可にする

                    tenji_flag = -1;//点字フラグを戻す
                    tenji_str = "";
                    if(tenji_export.Checked == true)
                    {
                        tenji_export.Checked = false;
                        tenji_CheckedChanged();
                    }
                    tenji_export.Enabled = true;//『点字表記』チェックボックスを可にする
                    break;
                case 2:
                    break;
            }
        }

        // ●バージョン情報を表示するメソッド
        private void version_info()
        {
            MessageBox.Show("文字認識システム\nVersion1.0\nSKYNST (System Knowledge Young geNeration Student Team)", "バージョン情報", MessageBoxButtons.OK);
        }

        /*================ ↓点字処理のイベント↓ ================*/
        private void tenji_export_CheckedChanged(object sender, EventArgs e)
        {
            tenji_CheckedChanged();
        }


        // ●トリミング処理ウィンドウを開くメソッド
        private void trimming_start()
        {
            Form3 f = new Form3();  // トリミングウィンドウを開く
            f.ShowDialog();
        }

        private void readout()
        {
            if (System.Diagnostics.Process.GetProcessesByName("BouyomiChan").Length <= 0)//棒読みちゃん起動していなければ起動
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.FileName = @"..\..\..\packages\BouyomiChan_0_1_11_0_Beta16\BouyomiChan.exe";
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                System.Threading.Thread.Sleep(5000);
            }
            System.Diagnostics.ProcessStartInfo qsi = new System.Diagnostics.ProcessStartInfo();
            qsi.FileName = @"..\..\..\packages\BouyomiChan_0_1_11_0_Beta16\RemoteTalk\RemoteTalk.exe";
            qsi.Arguments = String.Format("/T {0}", normal_str);
            qsi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process q = System.Diagnostics.Process.Start(qsi);
        }

        private void tenji_CheckedChanged()
        {
            if (tenji_flag == -1)
            {
                //リソース(BWLbrail)をバイト配列に読み込む
                byte[] fontBuf = Properties.Resources.BWLbrail;
                //ポインターで取得
                IntPtr fontBufPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontBuf.Length);
                System.Runtime.InteropServices.Marshal.Copy(fontBuf, 0, fontBufPtr, fontBuf.Length);

                //AddFontMemResourceExを呼び出す
                uint cFonts;
                AddFontMemResourceEx(fontBuf, fontBuf.Length, IntPtr.Zero, out cFonts);
                //PrivateFontCollectionにフォントを追加する
                pfc.AddMemoryFont(fontBufPtr, fontBuf.Length);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontBufPtr);
                System.Drawing.Font f = new System.Drawing.Font(pfc.Families[0], 12);
                //textBox_resultコントロールのフォントに設定する
                textBox_result.Font = f;

                int i = 0;
                int check = 0;
                int lengthcheck = normal_str.Length;
                char[] temp_c = normal_str.ToCharArray();
                for (i = 0; i < lengthcheck; i++)
                {
                    //きゃきゅきょ
                    if (temp_c[i] == 'き' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'キ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'か';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'き' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'キ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'く';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'き' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'キ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'こ';
                        lengthcheck++;
                    }
                    //しゃしゅしょ
                    else if (temp_c[i] == 'し' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'シ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'さ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'し' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'シ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'す';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'し' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'シ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'そ';
                        lengthcheck++;
                    }
                    //ちゃちゅちょ
                    else if (temp_c[i] == 'ち' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'チ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'た';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ち' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'チ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ち' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'チ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'と';
                        lengthcheck++;
                    }
                    //にゃにゅにょ
                    else if (temp_c[i] == 'に' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ニ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'な';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'に' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ニ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ぬ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'に' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ニ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'の';
                        lengthcheck++;
                    }
                    //ひゃひゅひょ
                    else if (temp_c[i] == 'ひ' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ヒ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'は';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ひ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ヒ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ひ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ヒ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    //みゃみゅみょ
                    else if (temp_c[i] == 'み' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ミ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ま';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'み' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ミ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'む';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'み' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ミ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'も';
                        lengthcheck++;
                    }
                    //りゃりゅりょ
                    else if (temp_c[i] == 'り' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'リ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ら';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'り' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'リ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'る';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'り' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'リ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ろ';
                        lengthcheck++;
                    }
                    //ぎゃぎゅぎょ
                    else if (temp_c[i] == 'ぎ' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ギ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'か';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぎ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ギ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'く';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぎ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ギ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'こ';
                        lengthcheck++;
                    }
                    //じゃじゅじょ
                    else if (temp_c[i] == 'じ' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ジ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'さ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'じ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ジ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'す';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'じ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ジ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'そ';
                        lengthcheck++;
                    }
                    //ぢゃぢゅぢょ
                    else if (temp_c[i] == 'ぢ' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ヂ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'た';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぢ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ヂ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぢ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ヂ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'と';
                        lengthcheck++;
                    }
                    //びゃびゅびょ
                    else if (temp_c[i] == 'び' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ビ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'は';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'び' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ビ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'び' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ビ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    //ぴゃぴゅぴょ
                    else if (temp_c[i] == 'ぴ' && temp_c[i + 1] == 'ゃ' || temp_c[i] == 'ピ' && temp_c[i + 1] == 'ャ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '斜';
                        temp_c[i + 1] = 'は';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぴ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ピ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '斜';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぴ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ピ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '斜';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    //特殊音群
                    //イェ、キェ、シェ、チェ、ニェ、ヒェ,じぇ
                    else if (temp_c[i] == 'い' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'イ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'え';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'き' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'キ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'け';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'し' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'シ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'せ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ち' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'チ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'て';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'に' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ニ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'ね';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ひ' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ヒ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '｀';
                        temp_c[i + 1] = 'へ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'じ' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ジ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'せ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'び' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ビ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'び' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ビ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '小';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    //うぃうぇうぉ
                    else if (temp_c[i] == 'う' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'ウ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'い';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'う' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ウ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'え';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'う' && temp_c[i + 1] == 'ぉ' || temp_c[i] == 'う' && temp_c[i + 1] == 'ォ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'お';
                        lengthcheck++;
                    }
                    //くぁ、くぃ、くぇ、くぉ
                    else if (temp_c[i] == 'く' && temp_c[i + 1] == 'ぁ' || temp_c[i] == 'ク' && temp_c[i + 1] == 'ァ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'か';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'く' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'ク' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'き';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'く' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ク' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'け';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'く' && temp_c[i + 1] == 'ぉ' || temp_c[i] == 'ク' && temp_c[i + 1] == 'ォ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'こ';
                        lengthcheck++;
                    }
                    //ふぁ、ふぃ、ふぇ、ふぉ
                    else if (temp_c[i] == 'ふ' && temp_c[i + 1] == 'ぁ' || temp_c[i] == 'フ' && temp_c[i + 1] == 'ァ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'は';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ふ' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'フ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'ひ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ふ' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'フ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'へ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ふ' && temp_c[i + 1] == 'ぉ' || temp_c[i] == 'フ' && temp_c[i + 1] == 'ォ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '？';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    //グァ グィ グェ グォ
                    else if (temp_c[i] == 'ぐ' && temp_c[i + 1] == 'ぁ' || temp_c[i] == 'グ' && temp_c[i + 1] == 'ァ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'か';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぐ' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'ぐ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'き';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぐ' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ク' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'け';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぐ' && temp_c[i + 1] == 'ぉ' || temp_c[i] == 'ク' && temp_c[i + 1] == 'ォ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'こ';
                        lengthcheck++;
                    }
                    //ヴぁヴぃヴヴぇヴぉ
                    else if (temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ぁ' || temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ァ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'あ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'い';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ぇ' || temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ェ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'え';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ぉ' || temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ォ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'お';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ヴ' || temp_c[i] == 'ヴ')
                    {
                        temp_c[i] = 'ぶ';
                    }
                    //すぃ
                    else if (temp_c[i] == 'す' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'ス' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '‘';
                        temp_c[i + 1] = 'し';
                        lengthcheck++;
                    }
                    //てぃ
                    else if (temp_c[i] == 'て' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'テ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '‘';
                        temp_c[i + 1] = 'ち';
                        lengthcheck++;
                    }
                    //とぅ
                    else if (temp_c[i] == 'と' && temp_c[i + 1] == 'ぅ' || temp_c[i] == 'ト' && temp_c[i + 1] == 'ゥ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '‘';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    //ズィ ディ ドゥ
                    else if (temp_c[i] == 'ず' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'ズ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'し';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'で' && temp_c[i + 1] == 'ぃ' || temp_c[i] == 'デ' && temp_c[i + 1] == 'ィ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'ち';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ど' && temp_c[i + 1] == 'ぅ' || temp_c[i] == 'ド' && temp_c[i + 1] == 'ゥ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '。';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    //テュ フュ フョ
                    else if (temp_c[i] == 'て' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'テ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '斜';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ふ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'フ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '斜';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ふ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'フ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '斜';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    //デュ ヴュ ヴョ
                    else if (temp_c[i] == 'で' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'デ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '拡';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ゅ' || temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ュ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'う';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ょ' || temp_c[i] == 'ヴ' && temp_c[i + 1] == 'ョ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'お';
                        lengthcheck++;
                    }
                    //濁音開始
                    if (temp_c[i] == 'が' || temp_c[i] == 'ガ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'か';
                        lengthcheck++;
                    }
                    else if ((temp_c[i] == 'ぎ' || temp_c[i] == 'ギ'))
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'き';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぐ' || temp_c[i] == 'グ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'く';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'げ' || temp_c[i] == 'ゲ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'け';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ご' || temp_c[i] == 'ゴ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'こ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ざ' || temp_c[i] == 'ザ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'さ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'じ' || temp_c[i] == 'ジ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'し';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ず' || temp_c[i] == 'ズ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'す';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぜ' || temp_c[i] == 'ゼ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'せ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぞ' || temp_c[i] == 'ゾ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'そ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'だ' || temp_c[i] == 'ダ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'た';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぢ' || temp_c[i] == 'ヂ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'ち';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'づ' || temp_c[i] == 'ヅ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'で' || temp_c[i] == 'デ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'て';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ド' || temp_c[i] == 'ど')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'と';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ば' || temp_c[i] == 'バ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'つ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'び' || temp_c[i] == 'ビ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'ひ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぶ' || temp_c[i] == 'ブ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'べ' || temp_c[i] == 'ベ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'へ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぼ' || temp_c[i] == 'ボ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '・';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    //半濁音
                    else if (temp_c[i] == 'ぱ' || temp_c[i] == 'パ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '半';
                        temp_c[i + 1] = 'は';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぴ' || temp_c[i] == 'ピ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '半';
                        temp_c[i + 1] = 'ひ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぷ' || temp_c[i] == 'プ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '半';
                        temp_c[i + 1] = 'ふ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぺ' || temp_c[i] == 'ペ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '半';
                        temp_c[i + 1] = 'へ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぽ' || temp_c[i] == 'ポ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '半';
                        temp_c[i + 1] = 'ほ';
                        lengthcheck++;
                    }
                    else if (temp_c[i] == 'ぱ' || temp_c[i] == 'パ')
                    {
                        Array.Resize(ref temp_c, lengthcheck + 1);
                        temp_c[i] = '半';
                        temp_c[i + 1] = 'は';
                        lengthcheck++;
                    }
                    check = i;
                }
                for (i = 0; i < check; i++)
                {
                    tenji_str += string.Join("", temp_c[i]);
                }
                textBox_result.Text = tenji_str;
                tenji_flag = 1;

                button_output.Enabled = false;
            }
            else if (tenji_flag == 0)
            {
                textBox_result.Font = new System.Drawing.Font(pfc.Families[0], 12);
                textBox_result.Text = tenji_str;
                tenji_flag = 1;

                button_output.Enabled = false;
            }
            else if (tenji_flag == 1)
            {
                textBox_result.Font = new Font("MS 明朝", 8);
                textBox_result.Text = normal_str;
                tenji_flag = 0;

                button_output.Enabled = true;
            }            
        }



































































        //1500行目
    }
}