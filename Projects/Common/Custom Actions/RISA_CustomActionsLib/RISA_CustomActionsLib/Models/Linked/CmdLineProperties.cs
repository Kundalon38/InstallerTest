using System.Collections.Generic;
using System.Linq;

namespace RISA_CustomActionsLib.Models.Linked
{
    public class CmdLineProperties : List<CmdLineProperty>
    {
        public CmdLineProperty this[string propName]
        {
            get
            {
                return this.SingleOrDefault(x => x.PropName == propName);
            }
        }
    }
}
