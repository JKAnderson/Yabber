
--| Yabber 1.0
--| By TKGP
--| https://www.nexusmods.com/darksouls3/mods/305
--| https://github.com/JKAnderson/Yabber

An unpacker/repacker for Demon's Souls, Dark Souls 1-3, and Bloodborne container formats. Supports .bnd, .bhd/.bdt, .tpf, and .dcx.
Does not support dvdbnds (dvdbnd0.bhd5 etc in DS1, GameDataEbl.bhd etc in DS2, Data1.bhd etc in DS3); use UDSFM or UXM to unpack those first.
https://www.nexusmods.com/darksouls/mods/1304
https://www.nexusmods.com/darksouls3/mods/286
Requires .NET 4.7.2 - Windows 10 users should already have this.
https://www.microsoft.com/net/download/thank-you/net472


--| Yabber.exe

This program is for unpacking and repacking container files. Drag and drop a file (bnd, bhd, or tpf) onto the exe to unpack it; drag and drop an unpacked folder to repack it. Multiple files or folders can be selected and dropped at a time.
DCX versions of supported formats can be dropped directly onto Yabber.exe without decompressing them separately; they will automatically be recompressed when repacking.
Edit the .xml file in the unpacked folder to add, remove or rename files before repacking.


--| Yabber.DCX.exe

This program is for decompressing and recompressing any DCX file. Drag and drop a DCX file onto the exe to decompress it; drag and drop the decompressed file to recompress it. Multiple files can be selected and dropped at a time.
You don't need to use this to decompress container formats before dropping them on Yabber.exe; this is only for compressed formats that aren't otherwise supported by Yabber.


--| Yabber.Context.exe

This program registers the other two so that they can be run by right-clicking on a file or folder. Run it to choose whether to register or unregister them.
The other two programs are assumed to be in the same folder. If you move them, just run it again from the new location.


--| Formats

BND3
Extension: .*bnd
A generic file container used in DeS and DS1. DS1 is fully supported; DeS is mostly supported.

BND4
Extension: .*bnd
A generic file container used in DS2, BB, and DS3.

BXF3
Extensions: .*bhd, .*bdt
A generic file container split into a header and data file, used in DS1. Only drag-and-drop the .bhd to unpack it; the .bdt is assumed to be in the same directory.

BXF4
Extensions: .*bhd, .*bdt
A generic file container split into a header and data file, used in DS2, BB, and DS3. Only drag-and-drop the .bhd to unpack it; the .bdt is assumed to be in the same directory.

DCX
Extension: .dcx
A single compressed file, used in all games.

TPF
Extension: .tpf
A DDS texture container, used in all games. Console versions are not supported.
