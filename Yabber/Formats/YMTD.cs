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
    static class YMTD
    {
        public static void Unpack(this MTD mtd, string sourceFile)
        {
            File.WriteAllText($"{sourceFile}.json", YBUtil.JsonSerialize(mtd));
        }

        public static void Repack(string sourceFile)
        {
            string outPath;
            if (sourceFile.EndsWith(".mtd.json"))
                outPath = sourceFile.Replace(".mtd.json", ".mtd");
            else if (sourceFile.EndsWith(".mtd.dcx.json"))
                outPath = sourceFile.Replace(".mtd.dcx.json", ".mtd.dcx");
            else
                throw new InvalidOperationException("Invalid MTD json filename.");

            YBUtil.Backup(outPath);
            YBUtil.JsonDeserialize<MTD>(File.ReadAllText(sourceFile)).Write(outPath);
        }
    }
}
