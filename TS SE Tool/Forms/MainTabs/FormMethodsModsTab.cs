/*
   Copyright 2016-2022 LIPtoH <liptoh.codebase@gmail.com>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TS_SE_Tool.Utilities;
using System.ComponentModel;
using System.Linq;

namespace TS_SE_Tool {
    public partial class FormMain {
        public static BindingList<ProfileMod> ProfileMods = new BindingList<ProfileMod>();
        #region ProfileTab
        private void FillModList() {
            //Text = $"Manage Plugins for {MainForm.GameType}";
            tableMods.Columns.Clear();
            tableMods.Rows.Clear();
            tableMods.DataSource = ProfileMods;
            tableMods.RowHeadersVisible = false;
            tableMods.AllowUserToAddRows = false;
            tableMods.AllowUserToDeleteRows = false;
            tableMods.AllowUserToOrderColumns = true;
            tableMods.MultiSelect = true;
            tableMods.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tableMods.CellContentClick += tableMods_CellContentClick;
            tableMods.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            PopulateMods();
            //foreach (var i in new[] { 0 })
            //    tableMods.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
        }

        private void PopulateMods() {
            ProfileMods.Clear();
            foreach (var mod in MainSaveFileProfileData.ActiveMods) {
                ProfileMods.Add(new ProfileMod(mod));
            }
        }

        private void tableMods_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            tableMods.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        #endregion ProfileTab
    }
}