using System.Xml;
using ua.model.sdk.Model;

// thank you Stefan Profanter for the tutorial
// https://opcua.rocks/custom-information-models/

var md = new uaModelDesign("https://opcua.rocks/UA", "animal");

var animalNameSpace = md.uaNameSpaces["animal"].NameSpace.Value;
var opcUaNameSpace = md.uaNameSpaces["OpcUa"].NameSpace.Value;

var animalType = md.uaObjectTypeDesignManager.AddBasicObjectTypeDesign(
    new XmlQualifiedName("AnimalType", animalNameSpace),
    new XmlQualifiedName("BaseObjectType", opcUaNameSpace)
    );

animalType.uaPropertyDesignManager.AddBasicPropertyDesign(
    new XmlQualifiedName("Name", animalNameSpace),
    new XmlQualifiedName("String", opcUaNameSpace)
    );

Console.WriteLine(md.uaModelDesignManager.GenerateXML());
Console.WriteLine(md.uaModelDesignManager.GenerateCSV());

var dirInfo = Directory.CreateDirectory($".\\out");

var xmlFileUrl = $"{dirInfo.FullName}\\test2.xml";
string aXmlFileContent = md.uaModelDesignManager.GenerateXML(xmlFileUrl);
Console.WriteLine(aXmlFileContent);

var csvFileUrl = $"{dirInfo.FullName}\\test2.csv";
string aCsvFileContent = md.uaModelDesignManager.GenerateCSV(csvFileUrl);
Console.WriteLine(aCsvFileContent);

var compilerExecutable = @"C:\Users\Public\source\repos\UA-ModelCompiler\build\bin\Debug\net6.0\Opc.Ua.ModelCompiler.exe";
md.uaModelDesignManager.CompileNodeset(compilerExecutable, xmlFileUrl, csvFileUrl, dirInfo.FullName);