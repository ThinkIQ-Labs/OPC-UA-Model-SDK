using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ua.model.sdk.Model;

namespace ua.model.sdk.demo.tests
{
    public static class _07_create_nested_type
    {
        public static void Run()
        {

            // thank you Stefan Profanter for the tutorial
            // https://opcua.rocks/custom-information-models/

            var md = new ModelDesign("https://opcua.rocks/UA", "motor");

            var motorNameSpace = md.NamespacesDictionary["motor"].Value;
            var opcUaNameSpace = md.NamespacesDictionary["OpcUa"].Value;

            var equType = md.ObjectTypeDesignsAdd(
                new XmlQualifiedName("Equipment", motorNameSpace),
                new XmlQualifiedName("BaseObjectType", opcUaNameSpace)
                );

            var motorType = md.ObjectTypeDesignsAdd(
                new XmlQualifiedName("MotorType", motorNameSpace),
                new XmlQualifiedName("Equipment", motorNameSpace)
                );

            var objType = md.ObjectTypeDesignsAdd(
                new XmlQualifiedName("Object", motorNameSpace),
                new XmlQualifiedName("BaseObjectType", opcUaNameSpace)
                );


            var namplateType = md.ObjectTypeDesignsAdd(
                new XmlQualifiedName("NameplateType", motorNameSpace),
                new XmlQualifiedName("Object", motorNameSpace)
                );

            var aObjectDesign = motorType.ObjectDesignsAdd(
                    new XmlQualifiedName("NameplateType", motorNameSpace),
                    new XmlQualifiedName("NameplateType", motorNameSpace)
                );


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
