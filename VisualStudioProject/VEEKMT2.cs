using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FpgaLcdUtils.QuartusProject;

namespace FpgaLcdUtils
{
  internal class VEEKMT2
  {
    private readonly SortedDictionary<string, QuartusProject.LAssignment> lass
         = new SortedDictionary<string, QuartusProject.LAssignment>();
    private readonly HashSet<string> lassRev = new HashSet<string>();
    // SortedDictionaly is faster for searching keys and they are already sorted.
    // From unsorted values, HashSet is created faster.
    private readonly SortedDictionary<string, string> gassCheck
         = new SortedDictionary<string, string>();

    private readonly StringBuilder reports = new StringBuilder();

    public VEEKMT2()
    {
      //    StringBuilder sb = new StringBuilder();
      for (int i = 0; i < lAssDefinitions.Length; i += 3)
      {
        QuartusProject.LAssignment la = new QuartusProject.LAssignment();
        la.loadFromArray(lAssDefinitions, i);
        if (lass.ContainsKey(la.SymbolicName)) { Debugger.Break(); continue; } // wrong table
        else
          lass.Add(la.SymbolicName, la);
        // some pins has more symbolic name aliases
        if (!lassRev.Contains(la.PinName)) lassRev.Add(la.PinName);
      }
      for (int i = 0; i < gassDefinitions.Length; i += 2)
      {
        gassCheck.Add(gassDefinitions[i], gassDefinitions[i + 1]);
      }
      //     string ss = sb.ToString();
    }

    public int CountLoc { get { return lass.Count; } }

    public string Report { get { return reports.ToString(); } }

    public void Clear()
    {
      foreach (KeyValuePair<string, QuartusProject.LAssignment> kv in lass)
      {
        kv.Value.Flag = 0;
      }
      reports.Clear();
    }
    public bool VerifyLoc(string symbolicName, string? pinName, string? iost)
    {
      reports.Clear();
      if (symbolicName == null || symbolicName.Length == 0)
      {
        if (pinName == null || pinName.Length == 0) return true; // nothing to check
        if (lassRev.Contains(pinName))
          reports.Append(String.Format("Incomplete assignment {0} to pin {1}", iost ?? String.Empty, pinName));
        else
          reports.Append(String.Format("Incomplete assignment {0} to UNKNOWN pin {1}",
            iost ?? String.Empty, pinName));
        return false;
      }
      LAssignment lAssignment;
      LAssignment.IO ioenum;
      if (lass.TryGetValue(symbolicName, out lAssignment) && lAssignment != null)
      {
        if (pinName != null && pinName.Length > 0)
        {
          if (lAssignment.PinName.Equals(pinName, StringComparison.InvariantCultureIgnoreCase))
            lAssignment.Flag |= LAssignment.FLAG_PIN;
          else
          {
            if (lassRev.Contains(pinName))
            {
              reports.Append(String.Format("Symbol {0} is not assigned to {1} pin, but to {2}",
                symbolicName, lAssignment.PinName, pinName));
            }
            else
            {
              reports.Append(String.Format("Symbol {0} is assigned to wrong {1} pin",
                symbolicName, pinName));
            }
          }

        }
        if (iost != null)
        {
          ioenum = LAssignment.GetIOstandard(iost);
          if (ioenum == lAssignment.IOstandard)
            lAssignment.Flag |= LAssignment.FLAG_IO;
          else
            reports.Append(String.Format(" Symbol {0} has incorrect IO standard {1}",
               symbolicName, iost));
        };
      }
      else
      {
        if (pinName != null && pinName.Length > 0)
        {
          if (lassRev.Contains(pinName))
          {
            reports.Append(String.Format("Found new alias assignment {0} to pin {1}!", symbolicName, pinName));
          }
          else
          {
            reports.Append(String.Format(" Wrong assignment {0} to unknown pin {1}!", symbolicName, pinName));
          }
        }
        else
        {
          reports.Append(String.Format("Unknown pin assignment {0} : {1} : {2}",
            symbolicName ?? "", pinName ?? "", iost ?? ""));
        }
      }
      return reports.Length == 0;
    }

    internal LAErrorsStatistic GetResultsOfVerifications(out string pinList)
    {
      LAErrorsStatistic ae = new LAErrorsStatistic();
      StringBuilder sbTop = new StringBuilder();
      StringBuilder sbSec = new StringBuilder();
      StringBuilder sbX = new StringBuilder();
      foreach (KeyValuePair<string, LAssignment> kv in lass)
      {
        LAssignment la = kv.Value;
        int pin = 0, ios = 0;

        if ((la.Flag & LAssignment.FLAG_PIN) == 0)
        { pin = 1; ae.CountPinErrors++; }
        if ((la.Flag & LAssignment.FLAG_IO) == 0)
        { ios = 1; ae.CountIosErrors++; }
        if (pin + ios == 0) { ae.CountOK++; continue; }
        else ae.CountErrors++;
        switch (la.Order)
        {
          case LAssignment.OTOP:
            ae.Top += pin; ae.TopIO += ios;
            sbTop.AppendFormat("{0}, ", kv.Key); break;
          case LAssignment.OSECOND:
            ae.Second += pin; ae.SecondIO += ios;
            sbSec.AppendFormat("{0}, ", kv.Key); break;
          default:
            ae.X += pin; ae.XIO += pin;
            sbX.AppendFormat("{0}, ", kv.Key);
            break;
        }
      }
      StringBuilder sb = new StringBuilder();
      if (ae.CountPinErrors > 0)
      {
        if (sbTop.Length > 0)
        {
          sb.Append("Essential PINs: "); sb.AppendLine(sbTop.ToString());
        }
        if (sbSec.Length > 0)
        {
          sb.Append("Useful PINs: "); sb.AppendLine(sbSec.ToString());
        }
        if (sbX.Length > 0)
        {
          sb.Append("Optional PINs: "); sb.AppendLine(sbX.ToString());
        }
        pinList = sb.ToString();
      }
      else pinList = String.Empty;

      return ae;
    }
    /// <summary>
    /// Search dictionary of Global definitions 
    /// that was created from array VEEKMT2.gassDefinitions
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Expected value name of the parameter</returns>
    internal string? GetGlob(string key)
    {
      string glob;
      if (gassCheck.TryGetValue(key, out glob)) { return glob; }
      else return null;
    }

