The FPGA-LCD Utils package includes four Windows utilities designed to create LCD images for FPGAs, such as backgrounds for control panels. Creating LCD images using logic gives more efficient results for some figures than PNG or JPEG compression. These utilities are intended for use within the Quartus development environment and are compatible with the Veek-MT2 development board, though they can also be adapted for other environments. While some components are tailored explicitly for VHDL, they can support other HDL languages after minor source code modifications.

![alt text](https://github.com/cvut/LSPtools/blob/main/images/LSPtoolStart.png)

- **Bitmap To VHDL** converts an image to a VHDL file that Quartus compiles as FPGA memory.
- **LCD Geometry Tool** measures image elements as lines, 
   squares, and ellipses and expresses and allows them to be expressed by optimized equations.
   
- **Testbench Viewer** displays results of image creations for testbench runs that were also successfully tested with the GHDL simulator. 
- **Quartus Project???** checker verifies if the project has the proper setting for a Veek-MT2 development board. 
  It also corrects VWF file errors of Quartus 18 and 20 Lite internal simulators. 
  In paid Quartus versions, the errors were patched, but they remain in Lite variants.
  The checker allows adjusting VWF files.

The LSPtools were developed in C# using Visual Studio Community 2022 and are compiled for the .NET Framework 4.8, preinstalled on Windows 10 and 11.

Note: FPGA-LCD Utils were previously distributed as LSP Tools, but this nick was unfortunately found to be duplicative with another branch of programs.
