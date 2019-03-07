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
            xw.WriteElementString("compression", gparam.Compression.ToString());
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
            Enum.TryParse(xml.SelectSingleNode("gparam/compression").InnerText, out gparam.Compression);
            gparam.Unk1 = int.Parse(xml.SelectSingleNode("gparam/unk1").InnerText);
            foreach (XmlNode groupNode in xml.SelectNodes("gparam/groups/group"))
            {
                string groupName1 = groupNode.Attributes["name1"].InnerText;
                string groupName2 = groupNode.Attributes["name2"].InnerText;
                var group = new GPARAM.Group(groupName1, groupName2);
                bool addedComments = false;
                foreach (XmlNode paramNode in groupNode.SelectNodes("param"))
                {
                    string paramName1 = paramNode.Attributes["name1"].InnerText;
                    string paramName2 = paramNode.Attributes["name2"].InnerText;
                    var paramType = (GPARAM.ParamType)Enum.Parse(typeof(GPARAM.ParamType), paramNode.Attributes["type"].InnerText);
                    var param = new GPARAM.Param(paramName1, paramName2, paramType);
                    foreach (XmlNode value in paramNode.SelectNodes("value"))
                    {
                        param.Unk1Values.Add(int.Parse(value.Attributes["id"].InnerText));
                        if (!addedComments)
                        {
                            group.Comments.Add(value.Attributes["comment"].InnerText);
                        }

                        switch (param.Type)
                        {
                            case GPARAM.ParamType.BoolA:
                                param.Values.Add(bool.Parse(value.InnerText));
                                break;

                            case GPARAM.ParamType.BoolB:
                                param.Values.Add(bool.Parse(value.InnerText));
                                break;

                            case GPARAM.ParamType.Byte:
                                param.Values.Add(byte.Parse(value.InnerText));
                                break;

                            case GPARAM.ParamType.Byte4:
                                param.Values.Add(value.InnerText.Split(' ').Select(x => byte.Parse(x)).ToArray());
                                break;

                            case GPARAM.ParamType.Float:
                                param.Values.Add(float.Parse(value.InnerText));
                                break;

                            case GPARAM.ParamType.Float2:
                                float[] arr = value.InnerText.Split(' ').Select(x => float.Parse(x)).ToArray();
                                param.Values.Add(new Vector2(arr[0], arr[1]));
                                break;

                            case GPARAM.ParamType.Float4:
                                float[] arr2 = value.InnerText.Split(' ').Select(x => float.Parse(x)).ToArray();
                                param.Values.Add(new Vector4(arr2[0], arr2[1], arr2[2], arr2[3]));
                                break;

                            case GPARAM.ParamType.IntA:
                                param.Values.Add(int.Parse(value.InnerText));
                                break;

                            case GPARAM.ParamType.IntB:
                                param.Values.Add(int.Parse(value.InnerText));
                                break;

                            case GPARAM.ParamType.Short:
                                param.Values.Add(short.Parse(value.InnerText));
                                break;
                        }
                    }
                    addedComments = true;
                    group.Params.Add(param);
                }
                gparam.Groups.Add(group);
            }

            foreach (XmlNode unk in xml.SelectNodes("gparam/unk3s/unk3"))
            {
                int id = int.Parse(unk.Attributes["id"].InnerText);
                var unk3 = new GPARAM.Unk3(id);
                foreach (XmlNode value in unk.SelectNodes("value"))
                {
                    unk3.Values.Add(int.Parse(value.InnerText));
                }
                gparam.Unk3s.Add(unk3);
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
