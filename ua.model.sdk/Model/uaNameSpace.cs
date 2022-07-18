using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAOOI.SemanticData.UAModelDesignExport.XML;

namespace ua.model.sdk.Model
{
    public class uaNameSpace
    {
        public Namespace NameSpace { get;set;}
        public uaNameSpace(Namespace nameSpace)
        {
            NameSpace = nameSpace;
        }
    }
}
