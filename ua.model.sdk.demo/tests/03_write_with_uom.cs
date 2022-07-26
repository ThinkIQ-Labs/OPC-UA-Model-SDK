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

            var propEU = variableHeight.PropertyDesignsAdd(
                new XmlQualifiedName("EngineeringUnits", animalNameSpace),
                new XmlQualifiedName("EUInformation", opcUaNameSpace)
                );

            EUInformation eui = EUInformation.EUInformationList.First(x => x.DisplayName.Text == "m");
            propEU.DefaultValue = eui.CreateXmlElement();


            Console.WriteLine(md.GenerateModelXML("./out/uomModel.xml"));

            Console.WriteLine(md.GenerateNodesetXML(null, "./out/uomNodeset.xml"));
        }
    }
}
