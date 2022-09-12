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
    static class YFXR3
    {
        public static void Unpack(this FXR3 fxr, string sourceFile)
        {
            File.WriteAllText($"{sourceFile}.json", YBUtil.JsonSerialize(fxr));
        }

        public static void Repack(string sourceFile)
        {
            string outPath;
            if (sourceFile.EndsWith(".fxr.json"))
                outPath = sourceFile.Replace(".fxr.json", ".fxr");
            else if (sourceFile.EndsWith(".fxr.dcx.json"))
                outPath = sourceFile.Replace(".fxr.dcx.json", ".fxr.dcx");
            else
                throw new InvalidOperationException("Invalid FXR3 json filename.");

            YBUtil.Backup(outPath);
            YBUtil.JsonDeserialize<FXR3>(File.ReadAllText(sourceFile)).Write(outPath);
        }
    }
}
