The LSPtools contain four utilities for Windows FPGA designs. They were written to use VHDL language in the Quartus development environment with the Veek-MT2 development board but could be modified for other environments.

![alt text](https://github.com/cvut/LSPtools/blob/main/images/LSPtoolStart.png)

The utilities are run from the Start Window, which is also possible to minimize the system notification area, aka tray icon.

- **Bitmap To VHDL** converts an image to a VHDL file that Quartus compiles as FPGA memory.
- **LCD Geometry Tool** measures image elements as lines, 
   squares, and ellipses and expresses and allows expressing them by equations. 
   We can create an LCD image using logic, which gives more efficient results for some figures than PNG or JPEG compression.
- **Testbench Viewer** displays results of image creations. 
- **Quartus Project???** checker verifies if the project has the proper setting. 
  It also corrects VWF files errors in Quartus 18 a 20 Lite. 
  Paid Quartus versions were patched, but the errors remain in Lite variants.
  The checker allows adjusting the VWF file.

The LSPtools were written in C# language under Visual Studio Community 2022 and compiled for 
target framework .NET 4.8 preinstalled part of Windows 10 and 11.
