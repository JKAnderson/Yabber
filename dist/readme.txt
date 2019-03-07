
--| Yabber 1.2
--| By TKGP
--| https://www.nexusmods.com/darksouls3/mods/305
--| https://github.com/JKAnderson/Yabber

An unpacker/repacker for Demon's Souls, Dark Souls 1-3, and Bloodborne container formats. Supports .bnd, .bhd/.bdt, .tpf, and .dcx.
Also supports the following single-file formats: .fmg, .gparam, .luagnl, and .luainfo.
Does not support dvdbnds (dvdbnd0.bhd5 etc in DS1, GameDataEbl.bhd etc in DS2, Data1.bhd etc in DS3); use UDSFM or UXM to unpack those first.
https://www.nexusmods.com/darksouls/mods/1304
https://www.nexusmods.com/darksouls3/mods/286
Requires .NET 4.7.2 - Windows 10 users should already have this.
https://www.microsoft.com/net/download/thank-you/net472


--| Yabber.exe

This program is for unpacking and repacking container files. Drag and drop a file (bnd, bhd, fmg, gparam, luagnl, luainfo, or tpf) onto the exe to unpack it; drag and drop an unpacked folder to repack it. Multiple files or folders can be selected and dropped at a time.
DCX versions of supported formats can be dropped directly onto Yabber.exe without decompressing them separately; they will automatically be recompressed when repacking.
Edit the .xml file in the unpacked folder to add, remove or rename files before repacking.

FMG files are simply extracted to an xml file with the same name. Drop the .xml back onto Yabber to repack it to the FMG.
In the output xml, "%null%" is a special value that indicates the ID is present in the file but has no text.


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

FMG
Extension: .fmg
A collection of text strings with an associated ID number, used in all games.

GPARAM
Extension: .gparam
A graphical configuration format used in BB and DS3.

LUAGNL/LUAINFO
Extension: .luagnl/.luainfo
Lua scripting support files used in DS1, BB, and DS3.

TPF
Extension: .tpf
A DDS texture container, used in all games. Console versions are not supported.


--| Contributors

katalash - GPARAM support
TKGP - Everything else


--| Changelog

1.2
	GPARAM, LUAGNL, and LUAINFO are now supported
	Breaking change: compressed FMG is now supported; please repack any in-progress FMGs with the previous version, then unpack them again with this one

1.1.1
	Fix repacked FMGs getting double-spaced
	Fix decompressing DCXs that aren't named .dcx

1.1
	Add FMG support

1.0.2
	Fix repacking DX10 textures

1.0.1
	Fix bad BXF4 repacking
