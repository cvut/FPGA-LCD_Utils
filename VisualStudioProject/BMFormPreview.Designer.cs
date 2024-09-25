namespace LSPtools
{
    partial class BMFormPreview
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BMFormPreview));
      this.txbMouseY = new System.Windows.Forms.TextBox();
      this.txbMouseX = new System.Windows.Forms.TextBox();
      this.numericUpDownScale = new System.Windows.Forms.NumericUpDown();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.panelBitmap = new System.Windows.Forms.Panel();
      this.btnClose = new System.Windows.Forms.Button();
      this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
      this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
      this.txbPixelValue = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScale)).BeginInit();
      this.SuspendLayout();
      // 
      // txbMouseY
      // 
      this.txbMouseY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txbMouseY.Location = new System.Drawing.Point(195, 461);
      this.txbMouseY.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.txbMouseY.Name = "txbMouseY";
      this.txbMouseY.ReadOnly = true;
      this.txbMouseY.Size = new System.Drawing.Size(44, 20);
      this.txbMouseY.TabIndex = 9;
      // 
      // txbMouseX
      // 
      this.txbMouseX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.txbMouseX.Location = new System.Drawing.Point(122, 461);
      this.txbMouseX.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.txbMouseX.Name = "txbMouseX";
      this.txbMouseX.ReadOnly = true;
      this.txbMouseX.Size = new System.Drawing.Size(44, 20);
      this.txbMouseX.TabIndex = 10;
      // 
      // numericUpDownScale
      // 
      this.numericUpDownScale.Location = new System.Drawing.Point(128, 9);
      this.numericUpDownScale.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.numericUpDownScale.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
      this.numericUpDownScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownScale.Name = "numericUpDownScale";
      this.numericUpDownScale.Size = new System.Drawing.Size(41, 20);
      this.numericUpDownScale.TabIndex = 8;
      this.numericUpDownScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownScale.ValueChanged += new System.EventHandler(this.numericUpDownScale_ValueChanged);
      // 
      // label10
      // 
      this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(171, 463);
      this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(20, 13);
      this.label10.TabIndex = 5;
      this.label10.Text = "Y=";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(11, 11);
      this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(114, 13);
      this.label11.TabIndex = 6;
      this.label11.Text = "Scale of result preview";
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(2, 463);
      this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(117, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Mouse point to pixel X=";
      // 
      // panelBitmap
      // 
      this.panelBitmap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panelBitmap.Cursor = System.Windows.Forms.Cursors.Cross;
      this.panelBitmap.Location = new System.Drawing.Point(2, 36);
      this.panelBitmap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.panelBitmap.Name = "panelBitmap";
      this.panelBitmap.Size = new System.Drawing.Size(560, 398);
      this.panelBitmap.TabIndex = 4;
      this.panelBitmap.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBitmap_Paint);
      this.panelBitmap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelBitmap_MouseMove);
      this.panelBitmap.Resize += new System.EventHandler(this.panelBitmap_Resize);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.btnClose.Location = new System.Drawing.Point(521, 460);
      this.btnClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(56, 19);
      this.btnClose.TabIndex = 11;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // vScrollBar1
      // 
      this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vScrollBar1.Location = new System.Drawing.Point(564, 36);
      this.vScrollBar1.Name = "vScrollBar1";
      this.vScrollBar1.Size = new System.Drawing.Size(25, 398);
      this.vScrollBar1.TabIndex = 12;
      this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
      // 
      // hScrollBar1
      // 
      this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hScrollBar1.Location = new System.Drawing.Point(2, 438);
      this.hScrollBar1.Name = "hScrollBar1";
      this.hScrollBar1.Size = new System.Drawing.Size(560, 21);
      this.hScrollBar1.TabIndex = 13;
      this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
      // 
      // txbPixelValue
      // 
      this.txbPixelValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txbPixelValue.Location = new System.Drawing.Point(340, 461);
      this.txbPixelValue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.txbPixelValue.Name = "txbPixelValue";
      this.txbPixelValue.ReadOnly = true;
      this.txbPixelValue.Size = new System.Drawing.Size(168, 20);
      this.txbPixelValue.TabIndex = 15;
      // 
      // label12
      // 
      this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(244, 463);
      this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(90, 13);
      this.label12.TabIndex = 14;
      this.label12.Text = "Value in memory=";
      // 
      // FormPreview
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(586, 485);
      this.Controls.Add(this.txbPixelValue);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.hScrollBar1);
      this.Controls.Add(this.vScrollBar1);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.txbMouseY);
      this.Controls.Add(this.txbMouseX);
      this.Controls.Add(this.numericUpDownScale);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.panelBitmap);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.Name = "FormPreview";
      this.Text = "FormPreview";
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScale)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txbMouseY;
        private System.Windows.Forms.TextBox txbMouseX;
        private System.Windows.Forms.NumericUpDown numericUpDownScale;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelBitmap;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.TextBox txbPixelValue;
        private System.Windows.Forms.Label label12;
    }
}