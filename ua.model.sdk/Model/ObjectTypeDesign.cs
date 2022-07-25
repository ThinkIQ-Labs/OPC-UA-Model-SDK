using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ua.model.sdk.Model
{
    public partial class ObjectTypeDesign
    {
        [XmlIgnore]
        public Dictionary<string, PropertyDesign> PropertyDesigns
        {
            get
            {
                if (Children.Items == null) Children.Items = new InstanceDesign[] { };
                Dictionary<string, PropertyDesign> pd = new Dictionary<string, PropertyDesign>();
                foreach (PropertyDesign aItem in Children.Items.Where(x => x.GetType() == typeof(PropertyDesign)))
                {
                    pd.Add(aItem.SymbolicName.ToString(), aItem);
                }
                return pd;
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
            if (Children.Items == null) Children.Items = new InstanceDesign[] { };
            var id = Children.Items.ToList();
            id.Add(newPropertyDesign);
            Children.Items = id.ToArray();
            return newPropertyDesign;
        }

    }
}