    public struct LAErrorsStatistic
    {
      public int CountOK = 0;
      public int CountErrors = 0;
      public int CountPinErrors = 0;
      public int CountIosErrors = 0;
      public int Top = 0;
      public int Second = 0;
      public int X = 0;
      public int TopIO = 0;
      public int SecondIO = 0;
      public int XIO = 0;
      public LAErrorsStatistic() { }
    }


    #region Text arrays with definitions
    private readonly string[] lAssDefinitions ={
"AUD_ADCDAT","PIN_D2","3T","AUD_ADCLRCK","PIN_C2","3T","AUD_BCLK","PIN_F2","3T",
"AUD_DACDAT","PIN_D1","3T","AUD_DACLRCK","PIN_E3","3T","AUD_XCK","PIN_E1","3T",
"CAMERA_I2C_SCL","PIN_R21","2S","CAMERA_I2C_SDA","PIN_F27","2S","CAMERA_PWDN_n","PIN_R22","2S",
"CLOCK_50","PIN_Y2","3T","CLOCK2_50","PIN_AG14","3T","CLOCK3_50","PIN_AG15","3T",
"DRAM_ADDR[0]","PIN_R6","3X","DRAM_ADDR[1]","PIN_V8","3X","DRAM_ADDR[10]","PIN_R5","3X",
"DRAM_ADDR[11]","PIN_AA5","3X","DRAM_ADDR[12]","PIN_Y7","3X","DRAM_ADDR[2]","PIN_U8","3X",
"DRAM_ADDR[3]","PIN_P1","3X","DRAM_ADDR[4]","PIN_V5","3X","DRAM_ADDR[5]","PIN_W8","3X",
"DRAM_ADDR[6]","PIN_W7","3X","DRAM_ADDR[7]","PIN_AA7","3X","DRAM_ADDR[8]","PIN_Y5","3X",
"DRAM_ADDR[9]","PIN_Y6","3X","DRAM_BA[0]","PIN_U7","3X","DRAM_BA[1]","PIN_R4","3X",
"DRAM_CAS_N","PIN_V7","3X","DRAM_CKE","PIN_AA6","3X","DRAM_CLK","PIN_AE5","3X",
"DRAM_CS_N","PIN_T4","3X","DRAM_DQ[0]","PIN_W3","3X","DRAM_DQ[1]","PIN_W2","3X",
"DRAM_DQ[10]","PIN_AB1","3X","DRAM_DQ[11]","PIN_AA3","3X","DRAM_DQ[12]","PIN_AB2","3X",
"DRAM_DQ[13]","PIN_AC1","3X","DRAM_DQ[14]","PIN_AB3","3X","DRAM_DQ[15]","PIN_AC2","3X",
"DRAM_DQ[16]","PIN_M8","3X","DRAM_DQ[17]","PIN_L8","3X","DRAM_DQ[18]","PIN_P2","3X",
"DRAM_DQ[19]","PIN_N3","3X","DRAM_DQ[2]","PIN_V4","3X","DRAM_DQ[20]","PIN_N4","3X",
"DRAM_DQ[21]","PIN_M4","3X","DRAM_DQ[22]","PIN_M7","3X","DRAM_DQ[23]","PIN_L7","3X",
"DRAM_DQ[24]","PIN_U5","3X","DRAM_DQ[25]","PIN_R7","3X","DRAM_DQ[26]","PIN_R1","3X",
"DRAM_DQ[27]","PIN_R2","3X","DRAM_DQ[28]","PIN_R3","3X","DRAM_DQ[29]","PIN_T3","3X",
"DRAM_DQ[3]","PIN_W1","3X","DRAM_DQ[30]","PIN_U4","3X","DRAM_DQ[31]","PIN_U1","3X",
"DRAM_DQ[4]","PIN_V3","3X","DRAM_DQ[5]","PIN_V2","3X","DRAM_DQ[6]","PIN_V1","3X",
"DRAM_DQ[7]","PIN_U3","3X","DRAM_DQ[8]","PIN_Y3","3X","DRAM_DQ[9]","PIN_Y4","3X",
"DRAM_DQM[0]","PIN_U2","3X","DRAM_DQM[1]","PIN_W4","3X","DRAM_DQM[2]","PIN_K8","3X",
"DRAM_DQM[3]","PIN_N8","3X","DRAM_RAS_N","PIN_U6","3X","DRAM_WE_N","PIN_V6","3X",
"EEP_I2C_SCLK","PIN_D14","3X","EEP_I2C_SDAT","PIN_E14","3X","ENET0_GTX_CLK","PIN_A17","2X",
"ENET0_INT_N","PIN_A21","2X","ENET0_LINK100","PIN_C14","3X","ENET0_MDC","PIN_C20","2X",
"ENET0_MDIO","PIN_B21","2X","ENET0_RST_N","PIN_C19","2X","ENET0_RX_CLK","PIN_A15","2X",
"ENET0_RX_COL","PIN_E15","2X","ENET0_RX_CRS","PIN_D15","2X","ENET0_RX_DATA[0]","PIN_C16","2X",
"ENET0_RX_DATA[1]","PIN_D16","2X","ENET0_RX_DATA[2]","PIN_D17","2X","ENET0_RX_DATA[3]","PIN_C15","2X",
"ENET0_RX_DV","PIN_C17","2X","ENET0_RX_ER","PIN_D18","2X","ENET0_TX_CLK","PIN_B17","2X",
"ENET0_TX_DATA[0]","PIN_C18","2X","ENET0_TX_DATA[1]","PIN_D19","2X","ENET0_TX_DATA[2]","PIN_A19","2X",
"ENET0_TX_DATA[3]","PIN_B19","2X","ENET0_TX_EN","PIN_A18","2X","ENET0_TX_ER","PIN_B18","2X",
"ENET1_GTX_CLK","PIN_C23","2X","ENET1_INT_N","PIN_D24","2X","ENET1_LINK100","PIN_D13","3X",
"ENET1_MDC","PIN_D23","2X","ENET1_MDIO","PIN_D25","2X","ENET1_RST_N","PIN_D22","2X",
"ENET1_RX_CLK","PIN_B15","2X","ENET1_RX_COL","PIN_B22","2X","ENET1_RX_CRS","PIN_D20","2X",
"ENET1_RX_DATA[0]","PIN_B23","2X","ENET1_RX_DATA[1]","PIN_C21","2X","ENET1_RX_DATA[2]","PIN_A23","2X",
"ENET1_RX_DATA[3]","PIN_D21","2X","ENET1_RX_DV","PIN_A22","2X","ENET1_RX_ER","PIN_C24","2X",
"ENET1_TX_CLK","PIN_C22","2X","ENET1_TX_DATA[0]","PIN_C25","2X","ENET1_TX_DATA[1]","PIN_A26","2X",
"ENET1_TX_DATA[2]","PIN_B26","2X","ENET1_TX_DATA[3]","PIN_C26","2X","ENET1_TX_EN","PIN_B25","2X",
"ENET1_TX_ER","PIN_A25","2X","ENETCLK_25","PIN_A14","3X","EX_IO[0]","PIN_J10","3X",
"EX_IO[1]","PIN_J14","3X","EX_IO[2]","PIN_H13","3X","EX_IO[3]","PIN_H14","3X",
"EX_IO[4]","PIN_F14","3X","EX_IO[5]","PIN_E10","3X","EX_IO[6]","PIN_D9","3X",
"FL_ADDR[0]","PIN_AG12","3X","FL_ADDR[1]","PIN_AH7","3X","FL_ADDR[10]","PIN_AE9","3X",
"FL_ADDR[11]","PIN_AF9","3X","FL_ADDR[12]","PIN_AA10","3X","FL_ADDR[13]","PIN_AD8","3X",
"FL_ADDR[14]","PIN_AC8","3X","FL_ADDR[15]","PIN_Y10","3X","FL_ADDR[16]","PIN_AA8","3X",
"FL_ADDR[17]","PIN_AH12","3X","FL_ADDR[18]","PIN_AC12","3X","FL_ADDR[19]","PIN_AD12","3X",
"FL_ADDR[2]","PIN_Y13","3X","FL_ADDR[20]","PIN_AE10","3X","FL_ADDR[21]","PIN_AD10","3X",
"FL_ADDR[22]","PIN_AD11","3X","FL_ADDR[3]","PIN_Y14","3X","FL_ADDR[4]","PIN_Y12","3X",
"FL_ADDR[5]","PIN_AA13","3X","FL_ADDR[6]","PIN_AA12","3X","FL_ADDR[7]","PIN_AB13","3X",
"FL_ADDR[8]","PIN_AB12","3X","FL_ADDR[9]","PIN_AB10","3X","FL_CE_N","PIN_AG7","3X",
"FL_DQ[0]","PIN_AH8","3X","FL_DQ[1]","PIN_AF10","3X","FL_DQ[2]","PIN_AG10","3X",
"FL_DQ[3]","PIN_AH10","3X","FL_DQ[4]","PIN_AF11","3X","FL_DQ[5]","PIN_AG11","3X",
"FL_DQ[6]","PIN_AH11","3X","FL_DQ[7]","PIN_AF12","3X","FL_OE_N","PIN_AG8","3X",
"FL_RST_N","PIN_AE11","3X","FL_RY","PIN_Y1","3X","FL_WE_N","PIN_AC10","3X",
"FL_WP_N","PIN_AE12","3X","GPIO[0]","PIN_AB22","3X","GPIO[1]","PIN_AC15","3X",
"GPIO[10]","PIN_AC19","3X","GPIO[11]","PIN_AF16","3X","GPIO[12]","PIN_AD19","3X",
"GPIO[13]","PIN_AF15","3X","GPIO[14]","PIN_AF24","3X","GPIO[15]","PIN_AE21","3X",
"GPIO[16]","PIN_AF25","3X","GPIO[17]","PIN_AC22","3X","GPIO[18]","PIN_AE22","3X",
"GPIO[19]","PIN_AF21","3X","GPIO[2]","PIN_AB21","3X","GPIO[20]","PIN_AF22","3X",
"GPIO[21]","PIN_AD22","3X","GPIO[22]","PIN_AG25","3X","GPIO[23]","PIN_AD25","3X",
"GPIO[24]","PIN_AH25","3X","GPIO[25]","PIN_AE25","3X","GPIO[26]","PIN_AG22","3X",
"GPIO[27]","PIN_AE24","3X","GPIO[28]","PIN_AH22","3X","GPIO[29]","PIN_AF26","3X",
"GPIO[3]","PIN_Y17","3X","GPIO[30]","PIN_AE20","3X","GPIO[31]","PIN_AG23","3X",
"GPIO[32]","PIN_AF20","3X","GPIO[33]","PIN_AH26","3X","GPIO[34]","PIN_AH23","3X",
"GPIO[35]","PIN_AG26","3X","GPIO[4]","PIN_AC21","3X","GPIO[5]","PIN_Y16","3X",
"GPIO[6]","PIN_AD21","3X","GPIO[7]","PIN_AE16","3X","GPIO[8]","PIN_AD15","3X",
"GPIO[9]","PIN_AE15","3X","HEX0[0]","PIN_G18","2T","HEX0[1]","PIN_F22","2T",
"HEX0[2]","PIN_E17","2T","HEX0[3]","PIN_L26","2T","HEX0[4]","PIN_L25","2T",
"HEX0[5]","PIN_J22","2T","HEX0[6]","PIN_H22","2T","HEX1[0]","PIN_M24","2T",
"HEX1[1]","PIN_Y22","2T","HEX1[2]","PIN_W21","2T","HEX1[3]","PIN_W22","2T",
"HEX1[4]","PIN_W25","2T","HEX1[5]","PIN_U23","2T","HEX1[6]","PIN_U24","2T",
"HEX2[0]","PIN_AA25","2T","HEX2[1]","PIN_AA26","2T","HEX2[2]","PIN_Y25","2T",
"HEX2[3]","PIN_W26","2T","HEX2[4]","PIN_Y26","2T","HEX2[5]","PIN_W27","2T",
"HEX2[6]","PIN_W28","2T","HEX3[0]","PIN_V21","2T","HEX3[1]","PIN_U21","2T",
"HEX3[2]","PIN_AB20","3T","HEX3[3]","PIN_AA21","3T","HEX3[4]","PIN_AD24","3T",
"HEX3[5]","PIN_AF23","3T","HEX3[6]","PIN_Y19","3T","HEX4[0]","PIN_AB19","3T",
"HEX4[1]","PIN_AA19","3T","HEX4[2]","PIN_AG21","3T","HEX4[3]","PIN_AH21","3T",
"HEX4[4]","PIN_AE19","3T","HEX4[5]","PIN_AF19","3T","HEX4[6]","PIN_AE18","3T",
"HEX5[0]","PIN_AD18","3T","HEX5[1]","PIN_AC18","3T","HEX5[2]","PIN_AB18","3T",
"HEX5[3]","PIN_AH19","3T","HEX5[4]","PIN_AG19","3T","HEX5[5]","PIN_AF18","3T",
"HEX5[6]","PIN_AH18","3T","HEX6[0]","PIN_AA17","3T","HEX6[1]","PIN_AB16","3T",
"HEX6[2]","PIN_AA16","3T","HEX6[3]","PIN_AB17","3T","HEX6[4]","PIN_AB15","3T",
"HEX6[5]","PIN_AA15","3T","HEX6[6]","PIN_AC17","3T","HEX7[0]","PIN_AD17","3T",
"HEX7[1]","PIN_AE17","3T","HEX7[2]","PIN_AG17","3T","HEX7[3]","PIN_AH17","3T",
"HEX7[4]","PIN_AF17","3T","HEX7[5]","PIN_AG18","3T","HEX7[6]","PIN_AA14","3T",
"HSMC_CLKIN_N1","PIN_J28","LX","HSMC_CLKIN_N2","PIN_Y28","LX","HSMC_CLKIN_P1","PIN_J27","LX",
"HSMC_CLKIN_P2","PIN_Y27","LX","HSMC_CLKIN0","PIN_AH15","UX","HSMC_CLKOUT_N1","PIN_G24","LX",
"HSMC_CLKOUT_N2","PIN_V24","LX","HSMC_CLKOUT_P1","PIN_G23","LX","HSMC_CLKOUT_P2","PIN_V23","LX",
"HSMC_CLKOUT0","PIN_AD28","2X","HSMC_D[0]","PIN_AE26","2X","HSMC_D[1]","PIN_AE28","2X",
"HSMC_D[2]","PIN_AE27","2X","HSMC_D[3]","PIN_AF27","2X","HSMC_RX_D_N[0]","PIN_F25","LX",
"HSMC_RX_D_N[1]","PIN_C27","LX","HSMC_RX_D_N[10]","PIN_U26","LX","HSMC_RX_D_N[11]","PIN_L22","LX",
"HSMC_RX_D_N[12]","PIN_N26","LX","HSMC_RX_D_N[13]","PIN_P26","LX","HSMC_RX_D_N[14]","PIN_R21","LX",
"HSMC_RX_D_N[15]","PIN_R23","LX","HSMC_RX_D_N[16]","PIN_T22","LX","HSMC_RX_D_N[2]","PIN_E26","LX",
"HSMC_RX_D_N[3]","PIN_G26","LX","HSMC_RX_D_N[4]","PIN_H26","LX","HSMC_RX_D_N[5]","PIN_K26","LX",
"HSMC_RX_D_N[6]","PIN_L24","LX","HSMC_RX_D_N[7]","PIN_M26","LX","HSMC_RX_D_N[8]","PIN_R26","LX",
"HSMC_RX_D_N[9]","PIN_T26","LX","HSMC_RX_D_P[0]","PIN_F24","LX","HSMC_RX_D_P[1]","PIN_D26","LX",
"HSMC_RX_D_P[10]","PIN_U25","LX","HSMC_RX_D_P[11]","PIN_L21","LX","HSMC_RX_D_P[12]","PIN_N25","LX",
"HSMC_RX_D_P[13]","PIN_P25","LX","HSMC_RX_D_P[14]","PIN_P21","LX","HSMC_RX_D_P[15]","PIN_R22","LX",
"HSMC_RX_D_P[16]","PIN_T21","LX","HSMC_RX_D_P[2]","PIN_F26","LX","HSMC_RX_D_P[3]","PIN_G25","LX",
"HSMC_RX_D_P[4]","PIN_H25","LX","HSMC_RX_D_P[5]","PIN_K25","LX","HSMC_RX_D_P[6]","PIN_L23","LX",
"HSMC_RX_D_P[7]","PIN_M25","LX","HSMC_RX_D_P[8]","PIN_R25","LX","HSMC_RX_D_P[9]","PIN_T25","LX",
"HSMC_TX_D_N[0]","PIN_D28","LX","HSMC_TX_D_N[1]","PIN_E28","LX","HSMC_TX_D_N[10]","PIN_J26","LX",
"HSMC_TX_D_N[11]","PIN_L28","LX","HSMC_TX_D_N[12]","PIN_V26","LX","HSMC_TX_D_N[13]","PIN_R28","LX",
"HSMC_TX_D_N[14]","PIN_U28","LX","HSMC_TX_D_N[15]","PIN_V28","LX","HSMC_TX_D_N[16]","PIN_V22","LX",
"HSMC_TX_D_N[2]","PIN_F28","LX","HSMC_TX_D_N[3]","PIN_G28","LX","HSMC_TX_D_N[4]","PIN_K28","LX",
"HSMC_TX_D_N[5]","PIN_M28","LX","HSMC_TX_D_N[6]","PIN_K22","LX","HSMC_TX_D_N[7]","PIN_H24","LX",
"HSMC_TX_D_N[8]","PIN_J24","LX","HSMC_TX_D_N[9]","PIN_P28","LX","HSMC_TX_D_P[0]","PIN_D27","LX",
"HSMC_TX_D_P[1]","PIN_E27","LX","HSMC_TX_D_P[10]","PIN_J25","LX","HSMC_TX_D_P[11]","PIN_L27","LX",
"HSMC_TX_D_P[12]","PIN_V25","LX","HSMC_TX_D_P[13]","PIN_R27","LX","HSMC_TX_D_P[14]","PIN_U27","LX",
"HSMC_TX_D_P[15]","PIN_V27","LX","HSMC_TX_D_P[16]","PIN_U22","LX","HSMC_TX_D_P[2]","PIN_F27","LX",
"HSMC_TX_D_P[3]","PIN_G27","LX","HSMC_TX_D_P[4]","PIN_K27","LX","HSMC_TX_D_P[5]","PIN_M27","LX",
"HSMC_TX_D_P[6]","PIN_K21","LX","HSMC_TX_D_P[7]","PIN_H23","LX","HSMC_TX_D_P[8]","PIN_J23","LX",
"HSMC_TX_D_P[9]","PIN_P27","LX","I2C_SCLK","PIN_B7","3T","I2C_SDAT","PIN_A8","3T",
"IRDA_RXD","PIN_Y15","3T","KEY[0]","PIN_M23","2T","KEY[1]","PIN_M21","2T",
"KEY[2]","PIN_N21","2T","KEY[3]","PIN_R24","2T","LCD_B[0]","PIN_P28","2T",
"LCD_B[1]","PIN_P27","2T","LCD_B[2]","PIN_J24","2T","LCD_B[3]","PIN_J23","2T",
"LCD_B[4]","PIN_T26","2T","LCD_B[5]","PIN_T25","2T","LCD_B[6]","PIN_R26","2T",
"LCD_B[7]","PIN_R25","2T","LCD_BLON","PIN_L6","3T","LCD_DATA[0]","PIN_L3","3T",
"LCD_DATA[1]","PIN_L1","3T","LCD_DATA[2]","PIN_L2","3T","LCD_DATA[3]","PIN_K7","3T",
"LCD_DATA[4]","PIN_K1","3T","LCD_DATA[5]","PIN_K2","3T","LCD_DATA[6]","PIN_M3","3T",
"LCD_DATA[7]","PIN_M5","3T","LCD_DCLK","PIN_V24","2T","LCD_DE","PIN_H23","2T",
"LCD_DIM","PIN_P21","2T","LCD_DITH","PIN_L23","2T","LCD_EN","PIN_L4","3T",
"LCD_G[0]","PIN_P26","2T","LCD_G[1]","PIN_P25","2T","LCD_G[2]","PIN_N26","2T",
"LCD_G[3]","PIN_N25","2T","LCD_G[4]","PIN_L22","2T","LCD_G[5]","PIN_L21","2T",
"LCD_G[6]","PIN_U26","2T","LCD_G[7]","PIN_U25","2T","LCD_HSD","PIN_U22","2T",
"LCD_MODE","PIN_L24","2T","LCD_ON","PIN_L5","3T","LCD_POWER_CTL","PIN_M28","2T",
"LCD_R[0]","PIN_V28","2T","LCD_R[1]","PIN_V27","2T","LCD_R[2]","PIN_U28","2T",
"LCD_R[3]","PIN_U27","2T","LCD_R[4]","PIN_R28","2T","LCD_R[5]","PIN_R27","2T",
"LCD_R[6]","PIN_V26","2T","LCD_R[7]","PIN_V25","2T","LCD_RS","PIN_M2","3T",
"LCD_RSTB","PIN_K22","2T","LCD_RW","PIN_M1","3T","LCD_SHLR","PIN_H24","2T",
"LCD_UPDN","PIN_K21","2T","LCD_VSD","PIN_V22","2T","LEDG[0]","PIN_E21","2T",
"LEDG[1]","PIN_E22","2T","LEDG[2]","PIN_E25","2T","LEDG[3]","PIN_E24","2T",
"LEDG[4]","PIN_H21","2T","LEDG[5]","PIN_G20","2T","LEDG[6]","PIN_G22","2T",
"LEDG[7]","PIN_G21","2T","LEDG[8]","PIN_F17","2T","LEDR[0]","PIN_G19","2T",
"LEDR[1]","PIN_F19","2T","LEDR[10]","PIN_J15","2T","LEDR[11]","PIN_H16","2T",
"LEDR[12]","PIN_J16","2T","LEDR[13]","PIN_H17","2T","LEDR[14]","PIN_F15","2T",
"LEDR[15]","PIN_G15","2T","LEDR[16]","PIN_G16","2T","LEDR[17]","PIN_H15","2T",
"LEDR[2]","PIN_E19","2T","LEDR[3]","PIN_F21","2T","LEDR[4]","PIN_F18","2T",
"LEDR[5]","PIN_E18","2T","LEDR[6]","PIN_J19","2T","LEDR[7]","PIN_H19","2T",
"LEDR[8]","PIN_J17","2T","LEDR[9]","PIN_G17","2T","LSENSOR_ADDR_SEL","PIN_J25","2S",
"LSENSOR_INT","PIN_L28","2S","LSENSOR_SCL","PIN_J26","2S","LSENSOR_SDA","PIN_L27","2S",
"MIPI_CS_n","PIN_D28","2S","MIPI_I2C_SCL","PIN_AE26","2S","MIPI_I2C_SDA","PIN_AE27","2S",
"MIPI_MCLK","PIN_G24","2S","MIPI_PIXEL_CLK","PIN_J27","2S","MIPI_PIXEL_D[0]","PIN_F24","2S",
"MIPI_PIXEL_D[1]","PIN_F25","2S","MIPI_PIXEL_D[10]","PIN_M26","2S","MIPI_PIXEL_D[11]","PIN_M25","2S",
"MIPI_PIXEL_D[12]","PIN_AF27","2S","MIPI_PIXEL_D[13]","PIN_AE28","2S","MIPI_PIXEL_D[2]","PIN_D26","2S",
"MIPI_PIXEL_D[3]","PIN_C27","2S","MIPI_PIXEL_D[4]","PIN_F26","2S","MIPI_PIXEL_D[5]","PIN_E26","2S",
"MIPI_PIXEL_D[6]","PIN_G25","2S","MIPI_PIXEL_D[7]","PIN_G26","2S","MIPI_PIXEL_D[8]","PIN_H25","2S",
"MIPI_PIXEL_D[9]","PIN_H26","2S","MIPI_PIXEL_HS","PIN_K26","2S","MIPI_PIXEL_VS","PIN_K25","2S",
"MIPI_REFCLK","PIN_G23","2S","MIPI_RESET_n","PIN_D27","2S","MPU_AD0_SDO","PIN_K27","2S",
"MPU_CS_n","PIN_F28","2S","MPU_FSYNC","PIN_G28","2S","MPU_INT","PIN_G27","2S",
"MPU_SCL_SCLK","PIN_M27","2S","MPU_SDA_SDI","PIN_K28","2S","OTG_ADDR[0]","PIN_H7","3X",
"OTG_ADDR[1]","PIN_C3","3X","OTG_CS_N","PIN_A3","3X","OTG_DACK_N[0]","PIN_C4","3X",
"OTG_DACK_N[1]","PIN_D4","3X","OTG_DATA[0]","PIN_J6","3X","OTG_DATA[1]","PIN_K4","3X",
"OTG_DATA[10]","PIN_G1","3X","OTG_DATA[11]","PIN_G2","3X","OTG_DATA[12]","PIN_G3","3X",
"OTG_DATA[13]","PIN_F1","3X","OTG_DATA[14]","PIN_F3","3X","OTG_DATA[15]","PIN_G4","3X",
"OTG_DATA[2]","PIN_J5","3X","OTG_DATA[3]","PIN_K3","3X","OTG_DATA[4]","PIN_J4","3X",
"OTG_DATA[5]","PIN_J3","3X","OTG_DATA[6]","PIN_J7","3X","OTG_DATA[7]","PIN_H6","3X",
"OTG_DATA[8]","PIN_H3","3X","OTG_DATA[9]","PIN_H4","3X","OTG_DREQ[0]","PIN_J1","3X",
"OTG_DREQ[1]","PIN_B4","3X","OTG_FSPEED","PIN_C6","3X","OTG_INT","PIN_D5","3X",
"OTG_INT[0]","PIN_A6","3X","OTG_INT[1]","PIN_D5","3X","OTG_LSPEED","PIN_B6","3X",
"OTG_RD_N","PIN_B3","3X","OTG_RST_N","PIN_C5","3X","OTG_WE_N","PIN_A4","3X",
"OTG_WR_N","PIN_A4","3X","PS2_CLK","PIN_G6","3S","PS2_CLK2","PIN_G5","3S",
"PS2_DAT","PIN_H5","3S","PS2_DAT2","PIN_F5","3S","SD_CLK","PIN_AE13","3X",
"SD_CMD","PIN_AD14","3X","SD_DAT[0]","PIN_AE14","3X","SD_DAT[1]","PIN_AF13","3X",
"SD_DAT[2]","PIN_AB14","3X","SD_DAT[3]","PIN_AC14","3X","SD_WP_N","PIN_AF14","3X",
"SMA_CLKIN","PIN_AH14","3X","SMA_CLKOUT","PIN_AE23","3X","SRAM_ADDR[0]","PIN_AB7","3X",
"SRAM_ADDR[1]","PIN_AD7","3X","SRAM_ADDR[10]","PIN_AF2","3X","SRAM_ADDR[11]","PIN_AD3","3X",
"SRAM_ADDR[12]","PIN_AB4","3X","SRAM_ADDR[13]","PIN_AC3","3X","SRAM_ADDR[14]","PIN_AA4","3X",
"SRAM_ADDR[15]","PIN_AB11","3X","SRAM_ADDR[16]","PIN_AC11","3X","SRAM_ADDR[17]","PIN_AB9","3X",
"SRAM_ADDR[18]","PIN_AB8","3X","SRAM_ADDR[19]","PIN_T8","3X","SRAM_ADDR[2]","PIN_AE7","3X",
"SRAM_ADDR[3]","PIN_AC7","3X","SRAM_ADDR[4]","PIN_AB6","3X","SRAM_ADDR[5]","PIN_AE6","3X",
"SRAM_ADDR[6]","PIN_AB5","3X","SRAM_ADDR[7]","PIN_AC5","3X","SRAM_ADDR[8]","PIN_AF5","3X",
"SRAM_ADDR[9]","PIN_T7","3X","SRAM_CE_N","PIN_AF8","3X","SRAM_DQ[0]","PIN_AH3","3X",
"SRAM_DQ[1]","PIN_AF4","3X","SRAM_DQ[10]","PIN_AE2","3X","SRAM_DQ[11]","PIN_AE1","3X",
"SRAM_DQ[12]","PIN_AE3","3X","SRAM_DQ[13]","PIN_AE4","3X","SRAM_DQ[14]","PIN_AF3","3X",
"SRAM_DQ[15]","PIN_AG3","3X","SRAM_DQ[2]","PIN_AG4","3X","SRAM_DQ[3]","PIN_AH4","3X",
"SRAM_DQ[4]","PIN_AF6","3X","SRAM_DQ[5]","PIN_AG6","3X","SRAM_DQ[6]","PIN_AH6","3X",
"SRAM_DQ[7]","PIN_AF7","3X","SRAM_DQ[8]","PIN_AD1","3X","SRAM_DQ[9]","PIN_AD2","3X",
"SRAM_LB_N","PIN_AD4","3X","SRAM_OE_N","PIN_AD5","3X","SRAM_UB_N","PIN_AC4","3X",
"SRAM_WE_N","PIN_AE8","3X","SW[0]","PIN_AB28","2T","SW[1]","PIN_AC28","2T",
"SW[10]","PIN_AC24","2T","SW[11]","PIN_AB24","2T","SW[12]","PIN_AB23","2T",
"SW[13]","PIN_AA24","2T","SW[14]","PIN_AA23","2T","SW[15]","PIN_AA22","2T",
"SW[16]","PIN_Y24","2T","SW[17]","PIN_Y23","2T","SW[2]","PIN_AC27","2T",
"SW[3]","PIN_AD27","2T","SW[4]","PIN_AB27","2T","SW[5]","PIN_AC26","2T",
"SW[6]","PIN_AD26","2T","SW[7]","PIN_AB26","2T","SW[8]","PIN_AC25","2T",
"SW[9]","PIN_AB25","2T","TD_CLK27","PIN_B14","3X","TD_DATA[0]","PIN_E8","3X",
"TD_DATA[1]","PIN_A7","3X","TD_DATA[2]","PIN_D8","3X","TD_DATA[3]","PIN_C7","3X",
"TD_DATA[4]","PIN_D7","3X","TD_DATA[5]","PIN_D6","3X","TD_DATA[6]","PIN_E7","3X",
"TD_DATA[7]","PIN_F7","3X","TD_HS","PIN_E5","3X","TD_RESET_N","PIN_G7","3X",
"TD_VS","PIN_E4","3X","TOUCH_I2C_SCL","PIN_T22","2T","TOUCH_I2C_SDA","PIN_T21","2T",
"TOUCH_INT_n","PIN_R23","2T","UART_CTS","PIN_J13","3S","UART_RTS","PIN_G14","3S",
"UART_RXD","PIN_G12","3S","UART_TXD","PIN_G9","3S","VGA_B[0]","PIN_B10","3S",
"VGA_B[1]","PIN_A10","3S","VGA_B[2]","PIN_C11","3S","VGA_B[3]","PIN_B11","3S",
"VGA_B[4]","PIN_A11","3S","VGA_B[5]","PIN_C12","3S","VGA_B[6]","PIN_D11","3S",
"VGA_B[7]","PIN_D12","3S","VGA_BLANK_N","PIN_F11","3S","VGA_CLK","PIN_A12","3S",
"VGA_G[0]","PIN_G8","3S","VGA_G[1]","PIN_G11","3S","VGA_G[2]","PIN_F8","3S",
"VGA_G[3]","PIN_H12","3S","VGA_G[4]","PIN_C8","3S","VGA_G[5]","PIN_B8","3S",
"VGA_G[6]","PIN_F10","3S","VGA_G[7]","PIN_C9","3S","VGA_HS","PIN_G13","3S",
"VGA_R[0]","PIN_E12","3S","VGA_R[1]","PIN_E11","3S","VGA_R[2]","PIN_D10","3S",
"VGA_R[3]","PIN_F12","3S","VGA_R[4]","PIN_G10","3S","VGA_R[5]","PIN_J12","3S",
"VGA_R[6]","PIN_H8","3S","VGA_R[7]","PIN_H10","3S","VGA_SYNC_N","PIN_C10","3S",
"VGA_VS","PIN_C13","3S"};

