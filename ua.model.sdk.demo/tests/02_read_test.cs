using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    public static class _02_read_test
    {
        public static void Run()
        {

            // thank you Macheronte for the tutorial
            // https://www.macheronte.com/en/opc-ua-model-design-lets-create-a-data-model/

            uaModelDesign md = new uaModelDesign("./data/modeldesign.xml");
            md.ModelDesign.TargetNamespace = "http://opcfoundation.org/OPCUAServer";

            md.uaModelDesignManager.XmlSerializerNamespaces.Add("opc", "http://opcfoundation.org/UA/ModelDesign.xsd");
            // use string.Empty to create xml namespace without prefix
            md.uaModelDesignManager.XmlSerializerNamespaces.Add(string.Empty, "http://opcfoundation.org/OPCUAServer");

            Console.WriteLine( md.uaModelDesignManager.GenerateXML());
            md.uaModelDesignManager.GenerateXML("./data/ModelDesign2.xml");
        }
    }
}
