using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    public static class _06_convert_model
    {
        public static void Run()
        {

            // thank you Macheronte for the tutorial
            // https://www.macheronte.com/en/opc-ua-model-design-lets-create-a-data-model/

            ModelDesign md = ModelDesign.InitializeFromFile("./data/modeldesignmotor.xml");
            md.TargetNamespace = "http://opcfoundation.org/OPCUAServer";

            md.XmlSerializerNamespaces.Add("opc", "http://opcfoundation.org/UA/ModelDesign.xsd");
            // use string.Empty to create xml namespace without prefix
            md.XmlSerializerNamespaces.Add(string.Empty, "http://opcfoundation.org/OPCUAServer");

            Console.WriteLine(md.GenerateModelXML());
            md.GenerateModelXML("./data/ModelDesignMotor2.xml");
        }
    }
}
