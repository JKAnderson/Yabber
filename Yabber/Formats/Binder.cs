using SoulsFormats;
using System;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YBinder
    {
        public static void WriteBinderFiles(IBinder bnd, XmlWriter xw, string targetDir)
        {
            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                BinderFile file = bnd.Files[i];

                string root = null;
                string path = null;
                if (Binder.HasName(bnd.Format))
                {
                    path = YBUtil.UnrootBNDPath(file.Name, out root);
                }
                else if (Binder.HasID(bnd.Format))
                {
                    path = file.ID.ToString();
                }
                else
                {
                    path = i.ToString();
                }

                xw.WriteStartElement("file");
                xw.WriteElementString("flags", $"0x{(byte)file.Flags:X2}");

                if (Binder.HasID(bnd.Format))
                    xw.WriteElementString("id", file.ID.ToString());

                if (root != null)
                    xw.WriteElementString("root", root);

                xw.WriteElementString("path", path);
                xw.WriteEndElement();

                path = $"{targetDir}\\{path}";
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllBytes(path, file.Bytes);
            }
            xw.WriteEndElement();
        }

        public static void ReadBinderFiles(IBinder bnd, XmlNode filesNode, string sourceDir)
        {
            foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
            {
                var flags = (Binder.FileFlags)Convert.ToByte(fileNode.SelectSingleNode("flags")?.InnerText ?? "0x40", 16);
                int id = int.Parse(fileNode.SelectSingleNode("id")?.InnerText ?? "-1");
                string root = fileNode.SelectSingleNode("root")?.InnerText ?? "";
                string path = fileNode.SelectSingleNode("path").InnerText;

                string name = null;
                if (Binder.HasName(bnd.Format))
                    name = root + path;

                byte[] bytes = File.ReadAllBytes($@"{sourceDir}\{root}{path}");
                bnd.Files.Add(new BinderFile(flags, id, name, bytes));
            }
        }
    }
}
