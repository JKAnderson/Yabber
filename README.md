# Yabber
An unpacker/repacker for Demon's Souls, Dark Souls 1-3, and Bloodborne container formats. Supports .bnd, .bhd/.bdt, .tpf, and .dcx.  
Also supports the following single-file formats: .fmg, .gparam, .luagnl, and .luainfo.  
Does not support dvdbnds (dvdbnd0.bhd5 etc in DS1, GameDataEbl.bhd etc in DS2, Data1.bhd etc in DS3); use [UDSFM](https://www.nexusmods.com/darksouls/mods/1304) or [UXM](https://www.nexusmods.com/darksouls3/mods/286) to unpack those first.  
Requires [.NET 4.7.2](https://www.microsoft.com/net/download/thank-you/net472) - Windows 10 users should already have this.  
[NexusMods Page](https://www.nexusmods.com/darksouls3/mods/305)  

Please see the included readme for detailed instructions.

# Contributors
*katalash* - GPARAM support
*TKGP* - Everything else

# Changelog
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