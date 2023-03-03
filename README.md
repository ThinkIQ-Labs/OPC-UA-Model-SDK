# OPC-UA-Model-SDK

## Why this Project?

Studying OPC UA, and in particular the Information Modeling aspects of it, we realized that most established code base revolves around the communication. There is, however, little guidance available for procedural creation of Information Models. The advice is either to use a GUI tool, such as the excellent UaModeler by Unified Automation, or to create your own XML and then compile it using the UA Model Compiler.

For interoperability projects, in particular, it is important to be able create such Information Models using code. Simply doing it "according to the ModelDesign.xsd" is harder than it sounds - especially if one wants to reproduce some of the available examlple model designs.

This project extends some of the pratical classes of the ModelDesign.xsd and Opc.Ua.Types.xsd to make compilation of XML easier. It also references the OPC UA model compiler binaries so a model design instance can be easily compiled into a nodeset XML file.

## Excellent Online Posts and Tutorials

This list is by no means complete, but these two references were especially helpful:

- Dr. Stefan Profanter: https://opcua.rocks/custom-information-models/
- Macheronte.com: https://www.macheronte.com/en/opc-ua-model-design-lets-create-a-data-model/

## Reference Repos

These are good references for obtaining the ModelDesign.xsd/.cs and Opc.Ua.Types.xsd/.cs files. 

- https://github.com/OPCFoundation/UA-ModelCompiler
- https://github.com/mpostol & https://www.nuget.org/packages/UAOOI.SemanticData.UAModelDesignExport/

Prof. Postol let's you reference a NuGet package, which the OPC Foundation appears to be lacking at this time.

## Things that Become Easier Using this SDK

### Start from Scratch or Use an Existing Model

From the write test sample script:
```C#
// this will start a new model, create the Target(XML)Namespace attributes,
// add XmlSerializerNamespaces
// add a Namespace to the UA Namespaces collection
var md = new ModelDesign("https://opcua.rocks/UA", "animal");
```

From the read test sample script:
```C#
// this will parse the xml file and start a model
// minor adjustments can be added to address TargetNamespace and XmlSerializerNamespaces
// re-serializing will produce an almost identical xml file
var md = new ModelDesign("./data/modeldesign.xml");
```
### XML Namespaces are Attached to XMLSerializer

Most sample model.xml files create XML Namespace prefixes in the ModelDesign root element. This cannot be done in the ModelDesign.cs scope, but must be added to the XMLSerializer instead.

From the read test sample script:
```C#
md.ModelDesignManager.XmlSerializerNamespaces.Add("opc", "http://opcfoundation.org/UA/ModelDesign.xsd");
md.ModelDesignManager.XmlSerializerNamespaces.Add(string.Empty, "http://opcfoundation.org/OPCUAServer");
// which then gets used at serialization
XmlSerializer.Serialize(xmlWriter, ModelDesign, XmlSerializerNamespaces);
```

### Object Types have Methods that Make it Easier to Create and Add Content

Adding custom types, and subsequently adding properties and variables to types can be done this way:

```C#
var animalNameSpace = md.NamespacesDictionary["animal"].Value;
var opcUaNameSpace = md.NamespacesDictionary["OpcUa"].Value;
var animalType = md.ObjectTypeDesignsAdd(
    new XmlQualifiedName("AnimalType", animalNameSpace),
    new XmlQualifiedName("BaseObjectType", opcUaNameSpace)
    );
animalType.PropertyDesignsAdd(
    new XmlQualifiedName("Name", animalNameSpace),
    new XmlQualifiedName("String", opcUaNameSpace)
    );
```

### Default Values for Properties and Variables

We extended the partial NodeDesign class to create an XML Element that can nicely serve as DefaultValue.

``` C#
variableHeight.DefaultValue = variableHeight.CreateDefaultValueXmlElement("Float", "10");
```

This becomes the following XML:

``` XML
<DefaultValue>
  <uax:Float>10</uax:Float>
</DefaultValue>
```

### Engineering Units

We extended the partial EUInformation class to make working with engineering units easire. We added the UNICE unit's libary as a csv resource to the project and added logic that creates an XMLElement that can be attached to the DefaultValue property of a EUInformation object. Below is an example of creating a "height in meters" variable with the UoM property.

