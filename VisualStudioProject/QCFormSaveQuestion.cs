﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FpgaLcdUtils
{
  public partial class QCFormSaveQuestion : Form
  {
    public QCFormSaveQuestion()
    {
      InitializeComponent();
      textBox1.SelectionLength = 0; 
      textBox1.SelectionStart = 0;
    }
  }
}
