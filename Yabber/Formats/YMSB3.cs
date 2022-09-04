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
            if (File.Exists(sourceFile) && !File.Exists(sourceFile + ".bak"))
                File.Copy(sourceFile, sourceFile + ".bak");

            string output = JsonConvert.SerializeObject(msb.Parts, Formatting.Indented);

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

            string bakPath = outPath + ".bak";

            if (!File.Exists(bakPath))
                throw new InvalidOperationException("Missing .msb.bak file.");

            string input = File.ReadAllText(sourceFile);
            var parts = JsonConvert.DeserializeObject<MSB3.PartsParam>(input);

            var msb = MSB3.Read(bakPath);
            msb.Parts = parts;

            msb.Write(outPath);
        }
    }
}
