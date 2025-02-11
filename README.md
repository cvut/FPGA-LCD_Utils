The FPGA-LCD Utils package includes four Windows utilities that simplify designing images for FPGAs with connected LCDs, such as drawing backgrounds for control panels. Creating images using logic equations gives more efficient results for some figures than classic image compressions such as PNG or JPEG. The FPGA-LCD Utils are intended for use within the Quartus development environment and are compatible with the Veek-MT2 development board, though they can also be adapted for other FPGA boards. While some components are tailored explicitly for VHDL, they can support other HDL languages after minor source code modifications.

![alt text](https://github.com/cvut/LSPtools/blob/main/images/LSPtoolStart.png)

1. **Bitmap To VHDL** converts an image to either a VHDL file that Quartus compiles as FPGA memory or to MIF file format - Intel Memory Initialization File.

2. **LCD Geometry Tool** measures image elements as lines, squares, and ellipses. They can be expressed as equations with possible built-in automatic optimization.
   
3. **Testbench Viewer** displays text results of LCD image testbench runs with the possibility of playing more frames. The cooperation with the GHDL simulator was successfully tested.

4. **Quartus Project???** checker
- verifies if a Quartus project has the proper settings for a Veek-MT2 development board.
- corrects VWF file errors of Quartus 18 and 20 Lite internal simulators. The WVF errors were patched in paid Quartus versions, but they remain in Lite variants. The checker allows adjusting VWF files.
- searches the compiled Quartus project messages for frequent student errors, such as latches, incomplete sensitivity lists, and inefficient usage by division/modulo operations by non-power of 2. 

The FPGA-LCD Utils were developed in C# using Visual Studio Community 2022 and are compiled for the .NET Framework 4.8, preinstalled on Windows 10 and 11. They have been used by many of our LSP course students for more than a year, so they are already stable release. 

_Note: FPGA-LCD Utils were previously distributed as LSP Tools, but our study course nickname was unfortunately found to be duplicative with another branch of programs._
