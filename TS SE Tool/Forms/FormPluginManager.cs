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

        public static List<GamePlugin> FindMatchingPlugins(DirectoryInfo gameDir) {
            var plugins = new List<GamePlugin>();
            var x86Dir = GetPluginsDir(gameDir, "win_x86");
            var all86Files = x86Dir.GetFiles("*.dll").Concat(x86Dir.GetFiles("*.disabled"));
            var x64Dir = GetPluginsDir(gameDir, "win_x64");
            var all64Files = x64Dir.GetFiles("*.dll").Concat(x64Dir.GetFiles("*.disabled"));
            var x86Files = all86Files.Where(f => !f.Is64Bit(true)).Concat(all64Files.Where(f => !f.Is64Bit()));
            var x64Files = all64Files.Where(f => f.Is64Bit(true)).Concat(all86Files.Where(f => f.Is64Bit()));
            foreach (var x86File in x86Files) {
                var plugin = new GamePlugin() { File32bit = x86File };
                //FileInfo foundMatch = null;
                foreach (var x64File in x64Files) {
                    var matchScore = Fuzz.Ratio(x86File.Name, x64File.Name);
                    if (matchScore > 70) {
                        plugin.File64bit = x64File;
                        break;
                    }
                }
                //if (!plugin.x64) {
                //    foreach (var _x86file in x86Files) {
                //        var matchScore = Fuzz.Ratio(x86File.Name, _x86file.Name);
                //        if (matchScore > 80) {
                //            plugin.File64bit = x86File;
                //            plugin.File32bit = _x86file;
                //            break;
                //        }
                //    }
                //}
                if (plugin.x86 || plugin.x64) plugins.Add(plugin);
            }
            plugins = plugins.Distinct().ToList();
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
            tablePlugins.CellContentClick += tablePlugins_CellContentClick;
            tablePlugins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            PopulatePlugins(MainForm.GameType);
            foreach (var i in new[] { 0, 3, 4 })
                tablePlugins.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            Plugins.ListChanged += Plugins_ListChanged;
            Plugins.AddingNew += Plugins_ListChanged;
        }

        private void Plugins_ListChanged(object sender, object e) {
            throw new NotImplementedException();
        }

        private void tablePlugins_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            tablePlugins.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void openPluginsDirToolStripMenuItem_Click(object sender, EventArgs e) {
            var arch = ((ToolStripItem)sender).Text;
            var dir = MainForm.GameType == "ATS" ? ATSGameDir : ETS2GameDir;
            GetPluginsDir(dir, "win_" + arch).OpenInExplorer();
        }

        //private void tablePlugins_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
        //    UpdateDataGridViewSite();
        //}
    }
}
