using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Hotfix.UI
{
    class testmodule : BaseModule
    {

        public testmodule()
        {
            PreFabs = "Perfabs/Panel/testPanel";
        }


        public override Type GetView()
        {
            return typeof(testView);
        }
    }
}
