using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    internal class _05_parse_EUInformation
    {
        public static void Run()
        {
            ModelDesign md = ModelDesign.InitializeFromFile("./out/uomModel.xml");

            var propDesign = md.ObjectTypeDesigns.First().Value.VariableDesigns.First().Value.PropertyDesigns.First().Value;

            EUInformation info = EUInformation.EUInformationFromXML(propDesign.DefaultValue);


        }

    }

}
