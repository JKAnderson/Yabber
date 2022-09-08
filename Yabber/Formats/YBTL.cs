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
    static class YBTL
    {
        public static void Unpack(this BTL btl, string sourceFile)
        {
            File.WriteAllText($"{sourceFile}.json", YBUtil.JsonSerialize(btl));
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

            YBUtil.Backup(outPath);
            YBUtil.JsonDeserialize<BTL>(File.ReadAllText(sourceFile)).Write(outPath);
        }
    }
}
