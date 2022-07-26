using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    internal class _04_read_with_uom
    {
        public static void Run()
        {
            ModelDesign md = ModelDesign.InitializeFromFile("./out/uomModel.xml");
            md.TargetNamespace = "https://opcua.rocks/UA/animal/";

            md.XmlSerializerNamespaces.Add(String.Empty, "http://opcfoundation.org/UA/ModelDesign.xsd");
            md.XmlSerializerNamespaces.Add("animal", "https://opcua.rocks/UA/animal/");

            md.GenerateNodesetXML(null, "./out/uomNodeset.xml");
            //Console.WriteLine(md.GenerateModelXML());
            //md.GenerateModelXML("./data/ModelDesign2.xml");


        }
    }
}
