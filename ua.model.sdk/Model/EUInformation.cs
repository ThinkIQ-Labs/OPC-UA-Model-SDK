using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ua.model.sdk.Properties;

namespace Opc.Ua
{
    public partial class EUInformation
    {

        //UNECECode,UnitId,DisplayName,Description
        private static List<EUInformation> _EUInformationList;
        public static List<EUInformation> EUInformationList
        {
            get
            {
                if (_EUInformationList == null)
                {
                    List<EUInformation> list = new List<EUInformation>();

                    var lines = Resources.UNECE_to_OPCUA.Split("\r\n");

                        var isHeader = true;
                    foreach (var aLine in lines)
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                        }
                        else
                        {
                            var data = aLine.Split(",");
                            if (data.Length == 4)
                            {
                                list.Add(new EUInformation
                                {
                                    UnitId = int.Parse(data[1]),
                                    DisplayName = new LocalizedText { Text = data[2].Replace(@"""", "") },
                                    Description = new LocalizedText { Text = data[3].Replace(@"""", "") }
                                });
                            }
                        }
                    }
                    _EUInformationList = list;
                }
                return _EUInformationList;
            }
        }

        // using the xsd based classes to serialize EUInformation object we don't need this anymore
        // yay!!!

        //public XmlElement CreateXmlElement(string uaxPrefix="uax")
        //{
        //    XmlDocument xmlDocument = new XmlDocument();

        //    XmlElement eObject = xmlDocument.CreateElement(uaxPrefix, "ExtensionObject", "http://opcfoundation.org/UA/2008/02/Types.xsd");

        //    var typeId = xmlDocument.CreateElement(uaxPrefix, "TypeId", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    var identifier = xmlDocument.CreateElement(uaxPrefix, "Identifier", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    identifier.InnerText = "i=888";
        //    typeId.AppendChild(identifier);
        //    eObject.AppendChild(typeId);

        //    var body = xmlDocument.CreateElement(uaxPrefix, "Body", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    var eui = xmlDocument.CreateElement(uaxPrefix, "EUInformation", "http://opcfoundation.org/UA/2008/02/Types.xsd");

        //    var nsUri = xmlDocument.CreateElement(uaxPrefix, "NamespaceUri", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    nsUri.InnerText = "http://www.opcfoundation.org/UA/units/un/cefact";
        //    eui.AppendChild(nsUri);

        //    var unitId = xmlDocument.CreateElement(uaxPrefix, "UnitId", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    unitId.InnerText = UnitId.ToString();
        //    eui.AppendChild(unitId);

        //    var dispName = xmlDocument.CreateElement(uaxPrefix, "DisplayName", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    var dispText = xmlDocument.CreateElement(uaxPrefix, "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    dispText.InnerText = DisplayName.Text; // "m";
        //    dispName.AppendChild(dispText);
        //    eui.AppendChild(dispName);


        //    var descrName = xmlDocument.CreateElement(uaxPrefix, "Description", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    var descrText = xmlDocument.CreateElement(uaxPrefix, "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
        //    descrText.InnerText = Description.Text; // "metre";
        //    descrName.AppendChild(descrText);
        //    eui.AppendChild(descrName);

        //    body.AppendChild(eui);

        //    eObject.AppendChild(body);

        //    return eObject;
        //}

        public XmlElement XMLFromEUInformation()
        {
            ExtensionObjectForEUInformation extensionObject = new ExtensionObjectForEUInformation();
            extensionObject.TypeId = new ExpandedNodeId()
            {
                Identifier = "888"
            };
            extensionObject.Body = new EUInformationBody()
            {
                EUInformation = this
            };

            XmlSerializerNamespaces XmlSerializerNamespaces = new XmlSerializerNamespaces();
            XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            XmlSerializer XmlSerializer = new XmlSerializer(typeof(ExtensionObjectForEUInformation));
            using (TextWriter writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            {
                XmlSerializer.Serialize(xmlWriter, extensionObject, XmlSerializerNamespaces);
                var xml = writer.ToString();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                return xmlDoc.DocumentElement;
            }
        }

        public static EUInformation EUInformationFromXML(XmlElement xmlElement)
        {
            ExtensionObjectForEUInformation defaultValueEuInformation = new ExtensionObjectForEUInformation();

            var xmlSerializer = new XmlSerializer(typeof(ExtensionObjectForEUInformation));

            StringReader rdr = new StringReader(xmlElement.OuterXml);

            defaultValueEuInformation = (ExtensionObjectForEUInformation)xmlSerializer.Deserialize(rdr);

            return defaultValueEuInformation.Body.EUInformation;
        }

    }

    [XmlRoot("ExtensionObject", Namespace = "http://opcfoundation.org/UA/2008/02/Types.xsd")]
    public class ExtensionObjectForEUInformation
    {
        [XmlElement("TypeId")]
        public ExpandedNodeId TypeId { get; set; }

        [XmlElement("Body")]
        public EUInformationBody Body { get; set; }
    }

    public class EUInformationBody
    {
        [XmlElement("EUInformation")]
        public EUInformation EUInformation { get; set; }
    }

}
