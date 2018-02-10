using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using TamanegiMage.FFXIV_MemoryReader.Core;

namespace TamanegiMage.FFXIV_MemoryReader
{
    public class MemoryPlugin : IActPluginV1
    {
        PluginCore core;
        public void DeInitPlugin()
        {
            
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            core = new PluginCore();
            core.Init(pluginScreenSpace, pluginStatusText);
        }
    }
}
