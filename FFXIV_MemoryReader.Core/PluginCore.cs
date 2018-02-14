using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    public class PluginCore : IDisposable
    {
        internal static NLog.Logger Logger;

        TabPage tabPage;
        Label label;
        ElementHost elementHost;
        MainControl mainControl = null;
        Memory memory;
        System.Timers.Timer processChecker;
        private object _lock = new object();

        public PluginCore()
        {
            InitizalizeLogger();
            Logger.Info("########## FFXIV_MemoryReader Started. ##########");
            Logger.Info("PluginCore Constructor Called.");
            processChecker = new System.Timers.Timer();
            processChecker.Elapsed += ProessChecker_Elapsed;
            processChecker.Interval = 200;
            Logger.Info("PluginCore Constructor End.");
        }

        public void Dispose()
        {
            Logger.Info("PluginCore Dispose Called.");
            processChecker.Stop();
            processChecker.Dispose();
            memory?.Dispose();
            label.Text = "Stopped.";
            Logger.Info("PluginCore Dispose Finished.");
            Logger.Info("########## FFXIV_MemoryReader Finished. ##########");
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
            Logger.Info("PluginCore Init Called.");

            tabPage = pluginScreenSpace;
            label = pluginStatusText;

            tabPage.Text = "FFXIV_MemoryReader";
            tabPage.Controls.Clear();

            Logger.Info("Creating Plugin Screen.");
            mainControl = new MainControl();
            tabPage.Controls.Add(elementHost = new ElementHost() { Child = mainControl, Dock = DockStyle.Fill });

            Logger.Info("ProcessChecker Timer Start.");
            processChecker.Start();
            label.Text = "Started.";
            Logger.Info("PluginCore Init Ended.");
        }
        private void ProessChecker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckProcessId();
        }

        internal void CheckProcessId()
        {
            if (memory == null)
            {
                ChangeProcessId();
            }
            else if (!memory.IsValid)
            {
                Logger.Error("FFXIV Process Lost.");
                memory?.Dispose();
                memory = null;
            }
            else if(!memory.HasAllPointers)
            {
                Logger.Error("FFXIV Process is alive, but some Pointer not found.");
                memory?.Dispose();
                memory = null;
            }
        }

        internal void ChangeProcessId(int processId = 0)
        {
            lock (_lock)
            {
                Process process = Helper.ProcessHelper.GetFFXIVProcess(processId);

                if ((process != null && memory == null) ||
                    (process != null && memory != null && process.Id != memory.Process.Id))
                {
                    try
                    {
                        Logger.Info("FFXIV Process Found: {0}", process.Id);
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


        public List<Model.CombatantV1> GetConbatantsV1()
        {
            List<Model.CombatantV1> result = new List<Model.CombatantV1>();
            try
            {
                if(memory != null && memory.IsValid)
                {
                    result = memory.GetCombatantsV1();
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


