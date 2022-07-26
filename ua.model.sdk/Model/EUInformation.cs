using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
                    var lines = File.ReadAllLines("./Schema/UNECE_to_OPCUA.csv");
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
                            list.Add(new EUInformation
                            {
                                UnitId = int.Parse(data[1]),
                                DisplayName = new LocalizedText { Text = data[2].Replace(@"""","") },
                                Description = new LocalizedText { Text = data[3].Replace(@"""", "") }
                            });
                        }
                    }
                    _EUInformationList = list;
                }
                return _EUInformationList;
            }
        }
        public XmlElement CreateXmlElement(string uaxPrefix="uax")
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlElement eObject = xmlDocument.CreateElement(uaxPrefix, "ExtensionObject", "http://opcfoundation.org/UA/2008/02/Types.xsd");

            var typeId = xmlDocument.CreateElement(uaxPrefix, "TypeId", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var identifier = xmlDocument.CreateElement(uaxPrefix, "Identifier", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            identifier.InnerText = "i=888";
            typeId.AppendChild(identifier);
            eObject.AppendChild(typeId);

            var body = xmlDocument.CreateElement(uaxPrefix, "Body", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var eui = xmlDocument.CreateElement(uaxPrefix, "EUInformation", "http://opcfoundation.org/UA/2008/02/Types.xsd");

            var nsUri = xmlDocument.CreateElement(uaxPrefix, "NamespaceUri", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            nsUri.InnerText = "http://www.opcfoundation.org/UA/units/un/cefact";
            eui.AppendChild(nsUri);

            var unitId = xmlDocument.CreateElement(uaxPrefix, "UnitId", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            unitId.InnerText = UnitId.ToString();
            eui.AppendChild(unitId);

            var dispName = xmlDocument.CreateElement(uaxPrefix, "DisplayName", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var dispText = xmlDocument.CreateElement(uaxPrefix, "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            dispText.InnerText = DisplayName.Text; // "m";
            dispName.AppendChild(dispText);
            eui.AppendChild(dispName);


            var descrName = xmlDocument.CreateElement(uaxPrefix, "Description", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var descrText = xmlDocument.CreateElement(uaxPrefix, "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            descrText.InnerText = Description.Text; // "metre";
            descrName.AppendChild(descrText);
            eui.AppendChild(descrName);

            body.AppendChild(eui);

            eObject.AppendChild(body);

            return eObject;
        }
    }
}
