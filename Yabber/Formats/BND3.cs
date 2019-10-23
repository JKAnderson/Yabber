using SoulsFormats;
using System;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YBND3
    {
        public static void Unpack(this BND3 bnd, string sourceName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{targetDir}\\_yabber-bnd3.xml", xws);
            xw.WriteStartElement("bnd3");

            xw.WriteElementString("filename", sourceName);
            xw.WriteElementString("compression", bnd.Compression.ToString());
            xw.WriteElementString("timestamp", bnd.Timestamp);
            xw.WriteElementString("format", $"0x{(byte)bnd.Format:X2}");
            xw.WriteElementString("bigendian", bnd.BigEndian.ToString());
            xw.WriteElementString("unk1", bnd.Unk1.ToString());
            xw.WriteElementString("unk2", $"0x{bnd.Unk2:X8}");
            YBinder.WriteBinderFiles(bnd, xw, targetDir);

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            BND3 bnd = new BND3();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bnd3.xml");

            if (xml.SelectSingleNode("bnd3/filename") == null)
                throw new FriendlyException("Missing filename tag.");

            string filename = xml.SelectSingleNode("bnd3/filename").InnerText;
            string strCompression = xml.SelectSingleNode("bnd3/compression")?.InnerText ?? "None";
            bnd.Timestamp = xml.SelectSingleNode("bnd3/timestamp")?.InnerText ?? "07D7R6";
            string strFormat = xml.SelectSingleNode("bnd3/format")?.InnerText ?? "0x74";
            string strBigEndian = xml.SelectSingleNode("bnd3/bigendian")?.InnerText ?? "False";
            string strUnk1 = xml.SelectSingleNode("bnd3/unk1")?.InnerText ?? "False";
            string strUnk2 = xml.SelectSingleNode("bnd3/unk2")?.InnerText ?? "0x00000000";

            if (!Enum.TryParse(strCompression, out bnd.Compression))
                throw new FriendlyException($"Could not parse compression type: {strCompression}");

            try
            {
                bnd.Format = (Binder.Format)Convert.ToByte(strFormat, 16);
            }
            catch
            {
                throw new FriendlyException($"Could not parse format: {strFormat}\nFormat must be a hex value.");
            }

            if (!bool.TryParse(strBigEndian, out bnd.BigEndian))
                throw new FriendlyException($"Could not parse big-endianness: {strBigEndian}\nBig-endianness must be true or false.");

            if (!bool.TryParse(strUnk1, out bnd.Unk1))
                throw new FriendlyException($"Could not parse unk1: {strUnk1}\nUnk1 must be true or false.");

            try
            {
                bnd.Unk2 = Convert.ToInt32(strUnk2, 16);
            }
            catch
            {
                throw new FriendlyException($"Could not parse unk2: {strUnk2}\nUnk2 must be a hex value.");
            }

            if (xml.SelectSingleNode("bnd3/files") != null)
                YBinder.ReadBinderFiles(bnd, xml.SelectSingleNode("bnd3/files"), sourceDir);

            string outPath = $"{targetDir}\\{filename}";
            YBUtil.Backup(outPath);
            bnd.Write(outPath);
        }
    }
}
