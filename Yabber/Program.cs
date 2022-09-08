using SoulsFormats;
using SoulsFormats.AC4;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Yabber
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Console.WriteLine(
                    $"{assembly.GetName().Name} {assembly.GetName().Version}\n\n" +
                    "Yabber has no GUI.\n" +
                    "Drag and drop a file onto the exe to unpack it,\n" +
                    "or an unpacked folder to repack it.\n\n" +
                    "DCX files will be transparently decompressed and recompressed;\n" +
                    "If you need to decompress or recompress an unsupported format,\n" +
                    "use Yabber.DCX instead.\n\n" +
                    "Press any key to exit."
                    );
                Console.ReadKey();
                return;
            }

            bool pause = false;

            foreach (string path in args)
            {
                try
                {
                    int maxProgress = Console.WindowWidth - 1;
                    int lastProgress = 0;
                    void report(float value)
                    {
                        int nextProgress = (int)Math.Ceiling(value * maxProgress);
                        if (nextProgress > lastProgress)
                        {
                            for (int i = lastProgress; i < nextProgress; i++)
                            {
                                if (i == 0)
                                    Console.Write('[');
                                else if (i == maxProgress - 1)
                                    Console.Write(']');
                                else
                                    Console.Write('=');
                            }
                            lastProgress = nextProgress;
                        }
                    }
                    IProgress<float> progress = new Progress<float>(report);

                    if (Directory.Exists(path))
                    {
                        pause |= RepackDir(path, progress);

                    }
                    else if (File.Exists(path))
                    {
                        pause |= UnpackFile(path, progress);
                    }
                    else
                    {
                        Console.WriteLine($"File or directory not found: {path}");
                        pause = true;
                    }

                    if (lastProgress > 0)
                    {
                        progress.Report(1);
                        Console.WriteLine();
                    }
                }
                catch (DllNotFoundException ex) when (ex.Message.Contains("oo2core_6_win64.dll"))
                {
                    Console.WriteLine("In order to decompress .dcx files from Sekiro, you must copy oo2core_6_win64.dll from Sekiro into Yabber's lib folder.");
                    pause = true;
                }
                catch (UnauthorizedAccessException)
                {
                    using (Process current = Process.GetCurrentProcess())
                    {
                        var admin = new Process();
                        admin.StartInfo = current.StartInfo;
                        admin.StartInfo.FileName = current.MainModule.FileName;
                        admin.StartInfo.Arguments = Environment.CommandLine.Replace($"\"{Environment.GetCommandLineArgs()[0]}\"", "");
                        admin.StartInfo.Verb = "runas";
                        admin.Start();
                        return;
                    }
                }
                catch (FriendlyException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Error: {ex.Message}");
                    pause = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Unhandled exception: {ex}");
                    pause = true;
                }

                Console.WriteLine();
            }

            if (pause)
            {
                Console.WriteLine("One or more errors were encountered and displayed above.\nPress any key to exit.");
                Console.ReadKey();
            }
        }

        private static bool UnpackFile(string sourceFile, IProgress<float> progress)
        {
            string sourceDir = Path.GetDirectoryName(sourceFile);
            string filename = Path.GetFileName(sourceFile);
            string targetDir = $"{sourceDir}\\{filename.Replace('.', '-')}";
            if (File.Exists(targetDir))
                targetDir += "-ybr";

            if (DCX.Is(sourceFile))
            {
                Console.WriteLine($"Decompressing DCX: {filename}...");
                byte[] bytes = DCX.Decompress(sourceFile, out DCX.Type compression);
                if (BND3.Is(bytes))
                {
                    Console.WriteLine($"Unpacking BND3: {filename}...");
                    using (var bnd = new BND3Reader(bytes))
                    {
                        bnd.Compression = compression;
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (BND4.Is(bytes))
                {
                    Console.WriteLine($"Unpacking BND4: {filename}...");
                    using (var bnd = new BND4Reader(bytes))
                    {
                        bnd.Compression = compression;
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (FFXDLSE.Is(bytes))
                {
                    Console.WriteLine($"Unpacking FFX: {filename}...");
                    var ffx = FFXDLSE.Read(bytes);
                    ffx.Compression = compression;
                    ffx.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg.dcx"))
                {
                    Console.WriteLine($"Unpacking FMG: {filename}...");
                    FMG fmg = FMG.Read(bytes);
                    fmg.Compression = compression;
                    fmg.Unpack(sourceFile);
                }
                else if (GPARAM.Is(bytes))
                {
                    Console.WriteLine($"Unpacking GPARAM: {filename}...");
                    GPARAM gparam = GPARAM.Read(bytes);
                    gparam.Compression = compression;
                    gparam.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".msb.dcx"))
                {
                    Console.WriteLine($"Unpacking MSB: {filename}...");

                    if (File.Exists($"{sourceDir}\\_er"))
                    {
                        var msb = MSBE.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_sekiro"))
                    {
                        var msb = MSBS.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_bb"))
                    {
                        var msb = MSBB.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_des"))
                    {
                        var msb = MSBD.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds3"))
                    {
                        var msb = MSB3.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds2"))
                    {
                        var msb = MSB2.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds1"))
                    {
                        var msb = MSB1.Read(bytes);
                        msb.Compression = compression;
                        msb.Unpack(sourceFile);
                    }
                    else
                    {
                        Console.WriteLine($"Create a file with name corresponding to the game.");
                        Console.WriteLine($"Valid names: _er, _sekiro, _bb, _des, _ds3, _ds2, _ds1");
                        return true;
                    }
                }
                else if (sourceFile.EndsWith(".btl.dcx"))
                {
                    Console.WriteLine($"Unpacking BTL: {filename}...");
                    BTL btl = BTL.Read(bytes);
                    btl.Compression = compression;
                    btl.Unpack(sourceFile);
                }
                else if (TPF.Is(bytes))
                {
                    Console.WriteLine($"Unpacking TPF: {filename}...");
                    TPF tpf = TPF.Read(bytes);
                    tpf.Compression = compression;
                    tpf.Unpack(filename, targetDir, progress);
                }
                else
                {
                    Console.WriteLine($"File format not recognized: {filename}");
                    return true;
                }
            }
            else
            {
                if (BND3.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking BND3: {filename}...");
                    using (var bnd = new BND3Reader(sourceFile))
                    {
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (BND4.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking BND4: {filename}...");
                    using (var bnd = new BND4Reader(sourceFile))
                    {
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (BXF3.IsBHD(sourceFile))
                {
                    string bdtExtension = Path.GetExtension(filename).Replace("bhd", "bdt");
                    string bdtFilename = $"{Path.GetFileNameWithoutExtension(filename)}{bdtExtension}";
                    string bdtPath = $"{sourceDir}\\{bdtFilename}";
                    if (File.Exists(bdtPath))
                    {
                        Console.WriteLine($"Unpacking BXF3: {filename}...");
                        using (var bxf = new BXF3Reader(sourceFile, bdtPath))
                        {
                            bxf.Unpack(filename, bdtFilename, targetDir, progress);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"BDT not found for BHD: {filename}");
                        return true;
                    }
                }
                else if (BXF4.IsBHD(sourceFile))
                {
                    string bdtExtension = Path.GetExtension(filename).Replace("bhd", "bdt");
                    string bdtFilename = $"{Path.GetFileNameWithoutExtension(filename)}{bdtExtension}";
                    string bdtPath = $"{sourceDir}\\{bdtFilename}";
                    if (File.Exists(bdtPath))
                    {
                        Console.WriteLine($"Unpacking BXF4: {filename}...");
                        using (var bxf = new BXF4Reader(sourceFile, bdtPath))
                        {
                            bxf.Unpack(filename, bdtFilename, targetDir, progress);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"BDT not found for BHD: {filename}");
                        return true;
                    }
                }
                else if (FFXDLSE.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking FFX: {filename}...");
                    var ffx = FFXDLSE.Read(sourceFile);
                    ffx.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".ffx.xml") || sourceFile.EndsWith(".ffx.dcx.xml"))
                {
                    Console.WriteLine($"Repacking FFX: {filename}...");
                    YFFX.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg"))
                {
                    Console.WriteLine($"Unpacking FMG: {filename}...");
                    FMG fmg = FMG.Read(sourceFile);
                    fmg.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg.xml") || sourceFile.EndsWith(".fmg.dcx.xml"))
                {
                    Console.WriteLine($"Repacking FMG: {filename}...");
                    YFMG.Repack(sourceFile);
                }
                else if (GPARAM.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking GPARAM: {filename}...");
                    GPARAM gparam = GPARAM.Read(sourceFile);
                    gparam.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".gparam.xml") || sourceFile.EndsWith(".gparam.dcx.xml")
                    || sourceFile.EndsWith(".fltparam.xml") || sourceFile.EndsWith(".fltparam.dcx.xml"))
                {
                    Console.WriteLine($"Repacking GPARAM: {filename}...");
                    YGPARAM.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".msb"))
                {

                    Console.WriteLine($"Unpacking MSB: {filename}...");

                    if (File.Exists($"{sourceDir}\\_er"))
                    {
                        var msb = MSBE.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_sekiro"))
                    {
                        var msb = MSBS.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_bb"))
                    {
                        var msb = MSBB.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_des"))
                    {
                        var msb = MSBD.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds3"))
                    {
                        var msb = MSB3.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds2"))
                    {
                        var msb = MSB2.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds1"))
                    {
                        var msb = MSB1.Read(sourceFile);
                        msb.Unpack(sourceFile);
                    }
                    else
                    {
                        Console.WriteLine($"Create a file with name corresponding to the game.");
                        Console.WriteLine($"Valid names: _er, _sekiro, _bb, _des, _ds3, _ds2, _ds1");
                        return true;
                    }
                }
                else if (sourceFile.EndsWith(".msb.json") || sourceFile.EndsWith(".msb.dcx.json"))
                {
                    Console.WriteLine($"Repacking MSB: {filename}...");

                    if (File.Exists($"{sourceDir}\\_er"))
                    {
                        YMSBE.Repack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_sekiro"))
                    {
                        YMSBS.Repack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_bb"))
                    {
                        YMSBB.Repack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_des"))
                    {
                        YMSBD.Repack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds3"))
                    {
                        YMSB3.Repack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds2"))
                    {
                        YMSB2.Repack(sourceFile);
                    }
                    else if (File.Exists($"{sourceDir}\\_ds1"))
                    {
                        YMSB1.Repack(sourceFile);
                    }
                    else
                    {
                        Console.WriteLine($"Create a file with name corresponding to the game.");
                        Console.WriteLine($"Valid names: _er, _sekiro, _bb, _des, _ds3, _ds2, _ds1");
                        return true;
                    }
                }
                else if (sourceFile.EndsWith(".btl"))
                {
                    Console.WriteLine($"Unpacking BTL: {filename}...");
                    BTL btl = BTL.Read(sourceFile);
                    btl.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".btl.json") || sourceFile.EndsWith(".btl.dcx.json"))
                {
                    Console.WriteLine($"Repacking BTL: {filename}...");
                    YBTL.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".luagnl"))
                {
                    Console.WriteLine($"Unpacking LUAGNL: {filename}...");
                    LUAGNL gnl = LUAGNL.Read(sourceFile);
                    gnl.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".luagnl.xml"))
                {
                    Console.WriteLine($"Repacking LUAGNL: {filename}...");
                    YLUAGNL.Repack(sourceFile);
                }
                else if (LUAINFO.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking LUAINFO: {filename}...");
                    LUAINFO info = LUAINFO.Read(sourceFile);
                    info.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".luainfo.xml"))
                {
                    Console.WriteLine($"Repacking LUAINFO: {filename}...");
                    YLUAINFO.Repack(sourceFile);
                }
                else if (TPF.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking TPF: {filename}...");
                    TPF tpf = TPF.Read(sourceFile);
                    tpf.Unpack(filename, targetDir, progress);
                }
                else if (Zero3.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking 000: {filename}...");
                    Zero3 z3 = Zero3.Read(sourceFile);
                    z3.Unpack(targetDir);
                }
                else
                {
                    Console.WriteLine($"File format not recognized: {filename}");
                    return true;
                }
            }
            return false;
        }

        private static bool RepackDir(string sourceDir, IProgress<float> progress)
        {
            string sourceName = new DirectoryInfo(sourceDir).Name;
            string targetDir = new DirectoryInfo(sourceDir).Parent.FullName;
            if (File.Exists($"{sourceDir}\\_yabber-bnd3.xml"))
            {
                Console.WriteLine($"Repacking BND3: {sourceName}...");
                YBND3.Repack(sourceDir, targetDir);
            }
            else if (File.Exists($"{sourceDir}\\_yabber-bnd4.xml"))
            {
                Console.WriteLine($"Repacking BND4: {sourceName}...");
                YBND4.Repack(sourceDir, targetDir);
            }
            else if (File.Exists($"{sourceDir}\\_yabber-bxf3.xml"))
            {
                Console.WriteLine($"Repacking BXF3: {sourceName}...");
                YBXF3.Repack(sourceDir, targetDir);
            }
            else if (File.Exists($"{sourceDir}\\_yabber-bxf4.xml"))
            {
                Console.WriteLine($"Repacking BXF4: {sourceName}...");
                YBXF4.Repack(sourceDir, targetDir);
            }
            else if (File.Exists($"{sourceDir}\\_yabber-tpf.xml"))
            {
                Console.WriteLine($"Repacking TPF: {sourceName}...");
                YTPF.Repack(sourceDir, targetDir);
            }
            else
            {
                Console.WriteLine($"Yabber XML not found in: {sourceName}");
                return true;
            }
            return false;
        }
    }
}
