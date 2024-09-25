namespace LSPtools
{
  partial class FormStartException
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStartException));
      this.noteTextBox = new System.Windows.Forms.TextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.linkLabel1 = new System.Windows.Forms.LinkLabel();
      this.copyToClipboard = new System.Windows.Forms.Button();
      this.closeButton = new System.Windows.Forms.Button();
      this.confirmationTextBox = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // noteTextBox
      // 
      this.noteTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.noteTextBox.Location = new System.Drawing.Point(0, 0);
      this.noteTextBox.Multiline = true;
      this.noteTextBox.Name = "noteTextBox";
      this.noteTextBox.ReadOnly = true;
      this.noteTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.noteTextBox.Size = new System.Drawing.Size(502, 393);
      this.noteTextBox.TabIndex = 3;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.confirmationTextBox);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.linkLabel1);
      this.panel1.Controls.Add(this.copyToClipboard);
      this.panel1.Controls.Add(this.closeButton);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 393);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(502, 57);
      this.panel1.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 35);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(250, 16);
      this.label1.TabIndex = 3;
      this.label1.Text = "We appreciate sending the text above to ";
      // 
      // linkLabel1
      // 
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.linkLabel1.Location = new System.Drawing.Point(259, 35);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new System.Drawing.Size(110, 16);
      this.linkLabel1.TabIndex = 2;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "susta@fel.cvut.cz";
      this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      // 
      // copyToClipboard
      // 
      this.copyToClipboard.Image = global::LSPtools.Properties.Resources.CopyHS;
      this.copyToClipboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.copyToClipboard.Location = new System.Drawing.Point(3, 3);
      this.copyToClipboard.Name = "copyToClipboard";
      this.copyToClipboard.Size = new System.Drawing.Size(122, 23);
      this.copyToClipboard.TabIndex = 1;
      this.copyToClipboard.Text = "Copy to Clipboard";
      this.copyToClipboard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.copyToClipboard.UseVisualStyleBackColor = true;
      this.copyToClipboard.Click += new System.EventHandler(this.copyToClipboard_Click);
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.closeButton.Image = global::LSPtools.Properties.Resources._109_AllAnnotations_Default_16x16_72;
      this.closeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.closeButton.Location = new System.Drawing.Point(420, 3);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(75, 25);
      this.closeButton.TabIndex = 0;
      this.closeButton.Text = "Close";
      this.closeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // confirmationTextBox
      // 
      this.confirmationTextBox.Location = new System.Drawing.Point(129, 5);
      this.confirmationTextBox.Name = "confirmationTextBox";
      this.confirmationTextBox.ReadOnly = true;
      this.confirmationTextBox.Size = new System.Drawing.Size(290, 20);
      this.confirmationTextBox.TabIndex = 4;
      // 
      // FormStartException
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(502, 450);
      this.Controls.Add(this.noteTextBox);
      this.Controls.Add(this.panel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormStartException";
      this.Text = "Bug Report in LSPtools";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private TextBox noteTextBox;
    private Panel panel1;
    private Button copyToClipboard;
    private Button closeButton;
    private Label label1;
    private LinkLabel linkLabel1;
    private TextBox confirmationTextBox;
  }
}