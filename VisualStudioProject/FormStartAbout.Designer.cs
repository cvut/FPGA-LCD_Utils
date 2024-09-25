namespace LSPtools
{
  partial class FormStartAbout
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStartAbout));
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.pictureBoxCVUT = new System.Windows.Forms.PictureBox();
      this.pictureBoxControl = new System.Windows.Forms.PictureBox();
      this.linkLabelSusta = new System.Windows.Forms.LinkLabel();
      this.closeButton = new System.Windows.Forms.Button();
      this.linkFPGA = new System.Windows.Forms.LinkLabel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label6 = new System.Windows.Forms.Label();
      this.linkEmail = new System.Windows.Forms.LinkLabel();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.labelVersion = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCVUT)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxControl)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.label2, 3);
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.label2.ForeColor = System.Drawing.Color.Blue;
      this.label2.Location = new System.Drawing.Point(4, 37);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(475, 37);
      this.label2.TabIndex = 8;
      this.label2.Text = "for FPGA Designs";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.label1.ForeColor = System.Drawing.Color.Blue;
      this.label1.Location = new System.Drawing.Point(4, 0);
      this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(475, 37);
      this.label1.TabIndex = 9;
      this.label1.Text = "LSP Course Tools";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.label3.Location = new System.Drawing.Point(1, 91);
      this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(164, 17);
      this.label3.TabIndex = 0;
      this.label3.Text = "Licence: GNU GPL 2024";
      // 
      // pictureBoxCVUT
      // 
      this.pictureBoxCVUT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBoxCVUT.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCVUT.Image")));
      this.pictureBoxCVUT.Location = new System.Drawing.Point(3, 139);
      this.pictureBoxCVUT.Name = "pictureBoxCVUT";
      this.pictureBoxCVUT.Size = new System.Drawing.Size(84, 64);
      this.pictureBoxCVUT.TabIndex = 11;
      this.pictureBoxCVUT.TabStop = false;
      this.pictureBoxCVUT.Click += new System.EventHandler(this.pictureBoxCVUT_Click);
      // 
      // pictureBoxControl
      // 
      this.pictureBoxControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBoxControl.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxControl.Image")));
      this.pictureBoxControl.Location = new System.Drawing.Point(396, 129);
      this.pictureBoxControl.Name = "pictureBoxControl";
      this.pictureBoxControl.Size = new System.Drawing.Size(84, 83);
      this.pictureBoxControl.TabIndex = 10;
      this.pictureBoxControl.TabStop = false;
      this.pictureBoxControl.Click += new System.EventHandler(this.pictureBoxControl_Click);
      // 
      // linkLabelSusta
      // 
      this.linkLabelSusta.AutoSize = true;
      this.linkLabelSusta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.linkLabelSusta.Location = new System.Drawing.Point(93, 45);
      this.linkLabelSusta.Name = "linkLabelSusta";
      this.linkLabelSusta.Size = new System.Drawing.Size(210, 17);
      this.linkLabelSusta.TabIndex = 3;
      this.linkLabelSusta.TabStop = true;
      this.linkLabelSusta.Text = "Richard Šusta (https://susta.cz/)";
      this.linkLabelSusta.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSusta_LinkClicked);
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.closeButton.Image = global::LSPtools.Properties.Resources._109_AllAnnotations_Default_16x16_72;
      this.closeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.closeButton.Location = new System.Drawing.Point(356, 80);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(121, 28);
      this.closeButton.TabIndex = 12;
      this.closeButton.Text = "&Close ";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // linkFPGA
      // 
      this.linkFPGA.AutoSize = true;
      this.linkFPGA.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.linkFPGA.Location = new System.Drawing.Point(93, 25);
      this.linkFPGA.Name = "linkFPGA";
      this.linkFPGA.Size = new System.Drawing.Size(225, 17);
      this.linkFPGA.TabIndex = 3;
      this.linkFPGA.TabStop = true;
      this.linkFPGA.Text = "https://dcenet.fel.cvut.cz/edu/fpga/";
      this.linkFPGA.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkFPGA_LinkClicked);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.closeButton);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.linkEmail);
      this.groupBox1.Controls.Add(this.linkLabelSusta);
      this.groupBox1.Controls.Add(this.linkFPGA);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(9, 237);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(483, 114);
      this.groupBox1.TabIndex = 14;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Contacts";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.label6.Location = new System.Drawing.Point(7, 26);
      this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(79, 16);
      this.label6.TabIndex = 0;
      this.label6.Text = "Homepage:";
      // 
      // linkEmail
      // 
      this.linkEmail.AutoSize = true;
      this.linkEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.linkEmail.Location = new System.Drawing.Point(93, 65);
      this.linkEmail.Name = "linkEmail";
      this.linkEmail.Size = new System.Drawing.Size(119, 17);
      this.linkEmail.TabIndex = 3;
      this.linkEmail.TabStop = true;
      this.linkEmail.Text = "susta@fel.cvut.cz";
      this.linkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEmail_LinkClicked);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tableLayoutPanel1.Controls.Add(this.labelVersion, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.pictureBoxCVUT, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.pictureBoxControl, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this.label4, 1, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 9);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(483, 228);
      this.tableLayoutPanel1.TabIndex = 15;
      // 
      // labelVersion
      // 
      this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.labelVersion.AutoSize = true;
      this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelVersion.ForeColor = System.Drawing.Color.Blue;
      this.labelVersion.Location = new System.Drawing.Point(93, 84);
      this.labelVersion.Name = "labelVersion";
      this.labelVersion.Size = new System.Drawing.Size(297, 20);
      this.labelVersion.TabIndex = 13;
      this.labelVersion.Text = "Version Alpha 1.4";
      this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(93, 114);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(297, 114);
      this.label4.TabIndex = 16;
      this.label4.Text = "Czech Technical University in Prague \r\nFaculty of Electrical Engineering\r\nDepartm" +
    "ent of Control Engineering - K13135\r\nTechnicka 2\r\n160 00, Prague 6, Czech Republ" +
    "ic ";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // FormStartAbout
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(501, 360);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Controls.Add(this.groupBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormStartAbout";
      this.Padding = new System.Windows.Forms.Padding(9);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "FormStartAbout";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCVUT)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxControl)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private Label label2;
    private Label label1;
    private Label label3;
    private PictureBox pictureBoxCVUT;
    private PictureBox pictureBoxControl;
    private LinkLabel linkLabelSusta;
    private Button closeButton;
    private LinkLabel linkFPGA;
    private GroupBox groupBox1;
    private Label label6;
    private TableLayoutPanel tableLayoutPanel1;
    private Label label4;
    private LinkLabel linkEmail;
    private Label labelVersion;
  }
}
