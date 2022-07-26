using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    internal class _03_write_with_uom
    {

        private static void SetPrefix(string prefix, XmlNode node)
        {
            node.Prefix = prefix;
            foreach (XmlNode n in node.ChildNodes)
            {
                //if (node.ParentNode != null)
                //{
                if (n.Name.Contains("QualifyingProperties"))
                {
                    break;
                }
                //}
                SetPrefix(prefix, n);
            }
        }


        public static void Run()
        {
            var md = new ModelDesign("https://opcua.rocks/UA", "animal");

            var animalNameSpace = md.NamespacesDictionary["animal"].Value;
            var opcUaNameSpace = md.NamespacesDictionary["OpcUa"].Value;

            var animalType = md.ObjectTypeDesignsAdd(
                new XmlQualifiedName("AnimalType", animalNameSpace),
                new XmlQualifiedName("BaseObjectType", opcUaNameSpace)
                );

            var propName = animalType.PropertyDesignsAdd(
                new XmlQualifiedName("Name", animalNameSpace),
                new XmlQualifiedName("String", opcUaNameSpace)
                );

            // add variable for height with unit 'm'
            var variableHeight = animalType.VariableDesignsAdd(
                new XmlQualifiedName("Height", animalNameSpace),
                new XmlQualifiedName("Float", opcUaNameSpace)
                );

            var eum = variableHeight.PropertyDesignsAdd(
                new XmlQualifiedName("EngineeringUnits", animalNameSpace),
                new XmlQualifiedName("EUInformation", opcUaNameSpace)
                );


            //< DefaultValue >
            //    < uax:ExtensionObject >
            //        < uax:TypeId >
            //            < uax:Identifier > i = 888 </ uax:Identifier >
            //        </ uax:TypeId >
            //        < uax:Body >
            //            < uax:EUInformation >
            //                < uax:NamespaceUri > http://www.opcfoundation.org/UA/units/un/cefact</uax:NamespaceUri>
            //                < uax:UnitId > 5067858 </ uax:UnitId >
            //                < uax:DisplayName >
            //                    < uax:Text > m </ uax:Text >
            //                </ uax:DisplayName >
            //                < uax:Description >
            //                    < uax:Text > metre </ uax:Text >
            //                </ uax:Description >
            //            </ uax:EUInformation >
            //        </ uax:Body >
            //    </ uax:ExtensionObject >
            //</ DefaultValue >

            XmlDocument xmlDocument = new XmlDocument();
            
            XmlElement eObject = xmlDocument.CreateElement("uax", "ExtensionObject", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            
            var typeId = xmlDocument.CreateElement("uax", "TypeId", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var identifier = xmlDocument.CreateElement("uax", "Identifier", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            identifier.InnerText = "i=888";
            typeId.AppendChild(identifier);
            eObject.AppendChild(typeId);

            var body = xmlDocument.CreateElement("uax", "Body", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var eui= xmlDocument.CreateElement("uax", "EUInformation", "http://opcfoundation.org/UA/2008/02/Types.xsd");

            var nsUri = xmlDocument.CreateElement("uax", "NamespaceUri", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            nsUri.InnerText = "http://www.opcfoundation.org/UA/units/un/cefact";
            eui.AppendChild(nsUri);

            var unitId = xmlDocument.CreateElement("uax", "UnitId", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            unitId.InnerText = "5067858";
            eui.AppendChild(unitId);

            var dispName = xmlDocument.CreateElement("uax", "DisplayName", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var dispText = xmlDocument.CreateElement("uax", "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            dispText.InnerText = "m";
            dispName.AppendChild(dispText);
            eui.AppendChild(dispName);


            var descrName = xmlDocument.CreateElement("uax", "Description", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            var descrText = xmlDocument.CreateElement("uax", "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            descrText.InnerText = "metre";
            descrName.AppendChild(descrText);
            eui.AppendChild(descrName);

            body.AppendChild(eui);

            eObject.AppendChild(body);

            eum.DefaultValue = eObject;




            // this is what we should have used
            // but couldn't get the serialization to work

            //var extensionObject = new ExtensionObject();
            //extensionObject.TypeId = new ExpandedNodeId { Identifier = "i=888" };
            //EUInformation eui2 = new EUInformation
            //{
            //    NamespaceUri = "http://www.opcfoundation.org/UA/units/un/cefact",
            //    UnitId = 5067858,
            //    DisplayName = new Opc.Ua.LocalizedText()
            //    {
            //        Text = "m"
            //    },
            //    Description = new Opc.Ua.LocalizedText()
            //    {
            //        Text = "metre"
            //    }
            //};

            ////XmlSerializerNamespaces XmlSerializerNamespaces = new XmlSerializerNamespaces();
            //XmlSerializer aXmlSerializer = new XmlSerializer(typeof(EUInformation));
            //using (TextWriter writer = new StringWriter())
            //using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            //{
            //    aXmlSerializer.Serialize(xmlWriter, eui, md.XmlSerializerNamespaces);
            //    Console.WriteLine(writer.ToString());
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(writer.ToString());
            //    XmlNode newNode = doc.DocumentElement;
            //    extensionObject.Body = new ExtensionObjectBody
            //    {
            //        Nodes = new XmlNode[] { newNode }
            //    };
            //}

            ////XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            //XmlSerializer aXmlSerializer2 = new XmlSerializer(typeof(ExtensionObject));
            //using (TextWriter writer = new StringWriter())
            //using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            //{
            //    aXmlSerializer2.Serialize(xmlWriter, extensionObject);
            //    Console.WriteLine(writer.ToString());
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(writer.ToString());

            //    //SetPrefix("uax", doc.DocumentElement);


            //    eum.DefaultValue = doc.DocumentElement;

            //}




            Console.WriteLine(md.GenerateModelXML("./out/uomModel.xml"));

            Console.WriteLine(md.GenerateNodesetXML(null, "./out/uomNodeset.xml"));
        }
    }
}
