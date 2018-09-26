using SoulsFormats;
using System;
using System.Collections.Generic;
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
            xw.WriteElementString("format", $"0x{bnd.Format:X2}");
            xw.WriteElementString("bigendian", bnd.BigEndian.ToString());
            xw.WriteElementString("flag1", bnd.Flag1.ToString());
            xw.WriteElementString("flag2", bnd.Flag2.ToString());
            xw.WriteElementString("unicode", bnd.Unicode.ToString());
            xw.WriteElementString("extended", $"0x{bnd.Extended:X2}");

            xw.WriteStartElement("files");
            foreach (BND4.File file in bnd.Files)
            {
                string outPath = Util.UnrootBNDPath(file.Name);

                xw.WriteStartElement("file");
                xw.WriteElementString("id", file.ID.ToString());
                xw.WriteElementString("name", file.Name);
                xw.WriteElementString("path", outPath);
                xw.WriteElementString("flags", $"0x{file.Flags:X2}");
                xw.WriteEndElement();

                outPath = $"{targetDir}\\{outPath}";
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, file.Bytes);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            BND4 bnd = new BND4();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bnd4.xml");

            string filename = xml.SelectSingleNode("bnd4/filename").InnerText;
            Enum.TryParse(xml.SelectSingleNode("bnd4/compression").InnerText, out bnd.Compression);
            bnd.Timestamp = xml.SelectSingleNode("bnd4/timestamp").InnerText;
            bnd.Format = Convert.ToByte(xml.SelectSingleNode("bnd4/format").InnerText, 16);
            bnd.BigEndian = bool.Parse(xml.SelectSingleNode("bnd4/bigendian").InnerText);
            bnd.Flag1 = bool.Parse(xml.SelectSingleNode("bnd4/flag1").InnerText);
            bnd.Flag2 = bool.Parse(xml.SelectSingleNode("bnd4/flag2").InnerText);
            bnd.Unicode = bool.Parse(xml.SelectSingleNode("bnd4/unicode").InnerText);
            bnd.Extended = Convert.ToByte(xml.SelectSingleNode("bnd4/extended").InnerText, 16);
            
            foreach (XmlNode fileNode in xml.SelectNodes("bnd4/files/file"))
            {
                int id = int.Parse(fileNode.SelectSingleNode("id").InnerText);
                string name = fileNode.SelectSingleNode("name").InnerText;
                string path = fileNode.SelectSingleNode("path").InnerText;
                byte flags = Convert.ToByte(fileNode.SelectSingleNode("flags").InnerText, 16);

                byte[] bytes = File.ReadAllBytes($"{sourceDir}\\{path}");

                bnd.Files.Add(new BND4.File(id, name, flags, bytes));
            }

            string outPath = $"{targetDir}\\{filename}";
            if (File.Exists(outPath) && !File.Exists(outPath + ".bak"))
                File.Move(outPath, outPath + ".bak");

            bnd.Write(outPath);
        }
    }
}
