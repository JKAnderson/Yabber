using SoulsFormats;
using System;
using System.IO;
using System.Numerics;
using System.Xml;

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
                            xw.WriteString($"{value.X}, {value.Y}");
                        }
                        else if (param.Type == GPARAM.ParamType.Float4)
                        {
                            var value = (Vector4)param.Values[i];
                            xw.WriteString($"{value.X}, {value.Y}, {value.Z}, {value.W}");
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

            // To implement...
        }
    }
}
