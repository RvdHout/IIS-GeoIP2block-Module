#nullable disable
/* GeoblockModulePage.cs
 *
 * Copyright (C) 2009 Triple IT.  All Rights Reserved.
 * Author: Frank Lippes, Modified for IIS 10 (.Net 4.6) by RvdH
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.Management.Client;
using Microsoft.Web.Management.Client.Win32;

namespace IISGeoIP2blockModule
{
    public class ComboboxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// Represents an interface towards IIS that lets the user manage the module configuration
    /// </summary>
    public sealed class GeoblockModuleDialogPage : Microsoft.Web.Management.Client.Win32.ModuleDialogPage
    //public sealed class GeoblockModuleDialogPage : System.Windows.Forms.Form
    {
        //Configuration objects
        private GeoblockModuleServiceProxy serviceProxy;
        private GeoblockConfiguration moduleConfiguration;

        //Event values
        public bool hasChanges = false;
        public bool formLoaded = false;
        public bool isValidDatabase = true;

        //Interface elements
        private System.Windows.Forms.CheckBox enabledCB;
        private System.Windows.Forms.CheckBox verifyAllCB;
        private System.Windows.Forms.ComboBox comboBoxDenyAction;
        private System.Windows.Forms.RadioButton allowedRB;
        private System.Windows.Forms.RadioButton deniedRB;
        private System.Windows.Forms.OpenFileDialog selectFileDialog;
        private System.Windows.Forms.TextBox geoIpFilepathTB;
        private System.Windows.Forms.Button geoIpFilepathB;
        private System.Windows.Forms.Label geoIpFilepathL;
        private System.Windows.Forms.Label geoIpFileinfoL;
        private System.Windows.Forms.CheckedListBox selectedCountryCodesLB;
        private System.Windows.Forms.Label exceptionsL;
        private System.Windows.Forms.Label DenyActionL;
        private System.Windows.Forms.DataGridView exceptionRulesDGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn modeC;
        private System.Windows.Forms.DataGridViewTextBoxColumn requestorC;
        private System.Windows.Forms.ContextMenuStrip exceptionRulesContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addAllowEntryM;
        private System.Windows.Forms.ToolStripMenuItem addDenyEntryM;
        private System.Windows.Forms.ToolStripSeparator separatorM;
        private System.Windows.Forms.ToolStripMenuItem removeEntryM;

        
        /// <summary>
        /// Creates a new GeoblockModulePage
        /// </summary>
        public GeoblockModuleDialogPage()
        {
            this.Initialize();
        }

        /// <summary>
        /// Is called when the module is activated. Displays the interface with the configured values
        /// </summary>
        /// <param name="initialActivation">Whether this is the first time the module is activated</param>
        protected override void OnActivated(bool initialActivation)
        {
            base.OnActivated(initialActivation);
            if (initialActivation)
            {
                GetConfiguration();
                DisplayConfiguration();
            }
        }

        /// <summary>
        /// Initializes the interface
        /// </summary>
        private void Initialize()
        {
            this.enabledCB = new System.Windows.Forms.CheckBox();
            this.verifyAllCB = new System.Windows.Forms.CheckBox();
            this.allowedRB = new System.Windows.Forms.RadioButton();
            this.deniedRB = new System.Windows.Forms.RadioButton();
            this.geoIpFilepathTB = new System.Windows.Forms.TextBox();
            this.geoIpFilepathB = new System.Windows.Forms.Button();
            this.geoIpFilepathL = new System.Windows.Forms.Label();
            this.geoIpFileinfoL = new System.Windows.Forms.Label();
            this.selectedCountryCodesLB = new System.Windows.Forms.CheckedListBox();
            this.exceptionsL = new System.Windows.Forms.Label();
            this.DenyActionL = new System.Windows.Forms.Label();
            this.exceptionRulesDGV = new System.Windows.Forms.DataGridView();
            this.modeC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.requestorC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exceptionRulesContextMenu = new System.Windows.Forms.ContextMenuStrip();
            this.addAllowEntryM = new System.Windows.Forms.ToolStripMenuItem();
            this.addDenyEntryM = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorM = new System.Windows.Forms.ToolStripSeparator();
            this.removeEntryM = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxDenyAction = new System.Windows.Forms.ComboBox();

            this.enabledCB.AutoSize = true;
            this.enabledCB.Left = 5;
            this.enabledCB.Top = 10;
            this.enabledCB.Text = "Enable geoblock module";
            this.enabledCB.CheckedChanged += new EventHandler(enabledCB_CheckedChanged);

            this.DenyActionL.AutoSize = true;
            this.DenyActionL.Left = 5;
            this.DenyActionL.Top = 40;
            this.DenyActionL.Text = "Deny Action Type";

            this.comboBoxDenyAction.AutoSize = true;
            this.comboBoxDenyAction.Left = 5;
            this.comboBoxDenyAction.Top = 60;
            this.comboBoxDenyAction.Name = "comboBoxDenyAction";
            ComboboxItem item1 = new ComboboxItem();
            item1.Text = "Unauthorized (401)";
            item1.Value ="Unauthorized";
            this.comboBoxDenyAction.Items.Add(item1);
            ComboboxItem item2 = new ComboboxItem();
            item2.Text = "Forbidden (403)";
            item2.Value = "Forbidden";
            this.comboBoxDenyAction.Items.Add(item2);
            ComboboxItem item3 = new ComboboxItem();
            item3.Text = "Not Found (404)";
            item3.Value = "NotFound";
            this.comboBoxDenyAction.Items.Add(item3);
            ComboboxItem item4 = new ComboboxItem();
            item4.Text = "Gone (410)";
            item4.Value = "Gone";
            this.comboBoxDenyAction.Items.Add(item4);
            ComboboxItem item5 = new ComboboxItem();
            item5.Text = "Abort";
            item5.Value = "Abort";
            this.comboBoxDenyAction.Items.Add(item5);
            this.comboBoxDenyAction.SelectedIndex = 0;
            this.comboBoxDenyAction.SelectedIndexChanged += new EventHandler(comboBoxDenyAction_SelectedIndexChanged);

            this.geoIpFilepathL.AutoSize = true;
            this.geoIpFilepathL.Left = 5;
            this.geoIpFilepathL.Top = 90;
            this.geoIpFilepathL.Text = "GeoIP2 Country Database";
            
            this.geoIpFilepathTB.Left = 5;
            this.geoIpFilepathTB.Top = 110;
            this.geoIpFilepathTB.Width = 279;
            this.geoIpFilepathTB.Height = 20;
            this.geoIpFilepathTB.TextChanged += new EventHandler(geoIpFilepathTB_TextChanged);
            this.geoIpFilepathTB.Validating += new CancelEventHandler(geoIpFilepathTB_Validating);

            this.geoIpFilepathB.Left = 209;
            this.geoIpFilepathB.Top = 136;
            this.geoIpFilepathB.Width = 75;
            this.geoIpFilepathB.Height = 23;
            this.geoIpFilepathB.Text = "Select file";
            this.geoIpFilepathB.Click += new System.EventHandler(this.geoIpFilepathB_Click);

            this.geoIpFileinfoL.AutoSize = true;
            this.geoIpFileinfoL.Left = 5;
            this.geoIpFileinfoL.Top = 140;
            this.geoIpFileinfoL.Text = string.Empty;

            this.verifyAllCB.AutoSize = true;
            this.verifyAllCB.Left = 5;
            this.verifyAllCB.Top = 170;
            this.verifyAllCB.Text = "Verify all IP addresses in HTTP_X_FORWARDED_FOR";
            this.verifyAllCB.CheckedChanged += new EventHandler(verifyAllCB_CheckedChanged);

            this.allowedRB.AutoSize = true;
            this.allowedRB.Checked = true;
            this.allowedRB.Left = 5;
            this.allowedRB.Top = 194;
            this.allowedRB.Text = "Allow access for";
            this.allowedRB.CheckedChanged += new EventHandler(allowedRB_CheckedChanged);
            
            this.deniedRB.AutoSize = true;
            this.deniedRB.Left = 5;
            this.deniedRB.Top = 214;
            this.deniedRB.Text = "Deny access for";
            this.deniedRB.CheckedChanged += new EventHandler(deniedRB_CheckedChanged);

            this.selectedCountryCodesLB.Left = 5;
            this.selectedCountryCodesLB.Top = 237;
            this.selectedCountryCodesLB.Width = 279;
            this.selectedCountryCodesLB.Height = 184;
            this.selectedCountryCodesLB.CheckOnClick = true;
            this.selectedCountryCodesLB.SelectedIndexChanged += new EventHandler(selectedCountryCodesLB_SelectedIndexChanged);

            //fill countries
            for (int i = 0; i < Countries.CountryCodes.Length; i++)
            {
                this.selectedCountryCodesLB.Items.Add(new Country(Countries.CountryCodes[i], Countries.CountryNames[i]));
            }
            this.selectedCountryCodesLB.Sorted = true;


            this.exceptionsL.AutoSize = true;
            this.exceptionsL.Left = 5;
            this.exceptionsL.Top = 430;
            this.exceptionsL.Text = "Exception rules";

            ((System.ComponentModel.ISupportInitialize)(this.exceptionRulesDGV)).BeginInit();
            this.exceptionRulesDGV.AutoGenerateColumns = false;
            this.exceptionRulesDGV.AllowUserToAddRows = false;
            this.exceptionRulesDGV.AllowUserToResizeRows = false;
            this.exceptionRulesDGV.BackgroundColor = System.Drawing.SystemColors.Window;
            this.exceptionRulesDGV.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.exceptionRulesDGV.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.exceptionRulesDGV.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.exceptionRulesDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.exceptionRulesDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.modeC, this.requestorC });
            this.exceptionRulesDGV.ContextMenuStrip = this.exceptionRulesContextMenu;
            this.exceptionRulesDGV.MultiSelect = false;
            this.exceptionRulesDGV.RowHeadersVisible = false;
            this.exceptionRulesDGV.EnableHeadersVisualStyles = false;
            this.exceptionRulesDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.exceptionRulesDGV.ShowCellErrors = false;
            this.exceptionRulesDGV.ShowCellToolTips = false;
            this.exceptionRulesDGV.ShowEditingIcon = false;
            this.exceptionRulesDGV.ShowRowErrors = false;
            this.exceptionRulesDGV.Width = 279;
            this.exceptionRulesDGV.Height = 184;
            this.exceptionRulesDGV.Left = 5;
            this.exceptionRulesDGV.Top = 446;
            this.exceptionRulesDGV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ExceptionRules_MouseDown);
            this.exceptionRulesDGV.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.ExceptionRules_UserDeletingRow);
            this.exceptionRulesDGV.SelectionChanged += new EventHandler(exceptionRulesDGV_SelectionChanged);
            this.exceptionRulesDGV.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(exceptionRulesDGV_DataBindingComplete);
            ((System.ComponentModel.ISupportInitialize)(this.exceptionRulesDGV)).EndInit();

            this.modeC.DataPropertyName = "Mode";
            this.modeC.HeaderText = "Mode";
            this.modeC.ReadOnly = true;
            this.modeC.Width = 65;
            
            this.requestorC.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.requestorC.DataPropertyName = "Requestor";
            this.requestorC.HeaderText = "Requestor";
            this.requestorC.ReadOnly = true;

            this.exceptionRulesContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.addAllowEntryM, this.addDenyEntryM, this.separatorM, this.removeEntryM });
            
            this.addAllowEntryM.Text = "Add Allow Exception Rule...";
            this.addAllowEntryM.Click += new System.EventHandler(this.AddAllowEntry_Click);

            this.addDenyEntryM.Text = "Add Deny Exception Rule...";
            this.addDenyEntryM.Click += new System.EventHandler(this.AddDenyEntry_Click);

            System.IO.Stream removeImageStream = this.GetType().Assembly.GetManifestResourceStream("IISGeoIP2blockModule.resources.remove.png");
            this.removeEntryM.Image = System.Drawing.Image.FromStream(removeImageStream);
            this.removeEntryM.Text = "Remove Exception Rule";
            this.removeEntryM.Click += new System.EventHandler(this.RemoveEntry_Click);

            Controls.Add(enabledCB);
            Controls.Add(verifyAllCB);
            Controls.Add(comboBoxDenyAction);
            Controls.Add(allowedRB);
            Controls.Add(deniedRB);
            Controls.Add(geoIpFilepathTB);
            Controls.Add(geoIpFilepathB);
            Controls.Add(geoIpFilepathL);
            Controls.Add(geoIpFileinfoL);
            Controls.Add(selectedCountryCodesLB);
            Controls.Add(exceptionsL);
            Controls.Add(DenyActionL);
            Controls.Add(exceptionRulesDGV);
            
            this.AutoScroll = true;
        }

        /// <summary>
        /// Indicate that a form value changed and update the tasklist
        /// </summary>
        private void ConfigChanged()
        {
            if (formLoaded)
                this.hasChanges = true;
            this.Update();
        }
        
        /// <summary>
        /// Shows a dialog on which the user can specify an allow exception rule
        /// </summary>
        public void AddAllowExceptionRule()
        {
            AddExceptionRuleForm form = new AddExceptionRuleForm(true, (SortableBindingList<ExceptionRule>)this.exceptionRulesDGV.DataSource);
            DialogResult result = form.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ((SortableBindingList<ExceptionRule>)exceptionRulesDGV.DataSource).Add(form.ExceptionRule);
                ConfigChanged();
            }
            form.Dispose();
        }

        /// <summary>
        /// Shows a dialog on which the user can specify an deny exception rule
        /// </summary>
        public void AddDenyExceptionRule()
        {
            AddExceptionRuleForm form = new AddExceptionRuleForm(false, (SortableBindingList<ExceptionRule>)this.exceptionRulesDGV.DataSource);
            DialogResult result = form.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                ((SortableBindingList<ExceptionRule>)exceptionRulesDGV.DataSource).Add(form.ExceptionRule);
                ConfigChanged();
            }
            form.Dispose();
        }

        /// <summary>
        /// Asks the user to confirm the removal. Removes the exception rule when the user confirms
        /// </summary>
        public void RemoveExceptionRule()
        {
            foreach (DataGridViewRow row in exceptionRulesDGV.SelectedRows)
            {
                if (ConfirmDeleteExceptionRule())
                {
                    exceptionRulesDGV.Rows.Remove(row);
                    ConfigChanged();
                }
            }
        }

        /// <summary>
        /// Asks the user to confirm the removal
        /// </summary>
        /// <returns>Whether or not the user confirmed</returns>
        private bool ConfirmDeleteExceptionRule()
        {
            if (MessageBox.Show(this, "Are you sure that you want to remove the selected exception rule?", "Geoblocker exception rule", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
            {
                return false;
            }
            else
            {
                ConfigChanged();
                return true;
            }
        }


        /// <summary>
        /// Fills the interface with the current configuration
        /// </summary>
        void DisplayConfiguration()
        {
            enabledCB.Checked = this.moduleConfiguration.Enabled;
            enabledCB.Enabled = !string.IsNullOrEmpty(this.moduleConfiguration.GeoIpFilepath.Trim()) && System.IO.File.Exists(this.moduleConfiguration.GeoIpFilepath.Trim());
            verifyAllCB.Checked = this.moduleConfiguration.VerifyAll;
            for (int i = 0; i < comboBoxDenyAction.Items.Count; i++)
            {
                ComboboxItem ci = (ComboboxItem)comboBoxDenyAction.Items[i];
                if (ci != null && ci.Value.ToString() == this.moduleConfiguration.DenyAction)
                {
                    comboBoxDenyAction.SelectedIndex = i;
                    break;
                }
            }
            if (this.moduleConfiguration.AllowedMode)
                allowedRB.Checked = true;
            else
                deniedRB.Checked = true;
            geoIpFilepathTB.Text = this.moduleConfiguration.GeoIpFilepath;
            geoIpFileinfoL.Text = getGeoIPDatabaseInfo(geoIpFilepathTB.Text);

            //Selected countries
            this.selectedCountryCodesLB.ClearSelected();
            if (this.moduleConfiguration.SelectedCountryCodes != null)
            {
                for (int i = 0; i < this.selectedCountryCodesLB.Items.Count; i++)
                {
                    bool found = false;
                    Country geoblockItem = selectedCountryCodesLB.Items[i] as Country;
                    foreach (Country selectedGeoblockItem in this.moduleConfiguration.SelectedCountryCodes)
                    {
                        if (geoblockItem.CountryCode == selectedGeoblockItem.CountryCode)
                        {
                            this.selectedCountryCodesLB.SetItemChecked(i, true);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        this.selectedCountryCodesLB.SetItemChecked(i, false);
                    }
                }
            }

            //Exception rules
            SortableBindingList<ExceptionRule> datasource = new SortableBindingList<ExceptionRule>();
            if (this.moduleConfiguration.ExceptionRules != null)
            {
                foreach (ExceptionRule exceptionRule in this.moduleConfiguration.ExceptionRules)
                {
                    datasource.Add(exceptionRule);
                }
            }
            this.exceptionRulesDGV.DataSource = datasource;
            this.exceptionRulesDGV.ClearSelection();

            formLoaded = true;
        }

        /// <summary>
        /// Puts the values from the page into the configuration
        /// </summary>
        protected override bool ApplyChanges()
        {
            if (this.isValidDatabase)
            {
                this.moduleConfiguration.Enabled = !string.IsNullOrEmpty(geoIpFilepathTB.Text.Trim()) && System.IO.File.Exists(geoIpFilepathTB.Text.Trim()) ? enabledCB.Checked : false;
                this.moduleConfiguration.VerifyAll = verifyAllCB.Checked;
                this.moduleConfiguration.DenyAction = (comboBoxDenyAction.SelectedItem as ComboboxItem).Value.ToString();
                this.moduleConfiguration.AllowedMode = allowedRB.Checked;
                this.moduleConfiguration.GeoIpFilepath = geoIpFilepathTB.Text;

                this.moduleConfiguration.SelectedCountryCodes.Clear();
                foreach (Country geoblockItem in selectedCountryCodesLB.CheckedItems)
                {
                    this.moduleConfiguration.SelectedCountryCodes.Add(geoblockItem);
                }

                this.moduleConfiguration.ExceptionRules.Clear();
                foreach (DataGridViewRow row in this.exceptionRulesDGV.Rows)
                {
                    ExceptionRule item = (ExceptionRule)row.DataBoundItem;
                    this.moduleConfiguration.ExceptionRules.Add(item);
                }

                UpdateConfiguration();

                hasChanges = false;
                this.Update();//reloads tasks

                return true;
            }
            MessageBox.Show("This is not a GeoLite2-Country or GeoIP2-Country database");
            return false;
        }

        /// <summary>
        /// Indicates if the configuration values have changed
        /// </summary>
        protected override bool HasChanges
        {
            get { return this.hasChanges; }
        }

        /// <summary>
        /// Indicates that this page can apply changes
        /// </summary>
        protected override bool CanApplyChanges
        {
            get { return true; }
        }

        /// <summary>
        /// Cancels the changes in the page and reverts to the last saved configuration
        /// </summary>
        protected override void CancelChanges()
        {
            DisplayConfiguration();
            hasChanges = false;
            this.Update();
        }

        /// <summary>
        /// Gets the configuration for this module
        /// </summary>
        public void GetConfiguration()
        {
            if(serviceProxy == null)
                serviceProxy = (GeoblockModuleServiceProxy)CreateProxy(typeof(GeoblockModuleServiceProxy));
            moduleConfiguration = serviceProxy.GetGeoblockConfiguration();
        }

        /// <summary>
        /// Updates the configuration for this module
        /// </summary>
        public void UpdateConfiguration()
        {
            serviceProxy.UpdateGeoblockConfiguration(moduleConfiguration);
        }

        private AddExceptionRuleTaskList addExceptionRuleTaskList;
        private RemoveExceptionRuleTaskList removeExceptionRuleTaskList;
        /// <summary>
        /// The tasks for this module page to display
        /// </summary>
        protected override TaskListCollection Tasks
        {
            get
            {
                if (addExceptionRuleTaskList == null)
                {
                    addExceptionRuleTaskList = new AddExceptionRuleTaskList(this);
                }
                if (removeExceptionRuleTaskList == null)
                {
                    removeExceptionRuleTaskList = new RemoveExceptionRuleTaskList(this);
                }

                TaskListCollection taskListCollection = base.Tasks;
                taskListCollection.Add(addExceptionRuleTaskList);

                if (this.exceptionRulesDGV.SelectedRows.Count > 0)
                {
                    taskListCollection.Add(removeExceptionRuleTaskList);
                }

                return taskListCollection;
            }
        }

        /// <summary>
        /// Lets the user choose the geo ip data file
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void geoIpFilepathB_Click(object sender, EventArgs e)
        {
            this.selectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.selectFileDialog.FileName = string.Empty;
            this.selectFileDialog.Filter = "mmdb files|*.mmdb";
            this.selectFileDialog.Title = "Select GeoIP2 Country Database";
            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                geoIpFilepathTB.Text = selectFileDialog.FileName;
                geoIpFileinfoL.Text = getGeoIPDatabaseInfo(selectFileDialog.FileName);
            }
        }

        /// <summary>
        /// Controls the way the datagridview handles row selections
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Mouse event args</param>
        private void ExceptionRules_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti;
            hti = exceptionRulesDGV.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (hti.Type == DataGridViewHitTestType.Cell)
                {
                    if (!((DataGridViewRow)(exceptionRulesDGV.Rows[hti.RowIndex])).Selected)
                    {
                        exceptionRulesDGV.ClearSelection();
                        ((DataGridViewRow)exceptionRulesDGV.Rows[hti.RowIndex]).Selected = true;
                    }
                    separatorM.Visible = true;
                    removeEntryM.Visible = true;
                }
                else
                {
                    separatorM.Visible = false;
                    removeEntryM.Visible = false;
                    this.exceptionRulesDGV.ClearSelection();
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                if (hti.Type != DataGridViewHitTestType.Cell)
                {
                    this.exceptionRulesDGV.ClearSelection();
                }
            }
        }

        void exceptionRulesDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            exceptionRulesDGV.ClearSelection();
        }

        /// <summary>
        /// Updates the page to update the tasklist. If an exception rule is selected, the remove task is shown in the tasklist
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void exceptionRulesDGV_SelectionChanged(object sender, EventArgs e)
        {
            this.Update();
        }

        private void ExceptionRules_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!ConfirmDeleteExceptionRule())
            {
                e.Cancel = true;
            }
        }

        private void AddAllowEntry_Click(object sender, EventArgs e)
        {
            AddAllowExceptionRule();
        }

        private void AddDenyEntry_Click(object sender, EventArgs e)
        {
            AddDenyExceptionRule();
        }

        public void RemoveEntry_Click(object sender, EventArgs e)
        {
            RemoveExceptionRule();
        }

        private void enabledCB_CheckedChanged(object sender, EventArgs e)
        {
            ConfigChanged();
        }

        private void verifyAllCB_CheckedChanged(object sender, EventArgs e)
        {
            ConfigChanged();
        }

        private void geoIpFilepathTB_TextChanged(object sender, EventArgs e)
        {
            geoIpFileinfoL.Text = getGeoIPDatabaseInfo(geoIpFilepathTB.Text);
            ConfigChanged();
        }

        private void geoIpFilepathTB_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(geoIpFilepathTB.Text))
            {
                enabledCB.Checked = false;
                enabledCB.Enabled = false;
                this.isValidDatabase = true;
            }
        }

        private void allowedRB_CheckedChanged(object sender, EventArgs e)
        {
            ConfigChanged();
        }

        private void deniedRB_CheckedChanged(object sender, EventArgs e)
        {
            ConfigChanged();
        }

        private void selectedCountryCodesLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigChanged();
        }

        private void comboBoxDenyAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigChanged();
        }

        private string getGeoIPDatabaseInfo(string path)
        {
            if (System.IO.File.Exists(path))
            {
                using (var reader = new MaxMind.GeoIP2.DatabaseReader(path, MaxMind.Db.FileAccessMode.MemoryMapped))
                {
                    if (reader.Metadata.DatabaseType.ToLower().IndexOf("country") == -1)
                    {
                        enabledCB.Enabled = false;
                        this.isValidDatabase = false;
                        geoIpFileinfoL.ForeColor = System.Drawing.Color.Red;
                        return "Invalid GeoIP2 Database Type!";
                    }
                    enabledCB.Enabled = true;
                    this.isValidDatabase = true;
                    geoIpFileinfoL.ForeColor = SystemColors.ControlText;
                    return string.Format("{0}, {1}", reader.Metadata.DatabaseType, reader.Metadata.BuildDate);
                }
            }
            return string.Empty;
        }
    }
}
