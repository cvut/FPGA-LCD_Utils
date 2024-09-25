namespace LSPtools
{
  partial class BMInfoForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BMInfoForm));
      this.palettePanel = new System.Windows.Forms.Panel();
      this.button1 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.colorCountTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.infoTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.messageLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.bitmapPanel = new System.Windows.Forms.Panel();
      this.numericUpDownScale = new System.Windows.Forms.NumericUpDown();
      this.label11 = new System.Windows.Forms.Label();
      this.statusStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScale)).BeginInit();
      this.SuspendLayout();
      // 
      // palettePanel
      // 
      this.palettePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.palettePanel.Location = new System.Drawing.Point(0, 448);
      this.palettePanel.Name = "palettePanel";
      this.palettePanel.Size = new System.Drawing.Size(781, 120);
      this.palettePanel.TabIndex = 1;
      this.palettePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.palettePanel_Paint);
      this.palettePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.palettePanel_MouseMove);
      this.palettePanel.Resize += new System.EventHandler(this.palettePanel_Resize);
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.button1.Image = global::LSPtools.Properties.Resources.ClosePreviewHS;
      this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.button1.Location = new System.Drawing.Point(642, 419);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(133, 27);
      this.button1.TabIndex = 2;
      this.button1.Text = "Reject Bitmap";
      this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(179, 426);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(135, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "different colors were found.";
      // 
      // colorCountTextBox
      // 
      this.colorCountTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.colorCountTextBox.Location = new System.Drawing.Point(54, 422);
      this.colorCountTextBox.Name = "colorCountTextBox";
      this.colorCountTextBox.ReadOnly = true;
      this.colorCountTextBox.Size = new System.Drawing.Size(119, 20);
      this.colorCountTextBox.TabIndex = 4;
      this.colorCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(1, 427);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(47, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Palette";
      // 
      // infoTextBox
      // 
      this.infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.infoTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.infoTextBox.Location = new System.Drawing.Point(508, 25);
      this.infoTextBox.Multiline = true;
      this.infoTextBox.Name = "infoTextBox";
      this.infoTextBox.ReadOnly = true;
      this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.infoTextBox.Size = new System.Drawing.Size(272, 394);
      this.infoTextBox.TabIndex = 6;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(1, 7);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(52, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Preview";
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageLabel});
      this.statusStrip1.Location = new System.Drawing.Point(0, 568);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(781, 22);
      this.statusStrip1.TabIndex = 7;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // messageLabel
      // 
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(39, 17);
      this.messageLabel.Text = "Color:";
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // bitmapPanel
      // 
      this.bitmapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.bitmapPanel.Location = new System.Drawing.Point(0, 25);
      this.bitmapPanel.Name = "bitmapPanel";
      this.bitmapPanel.Size = new System.Drawing.Size(502, 394);
      this.bitmapPanel.TabIndex = 8;
      this.bitmapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
      this.bitmapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bitmapPanel_MouseMove);
      // 
      // numericUpDownScale
      // 
      this.numericUpDownScale.Location = new System.Drawing.Point(137, 3);
      this.numericUpDownScale.Margin = new System.Windows.Forms.Padding(2);
      this.numericUpDownScale.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
      this.numericUpDownScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownScale.Name = "numericUpDownScale";
      this.numericUpDownScale.Size = new System.Drawing.Size(47, 20);
      this.numericUpDownScale.TabIndex = 10;
      this.numericUpDownScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(74, 7);
      this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(50, 13);
      this.label11.TabIndex = 9;
      this.label11.Text = "Magnifier";
      // 
      // BMInfoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(781, 590);
      this.Controls.Add(this.numericUpDownScale);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.bitmapPanel);
      this.Controls.Add(this.infoTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.colorCountTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.palettePanel);
      this.Controls.Add(this.statusStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "BMInfoForm";
      this.Text = "Rejected Bitmap ";
      this.Load += new System.EventHandler(this.BMInfoForm_Load);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScale)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private Panel palettePanel;
    private Button button1;
    private Label label1;
    private TextBox colorCountTextBox;
    private Label label2;
    private TextBox infoTextBox;
    private Label label3;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel messageLabel;
    private Panel bitmapPanel;
    private NumericUpDown numericUpDownScale;
    private Label label11;
  }
}