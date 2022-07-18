using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ua.model.sdk.Model;
using UAOOI.SemanticData.UAModelDesignExport.XML;

namespace ua.model.sdk.Managers
{
    public class uaModelDesignManager
    {
        public ModelDesign ModelDesign { get; set; }
        public XmlSerializer XmlSerializer = new XmlSerializer(typeof(ModelDesign));
        public XmlSerializerNamespaces XmlSerializerNamespaces = new XmlSerializerNamespaces();

        public uaModelDesignManager(ModelDesign modelDesign)
        {
            ModelDesign = modelDesign;
        }

        public string GenerateXML(string? fileUrl = null)
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

        public string GenerateCSV(string? fileUrl = null)
        {
            using (TextWriter writer = new StringWriter())
            {
                var typeCounter = 0;
                var propertyCounter = 0;
                foreach (ObjectTypeDesign aType in ModelDesign.Items.Where(x=>x.GetType()==typeof(ObjectTypeDesign)))
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
