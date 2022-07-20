using ModelCompiler;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ua.model.sdk.Managers;

namespace ua.model.sdk.Model
{
    public class uaModelDesign
    {
        public ModelDesign? ModelDesign { get; set; }


        private uaModelDesignManager _uaModelDesignManager;
        public uaModelDesignManager uaModelDesignManager
        {
            get
            {
                if (_uaModelDesignManager == null) _uaModelDesignManager = new uaModelDesignManager(ModelDesign);
                return _uaModelDesignManager;
            }
        }


        public Dictionary<string, uaNameSpace> uaNameSpaces
        {
            get
            {
                var returnObject = new Dictionary<string, uaNameSpace>();
                if (ModelDesign.Namespaces == null) ModelDesign.Namespaces = new Namespace[] { };
                foreach (var aNameSpace in ModelDesign.Namespaces)
                {
                    returnObject.Add(aNameSpace.Name, new uaNameSpace(aNameSpace));
                }
                return returnObject;
            }
        }

        private uaNameSpaceManager _uaNameSpaceManager;
        public uaNameSpaceManager uaNameSpaceManager
        {
            get
            {
                if (_uaNameSpaceManager == null) _uaNameSpaceManager = new uaNameSpaceManager(ModelDesign);
                return _uaNameSpaceManager;
            }
        }


        public List<uaObjectTypeDesign> uaObjectTypeDesigns
        {
            get
            {
                var returnObject = new List<uaObjectTypeDesign>();
                if (ModelDesign.Items == null) ModelDesign.Items = new NodeDesign[] { };
                foreach (ObjectTypeDesign aObjectTypeDesign in ModelDesign.Items.Where(x => x.GetType() == typeof(ObjectTypeDesign)))
                {
                    returnObject.Add(new uaObjectTypeDesign(aObjectTypeDesign));
                }
                return returnObject;
            }
        }

        private uaObjectTypeDesignManager _uaObjectTypeDesignManager;
        public uaObjectTypeDesignManager uaObjectTypeDesignManager
        {
            get
            {
                if (_uaObjectTypeDesignManager == null) _uaObjectTypeDesignManager = new uaObjectTypeDesignManager(ModelDesign);
                return _uaObjectTypeDesignManager;
            }
        }


        public uaModelDesign(ModelDesign modelDesign)
        {
            ModelDesign = modelDesign;
        }

        public uaModelDesign(string domain, string name)
        {

            ModelDesign = new ModelDesign();
            ModelDesign.TargetNamespace = $"{domain}/{name}/";
            ModelDesign.TargetXmlNamespace = $"{domain}/{name}/";

            uaModelDesignManager.XmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            uaModelDesignManager.XmlSerializerNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

            uaModelDesignManager.XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            uaModelDesignManager.XmlSerializerNamespaces.Add("ua", "http://opcfoundation.org/UA/");
            uaModelDesignManager.XmlSerializerNamespaces.Add(name, $"{domain}/{name}/");

            uaNameSpaceManager.AddVanillaUaNameSpace();
            uaNameSpaceManager.AddBasicNameSpace(domain, name);

        }

        public uaModelDesign(string fileUrl)
        {
            var xmlSerializer = new XmlSerializer(typeof(ModelDesign));
            using (FileStream fileStream = new FileStream(fileUrl, FileMode.Open))
            {
                ModelDesign = (ModelDesign)xmlSerializer.Deserialize(fileStream);

                uaModelDesignManager.XmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                uaModelDesignManager.XmlSerializerNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

                uaModelDesignManager.XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
                uaModelDesignManager.XmlSerializerNamespaces.Add("ua", "http://opcfoundation.org/UA/");

            }
        }
    }
}