namespace TextAnalysis
{
    partial class FormSplash
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
      this.components = new System.ComponentModel.Container();
      this.label1 = new System.Windows.Forms.Label();
      this.timerAnim = new System.Windows.Forms.Timer(this.components);
      this.labelBeta = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.labelVersion = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.label1.ForeColor = System.Drawing.Color.DarkRed;
      this.label1.Location = new System.Drawing.Point(29, 9);
      this.label1.MinimumSize = new System.Drawing.Size(270, 40);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(270, 40);
      this.label1.TabIndex = 0;
      this.label1.Text = "FPGA-LCD Utils";
      this.label1.UseWaitCursor = true;
      // 
      // timerAnim
      // 
      this.timerAnim.Interval = 50;
      this.timerAnim.Tick += new System.EventHandler(this.timerAnim_Tick);
      // 
      // labelBeta
      // 
      this.labelBeta.AutoSize = true;
      this.labelBeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.labelBeta.ForeColor = System.Drawing.Color.Red;
      this.labelBeta.Location = new System.Drawing.Point(31, 316);
      this.labelBeta.Name = "labelBeta";
      this.labelBeta.Size = new System.Drawing.Size(165, 25);
      this.labelBeta.TabIndex = 0;
      this.labelBeta.Text = "Beta Release";
      this.labelBeta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.labelBeta.UseWaitCursor = true;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::FpgaLcdUtils.Properties.Resources.LSPToolsAll;
      this.pictureBox1.Location = new System.Drawing.Point(31, 49);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(256, 256);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.UseWaitCursor = true;
      this.pictureBox1.Visible = false;
      // 
      // labelVersion
      // 
      this.labelVersion.AutoSize = true;
      this.labelVersion.BackColor = System.Drawing.Color.White;
      this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.labelVersion.ForeColor = System.Drawing.SystemColors.WindowText;
      this.labelVersion.Location = new System.Drawing.Point(200, 316);
      this.labelVersion.Name = "labelVersion";
      this.labelVersion.Size = new System.Drawing.Size(85, 25);
      this.labelVersion.TabIndex = 0;
      this.labelVersion.Text = "1.0.0.1\r\n";
      this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.labelVersion.UseWaitCursor = true;
      // 
      // FormSplash
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Info;
      this.ClientSize = new System.Drawing.Size(326, 352);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.labelBeta);
      this.Controls.Add(this.labelVersion);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "FormSplash";
      this.Opacity = 0.75D;
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "FormSplash";
      this.TopMost = true;
      this.TransparencyKey = System.Drawing.SystemColors.MenuHighlight;
      this.UseWaitCursor = true;
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerAnim;
    private Label labelBeta;
    private PictureBox pictureBox1;
    private Label labelVersion;
  }
}