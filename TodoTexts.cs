using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace LSPtools
{
  public static class TodoTexts
  {
    public enum TT
    {
      NONE,
      WRONG_DEVICE,
      IMPORT_ASSIGNMENTS,
      WRONG_FILENAME,
      WRONG_PATH,
      CHANGE_REVISION,
      VHDL2008,
      nCEO,
      FLOW_DISABLE_ASSEMBLER,
      FILE_OUTSIDE,
      FILE_NOTFOUND,
      SDC_EXT,
      MISSING_SDC,
      SIMULATION_MODELSIM,
      SIMULATION_QUESTA,
      SIMULATION_DIR,
      SIMULATION_DIRQUESTA,
      SMART_RECOMPILE_OFF,
      WARNING_DIVIDE, WARNING_LOOP, WARNING_LATCH, WARNING_SENSITIVITY,
      NOREPORTS,
    }
    public static string GetCategory(TT tt)
    {
      switch (tt)
      {
        case TT.SIMULATION_DIR:
        case TT.SIMULATION_DIRQUESTA:
        case TT.SMART_RECOMPILE_OFF:
        case TT.VHDL2008:
          return "Warning";
        case TT.NOREPORTS:
          return "Info";
        case TT.WARNING_DIVIDE:
        case TT.WARNING_LATCH:
        case TT.WARNING_SENSITIVITY:
        case TT.WARNING_LOOP:
          return "Disallowed";
        default:
          return "Error";
      }
    }

    public static string Get(TT tt, out string cz)
    {
      cz = String.Empty;
      string? eng = String.Empty;
      switch (tt)
      {
        default:
        case TT.NONE: return String.Empty;
        case TT.WRONG_DEVICE:
#pragma warning disable VSSpell001
          cz =
@"V projektu máte jiný FPGA obvod, než obsahuje deska VEEK-MT2.
Zvolte z Quartus menu: Assignments->Device...
V poli Family vyberte Cyclone IV E a do Name Filter vložte EP4CE115F29C7
Stejný obvod zvolte myší v Available Devices a zavřete dialog [OK].";
          eng =
@"In your project you have a different FPGA circuit than the VEEK-MT2 board.
Select from Quartus menu: Assignments->Device
In the Family field, select Cyclone IV E  and enter EP4CE115F29C7 in the Name Filter.
In Available Devices, select the same circuit with the mouse and close the dialog [OK].";
          break;
        case TT.IMPORT_ASSIGNMENTS:
          cz =
@"Názvy pinů neodpovídají desce VEEK-MT2. 
Zvolte z Quartus menu: Assignments->Import Assignments
Najděte soubor VeekMT2_PinAssignments.csv s datem říjen 2023 a novějším. Správný si lze stáhnout i ze stránky
https://dcenet.fel.cvut.cz/edu/fpga/veek-mt2/
Dejte [Open] a poté v Import Assignments dialogu volte [Advanced...] a překontrolujte, že obě dolní Import Options jsou zaškrtnuté. 
Zavřete Advanced a Import potvrďte [OK]. Quartus vypíše do okna Messages:
1202 assignments were written... 
";
          eng =
@"The pin names do not match the VEEK-MT2 board.
Select from the Quartus menu: Assignments->Import Assignments
Locate the VeekMT2_PinAssignments.csv file with a date of October 2023 or later. The correct one can also be downloaded from the
https://dcenet.fel.cvut.cz/edu/fpga/veek-mt2/
Put [Open] and then in the Import Assignments dialog put [Advanced...] and double check that both lower Import Options are checked. Close Advanced and confirm the Import with [OK].Quartus will print to the Messages window
1202 assignments were written... ";
          break;
        case TT.WRONG_FILENAME:
          cz =
@"Jméno souboru obsahuje znak, který není podporovaný Linuxem. Překontrolujte, zda neukládáte na síťový disk a nemáte v cestě $. 
Nesmíte v názvech ani používat diakritiku a mezery. Doporučují se jen písmena a číslice, případně podtržítko.";
          eng =
@"The filename contains a character that is not supported by Linux. Check that you are not saving to a network drive and have $ in the path. 
You must not use diacritics and spaces in filenames either. Only letters and numbers, or an underscore, are recommended.";
          break;
        case TT.WRONG_PATH:
          cz =
@"Cesta k projektu Quartusu obsahuje nepovolené znaky. Překontrolujte, zda neukládáte na síťový disk a nemáte v cestě $. Nesmíte také používat diakritiku a mezery. 
Doporučují se jen písmena a číslice, případně podtržítko.";
          eng =
@"The path to the Quartus project contains illegal characters. Check that you are not saving to a network drive and have $ in the path. 
You must also not use diacritics and spaces. Only letters and numbers, or an underscore, are recommended.";
          break;
        case TT.CHANGE_REVISION:
          cz =
@"Soubor projektu odkazuje na revizi, která neexistuje. Pravděpodobně bude celý projekt poškozený. Doporučujeme založit nový projekt.";
          eng =
@"The project file references a revision that does not exist. The entire project is likely to be corrupted. It is recommended to create a new one.";
          break;
        case TT.VHDL2008:
          cz =
@"Nastavte překladač na VHDL2008 v Quartus menu: Assignments->Setting... 
Category: Analysis & Synthesis Settings->VHDL Input - VHDL2008";
          eng =
@"Set the compiler to VHDL2008 in Quartus menu: Assignments->Setting... 
Analysis & Synthesis Settings->VHDL Input - VHDL2008";
          break;
        case TT.nCEO:
          cz =
@"Není nakonfigurovaný pin nCEO (Chip Enable Output) na běžný výstup. Deska VEEK-MT2 ho přiřadila zadnímu LCD displeji, neboť nevyužívá nCEO k zřetězení FPGA obvodů. Obsahuje jen jeden.
Zvolte z Quartus menu: Assignments->Device. Překontrolujte napřed, že máte obvod EP4CE115F29C7.
Stiskněte [Device and Pin Options...] tlačítko a v Category volte Dual-Purpose Pins.
U pinu nCEO nastavte ""Use as regular I/O"". Zavřete oba dialogy tlačítky [OK].";
          eng =
@"The nCEO (Chip Enable Output) pin is not configured for normal output. The VEEK-MT2 board assigned it to the rear LCD, since it does not need nCEO to concatenate the FPGA circuits. It contains only one.
Select from the Quartus menu: Assignments->Device. Make sure you have the EP4CE115F29C7 circuit first.
Press the [Device and Pin Options...] button and select Dual-Purpose Pins in Category.
For the nCEO pin, set ""Use as regular I/O"". Close both dialog boxes by [OK].";
          break;
        case TT.FILE_OUTSIDE:
          cz =
@"Soubor se nachází mimo adresář projektu. Uložte ho File->Save As do projektu. 
V okně Project Navigator odstraníte původní soubor ze seznamu klávesou Delete. 
Neruší se jí ale soubor na disku, a tak ten musíte smazat v průzkumníku Windows.";
          eng =
@"The file is located outside the project directory. Save it File->Save As to the project. 
In the Project Navigator window, use the Delete key to remove the original file from the list. 
However, it does not delete the file on disk, so you must delete it in Windows Explorer.";
          break;
        case TT.FILE_NOTFOUND:
          cz =
@"Nepodařilo se nalézt soubor uvedený v seznamu projektu. Buď neexistuje nebo není přístupný. 
Zrušte klávesou Delete na záložce Files v okně Project Navigator.";
          eng =
 @"The file listed in the project list could not be found. It either does not exist or is not accessible. 
Delete it with the Delete key on the Files tab of the Project Navigator window.";
          break;

        case TT.FLOW_DISABLE_ASSEMBLER:
          cz =
 @"Máte zakázaný assembler, takže se nevytvoří soubor *.sof pro programátor desky.
Nastavte v Quartus menu: Assignments->Setting... Category: Compilation Process Settings, v níž zaškrtněte
Run Assembler during compilation";
          eng =
@"You have disabled the assembler, so it will not create a *.sof file for the board programmer.
Set in Quartus menu: Assignments->Setting... 
In category Compilation Process Settings Analysis, check Run Assembler during compilation";
          break;


        case TT.MISSING_SDC:
          cz =
@"Nenalezený SDC. Synchronní obvody nebudou bez něho správně fungovat!
Volte z Quartus menu: Assignments->Setting... Category: TimeQuest Timing Analyzer.
Přidejte *.SDC soubory. Najdete je také v Zipu s pin Assignment na stránce
https://dcenet.fel.cvut.cz/edu/fpga/veek-mt2/";
          eng =
 @"SDC was not found and without it, the synchronous circuits do not work properly!
Select from the Quartus menu: Assignments->Setting... Category: TimeQuest Timing Analyzer.
AddNoRepeat *.SDC files. These are also in the Zip with the Assignment pin on the page
https://dcenet.fel.cvut.cz/edu/fpga/veek-mt2/";
          break;
        case TT.SDC_EXT:
          cz =
@"Nalezené SDC soubory s jinou příponou než .sdc. Přejmenujte ji a opravte v SDC seznamu:
Quartus menu: Assignments->Setting... Category: TimeQuest Timing Analyzer.";
          eng =
 @"Found SDC file with extension other than .sdc. Rename it and fix filename in the SDC list:
Quartus menu: Assignments->Setting... Category: TimeQuest Timing Analyzer.";
          break;
        case TT.SIMULATION_MODELSIM:
          cz =
@"Pro Vaši verzi Quartusu nastavte nástroj pro interní simulaci na " + QuartusProject.MODELSIM + @"
V Quartus menu: Assignments->Setting... v Category: EDA Tool Settings
Ze seznamu Tool name: ModelSim-Altera;  Format(s): Verilog HDL
Interní Quartus simulator, který máme, pracuje jen ve Verilogu. Volba nemá vliv na vnější nástroje.
Podívejte se ještě v podkategorii Category: EDA Tool Settings->Simulation,
zda máte nastavený i Output directory: " + QuartusProject.SIMDIR + @" 
Pokud neexistuje, vytvoří se při překladu.";
          eng =
@"For your Quartus version, set the internal simulation tool to " + QuartusProject.MODELSIM + @" 
in Quartus menu: Assignments->Setting... 
In Category: EDA Tool Settings chose Tool: ModelSim-Altera; Format(s): Verilog HDL
The internal Quartus simulator we have only works in Verilog. The option does not affect external tools.
See also the subcategory Category: EDA Tool Settings->Simulation,
if you have the Output directory set up to the subdirectory: " + QuartusProject.SIMDIR + @"
If it does not exist, it will be created during compilation.";
          break;
        case TT.SIMULATION_QUESTA:
          cz =
@"Pro Vaši verzi Quartus nastavte nástroj pro interní simulaci na " + QuartusProject.QUESTA + @"
v Quartus menu: Assignments->Setting... 
V Category: EDA Tool Settings ze seznamu Tool name: Questa-Intel FPGA;  Format(s): Verilog HDL
Interní Quartus simulator, který máme, pracuje jen ve Verilogu. Volba nemá vliv na vnější nástroje.
Podívejte se ještě v podkategorii Category: EDA Tool Settings->Simulation,
zda máte nastavený i Output directory: " + QuartusProject.SIMDIRQUESTA + @" 
Pokud neexistuje, vytvoří se při překladu.";
          eng =
@"For your Quartus version, set the internal simulation tool to " + QuartusProject.QUESTA + @" in 
Quartus menu: Assignments->Setting... 
In Category: EDA Tool Settings chose Tool: Questa-Intel FPGA;  Format(s): Verilog HDL
The internal Quartus simulator we have only works in Verilog. The option does not affect external tools.
See also the subcategory Category: EDA Tool Settings->Simulation,
if you have the Output directory set up to the subdirectory: " + QuartusProject.SIMDIRQUESTA + @"
If it does not exist, it will be created during compilation.";
          break;
        case TT.SIMULATION_DIR:
          cz =
 @"Nalezena podezřelá hodnota adresáře interní simulace.
Zkontrolujte z Quartus menu: 
Assignments->Setting... Category: EDA Tool Settings->Simulation
Ověřte, že máte Format for output netlist = Verilog HDL 
(Interní Quartus simulátor pracuje jen ve Verilogu.)
Adresář by měl pro Vaši verzi Quartus být: " + QuartusProject.SIMDIR + @"
Pokud neexistuje, vytvoří se při překladu.";
          eng =
 @"Found suspicious internal simulation. Check Quartus menu: 
Assignments->Setting... Category: EDA Tool Settings->Simulation
Make sure you have Format for output netlist = Verilog HDL 
(The internal Quartus simulator only works in Verilog.)
For your Quartus version, specify the directory in Output directory: " + QuartusProject.SIMDIR + @"
If it does not exist, it will be created during compilation.";
          break;
        case TT.SIMULATION_DIRQUESTA:
          cz =
 @"Nalezena podezřelá hodnota adresáře interní simulace. 
Zkontrolujte z Quartus menu: 
Assignments->Setting... Category: EDA Tool Settings->Simulation
Ověřte, že máte Format for output netlist = Verilog HDL 
(Interní Quartus simulátor pracuje jen ve Verilogu.)
Adresář by měl pro Vaši verzi Quartus být: " + QuartusProject.SIMDIRQUESTA + @"
Pokud neexistuje, vytvoří se při překladu.";
          eng =
 @"Found suspicious internal simulation. Check Quartus menu: 
Assignments->Setting... Category: EDA Tool Settings->Simulation
Make sure you have Format for output netlist = Verilog HDL 
(The internal Quartus simulator only works in Verilog.)
For your Quartus version, specify the directory in Output directory: " + QuartusProject.SIMDIRQUESTA + @"
If it does not exist, it will be created during compilation.";
          break;
        case TT.SMART_RECOMPILE_OFF:
          cz =
@"Pokud nastavíte Smart Compilation na On, zrychlíte opakované překlady.
Zvolte z Quartus menu: Assignments->Setting...  
Na záložce Compilation Process Settings a zaškrtněnte Use smart compilation";
          eng =
@"Setting Smart Compilation to On will speed up repeated translations.
From Quartus menu, select Assignments->Setting...  
In Compilation Process Settings tab, check Use smart compilation";
          break;

        case TT.WARNING_DIVIDE:
          cz =
@"Váš návrh vytvořil nepovolený obvod dělení.
Pokud potřebujete víc informací, než vidíte zde,
hledejte 'lpm_divide' ve zprávách překladače Quartus.";
           eng =
@"The code created an disallowed division circuit.
If you need more information than you see here, 
search for 'lpm_divide' in Quartus compiler messages.";
          break;
        
        case TT.WARNING_LOOP:
          cz =
@"Váš návrh vytvořil nedovolenou obvodovou smyčku.
Pokud potřebujete víc informací, než vidíte zde, 
hledejte 'combinational loop' ve zprávách překladače Quartus.";
          eng =
@"Your design created an illegal combinational loop.
If you need more information than you see here,
search for 'combinational loop' in Quartus compiler messages";
          break;
        case TT.WARNING_LATCH:
          cz =
@"Váš návrh vytvořil nepovolený úrovňový obvod typu LATCH.
Pokud potřebujete víc informací, než vidíte zde, 
hledejte 'latch' ve zprávách překladače Quartus.";
          eng =
@"Your design created an forbidden LATCH.
If you need more information than you see here, 
search for 'latch' in Quartus compiler messages.";
          break;
        case TT.WARNING_SENSITIVITY:
          cz =
@"Některý příkaz process má neúplný svůj sensitivity list.
Pokud potřebujete víc informací, než vidíte zde, 
hledejte 'sensitivity list' vve zprávách překladače Quartus.";
          eng =
@"Some process statement have an incomplete sensitivity list.
If you need more information than you see here, 
search for 'sensitivity list' in Quartus compiler messages.";
          break;
        case TT.NOREPORTS:
          cz =
@"Quartus projekt neobsahuje dočasné soubory překladu, což je správné pro odevzdání úlohy.
Měli byste ale nástroj kontroly též spustit na čerstvě přeložený projekt, 
aby prověřil sledované události jako LATCH, LOOP, dividers, etc.
Pokud jste je již vše překontrolovali, výzvu ignorujete.";
          eng =
    @"The Quartus project does not contain temporary compiler files, which is correct for its submission.
However, you should also run the check tool on a freshly compiled project.
It will search for monitored events like LATCH, LOOP, dividers, etc.
If you have already done so, ignore this message.";
          break;
      }
      return eng;


    }
  }
}
