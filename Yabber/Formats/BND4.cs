using SoulsFormats;
using System;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YBND4
    {
        public static void Unpack(this BND4 bnd, string sourceName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{targetDir}\\_yabber-bnd4.xml", xws);
            xw.WriteStartElement("bnd4");

            xw.WriteElementString("filename", sourceName);
            xw.WriteElementString("compression", bnd.Compression.ToString());
            xw.WriteElementString("timestamp", bnd.Timestamp);
            xw.WriteElementString("format", $"0x{(byte)bnd.Format:X2}");
            xw.WriteElementString("bigendian", bnd.BigEndian.ToString());
            xw.WriteElementString("flag1", bnd.Flag1.ToString());
            xw.WriteElementString("flag2", bnd.Flag2.ToString());
            xw.WriteElementString("unicode", bnd.Unicode.ToString());
            xw.WriteElementString("extended", $"0x{bnd.Extended:X2}");
            YBinder.WriteBinderFiles(bnd, xw, targetDir);
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            BND4 bnd = new BND4();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bnd4.xml");

            string filename = xml.SelectSingleNode("bnd4/filename").InnerText;
            Enum.TryParse(xml.SelectSingleNode("bnd4/compression")?.InnerText ?? "None", out bnd.Compression);
            bnd.Timestamp = xml.SelectSingleNode("bnd4/timestamp").InnerText;
            bnd.Format = (Binder.Format)Convert.ToByte(xml.SelectSingleNode("bnd4/format").InnerText, 16);
            bnd.BigEndian = bool.Parse(xml.SelectSingleNode("bnd4/bigendian").InnerText);
            bnd.Flag1 = bool.Parse(xml.SelectSingleNode("bnd4/flag1").InnerText);
            bnd.Flag2 = bool.Parse(xml.SelectSingleNode("bnd4/flag2").InnerText);
            bnd.Unicode = bool.Parse(xml.SelectSingleNode("bnd4/unicode").InnerText);
            bnd.Extended = Convert.ToByte(xml.SelectSingleNode("bnd4/extended").InnerText, 16);
            YBinder.ReadBinderFiles(bnd, xml.SelectSingleNode("bnd4/files"), sourceDir);

            string outPath = $"{targetDir}\\{filename}";
            if (File.Exists(outPath) && !File.Exists(outPath + ".bak"))
                File.Move(outPath, outPath + ".bak");

            bnd.Write(outPath);
        }
    }
}
