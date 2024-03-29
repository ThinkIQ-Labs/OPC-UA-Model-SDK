﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ua.model.sdk.Model
{
    public partial class NodeDesign
    {
        Dictionary<string, T> GetDictionaryOfT<T>()
        {
            if (Children.Items == null) Children.Items = new InstanceDesign[] { };
            Dictionary<string, T> allTs = new Dictionary<string, T>();
            foreach (T aItem in Children.Items.Where(x => x.GetType() == typeof(T)).Cast<T>())
            {
                allTs.Add((aItem as dynamic).SymbolicName.ToString(), aItem);
            }
            return allTs;
        }

        void AddToChildren<T>(T newT)
        {
            if (Children.Items == null) Children.Items = new InstanceDesign[] { };
            var itemList = Children.Items.ToList();
            itemList.Add(newT as InstanceDesign);
            Children.Items = itemList.ToArray();
            return;
        }

        [XmlIgnore]
        public Dictionary<string, PropertyDesign> PropertyDesigns
        {
            get
            {
                return GetDictionaryOfT<PropertyDesign>();
            }
        }
        public PropertyDesign PropertyDesignsAdd(XmlQualifiedName symbolicName, XmlQualifiedName dataType)
        {
            PropertyDesign newPropertyDesign = new PropertyDesign
            {
                SymbolicName = symbolicName,
                DataType = dataType,
                Children = new ListOfChildren()
            };
            AddToChildren<PropertyDesign>(newPropertyDesign);
            return newPropertyDesign;
        }

        [XmlIgnore]
        public Dictionary<string, VariableDesign> VariableDesigns
        {
            get
            {
                return GetDictionaryOfT<VariableDesign>();
            }
        }
        public VariableDesign VariableDesignsAdd(XmlQualifiedName symbolicName, XmlQualifiedName dataType)
        {
            VariableDesign newVariableDesign = new VariableDesign
            {
                SymbolicName = symbolicName,
                DataType = dataType,
                Children = new ListOfChildren()
            };
            AddToChildren<VariableDesign>(newVariableDesign);
            return newVariableDesign;
        }

        [XmlIgnore]
        public Dictionary<string, ObjectDesign> ObjectDesigns
        {
            get
            {
                return GetDictionaryOfT<ObjectDesign>();
            }
        }
        public ObjectDesign ObjectDesignsAdd(XmlQualifiedName symbolicName, XmlQualifiedName typeDefinition)
        {
            ObjectDesign newObjectDesign = new ObjectDesign
            {
                SymbolicName = symbolicName,
                TypeDefinition = typeDefinition,
                //ReferenceType = typeDefinition,
            };
            AddToChildren<ObjectDesign>(newObjectDesign);
            return newObjectDesign;
        }

        public XmlElement CreateDefaultValueXmlElement(string dataType, string defaultValue, string uaxPrefix= "uax")
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement defaultValueXML = xmlDocument.CreateElement(uaxPrefix, dataType, "http://opcfoundation.org/UA/2008/02/Types.xsd");
            defaultValueXML.InnerText = defaultValue;
            return defaultValueXML;
        }
    }
}
