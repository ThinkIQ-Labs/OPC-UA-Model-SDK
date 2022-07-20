using ModelCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ua.model.sdk.Model;

namespace ua.model.sdk.Managers
{
    public class uaObjectTypeDesignManager
    {
        ModelDesign ModelDesign { get; set; }

        public uaObjectTypeDesignManager(ModelDesign modelDesign)
        {
            ModelDesign = modelDesign;
        }

        //<ObjectType SymbolicName = "ANIMAL:AnimalType" BaseType="ua:BaseObjectType" >
        public uaObjectTypeDesign AddBasicObjectTypeDesign(XmlQualifiedName symbolicName, XmlQualifiedName baseType)
        {
            ObjectTypeDesign newObjectTypeDesign = new ObjectTypeDesign
            {
                SymbolicName = symbolicName,
                BaseType = baseType,
                Children = new ListOfChildren()
            };

            if (ModelDesign.Items == null) ModelDesign.Items = new NodeDesign[] { };
            List<NodeDesign> nodedesigns = ModelDesign.Items.ToList();
            nodedesigns.Add(newObjectTypeDesign);
            ModelDesign.Items = nodedesigns.ToArray();

            return new uaObjectTypeDesign(newObjectTypeDesign);
        }

    }
}
