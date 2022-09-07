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
    static class YMSB3
    {
        public static void Unpack(this MSB3 msb, string sourceFile)
        {
            string output = JsonConvert.SerializeObject(msb, Formatting.Indented);

            File.WriteAllText($"{sourceFile}.json", output);
        }

        public static void Repack(string sourceFile)
        {
            string outPath;
            if (sourceFile.EndsWith(".msb.json"))
                outPath = sourceFile.Replace(".msb.json", ".msb");
            else if (sourceFile.EndsWith(".msb.dcx.json"))
                outPath = sourceFile.Replace(".msb.dcx.json", ".msb.dcx");
            else
                throw new InvalidOperationException("Invalid MSB3 json filename.");

            YBUtil.Backup(outPath);

            string input = File.ReadAllText(sourceFile);
            var msb = JsonConvert.DeserializeObject<MSB3>(input);

            msb.Write(outPath);
        }
    }
}
