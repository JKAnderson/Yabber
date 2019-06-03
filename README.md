# Yabber
An unpacker/repacker for common Demon's Souls, Dark Souls 1-3, Bloodborne, and Sekiro file formats. Supports .bnd, .bhd/.bdt, .dcx, .fltparam, .fmg, .gparam, .luagnl, .luainfo, and .tpf.  
In order to decompress Sekiro files you must copy oo2core_6_win64.dll from Sekiro into Yabber's lib folder.  
Does not support dvdbnds (the very large bhd/bdt pairs in the main game directory); use [UDSFM](https://www.nexusmods.com/darksouls/mods/1304) or [UXM](https://www.nexusmods.com/sekiro/mods/26) to unpack those first.  
Also does not support encrypted files (enc_regulation.bnd.dcx in DS2, Data0.bdt in DS3); you can edit these with [Yapped](https://www.nexusmods.com/darksouls3/mods/306) or unpack them with [BinderTool](https://github.com/Atvaark/BinderTool).  
Requires [.NET 4.7.2](https://www.microsoft.com/net/download/thank-you/net472) - Windows 10 users should already have this.  
[NexusMods Page](https://www.nexusmods.com/sekiro/mods/42)  

Please see the included readme for detailed instructions.

# Contributors
*katalash* - GPARAM support  
*TKGP* - Everything else

# Changelog
### 1.3.1
* DS2 .fltparams are now supported
* BXF4 repacking fixed
* Prompt for administrator access if necessary
* Breaking change: GPARAM format changed again; please repack any in-progress GPARAMs with the previous version, then unpack them again with this one

### 1.3
* Sekiro support
* Breaking change: GPARAM format has changed in a few ways; please repack any in-progress GPARAMs with the previous version, then unpack them again with this one

### 1.2.2
* Fix not being able to repack bnds with roots

### 1.2.1
* Fix LUAINFO not working on files with 2 or fewer goals
* Fix LUAGNL not working on some files
* Fix GPARAM not repacking files with Byte4 params
* Better support for weird BND/BXF formats without IDs or names

### 1.2
* GPARAM, LUAGNL, and LUAINFO are now supported
* Breaking change: compressed FMG is now supported; please repack any in-progress FMGs with the previous version, then unpack them again with this one

### 1.1.1
* Fix repacked FMGs getting double-spaced
* Fix decompressing DCXs that aren't named .dcx

### 1.1
* Add FMG support

### 1.0.2
* Fix repacking DX10 textures

### 1.0.1
* Fix bad BXF4 repacking