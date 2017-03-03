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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Form1.storeimg;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            Form1.image = Form1.storeimg;
            Form1.storeimg = null;
            this.Close();
        }

        private void button_retry_Click(object sender, EventArgs e)
        {
            // やり直しボタンの押下時
            // Form3の呼び出し
            Form3 form3 = new Form3();
            form3.Show();
            // このウィンドウを閉じる
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            // キャンセルボタンの押下時
            this.Close();
        }

        private void Form4_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }
    }
}
