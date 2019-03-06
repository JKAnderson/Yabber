using SoulsFormats;
using System;
using System.IO;
using System.Numerics;
using System.Xml;
using System.Linq;

namespace Yabber
{
    static class YGPARAM
    {
        public static void Unpack(this GPARAM gparam, string sourceFile)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{sourceFile}.xml", xws);

            xw.WriteStartElement("gparam");
            xw.WriteElementString("unk1", gparam.Unk1.ToString());
            xw.WriteStartElement("groups");
            foreach (GPARAM.Group group in gparam.Groups)
            {
                xw.WriteStartElement("group");
                xw.WriteAttributeString("name1", group.Name1);
                xw.WriteAttributeString("name2", group.Name2);
                foreach (GPARAM.Param param in group.Params)
                {
                    xw.WriteStartElement("param");
                    xw.WriteAttributeString("name1", param.Name1);
                    xw.WriteAttributeString("name2", param.Name2);
                    xw.WriteAttributeString("type", param.Type.ToString());
                    for (var i = 0; i < param.Values.Count; i++)
                    {
                        xw.WriteStartElement("value");
                        xw.WriteAttributeString("id", param.Unk1Values[i].ToString());
                        if (i < group.Comments.Count)
                            xw.WriteAttributeString("comment", group.Comments[i]);

                        if (param.Type == GPARAM.ParamType.Float2)
                        {
                            var value = (Vector2)param.Values[i];
                            xw.WriteString($"{value.X} {value.Y}");
                        }
                        else if (param.Type == GPARAM.ParamType.Float4)
                        {
                            var value = (Vector4)param.Values[i];
                            xw.WriteString($"{value.X} {value.Y} {value.Z} {value.W}");
                        }
                        else if (param.Type == GPARAM.ParamType.Byte4)
                        {
                            var value = (byte[])param.Values[i];
                            xw.WriteString($"{value[0]:X2} {value[1]:X2} {value[2]:X2} {value[3]:X2}");
                        }
                        else
                        {
                            xw.WriteString(param.Values[i].ToString());
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteStartElement("unk3s");
            foreach (var unk in gparam.Unk3s)
            {
                xw.WriteStartElement("unk3");
                xw.WriteAttributeString("id", unk.ID.ToString());
                foreach (var val in unk.Values)
                {
                    xw.WriteElementString("value", val.ToString());
                }
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            xw.WriteElementString("unkBlock", Convert.ToBase64String(gparam.UnkBlock2));

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceFile)
        {
            GPARAM gparam = new GPARAM();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);
            gparam.Unk1 = int.Parse(xml.SelectSingleNode("gparam/unk1").InnerText);
            foreach (XmlNode group in xml.SelectNodes("gparam/groups/group"))
            {
                var g = new GPARAM.Group();
                g.Name1 = group.Attributes.GetNamedItem("name1").InnerText;
                g.Name2 = group.Attributes.GetNamedItem("name2").InnerText;
                bool addedComments = false;
                foreach (XmlNode param in group.SelectNodes("param"))
                {
                    var p = new GPARAM.Param();
                    p.Name1 = param.Attributes.GetNamedItem("name1").InnerText;
                    p.Name2 = param.Attributes.GetNamedItem("name2").InnerText;
                    p.Type = (GPARAM.ParamType)Enum.Parse(typeof(GPARAM.ParamType), param.Attributes.GetNamedItem("type").InnerText);
                    foreach (XmlNode value in param.SelectNodes("value"))
                    {
                        p.Unk1Values.Add(int.Parse(value.Attributes.GetNamedItem("id").InnerText));
                        if (!addedComments)
                        {
                            g.Comments.Add(value.Attributes.GetNamedItem("comment").InnerText);
                        }
                        switch (p.Type)
                        {
                            case GPARAM.ParamType.BoolA:
                                p.Values.Add(bool.Parse(value.InnerText));
                                break;
                            case GPARAM.ParamType.BoolB:
                                p.Values.Add(bool.Parse(value.InnerText));
                                break;
                            case GPARAM.ParamType.Byte:
                                p.Values.Add(byte.Parse(value.InnerText));
                                break;
                            case GPARAM.ParamType.Byte4:
                                var str = value.InnerText;
                                p.Values.Add(str.Split(' ').Select(x => byte.Parse(x)).ToArray());
                                break;
                            case GPARAM.ParamType.Float:
                                p.Values.Add(float.Parse(value.InnerText));
                                break;
                            case GPARAM.ParamType.Float2:
                                var arr = value.InnerText.Split(' ').Select(x => float.Parse(x)).ToArray();
                                p.Values.Add(new Vector2(arr[0], arr[1]));
                                break;
                            case GPARAM.ParamType.Float4:
                                var arr2 = value.InnerText.Split(' ').Select(x => float.Parse(x)).ToArray();
                                p.Values.Add(new Vector4(arr2[0], arr2[1], arr2[2], arr2[3]));
                                break;
                            case GPARAM.ParamType.IntA:
                                p.Values.Add(int.Parse(value.InnerText));
                                break;
                            case GPARAM.ParamType.IntB:
                                p.Values.Add(int.Parse(value.InnerText));
                                break;
                            case GPARAM.ParamType.Short:
                                p.Values.Add(short.Parse(value.InnerText));
                                break;
                        }
                    }
                    addedComments = true;
                    g.Params.Add(p);
                }
                gparam.Groups.Add(g);
            }

            foreach (XmlNode unk in xml.SelectNodes("gparam/unk3s/unk3"))
            {
                var u = new GPARAM.Unk3();
                u.ID = int.Parse(unk.Attributes.GetNamedItem("id").InnerText);
                foreach (XmlNode value in unk.SelectNodes("value"))
                {
                    u.Values.Add(int.Parse(value.InnerText));
                }
                gparam.Unk3s.Add(u);
            }

            gparam.UnkBlock2 = Convert.FromBase64String(xml.SelectSingleNode("gparam/unkBlock").InnerText);

            if (sourceFile.EndsWith(".gparam.xml"))
            {
                string outPath = sourceFile.Replace(".gparam.xml", ".gparam");
                if (File.Exists(outPath) && !File.Exists(outPath + ".bak"))
                    File.Copy(outPath, outPath + ".bak");
                gparam.Write(outPath);
            }
            else
            {
                string outPath = sourceFile.Replace(".gparam.dcx.xml", ".gparam.dcx");
                if (File.Exists(outPath) && !File.Exists(outPath + ".bak"))
                    File.Copy(outPath, outPath + ".bak");
                gparam.Write(outPath, DCX.Type.DarkSouls3);
            }
        }
    }
}
