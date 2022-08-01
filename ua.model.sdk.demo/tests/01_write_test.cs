using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
            using System.Xml;
            using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    public static class _01_write_test
    {
        public static void Run()
        {

            // thank you Stefan Profanter for the tutorial
            // https://opcua.rocks/custom-information-models/

            var md = new ModelDesign("https://opcua.rocks/UA", "animal");

            var animalNameSpace = md.NamespacesDictionary["animal"].Value;
            var opcUaNameSpace = md.NamespacesDictionary["OpcUa"].Value;

            var animalType = md.ObjectTypeDesignsAdd(
                new XmlQualifiedName("AnimalType", animalNameSpace),
                new XmlQualifiedName("BaseObjectType", opcUaNameSpace)
                );

            var nameProperty = animalType.PropertyDesignsAdd(
                new XmlQualifiedName("Name", animalNameSpace),
                new XmlQualifiedName("String", opcUaNameSpace)
                );

            nameProperty.DefaultValue = nameProperty.CreateDefaultValueXmlElement("String", "Rudolph");

            Console.WriteLine(md.GenerateModelXML());
            Console.WriteLine(md.GenerateModelCSV());

            var dirInfo = Directory.CreateDirectory($".\\out");

            var modelXmlFileUrl = $"{dirInfo.FullName}\\test2.xml";
            string aModelXmlFileContent = md.GenerateModelXML(modelXmlFileUrl);
            Console.WriteLine(aModelXmlFileContent);

            var csvFileUrl = $"{dirInfo.FullName}\\test2.csv";
            string aCsvFileContent = md.GenerateModelCSV(csvFileUrl);
            Console.WriteLine(aCsvFileContent);

            var nodesetXmlFileUrl = $"{dirInfo.FullName}\\test2nodeset2.xml";
            string aNodesetXmlFileContent = md.GenerateNodesetXML(modelXmlFileUrl, nodesetXmlFileUrl);
            Console.WriteLine(aNodesetXmlFileContent);


            var compilerExecutable = @"C:\Users\Public\source\repos\UA-ModelCompiler\build\bin\Debug\net6.0\Opc.Ua.ModelCompiler.exe";
            md.CompileNodeset(compilerExecutable, modelXmlFileUrl, csvFileUrl, dirInfo.FullName);
        }
    }
}
