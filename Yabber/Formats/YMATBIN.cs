using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.IO;
using Newtonsoft.Json;

namespace Yabber
{
    static class YMATBIN
    {
        public static void Unpack(this MATBIN matbin, string sourceFile)
        {
            File.WriteAllText($"{sourceFile}.json", YBUtil.JsonSerialize(matbin));
        }

        public static void Repack(string sourceFile)
        {
            string outPath;
            if (sourceFile.EndsWith(".matbin.json"))
                outPath = sourceFile.Replace(".matbin.json", ".matbin");
            else if (sourceFile.EndsWith(".matbin.dcx.json"))
                outPath = sourceFile.Replace(".matbin.dcx.json", ".matbin.dcx");
            else
                throw new InvalidOperationException("Invalid MATBIN json filename.");

            YBUtil.Backup(outPath);
            YBUtil.JsonDeserialize<MATBIN>(File.ReadAllText(sourceFile)).Write(outPath);
        }
    }
}
