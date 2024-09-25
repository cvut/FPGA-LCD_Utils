namespace LSPtools
{
  partial class FormStart
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
      this.button1 = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.bitmapButton = new System.Windows.Forms.Button();
      this.button4 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.notifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.runBitmap = new System.Windows.Forms.ToolStripMenuItem();
      this.runMeasureRulers = new System.Windows.Forms.ToolStripMenuItem();
      this.runQuartusProjectCheck = new System.Windows.Forms.ToolStripMenuItem();
      this.runTestbenchViewer = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.fullScreenTool = new System.Windows.Forms.ToolStripMenuItem();
      this.emergencyRestart = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.button5 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.infoButton = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.notifyIconContextMenuStrip.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.AutoSize = true;
      this.button1.BackColor = System.Drawing.SystemColors.ControlLight;
      this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.button1.Image = global::LSPtools.Properties.Resources.tb;
      this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.button1.Location = new System.Drawing.Point(4, 92);
      this.button1.Margin = new System.Windows.Forms.Padding(4);
      this.button1.MinimumSize = new System.Drawing.Size(32, 32);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(182, 38);
      this.button1.TabIndex = 0;
      this.button1.Text = "Testbench Viewer";
      this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip1.SetToolTip(this.button1, "View LCD-testbench result");
      this.button1.UseVisualStyleBackColor = false;
      this.button1.Click += new System.EventHandler(this.runTestbenchViewer_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Controls.Add(this.bitmapButton, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.button4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.button1, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.button3, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 52);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(190, 178);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // bitmapButton
      // 
      this.bitmapButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.bitmapButton.AutoSize = true;
      this.bitmapButton.BackColor = System.Drawing.SystemColors.ControlLight;
      this.bitmapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.bitmapButton.Image = global::LSPtools.Properties.Resources.BM32x32;
      this.bitmapButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.bitmapButton.Location = new System.Drawing.Point(3, 3);
      this.bitmapButton.MinimumSize = new System.Drawing.Size(32, 32);
      this.bitmapButton.Name = "bitmapButton";
      this.bitmapButton.Size = new System.Drawing.Size(184, 38);
      this.bitmapButton.TabIndex = 1;
      this.bitmapButton.Text = "Bitmap to VHDL";
      this.bitmapButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip1.SetToolTip(this.bitmapButton, "Converting images to VHDL code for FPGA memories");
      this.bitmapButton.UseVisualStyleBackColor = false;
      this.bitmapButton.Click += new System.EventHandler(this.runBitmap2MIF_Click);
      // 
      // button4
      // 
      this.button4.AutoSize = true;
      this.button4.BackColor = System.Drawing.SystemColors.ControlLight;
      this.button4.Dock = System.Windows.Forms.DockStyle.Top;
      this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.button4.Image = global::LSPtools.Properties.Resources.QCtool32x32;
      this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.button4.Location = new System.Drawing.Point(3, 137);
      this.button4.MinimumSize = new System.Drawing.Size(32, 32);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(184, 38);
      this.button4.TabIndex = 2;
      this.button4.Text = "Quartus Project ???";
      this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip1.SetToolTip(this.button4, "The checker of Quartus Project settings");
      this.button4.UseVisualStyleBackColor = false;
      this.button4.Click += new System.EventHandler(this.runChecker_Click);
      // 
      // button3
      // 
      this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.button3.AutoSize = true;
      this.button3.BackColor = System.Drawing.SystemColors.ControlLight;
      this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.button3.Image = global::LSPtools.Properties.Resources.Measure32x32;
      this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.button3.Location = new System.Drawing.Point(3, 47);
      this.button3.MinimumSize = new System.Drawing.Size(32, 32);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(184, 38);
      this.button3.TabIndex = 2;
      this.button3.Text = "LCD Geometry Rulers";
      this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip1.SetToolTip(this.button3, "Measurement rulers of LCD images");
      this.button3.UseVisualStyleBackColor = false;
      this.button3.Click += new System.EventHandler(this.runLCDGeometry_Click);
      // 
      // notifyIconContextMenuStrip
      // 
      this.notifyIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runBitmap,
            this.runMeasureRulers,
            this.runQuartusProjectCheck,
            this.runTestbenchViewer,
            this.toolStripSeparator1,
            this.fullScreenTool,
            this.emergencyRestart,
            this.toolStripSeparator2,
            this.closeToolStripMenuItem});
      this.notifyIconContextMenuStrip.Name = "notifyIconContextMenuStrip";
      this.notifyIconContextMenuStrip.Size = new System.Drawing.Size(242, 192);
      // 
      // runBitmap
      // 
      this.runBitmap.Image = global::LSPtools.Properties.Resources.BM16x16;
      this.runBitmap.Name = "runBitmap";
      this.runBitmap.Size = new System.Drawing.Size(241, 22);
      this.runBitmap.Text = "Bitmap to VHDL";
      this.runBitmap.ToolTipText = "Converting images to VHDL code for FPGA memories";
      this.runBitmap.Click += new System.EventHandler(this.runBitmap2MIF_Click);
      // 
      // runMeasureRulers
      // 
      this.runMeasureRulers.Image = global::LSPtools.Properties.Resources.Measure32x32;
      this.runMeasureRulers.Name = "runMeasureRulers";
      this.runMeasureRulers.Size = new System.Drawing.Size(241, 22);
      this.runMeasureRulers.Text = "LCD Geometry Rulers";
      this.runMeasureRulers.ToolTipText = "Measurement rulers of LCD images";
      this.runMeasureRulers.Click += new System.EventHandler(this.runLCDGeometry_Click);
      // 
      // runQuartusProjectCheck
      // 
      this.runQuartusProjectCheck.Image = global::LSPtools.Properties.Resources.QCtool32x32;
      this.runQuartusProjectCheck.Name = "runQuartusProjectCheck";
      this.runQuartusProjectCheck.Size = new System.Drawing.Size(241, 22);
      this.runQuartusProjectCheck.Text = "Quartus Project ???";
      this.runQuartusProjectCheck.ToolTipText = "The checker of Quartus Project settings";
      this.runQuartusProjectCheck.Click += new System.EventHandler(this.runChecker_Click);
      // 
      // runTestbenchViewer
      // 
      this.runTestbenchViewer.Image = global::LSPtools.Properties.Resources.tb1;
      this.runTestbenchViewer.Name = "runTestbenchViewer";
      this.runTestbenchViewer.Size = new System.Drawing.Size(241, 22);
      this.runTestbenchViewer.Text = "Testbench Viewer";
      this.runTestbenchViewer.ToolTipText = "View LCD-testbench result";
      this.runTestbenchViewer.Click += new System.EventHandler(this.runTestbenchViewer_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
      // 
      // fullScreenTool
      // 
      this.fullScreenTool.Image = global::LSPtools.Properties.Resources._125_FullView_16x16_72;
      this.fullScreenTool.Name = "fullScreenTool";
      this.fullScreenTool.Size = new System.Drawing.Size(241, 22);
      this.fullScreenTool.Text = "Show Tool Window";
      this.fullScreenTool.ToolTipText = "Force Show of LSPtools Main Windows";
      this.fullScreenTool.Click += new System.EventHandler(this.fullScreenTool_Click);
      // 
      // emergencyRestart
      // 
      this.emergencyRestart.Image = global::LSPtools.Properties.Resources.Restart_16x;
      this.emergencyRestart.Name = "emergencyRestart";
      this.emergencyRestart.Size = new System.Drawing.Size(241, 22);
      this.emergencyRestart.Text = "Emergency Restart of LSP Tools ";
      this.emergencyRestart.ToolTipText = "An emergency restart of LSP Tools not using the stored setting";
      this.emergencyRestart.Click += new System.EventHandler(this.emergencyRestart_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(238, 6);
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Image = global::LSPtools.Properties.Resources.ClosePreviewHS;
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
      this.closeToolStripMenuItem.Text = "Close LSP Tools";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // notifyIconMain
      // 
      this.notifyIconMain.BalloonTipText = "LSP Tools";
      this.notifyIconMain.ContextMenuStrip = this.notifyIconContextMenuStrip;
      this.notifyIconMain.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconMain.Icon")));
      this.notifyIconMain.Text = "LSPtools";
      this.notifyIconMain.Visible = true;
      this.notifyIconMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
      this.notifyIconMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
      // 
      // button5
      // 
      this.button5.Image = global::LSPtools.Properties.Resources.minimize_24x24;
      this.button5.Location = new System.Drawing.Point(5, 5);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(28, 28);
      this.button5.TabIndex = 3;
      this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.toolTip1.SetToolTip(this.button5, "Minimize the LSP tools into system tray icon");
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new System.EventHandler(this.minimizeButton_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(35, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(64, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "to Tray Icon";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.panel1.Controls.Add(this.infoButton);
      this.panel1.Controls.Add(this.button5);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(190, 39);
      this.panel1.TabIndex = 6;
      // 
      // infoButton
      // 
      this.infoButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.infoButton.AutoSize = true;
      this.infoButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.infoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.infoButton.ForeColor = System.Drawing.SystemColors.ControlText;
      this.infoButton.Image = global::LSPtools.Properties.Resources.HelpHS1;
      this.infoButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.infoButton.Location = new System.Drawing.Point(120, 6);
      this.infoButton.Name = "infoButton";
      this.infoButton.Size = new System.Drawing.Size(66, 26);
      this.infoButton.TabIndex = 8;
      this.infoButton.Text = "V1.3";
      this.infoButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.infoButton.UseVisualStyleBackColor = false;
      this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
      // 
      // FormStart
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Info;
      this.ClientSize = new System.Drawing.Size(190, 230);
      this.ContextMenuStrip = this.notifyIconContextMenuStrip;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Controls.Add(this.panel1);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(210, 273);
      this.Name = "FormStart";
      this.Text = "LSP tool for FPGA";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormStart_FormClosing);
      this.Load += new System.EventHandler(this.FormStart_Load);
      this.Shown += new System.EventHandler(this.FormStart_Shown);
      this.Resize += new System.EventHandler(this.FormStart_Resize);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.notifyIconContextMenuStrip.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Button button1;
    private TableLayoutPanel tableLayoutPanel1;
    private Button bitmapButton;
    private ContextMenuStrip notifyIconContextMenuStrip;
    private ToolStripMenuItem runBitmap;
    private ToolStripMenuItem runTestbenchViewer;
//   private ToolStripMenuItem separatorToolStripMenuItem1;
    private ToolStripMenuItem closeToolStripMenuItem;
    private NotifyIcon notifyIconMain;
    private Button button3;
    private ToolStripMenuItem runMeasureRulers;
    private Button button4;
    private ToolTip toolTip1;
    private Label label1;
    private Button button5;
    private ToolStripMenuItem runQuartusProjectCheck;
    private Panel panel1;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem fullScreenTool;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem emergencyRestart;
    private Button infoButton;
  }
}