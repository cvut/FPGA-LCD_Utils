namespace FpgaLcdUtils
{
  partial class QCToDoForm_notused
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QCToDoForm_notused));
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.closeButton = new System.Windows.Forms.Button();
      this.copyToClipboard = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPageENG = new System.Windows.Forms.TabPage();
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.tabPageCZ = new System.Windows.Forms.TabPage();
      this.czRTB = new System.Windows.Forms.RichTextBox();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.panel1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPageENG.SuspendLayout();
      this.tabPageCZ.SuspendLayout();
      this.SuspendLayout();
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.closeButton.Image = global::FpgaLcdUtils.Properties.Resources._109_AllAnnotations_Default_16x16_72;
      this.closeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.closeButton.Location = new System.Drawing.Point(443, 1);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(75, 25);
      this.closeButton.TabIndex = 0;
      this.closeButton.Text = "Close";
      this.closeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip1.SetToolTip(this.closeButton, "Close Note Window\r\nAll content is cleared.");
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // copyToClipboard
      // 
      this.copyToClipboard.Image = global::FpgaLcdUtils.Properties.Resources.CopyHS;
      this.copyToClipboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.copyToClipboard.Location = new System.Drawing.Point(4, 2);
      this.copyToClipboard.Name = "copyToClipboard";
      this.copyToClipboard.Size = new System.Drawing.Size(209, 23);
      this.copyToClipboard.TabIndex = 1;
      this.copyToClipboard.Text = "Copy to Clipboard";
      this.copyToClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip1.SetToolTip(this.copyToClipboard, "Copy All Tab Content to Clipboard");
      this.copyToClipboard.UseVisualStyleBackColor = true;
      this.copyToClipboard.Click += new System.EventHandler(this.copyToClipboard_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.copyToClipboard);
      this.panel1.Controls.Add(this.closeButton);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 403);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(521, 29);
      this.panel1.TabIndex = 0;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPageENG);
      this.tabControl1.Controls.Add(this.tabPageCZ);
      this.tabControl1.ImageList = this.imageList1;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(347, 403);
      this.tabControl1.TabIndex = 2;
      // 
      // tabPageENG
      // 
      this.tabPageENG.Controls.Add(this.richTextBox1);
      this.tabPageENG.ImageIndex = 0;
      this.tabPageENG.Location = new System.Drawing.Point(4, 23);
      this.tabPageENG.Name = "tabPageENG";
      this.tabPageENG.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageENG.Size = new System.Drawing.Size(339, 376);
      this.tabPageENG.TabIndex = 0;
      this.tabPageENG.Text = "ENG";
      this.tabPageENG.UseVisualStyleBackColor = true;
      // 
      // richTextBox1
      // 
      this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.richTextBox1.Location = new System.Drawing.Point(3, 3);
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.Size = new System.Drawing.Size(333, 370);
      this.richTextBox1.TabIndex = 0;
      this.richTextBox1.Text = "";
      // 
      // tabPageCZ
      // 
      this.tabPageCZ.Controls.Add(this.czRTB);
      this.tabPageCZ.ImageIndex = 1;
      this.tabPageCZ.Location = new System.Drawing.Point(4, 23);
      this.tabPageCZ.Name = "tabPageCZ";
      this.tabPageCZ.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageCZ.Size = new System.Drawing.Size(339, 376);
      this.tabPageCZ.TabIndex = 1;
      this.tabPageCZ.Text = "CZ";
      this.tabPageCZ.UseVisualStyleBackColor = true;
      // 
      // czRTB
      // 
      this.czRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.czRTB.Location = new System.Drawing.Point(3, 3);
      this.czRTB.Name = "czRTB";
      this.czRTB.Size = new System.Drawing.Size(333, 370);
      this.czRTB.TabIndex = 0;
      this.czRTB.Text = "";
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "engflag.png");
      this.imageList1.Images.SetKeyName(1, "czflag.png");
      // 
      // QCToDoForm_notused
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(521, 432);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.panel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(530, 470);
      this.Name = "QCToDoForm_notused";
      this.Text = "ToDo in Quartus Project";
      this.panel1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPageENG.ResumeLayout(false);
      this.tabPageCZ.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private ToolTip toolTip1;
    private Panel panel1;
    private Button copyToClipboard;
    private Button closeButton;
    private TabControl tabControl1;
    private TabPage tabPageENG;
    private TabPage tabPageCZ;
    private RichTextBox richTextBox1;
    private RichTextBox czRTB;
    private ImageList imageList1;
  }
}