
namespace LSPtools
{
  partial class QCFormMain: Form
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QCFormMain));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openQuartusProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.vwfAdjustToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.vwfRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.tsslMessage = new System.Windows.Forms.ToolStripStatusLabel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.reportsRTB = new System.Windows.Forms.RichTextBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.clearLogs = new System.Windows.Forms.Button();
      this.copyToClipboard = new System.Windows.Forms.Button();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.timerUpdate = new System.Windows.Forms.Timer(this.components);
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.tabPageToDo = new System.Windows.Forms.TabPage();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.listViewReports = new System.Windows.Forms.ListView();
      this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeaderDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.descriptionRTB = new System.Windows.Forms.RichTextBox();
      this.panel3 = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.todoRTB = new System.Windows.Forms.RichTextBox();
      this.panelToDoEngCz = new System.Windows.Forms.Panel();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.czRadioButton = new System.Windows.Forms.RadioButton();
      this.engRadioButton = new System.Windows.Forms.RadioButton();
      this.labelToDo = new System.Windows.Forms.Label();
      this.tabPageLogs = new System.Windows.Forms.TabPage();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPageVWF = new System.Windows.Forms.TabPage();
      this.label4 = new System.Windows.Forms.Label();
      this.vwfAdjustButton = new System.Windows.Forms.Button();
      this.vwfRtb = new System.Windows.Forms.RichTextBox();
      this.button3 = new System.Windows.Forms.Button();
      this.vwfOpenButton = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.vwfQuartusProjectTextBox = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.vwfTopLevelEntity = new System.Windows.Forms.TextBox();
      this.vwfOpenedFilenameTextBox = new System.Windows.Forms.TextBox();
      this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
      this.timerReload = new System.Windows.Forms.Timer(this.components);
      this.columnHeaderCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.menuStrip1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.tabPageToDo.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.panelToDoEngCz.SuspendLayout();
      this.tabPageLogs.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPageVWF.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(754, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openQuartusProjectToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripMenuItem2,
            this.vwfAdjustToolStripMenuItem,
            this.vwfRecentToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // openQuartusProjectToolStripMenuItem
      // 
      this.openQuartusProjectToolStripMenuItem.Name = "openQuartusProjectToolStripMenuItem";
      this.openQuartusProjectToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.openQuartusProjectToolStripMenuItem.Text = "&Open Quartus Project";
      this.openQuartusProjectToolStripMenuItem.Click += new System.EventHandler(this.openQuartusProjectToolStripMenuItem_Click);
      // 
      // recentToolStripMenuItem
      // 
      this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
      this.recentToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.recentToolStripMenuItem.Text = "Recent Projects...";
      this.recentToolStripMenuItem.Click += new System.EventHandler(this.recentToolStripMenuItem_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(210, 6);
      // 
      // vwfAdjustToolStripMenuItem
      // 
      this.vwfAdjustToolStripMenuItem.Name = "vwfAdjustToolStripMenuItem";
      this.vwfAdjustToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.vwfAdjustToolStripMenuItem.Text = "Open *.vwf simulation file";
      this.vwfAdjustToolStripMenuItem.Click += new System.EventHandler(this.vwfAdjustToolStripMenuItem_Click);
      // 
      // vwfRecentToolStripMenuItem
      // 
      this.vwfRecentToolStripMenuItem.Name = "vwfRecentToolStripMenuItem";
      this.vwfRecentToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.vwfRecentToolStripMenuItem.Text = "Recent *.vwf simulations...";
      this.vwfRecentToolStripMenuItem.Click += new System.EventHandler(this.vwfRecentToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.exitToolStripMenuItem.Text = "E&xit";
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMessage});
      this.statusStrip1.Location = new System.Drawing.Point(0, 374);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(754, 22);
      this.statusStrip1.TabIndex = 1;
      this.statusStrip1.Text = "  ";
      // 
      // tsslMessage
      // 
      this.tsslMessage.Name = "tsslMessage";
      this.tsslMessage.Size = new System.Drawing.Size(0, 17);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.progressBar1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 24);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(754, 12);
      this.panel1.TabIndex = 2;
      // 
      // progressBar1
      // 
      this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.progressBar1.Location = new System.Drawing.Point(0, 0);
      this.progressBar1.Maximum = 256;
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(754, 12);
      this.progressBar1.Step = 16;
      this.progressBar1.TabIndex = 0;
      // 
      // reportsRTB
      // 
      this.reportsRTB.BackColor = System.Drawing.SystemColors.ControlLight;
      this.reportsRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.reportsRTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.reportsRTB.Location = new System.Drawing.Point(0, 0);
      this.reportsRTB.Name = "reportsRTB";
      this.reportsRTB.ReadOnly = true;
      this.reportsRTB.Size = new System.Drawing.Size(746, 273);
      this.reportsRTB.TabIndex = 3;
      this.reportsRTB.Text = "";
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.clearLogs);
      this.panel2.Controls.Add(this.copyToClipboard);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel2.Location = new System.Drawing.Point(0, 273);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(746, 38);
      this.panel2.TabIndex = 4;
      // 
      // clearLogs
      // 
      this.clearLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.clearLogs.Image = global::LSPtools.Properties.Resources.DeleteHS;
      this.clearLogs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.clearLogs.Location = new System.Drawing.Point(153, 6);
      this.clearLogs.Name = "clearLogs";
      this.clearLogs.Size = new System.Drawing.Size(144, 23);
      this.clearLogs.TabIndex = 2;
      this.clearLogs.Text = "Clear All Logs";
      this.clearLogs.UseVisualStyleBackColor = true;
      this.clearLogs.Click += new System.EventHandler(this.clearLogs_Click);
      // 
      // copyToClipboard
      // 
      this.copyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.copyToClipboard.Image = global::LSPtools.Properties.Resources.CopyHS;
      this.copyToClipboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.copyToClipboard.Location = new System.Drawing.Point(3, 6);
      this.copyToClipboard.Name = "copyToClipboard";
      this.copyToClipboard.Size = new System.Drawing.Size(144, 23);
      this.copyToClipboard.TabIndex = 2;
      this.copyToClipboard.Text = "Copy Logs to Clipboard";
      this.copyToClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip1.SetToolTip(this.copyToClipboard, "Copy All Content of Current tab to Clipboard");
      this.copyToClipboard.UseVisualStyleBackColor = true;
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.DefaultExt = "qpf";
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "Quartus Projects {*.qpf)|*.qpf|All Files (*.*)|*.*";
      this.openFileDialog1.Title = "Open Quartus Project";
      // 
      // timerUpdate
      // 
      this.timerUpdate.Interval = 1000;
      this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "engflag.png");
      this.imageList1.Images.SetKeyName(1, "czflag.png");
      this.imageList1.Images.SetKeyName(2, "QCtool16x16.bmp");
      this.imageList1.Images.SetKeyName(3, "Logs.bmp");
      this.imageList1.Images.SetKeyName(4, "VWF.bmp");
      // 
      // tabPageToDo
      // 
      this.tabPageToDo.BackColor = System.Drawing.Color.WhiteSmoke;
      this.tabPageToDo.Controls.Add(this.splitContainer1);
      this.tabPageToDo.ImageIndex = 2;
      this.tabPageToDo.Location = new System.Drawing.Point(4, 23);
      this.tabPageToDo.Name = "tabPageToDo";
      this.tabPageToDo.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageToDo.Size = new System.Drawing.Size(746, 311);
      this.tabPageToDo.TabIndex = 0;
      this.tabPageToDo.Text = "ToDo in Quartus Project";
      this.toolTip1.SetToolTip(this.tabPageToDo, "List of necessary repairs in the project");
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(3, 3);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.todoRTB);
      this.splitContainer1.Panel2.Controls.Add(this.panelToDoEngCz);
      this.splitContainer1.Panel2MinSize = 70;
      this.splitContainer1.Size = new System.Drawing.Size(740, 305);
      this.splitContainer1.SplitterDistance = 186;
      this.splitContainer1.TabIndex = 3;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.listViewReports);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.descriptionRTB);
      this.splitContainer2.Panel2.Controls.Add(this.panel3);
      this.splitContainer2.Size = new System.Drawing.Size(740, 186);
      this.splitContainer2.SplitterDistance = 316;
      this.splitContainer2.TabIndex = 0;
      // 
      // listViewReports
      // 
      this.listViewReports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderType,
            this.columnHeaderCode,
            this.columnHeaderDesc});
      this.listViewReports.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listViewReports.FullRowSelect = true;
      this.listViewReports.GridLines = true;
      this.listViewReports.HideSelection = false;
      this.listViewReports.HoverSelection = true;
      this.listViewReports.Location = new System.Drawing.Point(0, 0);
      this.listViewReports.MultiSelect = false;
      this.listViewReports.Name = "listViewReports";
      this.listViewReports.Size = new System.Drawing.Size(316, 186);
      this.listViewReports.TabIndex = 1;
      this.listViewReports.UseCompatibleStateImageBehavior = false;
      this.listViewReports.View = System.Windows.Forms.View.Details;
      this.listViewReports.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
      // 
      // columnHeaderType
      // 
      this.columnHeaderType.Text = "Type";
      this.columnHeaderType.Width = 50;
      // 
      // columnHeaderDesc
      // 
      this.columnHeaderDesc.Text = "Number of Descriptions";
      this.columnHeaderDesc.Width = 50;
      // 
      // descriptionRTB
      // 
      this.descriptionRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.descriptionRTB.Location = new System.Drawing.Point(0, 22);
      this.descriptionRTB.Name = "descriptionRTB";
      this.descriptionRTB.Size = new System.Drawing.Size(420, 164);
      this.descriptionRTB.TabIndex = 2;
      this.descriptionRTB.Text = "";
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.label1);
      this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel3.Location = new System.Drawing.Point(0, 0);
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size(420, 22);
      this.panel3.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(107, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Occurrences in Items";
      // 
      // todoRTB
      // 
      this.todoRTB.BackColor = System.Drawing.SystemColors.Info;
      this.todoRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.todoRTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.todoRTB.Location = new System.Drawing.Point(51, 0);
      this.todoRTB.MinimumSize = new System.Drawing.Size(290, 70);
      this.todoRTB.Name = "todoRTB";
      this.todoRTB.Size = new System.Drawing.Size(689, 115);
      this.todoRTB.TabIndex = 0;
      this.todoRTB.Text = "";
      // 
      // panelToDoEngCz
      // 
      this.panelToDoEngCz.Controls.Add(this.button2);
      this.panelToDoEngCz.Controls.Add(this.button1);
      this.panelToDoEngCz.Controls.Add(this.czRadioButton);
      this.panelToDoEngCz.Controls.Add(this.engRadioButton);
      this.panelToDoEngCz.Controls.Add(this.labelToDo);
      this.panelToDoEngCz.Dock = System.Windows.Forms.DockStyle.Left;
      this.panelToDoEngCz.Location = new System.Drawing.Point(0, 0);
      this.panelToDoEngCz.MinimumSize = new System.Drawing.Size(51, 70);
      this.panelToDoEngCz.Name = "panelToDoEngCz";
      this.panelToDoEngCz.Size = new System.Drawing.Size(51, 115);
      this.panelToDoEngCz.TabIndex = 1;
      this.panelToDoEngCz.Resize += new System.EventHandler(this.panelToDoEngCz_Resize);
      // 
      // button2
      // 
      this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.button2.ImageIndex = 1;
      this.button2.ImageList = this.imageList1;
      this.button2.Location = new System.Drawing.Point(22, 88);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(24, 24);
      this.button2.TabIndex = 1;
      this.button2.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.ImageIndex = 0;
      this.button1.ImageList = this.imageList1;
      this.button1.Location = new System.Drawing.Point(22, 2);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(24, 24);
      this.button1.TabIndex = 1;
      this.button1.UseVisualStyleBackColor = true;
      // 
      // czRadioButton
      // 
      this.czRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.czRadioButton.AutoSize = true;
      this.czRadioButton.ImageIndex = 0;
      this.czRadioButton.Location = new System.Drawing.Point(5, 93);
      this.czRadioButton.Name = "czRadioButton";
      this.czRadioButton.Size = new System.Drawing.Size(14, 13);
      this.czRadioButton.TabIndex = 0;
      this.czRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.czRadioButton.UseVisualStyleBackColor = true;
      this.czRadioButton.CheckedChanged += new System.EventHandler(this.czRadioButton_CheckedChanged);
      // 
      // engRadioButton
      // 
      this.engRadioButton.AutoSize = true;
      this.engRadioButton.Checked = true;
      this.engRadioButton.ImageIndex = 0;
      this.engRadioButton.Location = new System.Drawing.Point(5, 8);
      this.engRadioButton.Name = "engRadioButton";
      this.engRadioButton.Size = new System.Drawing.Size(14, 13);
      this.engRadioButton.TabIndex = 0;
      this.engRadioButton.TabStop = true;
      this.engRadioButton.UseVisualStyleBackColor = true;
      this.engRadioButton.CheckedChanged += new System.EventHandler(this.czRadioButton_CheckedChanged);
      // 
      // labelToDo
      // 
      this.labelToDo.AutoSize = true;
      this.labelToDo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelToDo.Location = new System.Drawing.Point(1, 29);
      this.labelToDo.Name = "labelToDo";
      this.labelToDo.Size = new System.Drawing.Size(46, 16);
      this.labelToDo.TabIndex = 2;
      this.labelToDo.Text = "ToDo";
      // 
      // tabPageLogs
      // 
      this.tabPageLogs.Controls.Add(this.reportsRTB);
      this.tabPageLogs.Controls.Add(this.panel2);
      this.tabPageLogs.ImageIndex = 3;
      this.tabPageLogs.Location = new System.Drawing.Point(4, 23);
      this.tabPageLogs.Name = "tabPageLogs";
      this.tabPageLogs.Size = new System.Drawing.Size(746, 311);
      this.tabPageLogs.TabIndex = 2;
      this.tabPageLogs.Text = "Log of Activities";
      this.toolTip1.SetToolTip(this.tabPageLogs, "Reports of perfomed check operations");
      this.tabPageLogs.UseVisualStyleBackColor = true;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPageToDo);
      this.tabControl1.Controls.Add(this.tabPageLogs);
      this.tabControl1.Controls.Add(this.tabPageVWF);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.ImageList = this.imageList1;
      this.tabControl1.Location = new System.Drawing.Point(0, 36);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(754, 338);
      this.tabControl1.TabIndex = 5;
      // 
      // tabPageVWF
      // 
      this.tabPageVWF.Controls.Add(this.label4);
      this.tabPageVWF.Controls.Add(this.vwfAdjustButton);
      this.tabPageVWF.Controls.Add(this.vwfRtb);
      this.tabPageVWF.Controls.Add(this.button3);
      this.tabPageVWF.Controls.Add(this.vwfOpenButton);
      this.tabPageVWF.Controls.Add(this.label6);
      this.tabPageVWF.Controls.Add(this.vwfQuartusProjectTextBox);
      this.tabPageVWF.Controls.Add(this.label5);
      this.tabPageVWF.Controls.Add(this.label3);
      this.tabPageVWF.Controls.Add(this.vwfTopLevelEntity);
      this.tabPageVWF.Controls.Add(this.vwfOpenedFilenameTextBox);
      this.tabPageVWF.ImageIndex = 4;
      this.tabPageVWF.Location = new System.Drawing.Point(4, 23);
      this.tabPageVWF.Name = "tabPageVWF";
      this.tabPageVWF.Size = new System.Drawing.Size(746, 311);
      this.tabPageVWF.TabIndex = 3;
      this.tabPageVWF.Text = "Adjust VWF";
      this.tabPageVWF.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(8, 9);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(633, 17);
      this.label4.TabIndex = 6;
      this.label4.Text = "Adjustement tool fixes simulation files *.vwf created in Quartus versions 18.0 to" +
    " 20.1.1";
      // 
      // vwfAdjustButton
      // 
      this.vwfAdjustButton.Enabled = false;
      this.vwfAdjustButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.vwfAdjustButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.vwfAdjustButton.ImageIndex = 4;
      this.vwfAdjustButton.ImageList = this.imageList1;
      this.vwfAdjustButton.Location = new System.Drawing.Point(11, 145);
      this.vwfAdjustButton.Name = "vwfAdjustButton";
      this.vwfAdjustButton.Size = new System.Drawing.Size(122, 23);
      this.vwfAdjustButton.TabIndex = 5;
      this.vwfAdjustButton.Text = "Overwrite VWF";
      this.vwfAdjustButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.vwfAdjustButton.UseVisualStyleBackColor = true;
      this.vwfAdjustButton.Click += new System.EventHandler(this.vwfAdjustButton_Click);
      // 
      // vwfRtb
      // 
      this.vwfRtb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vwfRtb.BackColor = System.Drawing.SystemColors.ControlLight;
      this.vwfRtb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.vwfRtb.Location = new System.Drawing.Point(3, 177);
      this.vwfRtb.Name = "vwfRtb";
      this.vwfRtb.ReadOnly = true;
      this.vwfRtb.Size = new System.Drawing.Size(740, 131);
      this.vwfRtb.TabIndex = 4;
      this.vwfRtb.Text = "";
      // 
      // button3
      // 
      this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.button3.Image = global::LSPtools.Properties.Resources.HelpHS;
      this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.button3.Location = new System.Drawing.Point(545, 144);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(193, 24);
      this.button3.TabIndex = 3;
      this.button3.Text = "Help: How to Change the Entity";
      this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Visible = false;
      // 
      // vwfOpenButton
      // 
      this.vwfOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.vwfOpenButton.Location = new System.Drawing.Point(716, 112);
      this.vwfOpenButton.Name = "vwfOpenButton";
      this.vwfOpenButton.Size = new System.Drawing.Size(24, 24);
      this.vwfOpenButton.TabIndex = 2;
      this.vwfOpenButton.Text = "...";
      this.vwfOpenButton.UseVisualStyleBackColor = true;
      this.vwfOpenButton.Click += new System.EventHandler(this.vwfAdjustToolStripMenuItem_Click);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(34, 38);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(99, 13);
      this.label6.TabIndex = 1;
      this.label6.Text = "Quartus Project ";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // vwfQuartusProjectTextBox
      // 
      this.vwfQuartusProjectTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vwfQuartusProjectTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.vwfQuartusProjectTextBox.Location = new System.Drawing.Point(143, 36);
      this.vwfQuartusProjectTextBox.Multiline = true;
      this.vwfQuartusProjectTextBox.Name = "vwfQuartusProjectTextBox";
      this.vwfQuartusProjectTextBox.ReadOnly = true;
      this.vwfQuartusProjectTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.vwfQuartusProjectTextBox.Size = new System.Drawing.Size(595, 70);
      this.vwfQuartusProjectTextBox.TabIndex = 0;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(135, 150);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(201, 13);
      this.label5.TabIndex = 1;
      this.label5.Text = " for the simulating Top-level Entity";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(8, 116);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(129, 13);
      this.label3.TabIndex = 1;
      this.label3.Text = "Vector Waveform File";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // vwfTopLevelEntity
      // 
      this.vwfTopLevelEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vwfTopLevelEntity.BackColor = System.Drawing.SystemColors.Info;
      this.vwfTopLevelEntity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.vwfTopLevelEntity.Location = new System.Drawing.Point(337, 146);
      this.vwfTopLevelEntity.Name = "vwfTopLevelEntity";
      this.vwfTopLevelEntity.ReadOnly = true;
      this.vwfTopLevelEntity.Size = new System.Drawing.Size(192, 22);
      this.vwfTopLevelEntity.TabIndex = 0;
      // 
      // vwfOpenedFilenameTextBox
      // 
      this.vwfOpenedFilenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vwfOpenedFilenameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.vwfOpenedFilenameTextBox.Location = new System.Drawing.Point(143, 113);
      this.vwfOpenedFilenameTextBox.Name = "vwfOpenedFilenameTextBox";
      this.vwfOpenedFilenameTextBox.Size = new System.Drawing.Size(566, 22);
      this.vwfOpenedFilenameTextBox.TabIndex = 0;
      // 
      // openFileDialog2
      // 
      this.openFileDialog2.DefaultExt = "qpf";
      this.openFileDialog2.FileName = "Open VWF Simulation File";
      this.openFileDialog2.Filter = "Quartus Projects {*.vwf)|*.vwf|All Files (*.*)|*.*";
      this.openFileDialog2.Title = "Open Quartus Project";
      // 
      // timerReload
      // 
      this.timerReload.Interval = 250;
      this.timerReload.Tick += new System.EventHandler(this.timerReload_Tick);
      // 
      // columnHeaderCode
      // 
      this.columnHeaderCode.Text = "Code";
      this.columnHeaderCode.Width = 150;
      // 
      // QCFormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(754, 396);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.MinimumSize = new System.Drawing.Size(770, 320);
      this.Name = "QCFormMain";
      this.Text = "Check Quartus Project and VWF Files";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QCFormMain_FormClosing);
      this.Shown += new System.EventHandler(this.QCFormMain_Shown);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.tabPageToDo.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.panel3.ResumeLayout(false);
      this.panel3.PerformLayout();
      this.panelToDoEngCz.ResumeLayout(false);
      this.panelToDoEngCz.PerformLayout();
      this.tabPageLogs.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPageVWF.ResumeLayout(false);
      this.tabPageVWF.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private MenuStrip menuStrip1;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel tsslMessage;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openQuartusProjectToolStripMenuItem;
    private ToolStripMenuItem recentToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStripMenuItem exitToolStripMenuItem;
    private Panel panel1;
    private RichTextBox reportsRTB;
    private Panel panel2;
    private OpenFileDialog openFileDialog1;
    private System.Windows.Forms.Timer timerUpdate;
    private ProgressBar progressBar1;
    private ImageList imageList1;
    private ToolTip toolTip1;
    private TabControl tabControl1;
    private TabPage tabPageLogs;
    private TabPage tabPageToDo;
    private RichTextBox todoRTB;
    private Button copyToClipboard;
    private Button clearLogs;
    private ListView listViewReports;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private RichTextBox descriptionRTB;
    private Panel panel3;
    private Panel panelToDoEngCz;
    private ColumnHeader columnHeaderType;
    private Label label1;
    private RadioButton engRadioButton;
    private RadioButton czRadioButton;
    private Label labelToDo;
    private Button button2;
    private Button button1;
    private ColumnHeader columnHeaderDesc;
    private ToolStripMenuItem vwfAdjustToolStripMenuItem;
    private ToolStripMenuItem vwfRecentToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private OpenFileDialog openFileDialog2;
    private TabPage tabPageVWF;
    private Button vwfOpenButton;
    private Label label3;
    private TextBox vwfOpenedFilenameTextBox;
    private TextBox vwfQuartusProjectTextBox;
    private Label label5;
    private Label label6;
    private Button button3;
    private TextBox vwfTopLevelEntity;
    private RichTextBox vwfRtb;
    private Button vwfAdjustButton;
    private System.Windows.Forms.Timer timerReload;
    private Label label4;
    private ColumnHeader columnHeaderCode;
  }
}