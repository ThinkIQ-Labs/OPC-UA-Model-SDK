using ModelCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ua.model.sdk.Model;

namespace ua.model.sdk.Managers
{
    public class uaNameSpaceManager
    {
        ModelDesign ModelDesign { get; set; }
        public uaNameSpaceManager(ModelDesign modelDesign)
        {
            ModelDesign = modelDesign;
        }

        List<uaNameSpace> uaNameSpaces
        {
            get
            {
                List<uaNameSpace> returnObject = new();
                foreach (var aNameSpace in ModelDesign.Namespaces)
                {
                    returnObject.Add(new uaNameSpace(aNameSpace));
                }
                return returnObject;
            }
        }

        // <Namespace Name="animal" Prefix="animal"
        //      XmlNamespace="https://opcua.rocks/UA/animal/Types.xsd" XmlPrefix="animal">https://opcua.rocks/UA/animal/</Namespace>
        public void AddBasicNameSpace(string domain, string name)
        {
            if (ModelDesign.Namespaces == null) ModelDesign.Namespaces = new Namespace[] { };
            List<Namespace> namespaces = ModelDesign.Namespaces.ToList();
            namespaces.Add(new Namespace
            {
                Name = name,
                Prefix = name,
                XmlNamespace = $"{domain}/{name}/Types.xsd",
                XmlPrefix = name,
                Value = $"{domain}/{name}/"
            });
            ModelDesign.Namespaces = namespaces.ToArray();
        }

        // <Namespace Name="OpcUa" Version="1.03" PublicationDate="2013-12-02T00:00:00Z"
        //      Prefix="Opc.Ua" InternalPrefix="Opc.Ua.Server"
        //      XmlNamespace="http://opcfoundation.org/UA/2008/02/Types.xsd" XmlPrefix="OpcUa">http://opcfoundation.org/UA/</Namespace>
        public void AddVanillaUaNameSpace()
        {
            if (ModelDesign.Namespaces == null) ModelDesign.Namespaces = new Namespace[] { };
            List<Namespace> namespaces = ModelDesign.Namespaces.ToList();
            namespaces.Add(new Namespace
            {
                Name = "OpcUa",
                Version = "1.03",
                PublicationDate = "2013-12-02T00:00:00Z",
                Prefix = "Opc.Ua",
                InternalPrefix = "Opc.Ua.Server",
                XmlNamespace = "http://opcfoundation.org/UA/2008/02/Types.xsd",
                XmlPrefix = "OpcUa",
                Value = "http://opcfoundation.org/UA/"
            });
            ModelDesign.Namespaces = namespaces.ToArray();
        }
    }
}
