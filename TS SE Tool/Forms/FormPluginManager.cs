using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using FuzzySharp;
using System.Linq;
using System.Security.AccessControl;
using TS_SE_Tool.Utilities;
using System.Xml.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using MoreLinq;

namespace TS_SE_Tool.Forms {
    public partial class FormPluginManager : Form {
        FormMain MainForm = Application.OpenForms.OfType<FormMain>().Single();
        public static DirectoryInfo ETS2GameDir { get; private set; }
        public static DirectoryInfo ATSGameDir { get; private set; }
        public static BindingList<GamePlugin> Plugins = new BindingList<GamePlugin>();


        public FormPluginManager() {
            InitializeComponent();
        }

        public static DirectoryInfo GetPluginsDir(DirectoryInfo gameDir, string arch = "win_x64") => gameDir.Combine("bin", arch, "plugins");
        public static DirectoryInfo GetGameDir(string _game) => _game == "ATS" ? ATSGameDir : ETS2GameDir;

        public static void Initialize() {
            if (Globals.SteamGameLocator.getIsSteamInstalled()) {
                Globals.SteamDir = new DirectoryInfo(Globals.SteamGameLocator.getSteamInstallLocation());
                try {
                    var dir = Globals.SteamGameLocator.getGameInfoByFolder("Euro Truck Simulator 2");
                    if (!string.IsNullOrWhiteSpace(dir.steamGameLocation)) {
                        ETS2GameDir = new DirectoryInfo(dir.steamGameLocation);
                        IO_Utilities.LogWriter($"Found ETS2 Game Directory: {ETS2GameDir.FullString()}");
                        //ETS2Plugins = FindMatchingPlugins(ETS2PluginsDir);
                    }
                } catch { }
                try {
                    var dir = Globals.SteamGameLocator.getGameInfoByFolder("American Truck Simulator");
                    if (!string.IsNullOrWhiteSpace(dir.steamGameLocation)) {
                        ATSGameDir = new DirectoryInfo(dir.steamGameLocation);
                        IO_Utilities.LogWriter($"Found ATS Game Directory: {ATSGameDir.FullString()}");
                        //ATSPlugins = FindMatchingPlugins(ATSPluginsDir);
                    }
                } catch { }
            }
        }

        public static GamePlugin? FindPluginByPath(IEnumerable<GamePlugin> plugins, FileInfo file) {
            foreach (var plugin in plugins) {
                if (plugin.File32bit.FullName == file.FullName) return plugin;
                if (plugin.File64bit.FullName == file.FullName) return plugin;
            }
            return null;
        }

        public static HashSet<GamePlugin> FindMatchingPlugins(DirectoryInfo gameDir) {
            var plugins = new HashSet<GamePlugin>();
            var x86Dir = GetPluginsDir(gameDir, "win_x86");
            var all86Files = x86Dir.GetFiles("*.dll").Concat(x86Dir.GetFiles("*.disabled"));
            var x64Dir = GetPluginsDir(gameDir, "win_x64");
            var all64Files = x64Dir.GetFiles("*.dll").Concat(x64Dir.GetFiles("*.disabled"));
            var x86Files = all86Files.Where(f => !f.Is64BitDll()).Concat(all64Files.Where(f => !f.Is64BitDll()));
            var x64Files = all64Files.Where(f => f.Is64BitDll()).Concat(all86Files.Where(f => f.Is64BitDll()));
            foreach (var x86File in x86Files) {
                var plugin = FindPluginByPath(plugins, x86File);
                //if (plugin != null && !plugin.x86) plugin.File32bit = x86File;
                if (plugin is null) plugin = new GamePlugin() { File32bit = x86File };
                //FileInfo foundMatch = null;
                foreach (var x64File in x64Files) {
                    var matchScore = Fuzz.Ratio(x86File.Name, x64File.Name);
                    if (matchScore > 70) {
                        plugin.File64bit = x64File;
                        break;
                    }
                }
                if (plugin.x86 || plugin.x64) plugins.Add(plugin);
            }
            foreach (var x64File in x64Files) {
                var plugin = FindPluginByPath(plugins, x64File);
                //if (plugin != null && !plugin.x64) plugin.File64bit = x64File;
                if (plugin is null) plugin = new GamePlugin() { File64bit = x64File };
                //FileInfo foundMatch = null;
                foreach (var x86File in x86Files) {
                    var matchScore = Fuzz.Ratio(x64File.Name, x86File.Name);
                    if (matchScore > 70) {
                        plugin.File32bit = x86File;
                        break;
                    }
                }
                if (plugin.x86 || plugin.x64) plugins.Add(plugin);
            }
            plugins = Enumerable.ToHashSet(plugins.Distinct());
            IO_Utilities.LogWriter($"Found {plugins.Count} distinct plugin pairs in {gameDir}");
            return plugins;
        }
        private void PopulatePlugins(string _game) {
            //tablePlugins.Rows.Clear();
            Plugins.Clear();
            FindMatchingPlugins(GetGameDir(_game)).ForEach(p => Plugins.Add(p));
            //foreach (var plugin in Plugins) {
            //    tablePlugins.Rows.Add(plugin.Enabled, plugin.Name, plugin.InstallDate, plugin.File32bit != null, plugin.File64bit != null);
            //}
        }

