using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using System.Numerics;

namespace Yabber
{
    static class YGPARAM
    {
        private static string TypeToString(GPARAM.ParamType type)
        {
            switch (type)
            {
                case GPARAM.ParamType.Unk1:
                    return "byte";
                case GPARAM.ParamType.Unk2:
                    return "short";
                case GPARAM.ParamType.Unk3:
                    return "int";
                case GPARAM.ParamType.Unk5:
                    return "bool";
                case GPARAM.ParamType.Unk7:
                    return "int2";
                case GPARAM.ParamType.Unk9:
                    return "float";
                case GPARAM.ParamType.UnkB:
                    return "bool";
                case GPARAM.ParamType.UnkC:
                    return "fvec2";
                case GPARAM.ParamType.UnkE:
                    return "fvec4";
                case GPARAM.ParamType.UnkF:
                    return "bytes";
            }
            return null;
        }

        public static void Unpack(this GPARAM gparam, string sourceFile)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "    ";
            XmlWriter xw = XmlWriter.Create($"{sourceFile}.xml", xws);
            xw.WriteStartElement("gparam");
            xw.WriteElementString("unk1", gparam.Unk1.ToString());
            xw.WriteStartElement("groups");
            foreach (var group in gparam.Groups)
            {
                xw.WriteStartElement("group");
                xw.WriteAttributeString("name1", group.Name1);
                xw.WriteAttributeString("name2", group.Name2);
                for (var p = 0; p < group.Params.Count; p++)
                {
                    var param = group.Params[p];
                    xw.WriteStartElement("params");
                    xw.WriteAttributeString("name1", param.Name1);
                    xw.WriteAttributeString("name2", param.Name2);
                    xw.WriteAttributeString("type", TypeToString(param.Type));
                    for (var i = 0; i < param.Values.Count; i++)
                    {
                        xw.WriteStartElement("value");
                        xw.WriteAttributeString("id", param.Unk1Values[i].ToString());
                        if (i < group.Comments.Count)
                            xw.WriteAttributeString("comment", group.Comments[i]);
                        if (param.Values[i] is Vector2)
                        {
                            var value = (Vector2)param.Values[i];
                            xw.WriteString($@"{value.X} {value.Y}");
                        }
                        else if (param.Values[i] is Vector4)
                        {
                            var value = (Vector4)param.Values[i];
                            xw.WriteString($@"{value.X} {value.Y} {value.Z} {value.W}");
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

            xw.WriteElementString("unkBlock", string.Join(" ", gparam.UnkBlock2));

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
