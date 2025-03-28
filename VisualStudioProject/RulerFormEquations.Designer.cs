namespace FpgaLcdUtils
{
  partial class RulerFormEquations
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RulerFormEquations));
      this.panel1 = new System.Windows.Forms.Panel();
      this.pictureBox3 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.closeButton = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.tabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.pictureBox3);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.closeButton);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 461);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(659, 40);
      this.panel1.TabIndex = 0;
      // 
      // pictureBox3
      // 
      this.pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.pictureBox3.Image = global::FpgaLcdUtils.Properties.Resources.Information32x32;
      this.pictureBox3.Location = new System.Drawing.Point(4, 3);
      this.pictureBox3.Name = "pictureBox3";
      this.pictureBox3.Size = new System.Drawing.Size(32, 32);
      this.pictureBox3.TabIndex = 2;
      this.pictureBox3.TabStop = false;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(42, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(460, 20);
      this.label1.TabIndex = 1;
      this.label1.Text = "Hardware implementations require all coefficients to be integers.";
      // 
      // closeButton
      // 
      this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.closeButton.Image = global::FpgaLcdUtils.Properties.Resources.OK;
      this.closeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.closeButton.Location = new System.Drawing.Point(556, 2);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(96, 29);
      this.closeButton.TabIndex = 0;
      this.closeButton.Text = "Close";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(659, 461);
      this.tabControl1.TabIndex = 1;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.pictureBox1);
      this.tabPage1.Location = new System.Drawing.Point(4, 25);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(651, 432);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Line - Linear Equation";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.BackColor = System.Drawing.Color.White;
      this.pictureBox1.Image = global::FpgaLcdUtils.Properties.Resources.EquationOfLine;
      this.pictureBox1.Location = new System.Drawing.Point(3, 3);
      this.pictureBox1.MinimumSize = new System.Drawing.Size(640, 420);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(642, 422);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.pictureBox2);
      this.tabPage2.Location = new System.Drawing.Point(4, 25);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(651, 432);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Ellipse - Standard Equation";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // pictureBox2
      // 
      this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox2.BackColor = System.Drawing.Color.White;
      this.pictureBox2.Image = global::FpgaLcdUtils.Properties.Resources.EquationOfEllipse;
      this.pictureBox2.Location = new System.Drawing.Point(6, 7);
      this.pictureBox2.MinimumSize = new System.Drawing.Size(640, 420);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(642, 420);
      this.pictureBox2.TabIndex = 0;
      this.pictureBox2.TabStop = false;
      // 
      // RulerFormEquations
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(659, 501);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.panel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(675, 540);
      this.Name = "RulerFormEquations";
      this.Text = "Explanation of Equations";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RulerFormEquations_FormClosing);
      this.Shown += new System.EventHandler(this.RulerFormEquations_Shown);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.tabPage2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Panel panel1;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private PictureBox pictureBox1;
    private TabPage tabPage2;
    private Button closeButton;
    private PictureBox pictureBox2;
    private Label label1;
    private PictureBox pictureBox3;
  }
}