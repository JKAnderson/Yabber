using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Yabber
{
    static class YBXF4
    {
        public static void Unpack(this BXF4 bxf, string bhdName, string bdtName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{targetDir}\\_yabber-bxf4.xml", xws);
            xw.WriteStartElement("bxf4");

            xw.WriteStartElement("bhd");
            xw.WriteElementString("filename", bhdName);
            xw.WriteElementString("timestamp", bxf.BHD.Timestamp);
            xw.WriteElementString("format", $"0x{bxf.BHD.Format:X2}");
            xw.WriteElementString("unicode", bxf.BHD.Unicode.ToString());
            xw.WriteElementString("bigendian", bxf.BHD.BigEndian.ToString());
            xw.WriteElementString("flag1", bxf.BHD.Flag1.ToString());
            xw.WriteElementString("flag2", bxf.BHD.Flag2.ToString());
            xw.WriteElementString("extended", $"0x{bxf.BHD.Extended:X2}");
            xw.WriteEndElement();

            xw.WriteStartElement("bdt");
            xw.WriteElementString("filename", bdtName);
            xw.WriteElementString("timestamp", bxf.BDT.Timestamp);
            xw.WriteElementString("bigendian", bxf.BDT.BigEndian.ToString());
            xw.WriteElementString("flag1", bxf.BDT.Flag1.ToString());
            xw.WriteElementString("flag2", bxf.BDT.Flag2.ToString());
            xw.WriteElementString("unk1", $"0x{bxf.BDT.Unk1:X16}");
            xw.WriteEndElement();

            xw.WriteStartElement("files");
            foreach (BXF4.File file in bxf.Files)
            {
                string path = YBUtil.UnrootBNDPath(file.Name, out string root);

                xw.WriteStartElement("file");
                xw.WriteElementString("id", file.ID.ToString());
                xw.WriteElementString("root", root);
                xw.WriteElementString("path", path);
                xw.WriteElementString("flags", $"0x{file.Flags:X2}");
                xw.WriteEndElement();

                path = $"{targetDir}\\{path}";
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, file.Bytes);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            BXF4 bxf = new BXF4();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bxf4.xml");

            string bhdFilename = xml.SelectSingleNode("bxf4/bhd/filename").InnerText;
            bxf.BHD.Timestamp = xml.SelectSingleNode("bxf4/bhd/timestamp").InnerText;
            bxf.BHD.Format = Convert.ToByte(xml.SelectSingleNode("bxf4/bhd/format").InnerText, 16);
            bxf.BHD.Unicode = bool.Parse(xml.SelectSingleNode("bxf4/bhd/unicode").InnerText);
            bxf.BHD.BigEndian = bool.Parse(xml.SelectSingleNode("bxf4/bhd/bigendian").InnerText);
            bxf.BHD.Flag1 = bool.Parse(xml.SelectSingleNode("bxf4/bhd/flag1").InnerText);
            bxf.BHD.Flag2 = bool.Parse(xml.SelectSingleNode("bxf4/bhd/flag2").InnerText);
            bxf.BHD.Extended = Convert.ToByte(xml.SelectSingleNode("bxf4/bhd/extended").InnerText, 16);

            string bdtFilename = xml.SelectSingleNode("bxf4/bdt/filename").InnerText;
            bxf.BDT.Timestamp = xml.SelectSingleNode("bxf4/bdt/timestamp").InnerText;
            bxf.BDT.BigEndian = bool.Parse(xml.SelectSingleNode("bxf4/bdt/bigendian").InnerText);
            bxf.BDT.Flag1 = bool.Parse(xml.SelectSingleNode("bxf4/bdt/flag1").InnerText);
            bxf.BDT.Flag2 = bool.Parse(xml.SelectSingleNode("bxf4/bdt/flag2").InnerText);
            bxf.BDT.Unk1 = Convert.ToInt64(xml.SelectSingleNode("bxf4/bdt/unk1").InnerText, 16);

            foreach (XmlNode fileNode in xml.SelectNodes("bxf4/files/file"))
            {
                int id = int.Parse(fileNode.SelectSingleNode("id").InnerText);
                string root = fileNode.SelectSingleNode("root").InnerText;
                string path = fileNode.SelectSingleNode("path").InnerText;
                byte flags = Convert.ToByte(fileNode.SelectSingleNode("flags").InnerText, 16);

                byte[] bytes = File.ReadAllBytes($"{sourceDir}\\{path}");
                bxf.Files.Add(new BXF4.File(id, root + path, flags, bytes));
            }

            string bhdPath = $"{targetDir}\\{bhdFilename}";
            if (File.Exists(bhdPath) && !File.Exists(bhdPath + ".bak"))
                File.Move(bhdPath, bhdPath + ".bak");

            string bdtPath = $"{targetDir}\\{bdtFilename}";
            if (File.Exists(bdtPath) && !File.Exists(bdtPath + ".bak"))
                File.Move(bdtPath, bdtPath + ".bak");

            bxf.Write(bhdPath, bdtPath);
        }
    }
}
