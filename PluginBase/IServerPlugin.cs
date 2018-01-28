using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginBase
{

    public interface IServerPlugin
    {

        void IServerPlugin(ref ServerEvents serverEvents);

    }

}
