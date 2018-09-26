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
    class TPF : SoulsFormats.TPF
    {
        //public void Unpack(string sourceName, string targetDir)
        //{
        //    Directory.CreateDirectory(targetDir);
        //    XmlWriterSettings xws = new XmlWriterSettings();
        //    xws.Indent = true;
        //    XmlWriter xw = XmlWriter.Create($"{targetDir}\\yabber-tpf.xml", xws);
        //    xw.WriteStartElement("tpf");

        //    xw.WriteElementString("filename", sourceName);
        //    xw.WriteElementString("compression", Compression.ToString());
        //    xw.WriteElementString("timestamp", Timestamp);
        //    xw.WriteElementString("format", $"0x{Format:X2}");
        //    xw.WriteElementString("bigendian", BigEndian.ToString());
        //    xw.WriteElementString("unk1", Unk1.ToString());
        //    xw.WriteElementString("unk2", $"0x{Unk2:X8}");

        //    xw.WriteStartElement("files");
        //    foreach (Texture texture in Textures)
        //    {
        //        string outPath = Util.UnrootBNDPath(texture.Name);

        //        xw.WriteStartElement("file");
        //        xw.WriteElementString("id", texture.ID.ToString());
        //        xw.WriteElementString("name", texture.Name ?? "<null>");
        //        xw.WriteElementString("path", outPath);
        //        xw.WriteElementString("flags", $"0x{texture.Flags:X2}");
        //        xw.WriteEndElement();

        //        outPath = $"{targetDir}\\{outPath}";
        //        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
        //        System.IO.File.WriteAllBytes(outPath, texture.Bytes);
        //    }
        //    xw.WriteEndElement();

        //    xw.WriteEndElement();
        //    xw.Close();
        //}

        //public static void Repack(string sourceDir, string targetDir)
        //{
        //    TPF tpf = new TPF();
        //    XmlDocument xml = new XmlDocument();
        //    xml.Load($"{sourceDir}\\yabber-tpf.xml");

        //    string filename = xml.SelectSingleNode("tpf/filename").InnerText;
        //    Enum.TryParse(xml.SelectSingleNode("tpf/compression").InnerText, out tpf.Compression);

        //    tpf.Textures = new List<Texture>();
        //    foreach (XmlNode fileNode in xml.SelectNodes("tpf/files/file"))
        //    {
        //        int id = int.Parse(fileNode.SelectSingleNode("id").InnerText);
        //        string name = fileNode.SelectSingleNode("name").InnerText;
        //        string path = fileNode.SelectSingleNode("path").InnerText;
        //        byte flags = Convert.ToByte(fileNode.SelectSingleNode("flags").InnerText, 16);
                
        //        byte[] bytes = File.ReadAllBytes($"{sourceDir}\\{path}");

        //        tpf.Textures.Add(new Texture(id, name, flags, bytes));
        //    }

        //    tpf.Write($"{targetDir}\\{filename}");
        //}
    }
}
