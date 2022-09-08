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
    static class YBTL
    {
        public static void Unpack(this BTL btl, string sourceFile)
        {
            string output = JsonConvert.SerializeObject(btl, Formatting.Indented);

            File.WriteAllText($"{sourceFile}.json", output);
        }

        public static void Repack(string sourceFile)
        {
            string outPath;
            if (sourceFile.EndsWith(".btl.json"))
                outPath = sourceFile.Replace(".btl.json", ".btl");
            else if (sourceFile.EndsWith(".btl.dcx.json"))
                outPath = sourceFile.Replace(".btl.dcx.json", ".btl.dcx");
            else
                throw new InvalidOperationException("Invalid BTL json filename.");

            string input = File.ReadAllText(sourceFile);
            var btl = JsonConvert.DeserializeObject<BTL>(input);

            YBUtil.Backup(outPath);
            btl.Write(outPath);
        }
    }
}
