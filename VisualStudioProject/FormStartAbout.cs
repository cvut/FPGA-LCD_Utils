﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FpgaLcdUtils
{
  partial class FormStartAbout : Form
  {
    public FormStartAbout()
    {
      InitializeComponent();
      this.Text = String.Format("About {0}", AssemblyTitle);
      Version v = Assembly.GetExecutingAssembly().GetName().Version;
      labelVersion.Text = String.Format("Version Beta {0}.{1}", v.Major, v.Minor, v.Build, v.MajorRevision);
    }

    #region Assembly Attribute Accessors

    public string AssemblyTitle
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0)
        {
          AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
          if (titleAttribute.Title != "")
          {
            return titleAttribute.Title;
          }
        }
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyVersion
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }

    public string AssemblyDescription
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (attributes.Length == 0)
        {
          return "";
        }
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    #endregion

    private void linkSusta_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("https://susta.cz/Default_en.aspx");
      }
      catch (Exception) { }
    }
    private void linkFPGA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("https://dcenet.fel.cvut.cz/edu/fpga/Default_en.aspx");
      }
      catch (Exception) { }
    }

    private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("mailto:susta@fel.cvut.cz");
      }
      catch (Exception) { }
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void pictureBoxCVUT_Click(object sender, EventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("https://fel.cvut.cz/en");
      }
      catch (Exception) { }

    }

    private void pictureBoxControl_Click(object sender, EventArgs e)
    {
      try
      {
        System.Diagnostics.Process.Start("https://control.fel.cvut.cz/en");
      }
      catch (Exception) { }
    }
  }
}
