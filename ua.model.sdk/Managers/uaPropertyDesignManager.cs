using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ua.model.sdk.Model;
using UAOOI.SemanticData.UAModelDesignExport.XML;

namespace ua.model.sdk.Managers
{
    public class uaPropertyDesignManager
    {
        public ListOfChildren ListOfChildren { get; set; }
        public uaPropertyDesignManager(ListOfChildren listOfChildren)
        {
            ListOfChildren = listOfChildren;
        }

        // <Property SymbolicName="ANIMAL:Name" DataType="ua:String" ValueRank="Scalar" ModellingRule="Mandatory">
        //      < Description > Name of the animal </ Description >
        //  </ Property >
        public uaPropertyDesign AddBasicPropertyDesign(XmlQualifiedName symbolicName, XmlQualifiedName dataType)
        {
            PropertyDesign newPropertyDesign = new PropertyDesign
            {
                SymbolicName = symbolicName,
                DataType = dataType,
            };

            if (ListOfChildren.Items == null) ListOfChildren.Items = new InstanceDesign[] { };
            List<InstanceDesign> items = ListOfChildren.Items.ToList();
            items.Add(newPropertyDesign);
            ListOfChildren.Items = items.ToArray();

            return new uaPropertyDesign(newPropertyDesign);
        }
    }
}
