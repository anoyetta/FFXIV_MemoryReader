using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        MainControlViewModel viewModel = null;
        Memory memory;
        System.Timers.Timer processChecker;
        const double processCheckerInterval = 500;
        const double signatureCheckerInterval = 2000;
        const double periodicScanInterval = 15000;
        private double signatureCheckTimer = 0;
        private double periodicScanTimer = 0;
        private object _lock = new object();

        public PluginCore()
        {
            InitizalizeLogger();
            Logger.Info("########## FFXIV_MemoryReader Started. ##########");
            Logger.Info("PluginCore Constructor Called.");
            processChecker = new System.Timers.Timer();
            processChecker.Elapsed += ProessChecker_Elapsed;
            processChecker.Interval = processCheckerInterval;
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
            viewModel = new MainControlViewModel();
            viewModel.InitPointerValues();
            mainControl.DataContext = viewModel;

            tabPage.Controls.Add(elementHost = new ElementHost()
            {
                Child = mainControl,
                Dock = DockStyle.Fill
            });

            Logger.Info("ProcessChecker Timer Start.");
            processChecker.Start();
            label.Text = "Started.";
            Logger.Info("PluginCore Init Ended.");
        }
        private void ProessChecker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lock)
            {
                CheckProcessId();

                if (signatureCheckTimer >= signatureCheckerInterval)
                {
                    signatureCheckTimer = 0;
                    CheckSignatures();
                }
                else
                {
                    signatureCheckTimer += processCheckerInterval;
                }

                if (periodicScanTimer >= periodicScanInterval)
                {
                    periodicScanTimer = 0;
                    RunPeriodicScan();
                }
                else
                {
                    periodicScanTimer += periodicScanInterval;
                }

            }
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
                viewModel.InitPointerValues();
                memory?.Dispose();
                memory = null;
            }
        }

        internal void ChangeProcessId(int processId = 0)
        {
            Process process = Helper.ProcessHelper.GetFFXIVProcess(processId);

            if ((process != null && memory == null) ||
                (process != null && memory != null && process.Id != memory.Process.Id))
            {
                try
                {
                    Logger.Info("FFXIV Process Found: {0}", process.Id);
                    viewModel.InitPointerValues();
                    memory = new Memory(process, Logger);
                }
                catch
                {
                    memory = null;
                }
            }
            else if (process == null && memory != null)
            {
                viewModel.InitPointerValues();
                memory?.Dispose();
                memory = null;
            }
        }

        private void CheckSignatures()
        {
            if (memory != null && memory.IsValid)
            {
                if(!memory.HasAllPointers)
                {
                    // Scan completed, but some signature not found
                    memory.ResolvePointers();
                }
                viewModel.SetPointerValues(memory.GetPointers());
            }
        }

        private void RunPeriodicScan()
        {
            if (memory != null && memory.IsValid)
            {
                memory.ResolvePointers(false, true, false);
                viewModel.SetPointerValues(memory.GetPointers());
            }
        }

        public List<Model.CombatantV1> GetCombatantsV1()
        {
            List<Model.CombatantV1> result = new List<Model.CombatantV1>();
            try
            {
                if (memory != null && memory.IsValid)
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

        public Model.CameraInfoV1 GetCameraInfoV1()
        {
            Model.CameraInfoV1 result = new Model.CameraInfoV1();
            try
            {
                if (memory != null && memory.IsValid)
                {
                    result = memory.GetCameraInfoV1();
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public List<Model.HotbarRecastV1> GetHotbarRecastV1()
        {
            List<Model.HotbarRecastV1> result = new List<Model.HotbarRecastV1>();
            try
            {
                if (memory != null && memory.IsValid)
                {
                    result = memory.GetHotbarRecastV1();
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

    }

    internal class MainControlViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal void InitPointerValues()
        {
            this.PointerValue_Target = "0";
            this.PointerValue_MobArray = "0";
            this.PointerValue_CameraInfo = "0";
            this.pointerValue_Hotbar = "0";
            this.PointerValue_Recast = "0";

        }

        internal void SetPointerValues(Dictionary<Memory.PointerType, IntPtr> pointers)
        {
            this.PointerValue_Target = pointers[Memory.PointerType.Target].ToInt64().ToString();
            this.PointerValue_MobArray = pointers[Memory.PointerType.MobArray].ToInt64().ToString();
            this.PointerValue_CameraInfo = pointers[Memory.PointerType.CameraInfo].ToInt64().ToString();
            this.PointerValue_Hotbar = pointers[Memory.PointerType.Hotbar].ToInt64().ToString();
            this.PointerValue_Recast = pointers[Memory.PointerType.Recast].ToInt64().ToString();
        }

        private string pointerValue_Target;
        public string PointerValue_Target
        {
            get { return this.pointerValue_Target; }
            set { this.SetProperty(ref this.pointerValue_Target, value); }
        }

        private string pointerValue_MobArray;
        public string PointerValue_MobArray
        {
            get { return this.pointerValue_MobArray; }
            set { this.SetProperty(ref this.pointerValue_MobArray, value); }
        }

        private string pointerValue_CameraInfo;
        public string PointerValue_CameraInfo
        {
            get { return this.pointerValue_CameraInfo; }
            set { this.SetProperty(ref this.pointerValue_CameraInfo, value); }
        }

        private string pointerValue_Hotbar;
        public string PointerValue_Hotbar
        {
            get { return this.pointerValue_Hotbar; }
            set { this.SetProperty(ref this.pointerValue_Hotbar, value); }
        }

        private string pointerValue_Recast;
        public string PointerValue_Recast
        {
            get { return this.pointerValue_Recast; }
            set { this.SetProperty(ref this.pointerValue_Recast, value); }
        }
    }

}


