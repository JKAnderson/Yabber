﻿using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.IO;
using Newtonsoft.Json;

namespace Yabber
{
    static class YMSB1
    {
        public static void Unpack(this MSB1 msb, string sourceFile)
        {
            File.WriteAllText($"{sourceFile}.json", YBUtil.JsonSerialize(msb));
        }

        public static void Repack(string sourceFile)
        {
            string outPath;
            if (sourceFile.EndsWith(".msb.json"))
                outPath = sourceFile.Replace(".msb.json", ".msb");
            else if (sourceFile.EndsWith(".msb.dcx.json"))
                outPath = sourceFile.Replace(".msb.dcx.json", ".msb.dcx");
            else
                throw new InvalidOperationException("Invalid MSB1 json filename.");

            YBUtil.Backup(outPath);
            YBUtil.JsonDeserialize<MSB1>(File.ReadAllText(sourceFile)).Write(outPath);
        }
    }
}
