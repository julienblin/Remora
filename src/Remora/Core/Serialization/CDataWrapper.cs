using System;
using System.Xml;
using System.Xml.Serialization;

namespace Remora.Core.Serialization
{
    public sealed class CDataWrapper : IXmlSerializable
    {
        
        public static implicit operator string(CDataWrapper value)
        {
            return value == null ? null : value.Value;
        }

        public static implicit operator CDataWrapper(string value)
        {
            return value == null ? null : new CDataWrapper { Value = value };
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        
        public void WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Value))
            {
                writer.WriteCData(Value);
            }
        }
        
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                Value = "";
            }
            else
            {
                reader.Read();
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        Value = ""; // empty after all...
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        Value = reader.ReadContentAsString();
                        break;
                    default:
                        throw new InvalidOperationException("Expected text/cdata");
                }
            }
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
