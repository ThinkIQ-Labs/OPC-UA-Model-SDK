using ModelCompiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ua.model.sdk.Model
{
    public partial class ModelDesign
    {
        [XmlIgnore]
        public XmlSerializer XmlSerializer = new XmlSerializer(typeof(ModelDesign));
        [XmlIgnore]
        public XmlSerializerNamespaces XmlSerializerNamespaces = new XmlSerializerNamespaces();

        public ModelDesign(string domain, string name)
        {

            TargetNamespace = $"{domain}/{name}/";
            TargetXmlNamespace = $"{domain}/{name}/";

            XmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            XmlSerializerNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

            XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            XmlSerializerNamespaces.Add("ua", "http://opcfoundation.org/UA/");
            XmlSerializerNamespaces.Add(name, $"{domain}/{name}/");

            NamespacesAddUa();
            NamespacesAdd(domain, name);

        }

        public static ModelDesign InitializeFromFile(string fileUrl)
        {
            ModelDesign returnObject = new ModelDesign();
            var xmlSerializer = new XmlSerializer(typeof(ModelDesign));
            using (FileStream fileStream = new FileStream(fileUrl, FileMode.Open))
            {
                returnObject = (ModelDesign)xmlSerializer.Deserialize(fileStream);

                returnObject.XmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                returnObject.XmlSerializerNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

                returnObject.XmlSerializerNamespaces.Add("uax", "http://opcfoundation.org/UA/2008/02/Types.xsd");
                returnObject.XmlSerializerNamespaces.Add("ua", "http://opcfoundation.org/UA/");
            }
            return returnObject;
        }

        [XmlIgnore]
        public Dictionary<string, Namespace> NamespacesDictionary
        {
            get
            {
                Dictionary<string, Namespace> returnObject = new Dictionary<string, Namespace>();
                if (Namespaces == null) Namespaces = new Namespace[] { };
                foreach(var aNamespace in Namespaces)
                {
                    returnObject.Add(aNamespace.Name, aNamespace);
                }
                return returnObject;
            }
        }
        public void NamespacesAdd(Namespace aNamespace)
        {
            if (Namespaces == null) Namespaces = new Namespace[] { };
            var ns = Namespaces.ToList();
            ns.Add(aNamespace);
            Namespaces = ns.ToArray();
        }
        public void NamespacesAdd(string domain, string name)
        {
            if (Namespaces == null) Namespaces = new Namespace[] { };
            List<Namespace> ns = Namespaces.ToList();
            ns.Add(new Namespace
            {
                Name = name,
                Prefix = name,
                XmlNamespace = $"{domain}/{name}/Types.xsd",
                XmlPrefix = name,
                Value = $"{domain}/{name}/"
            });
            Namespaces = ns.ToArray();
        }
        public void NamespacesAddUa()
        {
            if (Namespaces == null) Namespaces = new Namespace[] { };
            List<Namespace> ns = Namespaces.ToList();
            ns.Add(new Namespace
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
            Namespaces = ns.ToArray();
        }


        [XmlIgnore]
        public Dictionary<string, ObjectTypeDesign> ObjectTypeDesigns
        {
            get
            {
                if (Items == null) Items = new NodeDesign[] { };
                Dictionary<string, ObjectTypeDesign> otd = new Dictionary<string, ObjectTypeDesign>();  
                foreach(ObjectTypeDesign aItem in Items.Where(x => x.GetType() == typeof(ObjectTypeDesign)))
                {
                    otd.Add(aItem.SymbolicName.ToString(), aItem);
                }
                return otd;
            }
        }
        public ObjectTypeDesign ObjectTypeDesignsAdd(XmlQualifiedName symbolicName, XmlQualifiedName baseType)
        {
            ObjectTypeDesign newObjectTypeDesign = new ObjectTypeDesign
            {
                SymbolicName = symbolicName,
                BaseType = baseType,
                Children = new ListOfChildren()
            };
            if (Items == null) Items = new NodeDesign[] { };
            var nd = Items.ToList();
            nd.Add(newObjectTypeDesign);
            Items = nd.ToArray();
            return newObjectTypeDesign;
        }


        // generates XML for the model using the xml serializer
        public string GenerateModelXML(string? fileUrl = null)
        {
            using (TextWriter writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            {
                XmlSerializer.Serialize(xmlWriter, this, XmlSerializerNamespaces);
                if (fileUrl != null)
                {
                    File.WriteAllText(fileUrl, writer.ToString());
                }
                return writer.ToString();
            }
        }

        // generates an index CSV for the model
        public string GenerateModelCSV(string? fileUrl = null)
        {
            using (TextWriter writer = new StringWriter())
            {
                var typeCounter = 0;
                var propertyCounter = 0;
                foreach (ObjectTypeDesign aType in Items.Where(x => x.GetType() == typeof(ObjectTypeDesign)))
                {
                    typeCounter++;
                    writer.WriteLine($"{aType.SymbolicName.Name},{10000 + typeCounter},ObjectType");
                    foreach (var aProperty in aType.Children.Items)
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

        // generates XML for the nodeset using the OPC Foundation Compiler
        public string GenerateNodesetXML(string? xmlFileUrl=null, string? fileUrl = null, string? csvFileUrl=null)
        {

            var dir = Path.GetTempPath();
            string sessionId = Guid.NewGuid().ToString().Split("-")[0];
            var dirInfo = Directory.CreateDirectory($"{dir}{sessionId}");
            //Console.WriteLine(dirInfo.FullName);

            string xml = xmlFileUrl == null ? GenerateModelXML() : File.ReadAllText(xmlFileUrl);
            var xmlInputFileUrl = $@"{dirInfo.FullName}\input.xml";
            File.WriteAllText(xmlInputFileUrl, xml);
            //Console.WriteLine("xml created");

            if (csvFileUrl != null)
            {
                string csv = File.ReadAllText(csvFileUrl);
                var csvInputFileUrl = $@"{dirInfo.FullName}\input.csv";
                File.WriteAllText(csvInputFileUrl, csv);
                //Console.WriteLine("csv created");
            } else
            {
                //Console.WriteLine("csv creation skipped");
            }

            try
            {
                var args = new string[]
                    {
        "compile",
        "-d2",
        $@"{dirInfo.FullName}\input.xml",
        "-cg",
        $@"{dirInfo.FullName}\input.csv",
        "-o2",
        $"{dirInfo.FullName}"
                    };

                for (int ii = 0; ii < args.Length; ii++)
                {
                    args[ii] = args[ii].Replace("\n", "\\n");
                }

                ModelCompilerApplication.Run(args);

            }
            catch (Exception e)
            {
                //Console.WriteLine("error message:");
                return e.Message;
            }


            // find the nodeset2.xml file
            string xmlNodeSet = File.ReadAllText(Directory.EnumerateFiles($"{dirInfo.FullName}").First(x => x.Contains("NodeSet2.xml")));

            // clean up
            Directory.Delete(dirInfo.FullName, true);

            if (fileUrl != null)
            {
                File.WriteAllText(fileUrl, xmlNodeSet);
            }

            return xmlNodeSet;

        }

        // compiles the nodeset into an output folder using the OPC Foundation Compiler binaries
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
