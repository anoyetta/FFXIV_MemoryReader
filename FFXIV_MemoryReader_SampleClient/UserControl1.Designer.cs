namespace FFXIV_MemoryReader_SampleClient
{
    partial class UserControl1
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.Button_GetCombatants = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CharacterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EffectiveDistance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Heading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Button_CameraInfo = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Camera_Elevation = new System.Windows.Forms.TextBox();
            this.textBox_Camera_Heading = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Camera_Mode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.Button_GetHotbarRecast = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.Hotbar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Slot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Icon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CoolDown = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Available = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemainingOrCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InRange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsProc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // Button_GetCombatants
            // 
            this.Button_GetCombatants.Location = new System.Drawing.Point(6, 6);
            this.Button_GetCombatants.Name = "Button_GetCombatants";
            this.Button_GetCombatants.Size = new System.Drawing.Size(119, 23);
            this.Button_GetCombatants.TabIndex = 0;
            this.Button_GetCombatants.Text = "GetCombatants";
            this.Button_GetCombatants.UseVisualStyleBackColor = true;
            this.Button_GetCombatants.Click += new System.EventHandler(this.Button_GetCombatants_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.CharacterName,
            this.EffectiveDistance,
            this.Heading});
            this.dataGridView1.Location = new System.Drawing.Point(6, 35);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(448, 303);
            this.dataGridView1.TabIndex = 1;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            // 
            // CharacterName
            // 
            this.CharacterName.HeaderText = "Name";
            this.CharacterName.Name = "CharacterName";
            this.CharacterName.ReadOnly = true;
            // 
            // EffectiveDistance
            // 
            this.EffectiveDistance.HeaderText = "EffectiveDistance";
            this.EffectiveDistance.Name = "EffectiveDistance";
            this.EffectiveDistance.ReadOnly = true;
            // 
            // Heading
            // 
            this.Heading.HeaderText = "Heading";
            this.Heading.Name = "Heading";
            this.Heading.ReadOnly = true;
            // 
            // Button_CameraInfo
            // 
            this.Button_CameraInfo.Location = new System.Drawing.Point(6, 6);
            this.Button_CameraInfo.Name = "Button_CameraInfo";
            this.Button_CameraInfo.Size = new System.Drawing.Size(119, 23);
            this.Button_CameraInfo.TabIndex = 2;
            this.Button_CameraInfo.Text = "Start";
            this.Button_CameraInfo.UseVisualStyleBackColor = true;
            this.Button_CameraInfo.Click += new System.EventHandler(this.Button_CameraInfo_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(468, 370);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Button_GetCombatants);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(460, 344);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Combatants";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.textBox_Camera_Elevation);
            this.tabPage2.Controls.Add(this.textBox_Camera_Heading);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.textBox_Camera_Mode);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.Button_CameraInfo);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(460, 344);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Camera";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Elevation";
            // 
            // textBox_Camera_Elevation
            // 
            this.textBox_Camera_Elevation.Location = new System.Drawing.Point(79, 114);
            this.textBox_Camera_Elevation.Name = "textBox_Camera_Elevation";
            this.textBox_Camera_Elevation.ReadOnly = true;
            this.textBox_Camera_Elevation.Size = new System.Drawing.Size(100, 19);
            this.textBox_Camera_Elevation.TabIndex = 7;
            // 
            // textBox_Camera_Heading
            // 
            this.textBox_Camera_Heading.Location = new System.Drawing.Point(79, 89);
            this.textBox_Camera_Heading.Name = "textBox_Camera_Heading";
            this.textBox_Camera_Heading.ReadOnly = true;
            this.textBox_Camera_Heading.Size = new System.Drawing.Size(100, 19);
            this.textBox_Camera_Heading.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Heading";
            // 
            // textBox_Camera_Mode
            // 
            this.textBox_Camera_Mode.Location = new System.Drawing.Point(79, 64);
            this.textBox_Camera_Mode.Name = "textBox_Camera_Mode";
            this.textBox_Camera_Mode.ReadOnly = true;
            this.textBox_Camera_Mode.Size = new System.Drawing.Size(100, 19);
            this.textBox_Camera_Mode.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Mode";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Button_GetHotbarRecast);
            this.tabPage3.Controls.Add(this.dataGridView2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(460, 344);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Hotbar";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Button_GetHotbarRecast
            // 
            this.Button_GetHotbarRecast.Location = new System.Drawing.Point(6, 6);
            this.Button_GetHotbarRecast.Name = "Button_GetHotbarRecast";
            this.Button_GetHotbarRecast.Size = new System.Drawing.Size(118, 23);
            this.Button_GetHotbarRecast.TabIndex = 1;
            this.Button_GetHotbarRecast.Text = "GetHotbarRecast";
            this.Button_GetHotbarRecast.UseVisualStyleBackColor = true;
            this.Button_GetHotbarRecast.Click += new System.EventHandler(this.Button_GetHotbarRecast_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Hotbar,
            this.ItemID,
            this.Slot,
            this.ItemName,
            this.Category,
            this.Type,
            this.Icon,
            this.CoolDown,
            this.Available,
            this.RemainingOrCost,
            this.Amount,
            this.InRange,
            this.IsProc});
            this.dataGridView2.Location = new System.Drawing.Point(6, 35);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 21;
            this.dataGridView2.Size = new System.Drawing.Size(448, 303);
            this.dataGridView2.TabIndex = 0;
            // 
            // Hotbar
            // 
            this.Hotbar.HeaderText = "Hotbar";
            this.Hotbar.Name = "Hotbar";
            this.Hotbar.ReadOnly = true;
            // 
            // ItemID
            // 
            this.ItemID.HeaderText = "ID";
            this.ItemID.Name = "ItemID";
            this.ItemID.ReadOnly = true;
            // 
            // Slot
            // 
            this.Slot.HeaderText = "Slot";
            this.Slot.Name = "Slot";
            this.Slot.ReadOnly = true;
            // 
            // ItemName
            // 
            this.ItemName.HeaderText = "Name";
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            // 
            // Category
            // 
            this.Category.HeaderText = "Category";
            this.Category.Name = "Category";
            this.Category.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            // 
            // Icon
            // 
            this.Icon.HeaderText = "Icon";
            this.Icon.Name = "Icon";
            this.Icon.ReadOnly = true;
            // 
            // CoolDown
            // 
            this.CoolDown.HeaderText = "CoolDown";
            this.CoolDown.Name = "CoolDown";
            this.CoolDown.ReadOnly = true;
            // 
            // Available
            // 
            this.Available.HeaderText = "Available";
            this.Available.Name = "Available";
            this.Available.ReadOnly = true;
            // 
            // RemainingOrCost
            // 
            this.RemainingOrCost.HeaderText = "Remaining";
            this.RemainingOrCost.Name = "RemainingOrCost";
            this.RemainingOrCost.ReadOnly = true;
            // 
            // Amount
            // 
            this.Amount.HeaderText = "Amount";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            // 
            // InRange
            // 
            this.InRange.HeaderText = "InRange";
            this.InRange.Name = "InRange";
            this.InRange.ReadOnly = true;
            // 
            // IsProc
            // 
            this.IsProc.HeaderText = "IsProc";
            this.IsProc.Name = "IsProc";
            this.IsProc.ReadOnly = true;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(483, 376);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Button_GetCombatants;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CharacterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn EffectiveDistance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Heading;
        private System.Windows.Forms.Button Button_CameraInfo;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox_Camera_Heading;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Camera_Mode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Camera_Elevation;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button Button_GetHotbarRecast;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hotbar;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Slot;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Category;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Icon;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoolDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn Available;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemainingOrCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn InRange;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsProc;
    }
}
