﻿/*
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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32;
using FuzzySharp;
using static System.Diagnostics.Process;

using TS_SE_Tool.Utilities;
using TS_SE_Tool.Forms;
using Narod.SteamGameFinder;

namespace TS_SE_Tool {
    public partial class FormMain {
        //Menu controls
        private void programSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
            FormProgramSettings FormWindow = new FormProgramSettings();
            FormWindow.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            FormSettings FormWindow = new FormSettings();
            FormWindow.ShowDialog();

            ApplySettings();
            ApplySettingsUI();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        //Language
        //tool strip click
        private void toolstripChangeLanguage(object sender, EventArgs e) {
            ToolStripItem obj = sender as ToolStripItem;
            string _objname = obj.Name;

            string[] cult = _objname.ToString().Split('_');
            string ButtonCultureInfo = cult[0] + "-" + cult[1];

            ProgSettingsV.Language = ButtonCultureInfo;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(ButtonCultureInfo);

            ChangeLanguage();
        }

        private void ChangeLanguage() {
            try {
                if (ProgSettingsV.Language != "Default")
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(ProgSettingsV.Language);//CultureInfo.GetCultureInfo("en-US");
            } catch {
                IO_Utilities.LogWriter("Wrong language setting format");
            }

            CultureInfo ci = Thread.CurrentThread.CurrentUICulture;

            try {
                this.SuspendLayout();

                HelpTranslateFormMethod(this, toolTipMain);
                HelpTranslateMenuStripMethod(menuStripMain);

                HelpTranslatContextMenuStripMethod(contextMenuStripMain);

                this.ResumeLayout();

                LngFileLoader("countries_translate.txt", CountriesLngDict, ProgSettingsV.Language);
                LngFileLoader("cities_translate.txt", CitiesLngDict, ProgSettingsV.Language);
                LngFileLoader("companies_translate.txt", CompaniesLngDict, ProgSettingsV.Language);
                LngFileLoader("cargo_translate.txt", CargoLngDict, ProgSettingsV.Language);
                LngFileLoader("urgency_translate.txt", UrgencyLngDict, ProgSettingsV.Language);
                //LngFileLoader("custom_strings.txt", CustomStringsDict, ProgSettingsV.Language);

                LoadTruckBrandsLng();
                LoadDriverNamesLng();

                AddTranslationToData();
                TranslateComboBoxes();
                CorrectControlsPositions();
            } catch { }

            //rm.ReleaseAllResources();
        }

        //About
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            FormAboutBox aboutWindow = new FormAboutBox();
            aboutWindow.ShowDialog();
        }

        //How to
        private void localPDFToolStripMenuItem_Click(object sender, EventArgs e) {
            string pdf_path = Directory.GetCurrentDirectory() + @"\HowTo.pdf";

            if (File.Exists(pdf_path))
                System.Diagnostics.Process.Start(pdf_path);
            else
                MessageBox.Show("Missing manual. Try to repair via update", "HowTo.pdf not found");
        }

        private void youTubeVideoToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(Utilities.Web_Utilities.External.linkYoutubeTutorial);
        }

        //Downloads
        private void checkGitHubRelesesToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(Utilities.Web_Utilities.External.linkGithubReleases);
        }

        private void checkTMPForumToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(Utilities.Web_Utilities.External.linkTMPforum);
        }

        private void checkSCSForumToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(Utilities.Web_Utilities.External.linkSCSforum);
        }

        private void latestStableToolStripMenuItem_Click(object sender, EventArgs e) {
            FormCheckUpdates FormWindow = new FormCheckUpdates("check");
            FormWindow.ShowDialog();
        }

        //Menu controls End

        //Form methods
        private void ToggleControlsAccess(bool _state) {
            //Main Save controls
            buttonMainWriteSave.Enabled = _state;
            buttonMainWriteSave.Visible = _state;

            buttonMainCloseSave.Visible = _state;

            //Main tabs
            foreach (TabPage tp in tabControlMain.TabPages)
                tp.Enabled = _state;

            //Profile
            for (int i = 0; i < 6; i++) {
                Control[] tmp = this.Controls.Find("profileSkillsPanel" + i.ToString(), true);

                if (tmp[0] != null) {
                    Bitmap bgimg = new Bitmap(SkillImgS[i], 64, 64);

                    if (_state)
                        tmp[0].BackgroundImage = bgimg;
                    else
                        tmp[0].BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(bgimg);
                }
            }
        }

        private void ToggleMainControlsAccess(bool _state) {
            radioButtonMainGameSwitchETS.Enabled = _state;
            radioButtonMainGameSwitchATS.Enabled = _state;

            checkBoxProfilesAndSavesProfileBackups.Enabled = _state;
            buttonProfilesAndSavesRefreshAll.Enabled = _state;
            buttonProfilesAndSavesEditProfile.Enabled = _state;
            buttonProfilesAndSavesRestoreBackup.Enabled = _state;

            comboBoxRootFolders.Enabled = _state;
            comboBoxProfiles.Enabled = _state;
            comboBoxSaves.Enabled = _state;

            buttonMainDecryptSave.Enabled = _state;
            buttonMainLoadSave.Enabled = _state;

            buttonMainWriteSave.Enabled = _state;

            if (_state)
                CheckSaveControls();
        }

        private void CheckSaveControls() {
            // Root
            DataRowView drv = (DataRowView)comboBoxRootFolders.SelectedItem;

            Font loadButtonFont = buttonMainLoadSave.Font;

            // Change Load button properties based on Profile type
            if (drv["ProfileType"].ToString() == "steam") {
                buttonMainLoadSave.Enabled = false;
                buttonMainLoadSave.Text = ResourceManagerMain.GetString(buttonMainLoadSave.Name + "SteamCloud"); // Disable Steam Cloud

                buttonMainLoadSave.Font = new Font(loadButtonFont.FontFamily, 12f, FontStyle.Bold);

                labelHelpText.Visible = true;
                SteamSelectedToggler = false;
                SteamSelectedTimer.Start();
                SteamSelectedTimer.Enabled = true;
            } else {
                buttonMainLoadSave.Enabled = true;
                buttonMainLoadSave.Text = ResourceManagerMain.GetString(buttonMainLoadSave.Name); // Load

                buttonMainLoadSave.Font = new Font(loadButtonFont.FontFamily, 18F, FontStyle.Bold);

                labelHelpText.Visible = false;
                SteamSelectedTimer.Stop();
                SteamSelectedTimer.Enabled = false;
            }

            //===
            // Save
            drv = (DataRowView)comboBoxSaves.SelectedItem;

            string savePath = drv["savePath"].ToString() + @"\game.sii",
                   backupPath = drv["savePath"].ToString() + @"\game_backup.sii";

            //===
            // Backup button

            if (File.Exists(backupPath))
                buttonProfilesAndSavesRestoreBackup.Enabled = true;
            else
                buttonProfilesAndSavesRestoreBackup.Enabled = false;

            //===
            // Decode buton
            sbyte saveFileFormat = GetSaveFileFormat(savePath).saveFileFormat;

            if (saveFileFormat == 2 || saveFileFormat == 4)
                buttonMainDecryptSave.Enabled = true;
            else
                buttonMainDecryptSave.Enabled = false;
        }

        //Main part controls

        //Game select
        public void ToggleGame_Click(object sender, EventArgs e) {
            if (radioButtonMainGameSwitchETS.Checked)
                ToggleGame("ETS2");
            else
                ToggleGame("ATS");

            FillRootFoldersPaths(); // Populate with appropriate root folders
        }

        public void ToggleGame(string _game) {
            if (tempSavefileInMemory != null) {
                DialogResult result = MessageBox.Show("Savefile not saved." + Environment.NewLine + "Do you want to discard changes and switch game type?", "Switching game",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return;
                else {
                    buttonMainDecryptSave.Enabled = true;

                    ToggleControlsAccess(false);
                    SetDefaultValues(false);
                    ClearFormControls(true);
                }
            }

            SelectedGame.Type = _game;
        }

        private void buttonMainAddCustomFolder_Click(object sender, EventArgs e) {
            FormAddCustomFolder FormWindow = new FormAddCustomFolder();
            FormWindow.ShowDialog();
        }

        //Profile list
        private void buttonRefreshAll_Click(object sender, EventArgs e) {
            FillRootFoldersPaths(); // RePopulate root folders
        }

        private void buttonProfilesAndSavesEditProfile_Click(object sender, EventArgs e) {
            FormProfileEditor FormWindow = new FormProfileEditor();
            FormWindow.ParentForm = this;

            DialogResult t = FormWindow.ShowDialog();

            if (t != DialogResult.Cancel) {
                FillRootFoldersPaths(); // RePopulate root folders
            }
        }

        private void buttonProfilesAndSavesRestoreBackup_Click(object sender, EventArgs e) {
            //Set variables
            string SiiSavePath = Globals.SelectedSavePath + @"\game.sii",
                   SiiSavePathBackup = Globals.SelectedSavePath + @"\game_backup.sii";

            //If backups exist
            if (File.Exists(SiiSavePathBackup)) {
                DialogResult dr = MessageBox.Show("Restoring from backup file will overwrite existing save file." + Environment.NewLine +
                                                  "Select: Yes - Overwrite | No - Swap files | Cancel - Abort restoring.",
                                                  "Restoring Save file from Backup", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                //If Cancel - exit Method
                if (dr == DialogResult.Cancel)
                    return;

                //Set variables
                string SiiInfoPath = Globals.SelectedSavePath + @"\info.sii",
                       SiiInfoPathBackup = Globals.SelectedSavePath + @"\info_backup.sii";

                if (dr == DialogResult.No) {
                    //Swap
                    SwapFiles(SiiSavePath, SiiSavePathBackup);

                    if (File.Exists(SiiInfoPathBackup))
                        SwapFiles(SiiInfoPath, SiiInfoPathBackup);
                } else {
                    //Overwrite
                    File.Copy(SiiSavePathBackup, SiiSavePath, true);
                    File.Delete(SiiSavePathBackup);

                    if (File.Exists(SiiInfoPathBackup)) {
                        File.Copy(SiiInfoPathBackup, SiiInfoPath, true);
                        File.Delete(SiiInfoPathBackup);
                    }
                }

                //Swap Files Function
                void SwapFiles(string _firstFile, string _secondFile) {
                    string tmpFile = Directory.GetParent(_firstFile).FullName + "\\tmp";

                    File.Copy(_firstFile, tmpFile, true);
                    File.Copy(_secondFile, _firstFile, true);
                    File.Copy(tmpFile, _secondFile, true);

                    File.Delete(tmpFile);
                }
            }
        }

        //Buttons
        private void buttonDecryptSave_Click(object sender, EventArgs e) {
            //Initial State Setup
            SetDefaultValues(false);
            ClearFormControls(true);

            ToggleMainControlsAccess(false);

            //Set variables
            Globals.SelectedSavePath = Globals.SavesHex[comboBoxSaves.SelectedIndex];
            string SiiSavePath = Globals.SelectedSavePath + @"\game.sii";

            //Decrypt
            string[] file = NewDecodeFile(SiiSavePath);

            //Check result
            if (file != null) {
                IO_Utilities.LogWriter("Backing up file to: " + Globals.SelectedSavePath + @"\game_backup.sii");

                //Backup
                File.Copy(SiiSavePath, Globals.SelectedSavePath + @"\game_backup.sii", true);

                //Write Decrypted file
                File.WriteAllLines(SiiSavePath, file);

                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);
            } else
                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_decode_file");

            //Unlock controls
            ToggleMainControlsAccess(true);
            buttonMainDecryptSave.Enabled = false;

            ToggleGame(SelectedGame.Type);

            //GC
            GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        private void buttonOpenSaveFolder_Click(object sender, EventArgs e) {
            //Open Save Folder
            if (Directory.Exists(Globals.SavesHex[comboBoxSaves.SelectedIndex]))
                System.Diagnostics.Process.Start(Globals.SavesHex[comboBoxSaves.SelectedIndex]);
        }

        internal static BackgroundWorker workerLoadSaveFile;
        private void LoadSaveFile_Click(object sender, EventArgs e) {
            //Initial State Setup 
            ToggleMainControlsAccess(false);
            ToggleControlsAccess(false);
            ClearFormControls(true);

            SetDefaultValues(false);
            ClearJobData();

            //Load Save file

            //Set variables
            Globals.SelectedSavePath = Globals.SavesHex[comboBoxSaves.SelectedIndex];
            Globals.SelectedSave = Globals.SelectedSavePath.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
            Globals.SelectedSaveName = GetCustomSaveFilename(Globals.SelectedSavePath);

            Globals.SelectedProfilePath = Globals.ProfilesHex[comboBoxProfiles.SelectedIndex];
            Globals.SelectedProfile = Globals.SelectedProfilePath.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
            Globals.SelectedProfileName = Utilities.TextUtilities.FromHexToString(Globals.SelectedProfile);

            //Setup BG worker
            workerLoadSaveFile = new BackgroundWorker();
            workerLoadSaveFile.WorkerReportsProgress = true;

            workerLoadSaveFile.DoWork += LoadSaveFile;
            workerLoadSaveFile.ProgressChanged += worker_ProgressChanged;
            workerLoadSaveFile.RunWorkerCompleted += worker_RunWorkerCompleted;

            //Start BG worker
            workerLoadSaveFile.RunWorkerAsync();
        }

        private void buttonMainCloseSave_Click(object sender, EventArgs e) {
            ToggleControlsAccess(false);

            SetDefaultValues(false);

            ClearFormControls(true);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void buttonWriteSave_Click(object sender, EventArgs e) {
            if (extraDrivers.Count() > 0 || extraVehicles.Count() > 0) {
                DialogResult res = MessageBox.Show("Do you want to save Drivers and Trucks from sold garages?", "Attention! Loosing content", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes) {
                    FormGaragesSoldContent testDialog = new FormGaragesSoldContent();
                    testDialog.ShowDialog(this);
                }
            }

            ToggleMainControlsAccess(false);
            ToggleControlsAccess(false);

            //Write

            workerLoadSaveFile = new BackgroundWorker();
            workerLoadSaveFile.WorkerReportsProgress = true;

            workerLoadSaveFile.DoWork += NewWrireSaveFile;
            workerLoadSaveFile.ProgressChanged += worker_ProgressChanged;
            workerLoadSaveFile.RunWorkerCompleted += workerWrite_RunWorkerCompleted;

            workerLoadSaveFile.RunWorkerAsync();
        }

        //Profile and Saves groupbox
        private void checkBoxProfileBackups_CheckedChanged(object sender, EventArgs e) {
            comboBoxRootFolders.SelectedIndexChanged -= comboBoxRootFolders_SelectedIndexChanged;

            string sv = comboBoxRootFolders.SelectedValue.ToString();

            FillRootFoldersPaths(); // refresh list as backup entries was deleted\added

            int index = FindByValue(comboBoxRootFolders, sv); // try find previous value

            if (index > -1) {
                comboBoxRootFolders.SelectedValue = sv; // if exists - set as selected
                comboBoxRootFolders.SelectedIndexChanged += comboBoxRootFolders_SelectedIndexChanged;
            } else {
                comboBoxRootFolders.SelectedIndexChanged += comboBoxRootFolders_SelectedIndexChanged;
                comboBoxRootFolders.SelectedIndex = 0; // if not - select first in the list
            }
        }

        public void FillRootFoldersPaths() {
            try {
                string SteamError = "", MyDocError = "";
                bool SteamFolderExist = false, MyDocFolderExist = true;

                try {
                    //Globals.SteamDir = new DirectoryInfo(new SteamGameLocator().getSteamInstallLocation()); // Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null).ToString(); //string Globals.SteamDir = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null).ToString();
                    if (Globals.SteamDir is null || !Globals.SteamDir.Exists) { //unknown steam path
                        SteamError = "Can not detect Steam install folder.";
                    } else {
                        var steamCloudDir = Globals.GetLatestSteamUserDataDir();
                        if (steamCloudDir is null || !steamCloudDir.Exists) { //no userdata
                            SteamError = "No userdata in Steam folder or No user folders found in Steam folder.";
                        } else {
                            if (!SelectedGame.SteamRemoteDir.Exists) {
                                SteamError = "Game folder for - " + SelectedGame.Type + "game in Steam folder does not exist.";
                            } else {
                                SteamFolderExist = true;
                            }
                        }
                    }
                } catch { }

                if (!SteamFolderExist) IO_Utilities.LogWriter(SteamError);

                if (!SelectedGame.DocumentsDir.Exists) {
                    MyDocError = "Folder in \"My documents\" for - " + SelectedGame.Type + " game does not exist.";
                    MyDocFolderExist = false;
                    IO_Utilities.LogWriter(MyDocError);
                }

                //Setup combobox DataTable
                DataTable combDT = new DataTable();
                DataColumn dc = new DataColumn("ProfileID", typeof(string));
                combDT.Columns.Add(dc);

                dc = new DataColumn("ProfileName", typeof(string));
                combDT.Columns.Add(dc);

                dc = new DataColumn("ProfileType", typeof(string));
                combDT.Columns.Add(dc);

                //Collect Root folders
                List<DirectoryInfo> tempList = new();

                // Standart folders
                if (MyDocFolderExist || SteamFolderExist)
                    if (checkBoxProfilesAndSavesProfileBackups.Checked) {
                        //If backups selected
                        //My docs Profiles
                        if (MyDocFolderExist)
                            foreach (var folder in SelectedGame.DocumentsDir.GetDirectories()) {
                                if (folder.Name.StartsWith("profiles")) //Documents
                                {
                                    if (folder.Exists && folder.GetDirectories().Count() > 0) {
                                        combDT.Rows.Add(folder, "[L] " + folder.Name, "local");
                                        tempList.Add(folder);
                                    }
                                }
                            }

                        //Steam Profiles
                        if (SteamFolderExist)
                            foreach (var folder in SelectedGame.SteamUserDataDir.GetDirectories()) {
                                if (folder.Name.StartsWith("profiles")) //Steam
                                {
                                    if (folder.Exists && folder.GetDirectories().Count() > 0) {
                                        combDT.Rows.Add(folder, "[S] " + folder.Name, "steam");
                                        tempList.Add(folder);
                                    }
                                }
                            }
                    } else {
                        //Without backups
                        DirectoryInfo folder = null;

                        //My docs Profiles
                        if (MyDocFolderExist) {
                            folder = SelectedGame.DocumentsDir.Combine("profiles");

                            if (folder.Exists && folder.GetDirectories().Count() > 0) {
                                combDT.Rows.Add(folder, "[L] profiles", "local");
                                tempList.Add(folder);
                            }
                        }

                        //Steam Profiles
                        if (SteamFolderExist) {
                            folder = SelectedGame.SteamRemoteDir.Combine("profiles");

                            if (folder.Exists && folder.GetDirectories().Count() > 0) {
                                combDT.Rows.Add(folder, "[S] profiles", "steam");
                                tempList.Add(folder);
                            }
                        }
                    }

                // Custom folders
                int cpIndex = 0;
                if (ProgSettingsV.CustomPaths.Keys.Contains(SelectedGame.Type))
                    foreach (string CustPath in ProgSettingsV.CustomPaths[SelectedGame.Type]) {
                        cpIndex++;
                        var custDir = new DirectoryInfo(CustPath);
                        if (custDir.Exists) {
                            if (custDir.Combine("profiles").Exists) {
                                combDT.Rows.Add(CustPath + @"\profiles", "[C] Custom path " + cpIndex.ToString(), "custom");
                                tempList.Add(custDir.Combine("profiles"));
                            } else {
                                combDT.Rows.Add(CustPath, "[C] Custom path " + cpIndex.ToString(), "custom");
                                tempList.Add(custDir);
                            }
                        }
                    }

                if (!MyDocFolderExist && !SteamFolderExist) {
                    IO_Utilities.LogWriter("Standart Save folders does not exist for this game - " + SelectedGame.Type + "." + Environment.NewLine + MyDocError + " " + SteamError + Environment.NewLine +
                        "Check installation. Start game first (Steam).");
                }

                //Save Root paths
                Globals.ProfileDirs = tempList;

                //Populate combobox
                comboBoxRootFolders.ValueMember = "ProfileID";
                comboBoxRootFolders.DisplayMember = "ProfileName";

                comboBoxRootFolders.DataSource = combDT;

                if (comboBoxRootFolders.Items.Count > 0) {
                    comboBoxRootFolders.Enabled = true;
                } else {
                    comboBoxRootFolders.SelectedIndex = -1;
                    comboBoxRootFolders.Enabled = false;

                    comboBoxProfiles.Enabled = false;
                    comboBoxSaves.Enabled = false;

                    MessageBox.Show("Standart Save folders does not exist for this game - " + SelectedGame.Type + "." + Environment.NewLine +
                                    MyDocError + Environment.NewLine + SteamError + Environment.NewLine +
                                    "Check game installation, Start game and Refresh profiles list or Add Custom paths.");
                }
            } catch {
                IO_Utilities.ErrorLogWriter("Populating Root Profiles failed");
            }
        }

        private void comboBoxRootFolders_SelectedIndexChanged(object sender, EventArgs e) {
            if (!Globals.ProfileDirs[comboBoxRootFolders.SelectedIndex].Exists)
                return;

            // Disable save\profile control buttons
            buttonProfilesAndSavesEditProfile.Enabled = false;
            buttonMainDecryptSave.Enabled = false;
            buttonProfilesAndSavesOpenSaveFolder.Enabled = false;
            buttonMainLoadSave.Enabled = false;

            FillProfiles(); // Populate Profiles list

        }

        private void comboBoxRootFolders_DropDown(object sender, EventArgs e) {
            comboBoxRootFolders.SelectedIndexChanged -= comboBoxRootFolders_SelectedIndexChanged;

            string sv = comboBoxRootFolders.SelectedValue.ToString(); //save selected value

            FillRootFoldersPaths(); // refresh list in case entries was deleted\added

            int index = FindByValue(comboBoxRootFolders, sv); // try find previous value

            if (index > -1) {
                comboBoxRootFolders.SelectedValue = sv; // if exists - set as selected
                comboBoxRootFolders.SelectedIndexChanged += comboBoxRootFolders_SelectedIndexChanged;
            } else {
                comboBoxRootFolders.SelectedIndexChanged += comboBoxRootFolders_SelectedIndexChanged;
                comboBoxRootFolders.SelectedIndex = 0; // if not - select first in the list
            }
        }

        public void FillProfiles() {
            try {
                if (!Globals.ProfileDirs[comboBoxRootFolders.SelectedIndex].Exists) {
                    FillRootFoldersPaths();
                    return;
                }

                string ProfileName = "",
                       SelectedFolder = comboBoxRootFolders.SelectedValue.ToString();

                //Setup combobox DataTable
                DataTable combDT = new DataTable();
                DataColumn dc = new DataColumn("ProfilePath", typeof(string));
                combDT.Columns.Add(dc);

                dc = new DataColumn("ProfileName", typeof(string));
                combDT.Columns.Add(dc);

                DataColumn dcDisplay = new DataColumn("DisplayMember");
                dcDisplay.Expression = string.Format("IIF(ProfilePath <> 'null', {1}, '-- not found --')", "ProfilePath", "ProfileName");
                combDT.Columns.Add(dcDisplay);

                //Filter Profile folders
                Globals.ProfilesHex = Directory.GetDirectories(SelectedFolder).OrderByDescending(f => new FileInfo(f).LastWriteTime).ToList(); // todo: List<DirectoryInfo>

                if (Globals.ProfilesHex.Count > 0) {
                    List<string> NewProfileHex = new List<string>();

                    foreach (string profilePath in Globals.ProfilesHex) {
                        string profileFolder = profilePath.Substring(profilePath.LastIndexOf(@"\") + 1);

                        if (!profileFolder.Contains(" ") && Directory.Exists(profilePath + @"\save")) {
                            ProfileName = TextUtilities.FromHexToString(Path.GetFileName(profilePath));

                            if (ProfileName != null) {
                                combDT.Rows.Add(profilePath, ProfileName);
                                NewProfileHex.Add(profilePath);
                            }
                        }
                    }

                    Globals.ProfilesHex = NewProfileHex;

                    //
                    if (combDT.Rows.Count > 0) {
                        comboBoxProfiles.Enabled = true;

                        buttonProfilesAndSavesEditProfile.Enabled = true;

                        UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);
                    } else {
                        combDT.Rows.Add("null");

                        comboBoxProfiles.Enabled = false;
                        comboBoxSaves.Enabled = false;

                        buttonProfilesAndSavesEditProfile.Enabled = false;

                        UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_No valid Saves was found");
                    }

                    comboBoxProfiles.ValueMember = "ProfilePath";
                    comboBoxProfiles.DisplayMember = "DisplayMember";

                    comboBoxProfiles.DataSource = combDT;
                } else {
                    comboBoxProfiles.Enabled = false;
                    comboBoxSaves.Enabled = false;

                    buttonProfilesAndSavesOpenSaveFolder.Enabled = false;
                    buttonMainDecryptSave.Enabled = false;

                    MessageBox.Show("Please select another folder", "No valid profiles found");
                }
            } catch {
                IO_Utilities.ErrorLogWriter("Populating Profiles list failed");
            }
        }

        private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e) {
            if (Globals.ProfilesHex.Count != 0) {
                try {
                    string AvatarPath = Globals.ProfilesHex[comboBoxProfiles.SelectedIndex] + @"\avatar.png";

                    if (File.Exists(AvatarPath)) {
                        Bitmap SourceImg = new Bitmap(AvatarPath);
                        Rectangle AvatarArea = new Rectangle(0, 0, 95, 95);
                        Bitmap CroppedImg = SourceImg.Clone(AvatarArea, SourceImg.PixelFormat);

                        pictureBoxProfileAvatar.Image = CroppedImg;
                    } else {
                        pictureBoxProfileAvatar.Image = MainIcons[0]; // placeholder icon
                    }
                } catch {
                    pictureBoxProfileAvatar.Image = MainIcons[0]; // placeholder icon
                }

                try {
                    //Read profile data
                    LoadProfileDataFile(Globals.ProfilesHex[comboBoxProfiles.SelectedIndex] + @"\profile.sii"); // Profile file path

                    //Add tooltip to Avatar
                    toolTipMain.SetToolTip(pictureBoxProfileAvatar, MainSaveFileProfileData.getProfileSummary(PlayerLevelNames)); // Profile stats
                } catch { }
            } else {
                pictureBoxProfileAvatar.Image = null;
                //Add tooltip to Avatar
                toolTipMain.SetToolTip(pictureBoxProfileAvatar, "");
            }

            if (comboBoxProfiles.SelectedIndex > -1)
                FillProfileSaves(); // Populate Save folders list
        }

        private void comboBoxProfiles_DropDown(object sender, EventArgs e) {
            comboBoxProfiles.SelectedIndexChanged -= comboBoxProfiles_SelectedIndexChanged; // remove event to prevent unnecessary refreshing

            string sv = comboBoxProfiles.SelectedValue.ToString(); //save selected value

            FillProfiles(); // refresh list in case entries was deleted\added

            int index = FindByValue(comboBoxProfiles, sv); // try find previous value

            if (index > -1) {
                comboBoxProfiles.SelectedValue = sv; // if exists - set as selected
                comboBoxProfiles.SelectedIndexChanged += comboBoxProfiles_SelectedIndexChanged; // restore event
            } else {
                comboBoxProfiles.SelectedIndexChanged += comboBoxProfiles_SelectedIndexChanged; // restore event
                comboBoxProfiles.SelectedIndex = 0; // if not - select first in the list
            }
        }

        public void FillProfileSaves() {
            try {
                if (Globals.ProfilesHex.Count != 0 && !Directory.Exists(Globals.ProfilesHex[comboBoxProfiles.SelectedIndex])) {
                    FillProfiles();
                    return;
                }

                Globals.SavesHex = new string[0];

                if (Globals.ProfilesHex.Count != 0) {
                    string SelectedSaveFolder = Globals.ProfilesHex[comboBoxProfiles.SelectedIndex] + @"\save";

                    if (Directory.Exists(SelectedSaveFolder))
                        Globals.SavesHex = Directory.GetDirectories(SelectedSaveFolder).OrderByDescending(f => new FileInfo(f).LastWriteTime).ToArray();
                    else
                        Globals.SavesHex = new string[0];
                }

                //Setup combobox DataTable
                DataTable combDT = new DataTable();
                DataColumn dc = new DataColumn("savePath", typeof(string));
                combDT.Columns.Add(dc);

                dc = new DataColumn("saveName", typeof(string));
                combDT.Columns.Add(dc);

                DataColumn dcDisplay = new DataColumn("DisplayMember");
                dcDisplay.Expression = string.Format("IIF(savePath <> 'null', {1}, '-- not found --')", "savePath", "saveName");
                combDT.Columns.Add(dcDisplay);

                //if save folder contains any folders
                if (Globals.SavesHex.Length > 0) {
                    bool NotANumber = false;

                    //Check if any of the initial folders is a valid save folder
                    foreach (string saveFolder in Globals.SavesHex) {
                        if (!File.Exists(saveFolder + @"\game.sii") || !File.Exists(saveFolder + @"\info.sii"))
                            continue; //if folder does not contains essential files - skip it

                        string[] folders = saveFolder.Split(new string[] { "\\" }, StringSplitOptions.None);

                        if (folders.Last().Contains(' ')) {
                            string tmpName = GetCustomSaveFilename(saveFolder);

                            if (tmpName != "")
                                combDT.Rows.Add(saveFolder, tmpName);
                            else
                                combDT.Rows.Add(saveFolder, "NoName ( " + folders.Last() + " )");
                        } else {
                            foreach (char c in folders.Last()) {
                                if (c < '0' || c > '9') {
                                    NotANumber = true;
                                    break;
                                }
                            }

                            if (NotANumber) {
                                string[] namearr = folders.Last().Split(new char[] { '_' });
                                string ProfileName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(namearr[0]);

                                for (int i = 1; i < namearr.Length; i++) {
                                    ProfileName += " " + namearr[i];
                                }

                                combDT.Rows.Add(saveFolder, "- " + ProfileName + " -");
                            } else {
                                string tmpName = GetCustomSaveFilename(saveFolder);

                                if (tmpName != "")
                                    combDT.Rows.Add(saveFolder, tmpName);
                                else
                                    combDT.Rows.Add(saveFolder, "NoName ( " + folders.Last() + " )");
                            }

                            NotANumber = false;
                        }

                    }

                    //Check if save folders was found
                    if (combDT.Rows.Count > 0) {
                        comboBoxSaves.Enabled = true;
                        buttonProfilesAndSavesOpenSaveFolder.Enabled = true;
                        buttonMainDecryptSave.Enabled = true;

                        UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);
                    } else {
                        combDT.Rows.Add("null"); //Add fake item to indicate zero saves found

                        comboBoxSaves.Enabled = false;
                        buttonProfilesAndSavesOpenSaveFolder.Enabled = false;
                        buttonMainDecryptSave.Enabled = false;
                        buttonMainLoadSave.Enabled = false;

                        UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_No valid Saves was found");
                    }

                    comboBoxSaves.ValueMember = "savePath";
                    comboBoxSaves.DisplayMember = "DisplayMember";

                    comboBoxSaves.DataSource = combDT;
                } else //if zero folders found in "save" folder
                  {
                    combDT.Rows.Add("null"); //Add fake item to indicate zero saves found

                    comboBoxSaves.ValueMember = "savePath";
                    comboBoxSaves.DisplayMember = "DisplayMember";

                    comboBoxSaves.DataSource = combDT;

                    //Visuals
                    comboBoxSaves.Enabled = false;
                    buttonProfilesAndSavesOpenSaveFolder.Enabled = false;
                    buttonMainDecryptSave.Enabled = false;
                    buttonMainLoadSave.Enabled = false;

                    UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_No Save file folders found");
                }
            } catch {
                IO_Utilities.ErrorLogWriter("Populating Saves list failed");
            }
        }

        private void comboBoxSaves_SelectedIndexChanged(object sender, EventArgs e) {
            // Update save path
            Globals.SelectedSavePath = Globals.SavesHex[comboBoxSaves.SelectedIndex];
            Globals.SelectedSave = Globals.SelectedSavePath.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
            Globals.SelectedSaveName = GetCustomSaveFilename(Globals.SelectedSavePath);

            // Update Profile path
            Globals.SelectedProfilePath = Globals.ProfilesHex[comboBoxProfiles.SelectedIndex];
            Globals.SelectedProfile = Globals.SelectedProfilePath.Split(new string[] { "\\" }, StringSplitOptions.None).Last();
            Globals.SelectedProfileName = Utilities.TextUtilities.FromHexToString(Globals.SelectedProfile);

            CheckSaveControls();
        }

        private void comboBoxSaves_DropDown(object sender, EventArgs e) {
            comboBoxSaves.SelectedIndexChanged -= new EventHandler(comboBoxSaves_SelectedIndexChanged);

            string sv = comboBoxSaves.SelectedValue.ToString(); //save selected value

            FillProfileSaves(); // refresh list in case entries was deleted\added

            int index = FindByValue(comboBoxSaves, sv); // try find previous value

            if (index > -1) {
                comboBoxSaves.SelectedValue = sv; // if exists - set as selected
                comboBoxSaves.SelectedIndexChanged += new EventHandler(comboBoxSaves_SelectedIndexChanged);
            } else {
                comboBoxSaves.SelectedIndexChanged += new EventHandler(comboBoxSaves_SelectedIndexChanged);
                comboBoxSaves.SelectedIndex = 0; // if not - select first in the list
            }
        }
        //end Profile and Saves groupbox
        //end Main part controls
    }
}