    /// <summary>
    /// Fixed structure of tested pairs: KEY, VALUE
    /// </summary>
    private readonly string[] gassDefinitions = {
"BDF_FILE","*",
"CDF_FILE","*",
//"CRC_ERROR_OPEN_DRAIN","OFF",
"CYCLONEII_RESERVE_NCEO_AFTER_CONFIGURATION","USE AS REGULAR IO",
"DEVICE","EP4CE115F29C7",
"DEVICE_FILTER_PACKAGE","FBGA",
"DEVICE_FILTER_PIN_COUNT","780",
"DEVICE_FILTER_SPEED_GRADE","7",
"EDA_DESIGN_ENTRY_SYNTHESIS_TOOL","Design Compiler",
"EDA_GENERATE_FUNCTIONAL_NETLIST","ON",
"EDA_INPUT_DATA_FORMAT","VHDL",
//"EDA_INPUT_VCC_NAME","VDD",
"EDA_NETLIST_WRITER_OUTPUT_DIR","?",
"EDA_OUTPUT_DATA_FORMAT","VERILOG HDL",
"EDA_SIMULATION_TOOL","$",
//"EDA_TIME_SCALE","1 ps",
//"ERROR_CHECK_FREQUENCY_DIVISOR","1",
"FAMILY","Cyclone IV E",
"FLOW_DISABLE_ASSEMBLER","OFF",
"INCREMENTAL_VECTOR_INPUT_SOURCE","?",
"LAST_QUARTUS_VERSION","$",
//"MAX_CORE_JUNCTION_TEMP","85",
//"MIN_CORE_JUNCTION_TEMP","0",
"ORIGINAL_QUARTUS_VERSION","$",
//"OUTPUT_IO_TIMING_FAR_END_VMEAS","*",
//"OUTPUT_IO_TIMING_NEAR_END_VMEAS","*",
//"PARTITION_COLOR","*",
//"PARTITION_FITTER_PRESERVATION_LEVEL","*",
//"PARTITION_NETLIST_TYPE","*",
//"POWER_BOARD_THERMAL_MODEL","NONE (CONSERVATIVE)",
//"POWER_PRESET_COOLING_SOLUTION","23 MM HEAT SINK WITH 200 LFPM AIRFLOW",
//"PROJECT_CREATION_TIME_DATE","12:37:12  OCTOBER 20, 2023",
"PROJECT_OUTPUT_DIRECTORY","?",
"SDC_FILE","*",
"SEARCH_PATH","?",
"SIMULATION_MODE","FUNCTIONAL",
      //"STRATIX_DEVICE_IO_STANDARD","2.5 V",
"SMART_RECOMPILE","ON",
"TEXT_FILE","*",
"TIMING_ANALYZER_MULTICORNER_ANALYSIS","ON",
"TOP_LEVEL_ENTITY","?",
"USE_CONFIGURATION_DEVICE","OFF",
"VECTOR_WAVEFORM_FILE","*",
"VERILOG_FILE","*",
"VHDL_FILE","*",
"VHDL_INPUT_VERSION","VHDL_2008"
//"VHDL_SHOW_LMF_MAPPING_MESSAGES","OFF",
    };
    #endregion // Text arrays with definitions
  }
}
