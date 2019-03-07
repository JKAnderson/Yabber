using SoulsFormats;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YBXF3
    {
        public static void Unpack(this BXF3 bxf, string bhdName, string bdtName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{targetDir}\\_yabber-bxf3.xml", xws);
            xw.WriteStartElement("bxf3");

            xw.WriteStartElement("bhd");
            xw.WriteElementString("filename", bhdName);
            xw.WriteElementString("timestamp", bxf.BHDTimestamp);
            xw.WriteEndElement();

            xw.WriteStartElement("bdt");
            xw.WriteElementString("filename", bdtName);
            xw.WriteElementString("timestamp", bxf.BDTTimestamp);
            xw.WriteEndElement();

            xw.WriteStartElement("files");
            foreach (BinderFile file in bxf.Files)
            {
                string path = YBUtil.UnrootBNDPath(file.Name, out string root);

                xw.WriteStartElement("file");
                xw.WriteElementString("id", file.ID.ToString());
                xw.WriteElementString("root", root);
                xw.WriteElementString("path", path);
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
            BXF3 bxf = new BXF3();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bxf3.xml");

            string bhdFilename = xml.SelectSingleNode("bxf3/bhd/filename").InnerText;
            bxf.BHDTimestamp = xml.SelectSingleNode("bxf3/bhd/timestamp").InnerText;

            string bdtFilename = xml.SelectSingleNode("bxf3/bdt/filename").InnerText;
            bxf.BDTTimestamp = xml.SelectSingleNode("bxf3/bdt/timestamp").InnerText;

            foreach (XmlNode fileNode in xml.SelectNodes("bxf3/files/file"))
            {
                int id = int.Parse(fileNode.SelectSingleNode("id").InnerText);
                string root = fileNode.SelectSingleNode("root").InnerText;
                string path = fileNode.SelectSingleNode("path").InnerText;

                byte[] bytes = File.ReadAllBytes($"{sourceDir}\\{path}");
                bxf.Files.Add(new BinderFile(Binder.FileFlags.x40, id, root + path, bytes));
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
