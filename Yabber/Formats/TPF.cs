using SoulsFormats;
using System;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YTPF
    {
        public static void Unpack(this TPF tpf, string sourceName, string targetDir)
        {
            if (tpf.Platform != TPF.TPFPlatform.PC)
                throw new NotSupportedException("Yabber does not support console TPFs at the moment.");

            Directory.CreateDirectory(targetDir);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{targetDir}\\_yabber-tpf.xml", xws);
            xw.WriteStartElement("tpf");

            xw.WriteElementString("filename", sourceName);
            xw.WriteElementString("compression", tpf.Compression.ToString());
            xw.WriteElementString("encoding", $"0x{tpf.Encoding:X2}");
            xw.WriteElementString("flag2", $"0x{tpf.Flag2:X2}");

            xw.WriteStartElement("textures");
            foreach (TPF.Texture texture in tpf.Textures)
            {
                xw.WriteStartElement("texture");
                xw.WriteElementString("name", texture.Name + ".dds");
                xw.WriteElementString("format", $"0x{texture.Format:X2}");
                xw.WriteElementString("flags1", $"0x{texture.Flags1:X2}");
                xw.WriteElementString("flags2", $"0x{texture.Flags2:X8}");
                xw.WriteEndElement();

                File.WriteAllBytes($"{targetDir}\\{texture.Name}.dds", texture.Bytes);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            TPF tpf = new TPF();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-tpf.xml");

            string filename = xml.SelectSingleNode("tpf/filename").InnerText;
            Enum.TryParse(xml.SelectSingleNode("tpf/compression")?.InnerText ?? "None", out tpf.Compression);
            tpf.Encoding = Convert.ToByte(xml.SelectSingleNode("tpf/encoding").InnerText, 16);
            tpf.Flag2 = Convert.ToByte(xml.SelectSingleNode("tpf/flag2").InnerText, 16);

            foreach (XmlNode texNode in xml.SelectNodes("tpf/textures/texture"))
            {
                string name = Path.GetFileNameWithoutExtension(texNode.SelectSingleNode("name").InnerText);
                byte format = Convert.ToByte(texNode.SelectSingleNode("format").InnerText, 16);
                byte flags1 = Convert.ToByte(texNode.SelectSingleNode("flags1").InnerText, 16);
                int flags2 = Convert.ToInt32(texNode.SelectSingleNode("flags2").InnerText, 16);

                byte[] bytes = File.ReadAllBytes($"{sourceDir}\\{name}.dds");
                tpf.Textures.Add(new TPF.Texture(name, format, flags1, flags2, bytes));
            }

            string outPath = $"{targetDir}\\{filename}";
            YBUtil.Backup(outPath);
            tpf.Write(outPath);
        }
    }
}
