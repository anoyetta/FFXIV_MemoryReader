using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TamanegiMage.FFXIV_MemoryReader.Model;

namespace FFXIV_MemoryReader_SampleClient
{
    public partial class UserControl1 : UserControl, IActPluginV1
    {
        Label LabelStatus;

        public UserControl1()
        {
            InitializeComponent();
        }

        public void DeInitPlugin()
        {
            dataGridView1.Rows.Clear();
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            this.LabelStatus = pluginStatusText;
            pluginScreenSpace.Controls.Add(this);
            this.Dock = DockStyle.Fill;
        }

        private IActPluginV1 GetFFXIVMemoryReaderPlugin()
        {
            IActPluginV1 plugin = null;
            foreach (var p in ActGlobals.oFormActMain.ActPlugins)
            {
                if (p.pluginFile.Name == "FFXIV_MemoryReader.dll")
                {
                    plugin = p.pluginObj;
                    break;
                }
            }
            return plugin;
        }


        #region Combatants
        private void Button_GetCombatants_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();

            IActPluginV1 plugin = GetFFXIVMemoryReaderPlugin();

            if (plugin != null)
            {
                dynamic memoryPlugin = plugin;
                List<CombatantV1> combatants = memoryPlugin?.GetCombatantsV1();
                if (combatants != null)
                {
                    foreach (var combatant in combatants)
                    {
                        this.dataGridView1.Rows.Add(new string[] { combatant.ID.ToString(), combatant.Name, combatant.EffectiveDistance.ToString(), combatant.Heading.ToString(), combatant.TargetID.ToString() });
                    }
                }
            }
            plugin = null;
        }

        #endregion

        #region CameraInfo

        Timer CameraInfoTimer = new Timer();

        private void Button_CameraInfo_Click(object sender, EventArgs e)
        {
            if (Button_CameraInfo.Text == "Start")
            {
                CameraInfoTimer.Interval = 200;
                CameraInfoTimer.Tick += UpdateCameraInfo;
                CameraInfoTimer.Enabled = true;
                Button_CameraInfo.Text = "Stop";
            }
            else if (Button_CameraInfo.Text == "Stop")
            {
                CameraInfoTimer.Tick -= UpdateCameraInfo;
                CameraInfoTimer.Enabled = true;
                CameraInfoTimer.Enabled = false;
                Button_CameraInfo.Text = "Start";
            }
        }


        private void UpdateCameraInfo(object sender, EventArgs e)
        {
            IActPluginV1 plugin = GetFFXIVMemoryReaderPlugin();

            if (plugin != null)
            {
                dynamic memoryPlugin = plugin;

                CameraInfoV1 cameraInfo = memoryPlugin?.GetCameraInfoV1();
                if (cameraInfo != null)
                {
                    textBox_Camera_Mode.Text = cameraInfo.Mode.ToString();
                    textBox_Camera_Heading.Text = cameraInfo.Heading.ToString();
                    textBox_Camera_Elevation.Text = cameraInfo.Elevation.ToString();
                }
            }
            plugin = null;

        }

        #endregion

        #region HotbarRecast

        private void Button_GetHotbarRecast_Click(object sender, EventArgs e)
        {
            this.dataGridView2.Rows.Clear();

            IActPluginV1 plugin = GetFFXIVMemoryReaderPlugin();

            if (plugin != null)
            {
                dynamic memoryPlugin = plugin;
                List<HotbarRecastV1> recasts = memoryPlugin?.GetHotbarRecastV1();
                if (recasts != null)
                {
                    foreach (var recast in recasts)
                    {
                        this.dataGridView2.Rows.Add(new string[] {
                            recast.HotbarType.ToString(),
                            recast.ID.ToString(),
                            recast.Slot.ToString(),
                            recast.Name.ToString(),
                            recast.Category.ToString(),
                            recast.Type.ToString(),
                            recast.Icon.ToString(),
                            recast.CoolDownPercent.ToString(),
                            recast.IsAvailable.ToString(),
                            recast.RemainingOrCost.ToString(),
                            recast.Amount.ToString(),
                            recast.InRange.ToString(),
                            recast.IsProcOrCombo.ToString()
                        });
                    }
                }
            }
            plugin = null;
        }

        #endregion
    }
}
