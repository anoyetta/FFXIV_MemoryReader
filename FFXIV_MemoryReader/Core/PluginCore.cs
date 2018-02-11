using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Diagnostics;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    class PluginCore : IDisposable
    {
        internal static NLog.Logger Logger;

        TabPage tabPage;
        Label label;
        ElementHost elementHost;
        Memory memory;
        System.Timers.Timer proessChecker;
        private object _lock = new object();

        public PluginCore()
        {
            InitizalizeLogger();

            Logger.Trace("PluginCore Start.");
            proessChecker = new System.Timers.Timer();
            proessChecker.Elapsed += ProessChecker_Elapsed;
            proessChecker.Interval = 1000;

            Logger.Trace("PluginCore End.");
        }

        public void Dispose()
        {
            proessChecker.Stop();
        }

        private void InitizalizeLogger()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget("Logger")
            {
                Encoding = Encoding.UTF8,
                Layout = "${longdate} [${threadid:padding=4}] [${uppercase:${level:padding=-5}}] ${message} ${exception:format=tostring}",
                FileName = "${specialfolder:folder=ApplicationData}/TamanegiMage/FFXIV_MemoryReader-${shortdate}.log",
                ArchiveAboveSize = 5000000,
                MaxArchiveFiles = 5,
            };
            config.AddTarget(target);
            var rule = new LoggingRule("Logger", LogLevel.Trace, target);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
            Logger = NLog.LogManager.GetLogger("Logger");
        }

        public void Init(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            Logger.Trace("PluginCore->Init Start.");

            tabPage = pluginScreenSpace;
            label = pluginStatusText;

            tabPage.Text = "FFXIV_MemoryReader";
            tabPage.Controls.Clear();
            tabPage.Controls.Add(elementHost = new ElementHost() { Child = new MainControl(), Dock = DockStyle.Fill });

            Logger.Trace("Start ProcessChecker Timer");
            proessChecker.Start();

            Logger.Trace("PluginCore->Init End.");
        }
        private void ProessChecker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckProcessId();
        }

        public void CheckProcessId()
        {
            if (memory == null)
            {
                ChangeProcessId();
            }
            else if (!memory.IsValid)
            {
                Logger.Info("Lost Process.");
                memory?.Dispose();
                memory = null;
            }
        }

        public void ChangeProcessId(int processId = 0)
        {
            lock (_lock)
            {
                Process process = Helper.ProcessHelper.GetFFXIVProcess(processId);

                if ((process != null && memory == null) ||
                    (process != null && memory != null && process.Id != memory.Process.Id))
                {
                    try
                    {
                        Logger.Info("New Process Found. {0}", process.Id);
                        memory = new Memory(process, Logger);
                    }
                    catch
                    {
                        memory = null;
                    }
                }
                else if (process == null && memory != null)
                {
                    memory?.Dispose();
                    memory = null;
                }
            }

        }


        public List<Model.Combatant> GetConbatants()
        {
            List<Model.Combatant> result = new List<Model.Combatant>();
            try
            {
                if(memory != null && memory.IsValid)
                {
                    result = memory.GetCombatants();
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }



    }

}


