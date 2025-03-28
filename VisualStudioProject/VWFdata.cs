using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;

namespace FpgaLcdUtils
{
  internal class VWFdata
  {
    private readonly string filenameOfVWF = String.Empty;
    private string convertedVWF = String.Empty;

    public VWFdata(string filenameOfVWF) { this.filenameOfVWF = filenameOfVWF; }

    public string CreateForQuartusProject(QuartusProject quartusProject)
    {
      try
      {
        string lines = String.Empty;
        // we open in share mode
        using (FileStream fileStream = new FileStream(filenameOfVWF, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          StreamReader sr = new StreamReader(fileStream);
          lines = sr.ReadToEnd();
        }
        const string XMLEND = "</simulation_settings>";
        int ix1 = lines.IndexOf("<simulation_settings>");
        int ix2 = lines.IndexOf(XMLEND);
        if (ix2 > 0)
        {
          ix2 += XMLEND.Length;
          while (ix2 < lines.Length - 32)
          {
            if (lines[ix2] == '*' && lines[ix2 + 1] == '/')
            { ix2 += 2; break; }
          }
        }
        if (ix1 < 0 || ix2 < 0 || ix2 <= ix1 || ix2 > lines.Length - 32)
          return "VWF file was not created in Quartus version 18 or later.";
        lines = lines.Substring(ix2);

        string Dpar = QuartusProject.ToUnix(quartusProject.RootDir);
        string Ppar = Path.GetFileNameWithoutExtension(quartusProject.Filename);
        string Vpar = Path.GetFileNameWithoutExtension(filenameOfVWF);
        string Rpar = quartusProject.ActiveRevisionName;
        string Tpar = quartusProject.GetTopLevelEntity();
        string Spar = QuartusProject.ToUnix(quartusProject.GetSimulationDir());
        if (string.IsNullOrEmpty(Spar))
        {
          Spar = "simulation\\qsim";
          string sfull = Path.Combine(quartusProject.RootDir,Spar);
          if (!Directory.Exists(sfull)) 
          {
            return $"No simulation directory. Try first to compile {Tpar} top-level entity to create {sfull}.";
          }
          Spar = QuartusProject.ToUnix(Spar);
        }

        // Read prototype and modify lines
        int ix = 0; int len = _VWFprototype.Length;
        StringBuilder sb = new StringBuilder();
        while (ix < len)
        {
          string sc = _VWFprototype[ix++];
          if (sc[0] != '!') { sb.AppendLine(sc); continue; }
          string sf = _VWFprototype[ix++];
          List<object> par = new List<object>();
          for (int i = 1; i < sc.Length; i++)
          {
            switch (sc[i])
            {
              case 'D': par.Add(Dpar); break;
              case 'P': par.Add(Ppar); break;
              case 'V': par.Add(Vpar); break;
              case 'R': par.Add(Rpar); break;
              case 'T': par.Add(Tpar); break;
              case 'S': par.Add(Spar); break;
            }
          }
          sb.AppendFormat(sf, par.ToArray()); sb.AppendLine();
        }
        sb.AppendLine(lines);
        this.convertedVWF = sb.ToString();
        return String.Empty;

      }
      catch (Exception ex) { return ex.Message; }
    }

    internal string WriteVWF()
    {
      try
      {
        string bak1 = this.filenameOfVWF + ".bak";
        IniData.CreateBackups(new string[] { this.filenameOfVWF, bak1 }, false); // copy is more robust
      }
      catch(Exception ex)
        { Trace.WriteLine(ex.ToString()); }
      try
      { 
        File.WriteAllText(this.filenameOfVWF, this.convertedVWF);
        return "Overwritten VWF file: " + this.filenameOfVWF;
      }
      catch(Exception ex)
      { Trace.WriteLine(ex.ToString()); 
        return ex.Message + "\n";
      }
    }

    static string[] _VWFprototype = {
@"/*<simulation_settings>",  // parameter without starting ending / , backslashes are converted to /
      "!PRDVS", // P-ProjectName, R-Revision Name, D-Directory of Project, V-Vector WaveForm (no ext.), S-simulation directory
@"<ftestbench_cmd>quartus_eda --gen_testbench --tool=modelsim_oem --format=verilog --write_settings_files=off {0} -c {1} --vector_source=""{2}/{3}.vwf"" --testbench_file=""{2}/{4}/{3}.vwf.vt""</ftestbench_cmd>",
      "!PRDVS",
@"<ttestbench_cmd>quartus_eda --gen_testbench --tool=modelsim_oem --format=verilog --write_settings_files=off {0} -c {1} --vector_source=""{2}/{3}.vwf"" --testbench_file=""{2}/{4}/{3}.vwf.vt""</ttestbench_cmd>",
      "!DSPR",
@"<fnetlist_cmd>quartus_eda --write_settings_files=off --simulation --functional=on --flatten_buses=off --tool=modelsim_oem --format=verilog --output_directory=""{0}/{1}/"" {2} -c {3}</fnetlist_cmd>",
      "!DSPR",
@"<tnetlist_cmd>quartus_eda --write_settings_files=off --simulation --functional=off --flatten_buses=off --timescale=1ps --tool=modelsim_oem --format=verilog --output_directory=""{0}/{1}/"" {2} -c {3}</tnetlist_cmd>",
@"<modelsim_script>onerror {exit -code 1}",
@"vlib work",
"!R", @"vlog -work work {0}.vo",
"!DSV",@"vlog -work work {0}/{1}/{2}.vwf.vt",
"!T",@"vsim -c -t 1ps -L cycloneive_ver -L altera_ver -L altera_mf_ver -L 220model_ver -L sgate_ver -L altera_lnsim_ver work.{0}_vlg_vec_tst",
"!P",@"vcd file -direction {0}.msim.vcd",
"!T", @"vcd add -internal {0}_vlg_vec_tst/*",
"!T",@"vcd add -internal {0}_vlg_vec_tst/i1/*",
@"proc simTimestamp {} {",
@"    echo ""Simulation time: $::now ps""",
@"    if { [string equal running [runStatus]] } {",
@"        after 2500 simTimestamp",
@"    }",
@"}",
@"after 2500 simTimestamp",
@"run -all",
@"quit -f",
@"</modelsim_script>",
@"<modelsim_script_timing>onerror {exit -code 1}",
@"vlib work",
"!R",@"vlog -work work {0}.vo",
"!DSV",@"vlog -work work {0}/{1}/{2}.vwf.vt",
"!T",@"vsim -c -t 1ps -L cycloneive_ver -L altera_ver -L altera_mf_ver -L 220model_ver -L sgate_ver -L altera_lnsim_ver work.{0}_vlg_vec_tst",
"!P",@"vcd file -direction {0}.msim.vcd",
"!T",@"vcd add -internal {0}_vlg_vec_tst/*",
"!T",@"vcd add -internal {0}_vlg_vec_tst/i1/*",
@"proc simTimestamp {} {",
@"    echo ""Simulation time: $::now ps""",
@"    if { [string equal running [runStatus]] } {",
@"        after 2500 simTimestamp",
@"    }",
@"}",
@"after 2500 simTimestamp",
@"run -all",
@"quit -f",
@"</modelsim_script_timing>",
@"<hdl_lang>verilog</hdl_lang>",
@"</simulation_settings>*/" };
  }
}
