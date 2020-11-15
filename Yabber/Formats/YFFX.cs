using SoulsFormats;
using System.IO;

namespace Yabber
{
    static class YFFX
    {
        public static void Unpack(this FFXDLSE ffx, string sourceFile)
        {
            using (var sw = new StreamWriter($"{sourceFile}.xml"))
                ffx.XmlSerialize(sw);
        }

        public static void Repack(string sourceFile)
        {
            FFXDLSE ffx;
            using (var sr = new StreamReader(sourceFile))
                ffx = FFXDLSE.XmlDeserialize(sr);

            string outPath = sourceFile.Replace(".ffx.xml", ".ffx").Replace(".ffx.dcx.xml", ".ffx.dcx");
            YBUtil.Backup(outPath);
            ffx.Write(outPath);
        }
    }
}
