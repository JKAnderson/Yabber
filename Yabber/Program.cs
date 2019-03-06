using SoulsFormats;
using System;
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
                    if (Directory.Exists(path))
                    {
                        pause |= RepackDir(path);
                    }
                    else if (File.Exists(path))
                    {

                        pause |= UnpackFile(path);
                    }
                    else
                    {
                        Console.WriteLine($"File or directory not found: {path}");
                        pause = true;
                    }
                }
                catch (Exception ex)
                {
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

        private static bool UnpackFile(string sourceFile)
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
                    BND3 bnd = BND3.Read(bytes);
                    bnd.Compression = compression;
                    bnd.Unpack(filename, targetDir);
                }
                else if (BND4.Is(bytes))
                {
                    Console.WriteLine($"Unpacking BND4: {filename}...");
                    BND4 bnd = BND4.Read(bytes);
                    bnd.Compression = compression;
                    bnd.Unpack(filename, targetDir);
                }
                else if (TPF.Is(bytes))
                {
                    Console.WriteLine($"Unpacking TPF: {filename}...");
                    TPF tpf = TPF.Read(bytes);
                    tpf.Compression = compression;
                    tpf.Unpack(filename, targetDir);
                }
                else if (sourceFile.EndsWith(".gparam.dcx"))
                {
                    Console.WriteLine($"Unpacking GPARAM: {filename}...");
                    GPARAM gparam = GPARAM.Read(bytes);
                    gparam.Unpack(sourceFile);
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
                    BND3 bnd = BND3.Read(sourceFile);
                    bnd.Unpack(filename, targetDir);
                }
                else if (BND4.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking BND4: {filename}...");
                    BND4 bnd = BND4.Read(sourceFile);
                    bnd.Unpack(filename, targetDir);
                }
                else if (BXF3.IsBHD(sourceFile))
                {
                    string bdtExtension = Path.GetExtension(filename).Replace("bhd", "bdt");
                    string bdtFilename = $"{Path.GetFileNameWithoutExtension(filename)}{bdtExtension}";
                    string bdtPath = $"{sourceDir}\\{bdtFilename}";
                    if (File.Exists(bdtPath))
                    {
                        Console.WriteLine($"Unpacking BXF3: {filename}...");
                        BXF3 bxf = BXF3.Read(sourceFile, bdtPath);
                        bxf.Unpack(filename, bdtFilename, targetDir);
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
                        BXF4 bxf = BXF4.Read(sourceFile, bdtPath);
                        bxf.Unpack(filename, bdtFilename, targetDir);
                    }
                    else
                    {
                        Console.WriteLine($"BDT not found for BHD: {filename}");
                        return true;
                    }
                }
                else if (TPF.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking TPF: {filename}...");
                    TPF tpf = TPF.Read(sourceFile);
                    tpf.Unpack(filename, targetDir);
                }
                else if (sourceFile.EndsWith(".fmg"))
                {
                    Console.WriteLine($"Unpacking FMG: {filename}...");
                    FMG fmg = FMG.Read(sourceFile);
                    fmg.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg.xml"))
                {
                    Console.WriteLine($"Repacking FMG: {filename}...");
                    YFMG.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".gparam"))
                {
                    Console.WriteLine($"Unpacking GPARAM: {filename}...");
                    GPARAM gparam = GPARAM.Read(sourceFile);
                    gparam.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".gparam.xml") || sourceFile.EndsWith(".gparam.dcx.xml"))
                {
                    Console.WriteLine($"Repacking GPARAM: {filename}...");
                    YGPARAM.Repack(sourceFile);
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
                else
                {
                    Console.WriteLine($"File format not recognized: {filename}");
                    return true;
                }
            }
            return false;
        }

        private static bool RepackDir(string sourceDir)
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