``` C#
var variableHeight = animalType.VariableDesignsAdd(
    new XmlQualifiedName("Height", animalNameSpace),
    new XmlQualifiedName("Float", opcUaNameSpace)
    );

var propEU = variableHeight.PropertyDesignsAdd(
    new XmlQualifiedName("EngineeringUnits", animalNameSpace),
    new XmlQualifiedName("EUInformation", opcUaNameSpace)
    );
    
EUInformation eui = EUInformation.EUInformationList.First(x => x.DisplayName.Text == "m");
propEU.DefaultValue = eui.CreateXmlElement();
```

This becomes the following XML:

``` XML
  <Variable SymbolicName="animal:Height" DataType="ua:Float">
    <Children>
      <Property SymbolicName="animal:EngineeringUnits" DataType="ua:EUInformation">
        <Children />
        <DefaultValue>
          <uax:ExtensionObject>
            <uax:TypeId>
              <uax:Identifier>i=888</uax:Identifier>
            </uax:TypeId>
            <uax:Body>
              <uax:EUInformation>
                <uax:NamespaceUri>http://www.opcfoundation.org/UA/units/un/cefact</uax:NamespaceUri>
                <uax:UnitId>5067858</uax:UnitId>
                <uax:DisplayName>
                  <uax:Text>m</uax:Text>
                </uax:DisplayName>
                <uax:Description>
                  <uax:Text>metre</uax:Text>
                </uax:Description>
              </uax:EUInformation>
            </uax:Body>
          </uax:ExtensionObject>
        </DefaultValue>
      </Property>
    </Children>
  </Variable>
```

### Creation of XML, CSV, and NodeSet2 Files

The ModelDesign class has methods to create all the files needed for NodeSet2 compilation:

```C#
string aXmlFileContent = md.GenerateModelXML(xmlFileUrl);
Console.WriteLine(aXmlFileContent);

string aCsvFileContent = md.GenerateModelCSV(csvFileUrl);
Console.WriteLine(aCsvFileContent);

// GenerateNodesetXML creates a temp directory and only retrieves the nodeset XML
var nodesetXmlFileUrl = $"{dirInfo.FullName}\\test2nodeset2.xml";
string aNodesetXmlFileContent = md.GenerateNodesetXML(modelXmlFileUrl, nodesetXmlFileUrl);
Console.WriteLine(aNodesetXmlFileContent);

// CompileNodeset gets a full set of files created by the Opc.Ua.ModelCompiler using the compiled binaries
string compilerExecutable = @"C:\Users\Public\source\repos\UA-ModelCompiler\build\bin\Debug\net6.0\Opc.Ua.ModelCompiler.exe";
md.CompileNodeset(compilerExecutable, xmlFileUrl, csvFileUrl, ".\\out");

```

## Related Considerations

These were some of the challenges that took a long time to figure out.

### Model Compilation Requires Model.xml Files to be formatted correctly.

We discovered that the ModelCompiler needs to be provided with properly formated xml, e.g. Indent setting and the resulting line breaks are a must:

```C#
using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
```

### DataTime Serialization with Specific Formating

It's very straightforward to add a UTC-style time attribute when manually compiling a XML file:

From Dr. Stefan Profanter's animal type tutorial:
```XML
TargetPublicationDate="2019-04-01T00:00:00Z"
```
We appreciate Dr. Profanter's including this. This process is challengings using XML Serialization, but can be achieved by extending the partial ModelDesign.cs class:

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

Finally, a default XML Namespace without a prefix can be created using string.Empty. We tried "" but it did not.

```C#
md.uaModelDesignManager.XmlSerializerNamespaces.Add(string.Empty, "http://opcfoundation.org/OPCUAServer");
```

### Unfinished Items

#### EUInformation Objects for Engineering Units

Opc.Ua.Types.cs class has strongly typed EUInformation available. We haven't figured out how to serialize them correctly so that they can be attached as XMLElement to the DefaultValue of a EngineeringUnits ExtensionObject. 

#### Default Value XML Elements for NodeDesign

The same issue as above. We can not achieve this yet without manually creating an XML Element.

