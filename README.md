# Yabber
An unpacker/repacker for Demon's Souls, Dark Souls 1-3, and Bloodborne container formats. Supports .bnd, .bhd/.bdt, .tpf, and .dcx.  
Requires [.NET 4.7.2](https://www.microsoft.com/net/download/thank-you/net472) - Windows 10 users should already have this.  

# Usage
Drag-and-drop a file onto Yabber.exe to unpack it. Drag-and-drop an unpacked folder onto the exe to repack it. Multiple files can be selected and dropped at a time.  
DCX versions of supported formats like `c0000.chrbnd.dcx` can be dropped directly onto Yabber; you only need to use Yabber.DCX if you want to decompress other formats like `c0000.esd.dcx`. To recompress these, drag-and-drop the uncompressed file back onto Yabber.DCX.  

# Formats
### BND3
Extension: `.*bnd`  
A generic file container used in DeS and DS1. DS1 is fully supported; DeS is mostly supported.

### BND4
Extension: `.*bnd`  
A generic file container used in DS2, BB, and DS3.

### BXF3
Extensions: `.*bhd`, `.*bdt`  
A generic file container split into a header and data file, used in DS1. Only drag-and-drop the .bhd to unpack it; the .bdt is assumed to be in the same directory.

### BXF4
Extensions: `.*bhd`, `.*bdt`  
A generic file container split into a header and data file, used in DS2, BB, and DS3. Only drag-and-drop the .bhd to unpack it; the .bdt is assumed to be in the same directory.

### DCX
Extension: `*.dcx`  
A single compressed file, used in all games.

### TPF
Extension: `.tpf`
A DDS texture container, used in all games. Console versions are not supported.