using FpgaLcdUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace TextAnalysis
{
  public partial class FormSplash : Form
  {
    Size origsize;
    Point origpos;


    public FormSplash()
    {
      InitializeComponent();
 //     timer1.Interval = 2000;
      timerAnim.Interval = 100;
 //     timer1.Enabled = true;
      timerAnim.Enabled = true;
      Version v = Assembly.GetExecutingAssembly().GetName().Version;
      labelVersion.Text = String.Format("{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.MajorRevision);
      origpos = pictureBox1.Location;
      origsize = pictureBox1.Size;
    }

   
    const float sizemax = 1f;
    const float sizemin = 0.5f;
    float coef = sizemin;
    const float step = 1.033f;
    int safetyCounter=20;
    private void timerAnim_Tick(object sender, EventArgs e)
    {

        coef *= step; safetyCounter--;
        if (coef >= sizemax || safetyCounter<=0)
        {
          Close();
          timerAnim.Enabled = false;
          timerAnim.Dispose();
        }
      

      int width = (int)(origsize.Width * coef);
      int height = (int)(origsize.Height * coef);
      Size size = new Size(width, height);
      pictureBox1.Size = size;
      Size sdif = origsize - size;
      pictureBox1.Location
          = new Point(origpos.X + sdif.Width / 2,
                      origpos.Y + sdif.Height / 2);
      pictureBox1.Visible = true;
      labelBeta.Visible = (safetyCounter & 0x2)!=0;
    }

  }
}
