using ModelCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ua.model.sdk.Managers;

namespace ua.model.sdk.Model
{
    public class uaObjectTypeDesign
    {
        public ObjectTypeDesign ObjectTypeDesign { get; set; }


        public List<uaPropertyDesign> uaPropertyDesigns
        {
            get
            {
                var returnObject = new List<uaPropertyDesign>();
                foreach (PropertyDesign aPropertyDesign in ObjectTypeDesign.Children.Items.Where(x=>x.GetType()==typeof(PropertyDesign)))
                {
                    returnObject.Add(new uaPropertyDesign(aPropertyDesign));
                }
                return returnObject;
            }
        }

        private uaPropertyDesignManager _uaPropertyDesignManager;
        public uaPropertyDesignManager uaPropertyDesignManager
        {
            get
            {
                if (_uaPropertyDesignManager == null) _uaPropertyDesignManager = new uaPropertyDesignManager(ObjectTypeDesign.Children);
                return _uaPropertyDesignManager;
            }
        }

        public uaObjectTypeDesign(ObjectTypeDesign objectTypeDesign)
        {
            ObjectTypeDesign = objectTypeDesign;
        }

        

    }
}