        private void FormPluginManager_Load(object sender, EventArgs e) {
            Text = $"Manage Plugins for {MainForm.GameType}";
            Initialize();
            tablePlugins.Columns.Clear();
            tablePlugins.Rows.Clear();
            tablePlugins.DataSource = Plugins;
            tablePlugins.RowHeadersVisible = false;
            tablePlugins.AllowUserToAddRows = false;
            tablePlugins.AllowUserToDeleteRows = false;
            tablePlugins.AllowUserToOrderColumns = true;
            tablePlugins.MultiSelect = true;
            tablePlugins.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablePlugins.CellContentClick += tablePlugins_CellContentClick;
            tablePlugins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            PopulatePlugins(MainForm.GameType);
            foreach (var i in new[] { 0, 3, 4 })
                tablePlugins.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            //Plugins.ListChanged += Plugins_ListChanged;
            //Plugins.AddingNew += Plugins_ListChanged;
        }

        private void Plugins_ListChanged(object sender, object e) {
            //throw new NotImplementedException();
        }

        private void tablePlugins_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            tablePlugins.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void openPluginsDirToolStripMenuItem_Click(object sender, EventArgs e) {
            var arch = ((ToolStripItem)sender).Text;
            var dir = MainForm.GameType == "ATS" ? ATSGameDir : ETS2GameDir;
            GetPluginsDir(dir, "win_" + arch).OpenInExplorer();
        }

        private GamePlugin GetPluginFromRow(int rowIndex, DataGridView table = null) {
            table ??= tablePlugins;
            if (table is null || rowIndex < 0 || rowIndex > table.RowCount) return null;
            return table.Rows[rowIndex].DataBoundItem as GamePlugin;
        }

        private List<GamePlugin> GetPluginsFromSelected() {
            var list = new List<GamePlugin>();
            foreach (DataGridViewRow row in tablePlugins.SelectedRows) {
                list.Add(row.DataBoundItem as GamePlugin);
            }
            return list;
            //MyType selectedItem = (MyType)list[0].DataBoundItem; //[0] ---> first item
        }

        //private ContextMenuStrip strip;
        private void tablePlugins_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e) {
            //var table = sender as DataGridView;
            //if (sender is null || table is null || e is null || e.RowIndex < 0 || e.RowIndex > table.RowCount) {
            //    MessageBox.Show("fail");
            //    return;
            //}
            var plugins = GetPluginsFromSelected(); //var plugin = GetPluginFromRow(e.RowIndex, table);
            if (plugins.Count == 1) {
                pluginContextMenu.Items[0].Text = plugins.First().Enabled ? "Disable" : "Enable";
            } else {
                pluginContextMenu.Items[0].Text = "Toggle";
            }
            e.ContextMenuStrip = pluginContextMenu;
        }

        private DialogResult AskUser(string action, MessageBoxIcon icons = MessageBoxIcon.Question) {
            var plugins = GetPluginsFromSelected();
            if (plugins is null || plugins.Count == 0) return DialogResult.Cancel;
            var sb = new StringBuilder("Are you sure you want to " + action + " the following files:\n\n");
            foreach (var plugin in plugins) {
                var files = plugin.Files;
                if (files.Count > 0) sb.AppendLine(string.Join(", ", files.Select(f => f.Name.Quote())));
            }
            return MessageBox.Show(sb.ToString(), action + " files?", MessageBoxButtons.YesNo, icons);
        }

        private void toggleToolStripMenuItem_Click(object sender, EventArgs e) {
            //var menuItem = sender as ToolStripMenuItem;
            if (AskUser("toggle") != DialogResult.Yes) return;
            GetPluginsFromSelected().ForEach(p => p.Enabled = !p.Enabled);
            PopulatePlugins(MainForm.GameType);
        }

        private void openFoldersToolStripMenuItem_Click(object sender, EventArgs e) {
            //var menuItem = sender as ToolStripMenuItem;
            var plugins = GetPluginsFromSelected();
            var folders = new List<DirectoryInfo>();
            foreach (var plugin in plugins) {
                if (plugin.x86) folders.Add(plugin.File32bit.Directory);
                if (plugin.x64) folders.Add(plugin.File64bit.Directory);
            }
            foreach (var folder in folders.DistinctBy(x => x.FullName).ToList()) {
                folder.OpenInExplorer();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            //var menuItem = sender as ToolStripMenuItem;
            if (AskUser("delete", MessageBoxIcon.Warning) != DialogResult.Yes) return;
            foreach (var plugin in GetPluginsFromSelected()) {
                plugin.Delete();
            }
            PopulatePlugins(MainForm.GameType);
        }

        private void tablePlugins_MouseHover(object sender, EventArgs e) {
            //var p = tablePlugins.PointToClient(Cursor.Position);
            //var info = tablePlugins.HitTest(p.X, p.Y);
            //var plugin = GetPluginFromRow(info.RowIndex, tablePlugins);
            //tablePlugins.Rows[0].Cells
        }
        private void tablePlugins_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            var src = sender as DataGridView;
            if (sender is null || src is null || e.RowIndex < 0 || e.RowIndex > src.RowCount) return;
            var plugin = GetPluginFromRow(e.RowIndex, tablePlugins);
            var files = plugin.Files;
            switch (e.ColumnIndex) {
                case 2:
                    if (files.Count > 0) new ToolTip().Show(string.Join("\n", files.Select(f => $"{f.Name.Quote()} = {f.LastWriteTime}")), this, Cursor.Position);
                    break;
            }
        }

        private void showDebugToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show(GetPluginsFromSelected().ToJson(true));// string.Join("\n", GetPluginsFromSelected().Select(p => p.ToJson())));
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e) {
            PopulatePlugins(MainForm.GameType);
        }

        //private void tablePlugins_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
        //    UpdateDataGridViewSite();
        //}
    }
}
