# OPC-UA-Model-SDK

## Why this Project?

Studying OPC UA, and in particular the Information Modeling aspects of it, we quickly realized that most established code base revolves around the cummunication bits and pieces. There is, however little guidance available for procedural creation of Information Models. The advise is either use a GUI tool, such as the excellent UaModeler by Unified Automation, or roll your own XML and then compile it using the UA Model Compiler.

In particular for interoperability projects it is important to be able create such Information Models using code. Simply doing it "according to the ModelDesign.xsd" is harder than it sounds - especially if one wants to reproduce some of the examlple model designs out there.

## Excellent Online Posts and Tutorials

This list is by no means complete, but these 2 references were especially helpful:

- Dr. Stefan Profanter: https://opcua.rocks/custom-information-models/
- Macheronte.com: https://www.macheronte.com/en/opc-ua-model-design-lets-create-a-data-model/

## Reference Repos

These are good references for obtaining the ModelDesign.xsd and ModelDesign.xml files. Prof. Postol even let's you reference a Nuget package, which the OPC Foundation appears to be lacking at this time.

- https://github.com/OPCFoundation/UA-ModelCompiler
- https://github.com/mpostol & https://www.nuget.org/packages/UAOOI.SemanticData.UAModelDesignExport/

## Things that Become Easier Using this SDK

If you are an XML ninja - this should be trivial. This repo uses the ModelDesign.xsd specifcation, and simply wraps useful bits and functionality - by prefixing classes with "ua" (for instance uaPropertyDesign.cs) and by adding managers (for instance uaPropertyDesignManager.cs).

### Start from Scratch or Use an Existing Model

From the write test sample script:
```C#
// this will start a new model, create the Target(XML)Namespace attributes,
// add XmlSerializerNamespaces
// add a Namespace to the UA Namespaces collection
var md = new uaModelDesign("https://opcua.rocks/UA", "animal");
```

From the read test sample script:
```C#
// this will parse the xml file and start a model
// minor adjustments can be added to address TargetNamespace and XmlSerializerNamespaces
// re-serializing will produce an almost identical xml file
uaModelDesign md = new uaModelDesign("./data/modeldesign.xml");
```
### XML Namespaces are attached to XMLSerializer

Most sample model.xml files create XML Namespace prefixes in the ModelDesign root element. This cannot be done in the ModelDesign.cs scope, but must be added to the XMLSerializer instead.

From the read test sample script:
```C#
md.uaModelDesignManager.XmlSerializerNamespaces.Add("opc", "http://opcfoundation.org/UA/ModelDesign.xsd");
md.uaModelDesignManager.XmlSerializerNamespaces.Add(string.Empty, "http://opcfoundation.org/OPCUAServer");

```

### Object Types have Managers that makes it more simply to create and add content

Adding custom types, and subsequently adding proeperties to types can be done like so:

```C#
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
```

### Creation of XML, CSV, and NodeSet2 Files

The uaModelDesignManager has methods to create all the files needed for NodeSet2 compilation:

```C#
string aXmlFileContent = md.uaModelDesignManager.GenerateXML(xmlFileUrl);
Console.WriteLine(aXmlFileContent);

string aCsvFileContent = md.uaModelDesignManager.GenerateCSV(csvFileUrl);
Console.WriteLine(aCsvFileContent);

string compilerExecutable = @"C:\Users\Public\source\repos\UA-ModelCompiler\build\bin\Debug\net6.0\Opc.Ua.ModelCompiler.exe";
md.uaModelDesignManager.CompileNodeset(compilerExecutable, xmlFileUrl, csvFileUrl, ".\\out");

```

## Note to Self

Some of these things took a long time to figure out...

### Model Compilation Requires Model.xml Files to be Pretty-Printed

We found that some of the most difficulat things to figure out is the "what's the minimum amount of stuff needed to make this work" part. Among those excercises we discovered that the ModelCompiler needs to be fed with properly formated xml, i.e. that Indent setting is a must:

```C#
using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
```

### DataTime Serialization with Specific Formating

It's very straight forward to add a time attribute when manually compiling a XML file (thanks Stefan for throwing that in...):

From Dr. Stefan Profanter's animal type tutorial:
```XML
TargetPublicationDate="2019-04-01T00:00:00Z"
```
This can be achieved using an XMLSerializer by extending 

```C#
// we can extend the partial ModelDesign class to include a String attribute for our DateTime field
public partial class ModelDesign
{
    //https://riptutorial.com/dot-net/example/83/formatting--custom-datetime-format
    [XmlAttributeAttribute(AttributeName = "TargetPublicationDate")]
    public string TargetPublicationDateString
    {
        get
        {
            return TargetPublicationDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
        set
        {
            TargetPublicationDate = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }
    }
}
// we explicitely ignore the TargetPublicationDate property because we have an override for that
// https://stackoverflow.com/a/21407983
XmlAttributes toXmlIgnorePropertyAttributes = new XmlAttributes();
toXmlIgnorePropertyAttributes.XmlIgnore = true;
XmlAttributeOverrides toXmlIgnoreClassAttributes = new XmlAttributeOverrides();
toXmlIgnoreClassAttributes.Add(typeof(ModelDesign), "TargetPublicationDate", toXmlIgnorePropertyAttributes);
XmlSerializer serializer2 = new XmlSerializer(typeof(ModelDesign), toXmlIgnoreClassAttributes);
```

### Creation of XML Namespaces without Prefix

Finally, a default XML Namespace without a prefix can be created using string.Empty (yes, we tried "").

```C#
md.uaModelDesignManager.XmlSerializerNamespaces.Add(string.Empty, "http://opcfoundation.org/OPCUAServer");
```

