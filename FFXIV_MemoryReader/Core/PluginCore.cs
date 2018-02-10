using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Diagnostics;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    class PluginCore : IDisposable
    {
        TabPage tabPage;
        Label label;
        ElementHost elementHost;
        private object _lock = new object();

        public PluginCore()
        {

        }

        public void Dispose()
        {

        }

        public void Init(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            tabPage = pluginScreenSpace;
            label = pluginStatusText;

            tabPage.Text = "FFXIV_MemoryReader";
            tabPage.Controls.Clear();
            tabPage.Controls.Add(elementHost = new ElementHost() { Child = new MainControl(), Dock = DockStyle.Fill });

        }

        public void ChangeProcessId(int processId)
        {
            lock(_lock)
            {
                Process process = Helper.ProcessHelper.GetFFXIVProcess(processId);
                
            }
        }
    }
}
