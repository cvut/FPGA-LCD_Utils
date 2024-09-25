namespace LSPtools
{
    partial class TBFormMain
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TBFormMain));
      this.setZoom200MenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.hScrollBarGraph = new System.Windows.Forms.HScrollBar();
      this.vScrollBarGraph = new System.Windows.Forms.VScrollBar();
      this.panel2 = new System.Windows.Forms.Panel();
      this.adjustWindow = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.cbAutoReload = new System.Windows.Forms.CheckBox();
      this.zoomTextBox = new System.Windows.Forms.TextBox();
      this.xyCoordinatesTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.rgbTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.reloadTestbenchButton = new System.Windows.Forms.Button();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.messageToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openVGAImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveImageAsBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.recentTestbenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.showVisibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showWholeFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.waitForCompleteFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.lastFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.setZoom100MenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.setZoom125MenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.setZoom150MenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.panelGraph = new System.Windows.Forms.Panel();
      this.panelGraphContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.copyPixelInfoToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.sendPixelInfoToInfoWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openLCDImageFile = new System.Windows.Forms.OpenFileDialog();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.saveLCDImageAs = new System.Windows.Forms.SaveFileDialog();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.startPlayButton = new System.Windows.Forms.Button();
      this.playImageList = new System.Windows.Forms.ImageList(this.components);
      this.playPauseButton = new System.Windows.Forms.Button();
      this.timer2 = new System.Windows.Forms.Timer(this.components);
      this.panelPlay = new System.Windows.Forms.Panel();
      this.playProgressBar = new System.Windows.Forms.ProgressBar();
      this.endPlayButton = new System.Windows.Forms.Button();
      this.frameEndTextBox = new System.Windows.Forms.TextBox();
      this.frameNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.panel2.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.panelGraphContextMenuStrip1.SuspendLayout();
      this.panelPlay.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.frameNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // setZoom200MenuItem
      // 
      this.setZoom200MenuItem.Name = "setZoom200MenuItem";
      this.setZoom200MenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
      this.setZoom200MenuItem.Size = new System.Drawing.Size(124, 22);
      this.setZoom200MenuItem.Tag = "8";
      this.setZoom200MenuItem.Text = "200 %";
      this.setZoom200MenuItem.Click += new System.EventHandler(this.zoomSet_Click);
      // 
      // hScrollBarGraph
      // 
      this.hScrollBarGraph.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.hScrollBarGraph.Location = new System.Drawing.Point(0, 211);
      this.hScrollBarGraph.Maximum = 799;
      this.hScrollBarGraph.Name = "hScrollBarGraph";
      this.hScrollBarGraph.Size = new System.Drawing.Size(304, 21);
      this.hScrollBarGraph.TabIndex = 10;
      this.hScrollBarGraph.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBarGraph_Scroll);
      this.hScrollBarGraph.MouseEnter += new System.EventHandler(this.hScrollBarGraph_MouseEnter);
      // 
      // vScrollBarGraph
      // 
      this.vScrollBarGraph.Dock = System.Windows.Forms.DockStyle.Right;
      this.vScrollBarGraph.Location = new System.Drawing.Point(283, 24);
      this.vScrollBarGraph.Maximum = 524;
      this.vScrollBarGraph.Name = "vScrollBarGraph";
      this.vScrollBarGraph.Size = new System.Drawing.Size(21, 187);
      this.vScrollBarGraph.TabIndex = 11;
      this.vScrollBarGraph.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarGraph_Scroll);
      this.vScrollBarGraph.MouseEnter += new System.EventHandler(this.vScrollBarGraph_MouseEnter);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.adjustWindow);
      this.panel2.Controls.Add(this.label1);
      this.panel2.Controls.Add(this.cbAutoReload);
      this.panel2.Controls.Add(this.zoomTextBox);
      this.panel2.Controls.Add(this.xyCoordinatesTextBox);
      this.panel2.Controls.Add(this.label3);
      this.panel2.Controls.Add(this.rgbTextBox);
      this.panel2.Controls.Add(this.label2);
      this.panel2.Controls.Add(this.reloadTestbenchButton);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel2.Location = new System.Drawing.Point(0, 24);
      this.panel2.Margin = new System.Windows.Forms.Padding(2);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(283, 26);
      this.panel2.TabIndex = 12;
      // 
      // adjustWindow
      // 
      this.adjustWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.adjustWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.adjustWindow.Image = global::LSPtools.Properties.Resources.FullScreenHS;
      this.adjustWindow.Location = new System.Drawing.Point(256, 0);
      this.adjustWindow.Name = "adjustWindow";
      this.adjustWindow.Size = new System.Drawing.Size(24, 23);
      this.adjustWindow.TabIndex = 9;
      this.toolTip1.SetToolTip(this.adjustWindow, "Adjust windows size to fit \r\n100 % zoom of picture.");
      this.adjustWindow.UseVisualStyleBackColor = true;
      this.adjustWindow.Click += new System.EventHandler(this.adjustWindowMenuItem_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(121, 6);
      this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Zoom";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // cbAutoReload
      // 
      this.cbAutoReload.AutoSize = true;
      this.cbAutoReload.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.cbAutoReload.Checked = true;
      this.cbAutoReload.CheckState = System.Windows.Forms.CheckState.Checked;
      this.cbAutoReload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbAutoReload.Location = new System.Drawing.Point(2, 5);
      this.cbAutoReload.Margin = new System.Windows.Forms.Padding(2);
      this.cbAutoReload.Name = "cbAutoReload";
      this.cbAutoReload.Size = new System.Drawing.Size(52, 17);
      this.cbAutoReload.TabIndex = 5;
      this.cbAutoReload.Text = "Auto";
      this.cbAutoReload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.cbAutoReload.UseVisualStyleBackColor = true;
      this.cbAutoReload.CheckedChanged += new System.EventHandler(this.cbAutoReload_CheckedChanged);
      // 
      // zoomTextBox
      // 
      this.zoomTextBox.Location = new System.Drawing.Point(162, 3);
      this.zoomTextBox.Margin = new System.Windows.Forms.Padding(2);
      this.zoomTextBox.Name = "zoomTextBox";
      this.zoomTextBox.ReadOnly = true;
      this.zoomTextBox.Size = new System.Drawing.Size(80, 20);
      this.zoomTextBox.TabIndex = 4;
      this.zoomTextBox.Text = "000 000";
      this.toolTip1.SetToolTip(this.zoomTextBox, "zoom in xcolumn and yrow");
      // 
      // xyCoordinatesTextBox
      // 
      this.xyCoordinatesTextBox.Location = new System.Drawing.Point(284, 4);
      this.xyCoordinatesTextBox.Margin = new System.Windows.Forms.Padding(2);
      this.xyCoordinatesTextBox.Name = "xyCoordinatesTextBox";
      this.xyCoordinatesTextBox.ReadOnly = true;
      this.xyCoordinatesTextBox.Size = new System.Drawing.Size(58, 20);
      this.xyCoordinatesTextBox.TabIndex = 4;
      this.xyCoordinatesTextBox.Text = "000 000";
      this.toolTip1.SetToolTip(this.xyCoordinatesTextBox, "xcolumn and yrow coordinates");
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(253, 6);
      this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(27, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "X,Y";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // rgbTextBox
      // 
      this.rgbTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rgbTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.rgbTextBox.Location = new System.Drawing.Point(383, 4);
      this.rgbTextBox.Margin = new System.Windows.Forms.Padding(2);
      this.rgbTextBox.Name = "rgbTextBox";
      this.rgbTextBox.ReadOnly = true;
      this.rgbTextBox.Size = new System.Drawing.Size(0, 20);
      this.rgbTextBox.TabIndex = 4;
      this.rgbTextBox.Text = "00 00 00";
      this.toolTip1.SetToolTip(this.rgbTextBox, "RBG color of selected pixel");
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(347, 6);
      this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(33, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "RGB";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // reloadTestbenchButton
      // 
      this.reloadTestbenchButton.FlatAppearance.BorderSize = 2;
      this.reloadTestbenchButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ButtonHighlight;
      this.reloadTestbenchButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ButtonShadow;
      this.reloadTestbenchButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.reloadTestbenchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.reloadTestbenchButton.Location = new System.Drawing.Point(59, 3);
      this.reloadTestbenchButton.Margin = new System.Windows.Forms.Padding(2);
      this.reloadTestbenchButton.Name = "reloadTestbenchButton";
      this.reloadTestbenchButton.Size = new System.Drawing.Size(56, 20);
      this.reloadTestbenchButton.TabIndex = 0;
      this.reloadTestbenchButton.Text = "Reload";
      this.toolTip1.SetToolTip(this.reloadTestbenchButton, "Stop playing and reload the testbench");
      this.reloadTestbenchButton.UseVisualStyleBackColor = true;
      this.reloadTestbenchButton.Click += new System.EventHandler(this.reloadTestbenchButton_Click);
      // 
      // statusStrip1
      // 
      this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageToolStripStatusLabel});
      this.statusStrip1.Location = new System.Drawing.Point(0, 256);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
      this.statusStrip1.Size = new System.Drawing.Size(304, 22);
      this.statusStrip1.TabIndex = 14;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // messageToolStripStatusLabel
      // 
      this.messageToolStripStatusLabel.Name = "messageToolStripStatusLabel";
      this.messageToolStripStatusLabel.Size = new System.Drawing.Size(47, 17);
      this.messageToolStripStatusLabel.Text = "no data";
      // 
      // menuStrip1
      // 
      this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.showMenu,
            this.toolStripMenuItem2,
            this.helpToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
      this.menuStrip1.Size = new System.Drawing.Size(304, 24);
      this.menuStrip1.TabIndex = 13;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openVGAImageToolStripMenuItem,
            this.saveImageAsBitmapToolStripMenuItem,
            this.toolStripMenuItem1,
            this.recentTestbenchToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // openVGAImageToolStripMenuItem
      // 
      this.openVGAImageToolStripMenuItem.Name = "openVGAImageToolStripMenuItem";
      this.openVGAImageToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
      this.openVGAImageToolStripMenuItem.Text = "Open LCD Testbench...";
      this.openVGAImageToolStripMenuItem.Click += new System.EventHandler(this.openLCDImageToolStripMenuItem_Click);
      // 
      // saveImageAsBitmapToolStripMenuItem
      // 
      this.saveImageAsBitmapToolStripMenuItem.Name = "saveImageAsBitmapToolStripMenuItem";
      this.saveImageAsBitmapToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
      this.saveImageAsBitmapToolStripMenuItem.Text = "Save Image as...";
      this.saveImageAsBitmapToolStripMenuItem.Click += new System.EventHandler(this.saveImageAsBitmapToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(190, 6);
      // 
      // recentTestbenchToolStripMenuItem
      // 
      this.recentTestbenchToolStripMenuItem.Name = "recentTestbenchToolStripMenuItem";
      this.recentTestbenchToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
      this.recentTestbenchToolStripMenuItem.Text = "Recent Testbench";
      this.recentTestbenchToolStripMenuItem.Click += new System.EventHandler(this.recentTestbenchToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // showMenu
      // 
      this.showMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showVisibleToolStripMenuItem,
            this.showWholeFrameToolStripMenuItem,
            this.waitForCompleteFrameToolStripMenuItem,
            this.lastFrameToolStripMenuItem});
      this.showMenu.Name = "showMenu";
      this.showMenu.Size = new System.Drawing.Size(48, 20);
      this.showMenu.Text = "&Show";
      // 
      // showVisibleToolStripMenuItem
      // 
      this.showVisibleToolStripMenuItem.Checked = true;
      this.showVisibleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showVisibleToolStripMenuItem.Name = "showVisibleToolStripMenuItem";
      this.showVisibleToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
      this.showVisibleToolStripMenuItem.Text = "Visible 800x480 pixels";
      this.showVisibleToolStripMenuItem.ToolTipText = "Show only the visible part of LCD display";
      this.showVisibleToolStripMenuItem.Click += new System.EventHandler(this.showVisibleToolStripMenuItem_Click);
      // 
      // showWholeFrameToolStripMenuItem
      // 
      this.showWholeFrameToolStripMenuItem.Name = "showWholeFrameToolStripMenuItem";
      this.showWholeFrameToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
      this.showWholeFrameToolStripMenuItem.Text = "Whole frame 1024x525 pixels";
      this.showWholeFrameToolStripMenuItem.ToolTipText = "Show whole LCD pixel data";
      this.showWholeFrameToolStripMenuItem.Click += new System.EventHandler(this.showWholeFrameToolStripMenuItem_Click);
      // 
      // waitForCompleteFrameToolStripMenuItem
      // 
      this.waitForCompleteFrameToolStripMenuItem.Checked = true;
      this.waitForCompleteFrameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.waitForCompleteFrameToolStripMenuItem.Name = "waitForCompleteFrameToolStripMenuItem";
      this.waitForCompleteFrameToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
      this.waitForCompleteFrameToolStripMenuItem.Text = "Wait 2 s for Complete frame";
      this.waitForCompleteFrameToolStripMenuItem.ToolTipText = "Insert 2 second waiting for finished current frame. ";
      this.waitForCompleteFrameToolStripMenuItem.Click += new System.EventHandler(this.lastFullFrameToolStripMenuItem_Click);
      // 
      // lastFrameToolStripMenuItem
      // 
      this.lastFrameToolStripMenuItem.Name = "lastFrameToolStripMenuItem";
      this.lastFrameToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
      this.lastFrameToolStripMenuItem.Text = "Always show last frame";
      this.lastFrameToolStripMenuItem.ToolTipText = "The last frame is shown\r\n immediately";
      this.lastFrameToolStripMenuItem.Click += new System.EventHandler(this.lastFrameToolStripMenuItem_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setZoom100MenuItem,
            this.setZoom125MenuItem,
            this.setZoom150MenuItem,
            this.setZoom200MenuItem});
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.ShortcutKeys = System.Windows.Forms.Keys.F5;
      this.toolStripMenuItem2.Size = new System.Drawing.Size(51, 20);
      this.toolStripMenuItem2.Text = "Zoom";
      // 
      // setZoom100MenuItem
      // 
      this.setZoom100MenuItem.Name = "setZoom100MenuItem";
      this.setZoom100MenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
      this.setZoom100MenuItem.Size = new System.Drawing.Size(124, 22);
      this.setZoom100MenuItem.Tag = "5";
      this.setZoom100MenuItem.Text = "100 %";
      this.setZoom100MenuItem.ToolTipText = "Reset zoom to original size";
      this.setZoom100MenuItem.Click += new System.EventHandler(this.zoomSet_Click);
      // 
      // setZoom125MenuItem
      // 
      this.setZoom125MenuItem.Name = "setZoom125MenuItem";
      this.setZoom125MenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
      this.setZoom125MenuItem.Size = new System.Drawing.Size(124, 22);
      this.setZoom125MenuItem.Tag = "6";
      this.setZoom125MenuItem.Text = "125 %";
      this.setZoom125MenuItem.Click += new System.EventHandler(this.zoomSet_Click);
      // 
      // setZoom150MenuItem
      // 
      this.setZoom150MenuItem.Name = "setZoom150MenuItem";
      this.setZoom150MenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
      this.setZoom150MenuItem.Size = new System.Drawing.Size(124, 22);
      this.setZoom150MenuItem.Tag = "7";
      this.setZoom150MenuItem.Text = "150%";
      this.setZoom150MenuItem.Click += new System.EventHandler(this.zoomSet_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.ShortcutKeyDisplayString = "F1";
      this.helpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      this.helpToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.helpToolStripMenuItem.ToolTipText = "About LCD testbench";
      this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
      // 
      // panelGraph
      // 
      this.panelGraph.BackColor = System.Drawing.Color.Black;
      this.panelGraph.ContextMenuStrip = this.panelGraphContextMenuStrip1;
      this.panelGraph.Cursor = System.Windows.Forms.Cursors.Cross;
      this.panelGraph.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelGraph.Location = new System.Drawing.Point(0, 50);
      this.panelGraph.Margin = new System.Windows.Forms.Padding(2);
      this.panelGraph.Name = "panelGraph";
      this.panelGraph.Size = new System.Drawing.Size(283, 161);
      this.panelGraph.TabIndex = 9;
      this.panelGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.panelGraph_Paint);
      this.panelGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelGraf_MouseDown);
      this.panelGraph.MouseEnter += new System.EventHandler(this.panelGraph_MouseEnter);
      this.panelGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelGraf_MouseMove);
      this.panelGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelGraf_MouseUp);
      // 
      // panelGraphContextMenuStrip1
      // 
      this.panelGraphContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPixelInfoToClipboardToolStripMenuItem,
            this.sendPixelInfoToInfoWindowToolStripMenuItem});
      this.panelGraphContextMenuStrip1.Name = "panelGraphContextMenuStrip1";
      this.panelGraphContextMenuStrip1.Size = new System.Drawing.Size(245, 48);
      // 
      // copyPixelInfoToClipboardToolStripMenuItem
      // 
      this.copyPixelInfoToClipboardToolStripMenuItem.Name = "copyPixelInfoToClipboardToolStripMenuItem";
      this.copyPixelInfoToClipboardToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
      this.copyPixelInfoToClipboardToolStripMenuItem.Text = "Copy Pixel Info to Clipboard";
      this.copyPixelInfoToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyPixelInfoToClipboardToolStripMenuItem_Click);
      // 
      // sendPixelInfoToInfoWindowToolStripMenuItem
      // 
      this.sendPixelInfoToInfoWindowToolStripMenuItem.Name = "sendPixelInfoToInfoWindowToolStripMenuItem";
      this.sendPixelInfoToInfoWindowToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
      this.sendPixelInfoToInfoWindowToolStripMenuItem.Text = "Send Pixel Info to Note-Window";
      this.sendPixelInfoToInfoWindowToolStripMenuItem.Click += new System.EventHandler(this.sendPixelInfoToInfoWindowToolStripMenuItem_Click);
      // 
      // openLCDImageFile
      // 
      this.openLCDImageFile.DefaultExt = "txt";
      this.openLCDImageFile.FileName = "openFileDialog1";
      this.openLCDImageFile.Filter = "Testbench Image File (*.txt)|*.txt|All files (*.*)|*.*";
      this.openLCDImageFile.ShowReadOnly = true;
      // 
      // timer1
      // 
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // saveLCDImageAs
      // 
      this.saveLCDImageAs.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG image (*.png)|*.png|All files (*.*)|*" +
    ".*";
      this.saveLCDImageAs.Title = "Save displayed image as";
      // 
      // startPlayButton
      // 
      this.startPlayButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.startPlayButton.ImageIndex = 0;
      this.startPlayButton.ImageList = this.playImageList;
      this.startPlayButton.Location = new System.Drawing.Point(175, 1);
      this.startPlayButton.Name = "startPlayButton";
      this.startPlayButton.Size = new System.Drawing.Size(20, 20);
      this.startPlayButton.TabIndex = 3;
      this.toolTip1.SetToolTip(this.startPlayButton, "Play/Pause");
      this.startPlayButton.UseVisualStyleBackColor = true;
      this.startPlayButton.Click += new System.EventHandler(this.startPlayButton_Click);
      // 
      // playImageList
      // 
      this.playImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("playImageList.ImageStream")));
      this.playImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.playImageList.Images.SetKeyName(0, "Start.bmp");
      this.playImageList.Images.SetKeyName(1, "Play.bmp");
      this.playImageList.Images.SetKeyName(2, "Pause.bmp");
      this.playImageList.Images.SetKeyName(3, "End.bmp");
      // 
      // playPauseButton
      // 
      this.playPauseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.playPauseButton.ImageIndex = 1;
      this.playPauseButton.ImageList = this.playImageList;
      this.playPauseButton.Location = new System.Drawing.Point(202, 1);
      this.playPauseButton.Name = "playPauseButton";
      this.playPauseButton.Size = new System.Drawing.Size(20, 20);
      this.playPauseButton.TabIndex = 3;
      this.toolTip1.SetToolTip(this.playPauseButton, "Play frames, but auto-reload option will be disabled!");
      this.playPauseButton.UseVisualStyleBackColor = true;
      this.playPauseButton.Click += new System.EventHandler(this.playPauseButton_Click);
      // 
      // panelPlay
      // 
      this.panelPlay.Controls.Add(this.playProgressBar);
      this.panelPlay.Controls.Add(this.endPlayButton);
      this.panelPlay.Controls.Add(this.playPauseButton);
      this.panelPlay.Controls.Add(this.startPlayButton);
      this.panelPlay.Controls.Add(this.frameEndTextBox);
      this.panelPlay.Controls.Add(this.frameNumericUpDown);
      this.panelPlay.Controls.Add(this.label5);
      this.panelPlay.Controls.Add(this.label4);
      this.panelPlay.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panelPlay.Location = new System.Drawing.Point(0, 232);
      this.panelPlay.Name = "panelPlay";
      this.panelPlay.Size = new System.Drawing.Size(304, 24);
      this.panelPlay.TabIndex = 15;
      // 
      // playProgressBar
      // 
      this.playProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.playProgressBar.ForeColor = System.Drawing.SystemColors.Info;
      this.playProgressBar.Location = new System.Drawing.Point(256, 3);
      this.playProgressBar.Maximum = 1;
      this.playProgressBar.Minimum = 1;
      this.playProgressBar.Name = "playProgressBar";
      this.playProgressBar.Size = new System.Drawing.Size(45, 16);
      this.playProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.playProgressBar.TabIndex = 4;
      this.playProgressBar.Value = 1;
      // 
      // endPlayButton
      // 
      this.endPlayButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.endPlayButton.ImageIndex = 3;
      this.endPlayButton.ImageList = this.playImageList;
      this.endPlayButton.Location = new System.Drawing.Point(229, 1);
      this.endPlayButton.Name = "endPlayButton";
      this.endPlayButton.Size = new System.Drawing.Size(20, 20);
      this.endPlayButton.TabIndex = 3;
      this.endPlayButton.UseVisualStyleBackColor = true;
      this.endPlayButton.Click += new System.EventHandler(this.endPlayButton_Click);
      // 
      // frameEndTextBox
      // 
      this.frameEndTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.frameEndTextBox.Location = new System.Drawing.Point(137, 1);
      this.frameEndTextBox.Name = "frameEndTextBox";
      this.frameEndTextBox.ReadOnly = true;
      this.frameEndTextBox.Size = new System.Drawing.Size(31, 20);
      this.frameEndTextBox.TabIndex = 2;
      this.frameEndTextBox.Text = "001";
      // 
      // frameNumericUpDown
      // 
      this.frameNumericUpDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.frameNumericUpDown.Location = new System.Drawing.Point(49, 1);
      this.frameNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.frameNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.frameNumericUpDown.Name = "frameNumericUpDown";
      this.frameNumericUpDown.Size = new System.Drawing.Size(56, 20);
      this.frameNumericUpDown.TabIndex = 1;
      this.frameNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.frameNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.frameNumericUpDown.ValueChanged += new System.EventHandler(this.frameNumericUpDown_ValueChanged);
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(112, 5);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(18, 13);
      this.label5.TabIndex = 0;
      this.label5.Text = "of";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(1, 5);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(41, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Frame";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // TBFormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(304, 278);
      this.Controls.Add(this.panelGraph);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.vScrollBarGraph);
      this.Controls.Add(this.hScrollBarGraph);
      this.Controls.Add(this.panelPlay);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(320, 240);
      this.Name = "TBFormMain";
      this.Text = "Testbench Viewer";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
      this.Shown += new System.EventHandler(this.FormMain_Shown);
      this.Resize += new System.EventHandler(this.FormMain_Resize);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.panelGraphContextMenuStrip1.ResumeLayout(false);
      this.panelPlay.ResumeLayout(false);
      this.panelPlay.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.frameNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private ToolStripMenuItem setZoom200MenuItem;
        private HScrollBar hScrollBarGraph;
        private VScrollBar vScrollBarGraph;
        private Panel panel2;
        private Label label1;
        private CheckBox cbAutoReload;
        private TextBox zoomTextBox;
        private TextBox xyCoordinatesTextBox;
        private Label label3;
        private TextBox rgbTextBox;
        private Label label2;
        private Button reloadTestbenchButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel messageToolStripStatusLabel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openVGAImageToolStripMenuItem;
        private ToolStripMenuItem saveImageAsBitmapToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem setZoom100MenuItem;
        private ToolStripMenuItem setZoom125MenuItem;
        private ToolStripMenuItem setZoom150MenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private Panel panelGraph;
        private OpenFileDialog openLCDImageFile;
        private System.Windows.Forms.Timer timer1;
        private SaveFileDialog saveLCDImageAs;
        private ToolTip toolTip1;
        private ContextMenuStrip panelGraphContextMenuStrip1;
        private ToolStripMenuItem copyPixelInfoToClipboardToolStripMenuItem;
        private ToolStripMenuItem sendPixelInfoToInfoWindowToolStripMenuItem;
        private System.Windows.Forms.Timer timer2;
        private Button adjustWindow;
    private ToolStripMenuItem showMenu;
    private ToolStripMenuItem showVisibleToolStripMenuItem;
    private ToolStripMenuItem showWholeFrameToolStripMenuItem;
    private Panel panelPlay;
    private Label label4;
    private TextBox frameEndTextBox;
    private NumericUpDown frameNumericUpDown;
    private Label label5;
    private Button startPlayButton;
    private ImageList playImageList;
    private ProgressBar playProgressBar;
    private Button endPlayButton;
    private Button playPauseButton;
    private ToolStripMenuItem waitForCompleteFrameToolStripMenuItem;
    private ToolStripMenuItem lastFrameToolStripMenuItem;
    private ToolStripMenuItem recentTestbenchToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
  }
}