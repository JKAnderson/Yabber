using SoulsFormats;
using System;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YFMG
    {
        public static void Unpack(this FMG fmg, string sourceFile)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            // You need Indent for it to write newlines
            xws.Indent = true;
            // But don't actually indent so there's more room for the text
            xws.IndentChars = "";
            XmlWriter xw = XmlWriter.Create($"{sourceFile}.xml", xws);
            xw.WriteStartElement("fmg");
            xw.WriteElementString("version", fmg.Version.ToString());
            xw.WriteElementString("bigendian", fmg.BigEndian.ToString());
            xw.WriteStartElement("entries");

            // I think they're sorted already, but whatever
            fmg.Entries.Sort((e1, e2) => e1.ID.CompareTo(e2.ID));
            foreach (FMG.Entry entry in fmg.Entries)
            {
                xw.WriteStartElement("text");
                xw.WriteAttributeString("id", entry.ID.ToString());
                xw.WriteString(entry.Text ?? "%null%");
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceFile)
        {
            FMG fmg = new FMG();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);
            fmg.Version = (FMG.FMGVersion)Enum.Parse(typeof(FMG.FMGVersion), xml.SelectSingleNode("fmg/version").InnerText);
            fmg.BigEndian = bool.Parse(xml.SelectSingleNode("fmg/bigendian").InnerText);

            foreach (XmlNode textNode in xml.SelectNodes("fmg/entries/text"))
            {
                int id = int.Parse(textNode.Attributes["id"].InnerText);
                string text = textNode.InnerText;
                if (text == "%null%")
                    text = null;
                fmg.Entries.Add(new FMG.Entry(id, text));
            }

            string outPath = sourceFile.Replace(".fmg.xml", ".fmg");
            if (File.Exists(outPath) && !File.Exists(outPath + ".bak"))
                File.Copy(outPath, outPath + ".bak");
            fmg.Write(outPath);
        }
    }
}
