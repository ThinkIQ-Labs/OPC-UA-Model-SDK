using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ua.model.sdk.Managers;
using UAOOI.SemanticData.UAModelDesignExport.XML;

namespace ua.model.sdk.Model
{
    public class uaModelDesign
    {
        public ModelDesign? ModelDesign { get; set; }
        
        
        public Dictionary<string, uaNameSpace> uaNameSpaces
        {
            get
            {
                var returnObject = new Dictionary<string, uaNameSpace>();
                foreach(var aNameSpace in ModelDesign.Namespaces)
                {
                    returnObject.Add(aNameSpace.XmlPrefix, new uaNameSpace(aNameSpace));
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
                foreach (ObjectTypeDesign aObjectTypeDesign in ModelDesign.Items.Where(x=>x.GetType()==typeof(ObjectTypeDesign)))
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
        
        XmlSerializer XmlSerializer { get; set; }
        public XmlSerializerNamespaces XmlSerializerNamespaces { get; set; }

        public uaModelDesign(string domain, string name)
        {
            XmlSerializer = new XmlSerializer(typeof(ModelDesign));
            XmlSerializerNamespaces = new XmlSerializerNamespaces();
            XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            XmlSerializerNamespaces.Add("ua", "http://opcfoundation.org/UA/");
            XmlSerializerNamespaces.Add(name, $"{domain}/{name}/");

            ModelDesign = new ModelDesign();
            ModelDesign.TargetNamespace = $"{domain}/{name}/";
            ModelDesign.TargetXmlNamespace = $"{domain}/{name}/";

            uaNameSpaceManager.AddVanillaUaNameSpace();
            uaNameSpaceManager.AddBasicNameSpace(domain, name);

        }

        public string GenerateXML(string? fileUrl =null)
        {
            using (TextWriter writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            {
                XmlSerializer.Serialize(xmlWriter, ModelDesign, XmlSerializerNamespaces);
                if (fileUrl != null)
                {
                    File.WriteAllText(fileUrl, writer.ToString());
                }
                return writer.ToString();
            }
        }

        public string GenerateCSV(string? fileUrl =null)
        {
            using (TextWriter writer = new StringWriter())
            {
                var typeCounter = 0;
                var propertyCounter = 0;
                foreach (var aType in uaObjectTypeDesigns)
                {
                    typeCounter++;
                    writer.WriteLine($"{aType.ObjectTypeDesign.SymbolicName.Name},{10000 + typeCounter},ObjectType");
                    foreach (var aProperty in aType.ObjectTypeDesign.Children.Items)
                    {
                        propertyCounter++;
                        writer.WriteLine($"{aProperty.SymbolicName.Name},{20000 + propertyCounter},Variable");
                    }
                }
                if (fileUrl != null)
                {
                    File.WriteAllText(fileUrl, writer.ToString());
                }
                return writer.ToString();
            }
        }

        public void CompileNodeset(string compilerExecutableUrl, string xmlFileUrl, string csvFileUrl, string outputDirUrl)
        {
            var args = new string[]
                {
                    "compile",
                    "-d2",
                    xmlFileUrl,
                    "-cg",
                    csvFileUrl,
                    "-o2",
                    outputDirUrl
                };
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = $"{compilerExecutableUrl}";
            startInfo.Arguments = String.Join(" ", args);

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }

    }
}