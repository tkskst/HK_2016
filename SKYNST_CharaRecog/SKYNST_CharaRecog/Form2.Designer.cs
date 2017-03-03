namespace SKYNST_CharaRecog
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_cansel = new System.Windows.Forms.Button();
            this.button_chapture = new System.Windows.Forms.Button();
            this.Start_Camera = new System.Windows.Forms.Button();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.SuspendLayout();
            // 
            // button_cansel
            // 
            this.button_cansel.Location = new System.Drawing.Point(396, 304);
            this.button_cansel.Name = "button_cansel";
            this.button_cansel.Size = new System.Drawing.Size(75, 23);
            this.button_cansel.TabIndex = 3;
            this.button_cansel.Text = "キャンセル";
            this.button_cansel.UseVisualStyleBackColor = true;
            this.button_cansel.Click += new System.EventHandler(this.button_cansel_Click);
            // 
            // button_chapture
            // 
            this.button_chapture.Location = new System.Drawing.Point(305, 304);
            this.button_chapture.Name = "button_chapture";
            this.button_chapture.Size = new System.Drawing.Size(75, 23);
            this.button_chapture.TabIndex = 4;
            this.button_chapture.Text = "シャッター";
            this.button_chapture.UseVisualStyleBackColor = true;
            this.button_chapture.Click += new System.EventHandler(this.button_chapture_Click);
            // 
            // Start_Camera
            // 
            this.Start_Camera.Location = new System.Drawing.Point(208, 304);
            this.Start_Camera.Name = "Start_Camera";
            this.Start_Camera.Size = new System.Drawing.Size(75, 23);
            this.Start_Camera.TabIndex = 4;
            this.Start_Camera.Text = "カメラ起動";
            this.Start_Camera.UseVisualStyleBackColor = true;
            this.Start_Camera.Click += new System.EventHandler(this.Start_Camera_Click);
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Location = new System.Drawing.Point(12, 12);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(460, 277);
            this.videoSourcePlayer1.TabIndex = 5;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 342);
            this.Controls.Add(this.videoSourcePlayer1);
            this.Controls.Add(this.button_cansel);
            this.Controls.Add(this.Start_Camera);
            this.Controls.Add(this.button_chapture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "カメラフォーム";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_cansel;
        private System.Windows.Forms.Button button_chapture;
        private System.Windows.Forms.Button Start_Camera;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
    }
}