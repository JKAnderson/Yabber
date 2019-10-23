using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Yabber
{
    static class YBinder
    {
        public static void WriteBinderFiles(IBinder bnd, XmlWriter xw, string targetDir)
        {
            xw.WriteStartElement("files");
            var pathCounts = new Dictionary<string, int>();
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                BinderFile file = bnd.Files[i];

                string root = "";
                string path;
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

                if (root != "")
                    xw.WriteElementString("root", root);

                xw.WriteElementString("path", path);

                string suffix = "";
                if (pathCounts.ContainsKey(path))
                {
                    pathCounts[path]++;
                    suffix = $" ({pathCounts[path]})";
                    xw.WriteElementString("suffix", suffix);
                }
                else
                {
                    pathCounts[path] = 1;
                }
                xw.WriteEndElement();

                string outPath = $@"{targetDir}\{Path.GetDirectoryName(path)}\{Path.GetFileNameWithoutExtension(path)}{suffix}{Path.GetExtension(path)}";
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, file.Bytes);
            }
            xw.WriteEndElement();
        }

        public static void ReadBinderFiles(IBinder bnd, XmlNode filesNode, string sourceDir)
        {
            foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
            {
                if (fileNode.SelectSingleNode("path") == null)
                    throw new FriendlyException("File node missing path tag.");

                string strFlags = fileNode.SelectSingleNode("flags")?.InnerText ?? "0x40";
                string strID = fileNode.SelectSingleNode("id")?.InnerText ?? "-1";
                string root = fileNode.SelectSingleNode("root")?.InnerText ?? "";
                string path = fileNode.SelectSingleNode("path").InnerText;
                string suffix = fileNode.SelectSingleNode("suffix")?.InnerText ?? "";
                string name = root + path;

                Binder.FileFlags flags;
                try
                {
                    flags = (Binder.FileFlags)Convert.ToByte(strFlags, 16);
                }
                catch
                {
                    throw new FriendlyException($"Could not parse file flags: {strFlags}\nFlags must be a hex value.");
                }

                if (!int.TryParse(strID, out int id))
                    throw new FriendlyException($"Could not parse file ID: {strID}\nID must be a 32-bit signed integer.");

                string inPath = $@"{sourceDir}\{Path.GetDirectoryName(path)}\{Path.GetFileNameWithoutExtension(path)}{suffix}{Path.GetExtension(path)}";
                if (!File.Exists(inPath))
                    throw new FriendlyException($"File not found: {inPath}");

                byte[] bytes = File.ReadAllBytes(inPath);
                bnd.Files.Add(new BinderFile(flags, id, name, bytes));
            }
        }
    }
}
