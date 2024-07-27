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
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using TS_SE_Tool.Utilities;
using TS_SE_Tool.CustomClasses.Program;

namespace TS_SE_Tool {
    public partial class FormMain : Form {
        private BackgroundWorker generalWorker;

        //Check if tsset folders exist
        private void CheckTssetFoldersExist() {
            TssetFoldersExist = true;

            string[] folderPaths = new string[] { "libs", "img", "lang", "updater" };

            foreach (string path in folderPaths) {
                if (!Directory.Exists(path)) {
                    TssetFoldersExist = false;
                    break;
                }
            }
        }

        private void LoadExtCountries() {
            string[] inputFile;

            try {
                inputFile = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\lang\CityToCountry.csv");

                for (int i = 0; i < inputFile.Length; i++) {
                    CountryDictionary.AddCountry(inputFile[i].Split(new char[] { ';' })[0], inputFile[i].Split(new char[] { ';' })[1]);
                }
            } catch {
                IO_Utilities.LogWriter("CityToCountry.csv file is missing in lang directory");
            }

            try {
                inputFile = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\lang\CountryProperties.csv");

                for (int i = 0; i < inputFile.Length; i++) {
                    if (inputFile[i].StartsWith("#"))
                        continue;

                    string[] csvParts = inputFile[i].Split(new char[] { ';' });
                    CountriesDataList.Add(csvParts[0], new Country(csvParts[0], csvParts[1], csvParts[2].Replace('.', ',')));
                }
            } catch {
                IO_Utilities.LogWriter("CountryProperties.csv file is missing in lang directory");
            }
        }

        private void LoadExtCargoes() {
            string[] strArray;

            try {
                strArray = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\heavy_cargoes.csv");

                for (int i = 0; i < strArray.Length; i++) {
                    HeavyCargoList.Add(strArray[i]);
                }
            } catch {
                strArray = new string[] {
                    "asph_miller", "cable_reel", "concr_beams", "dozer", "locomotive", "metal_center", "mobile_crane", "transformat",
                    "case600", "cat627", "coil", "kalmar240", "kalmar240_s", "komatsu155", "terex3160", "transformer", "wirtgen250"
                    };

                File.WriteAllLines(Directory.GetCurrentDirectory() + @"\heavy_cargoes.csv", strArray);

                IO_Utilities.LogWriter("Default heavy_cargoes.csv created");

                for (int i = 0; i < strArray.Length; i++) {
                    HeavyCargoList.Add(strArray[i]);
                }
            }
        }

        private void LngFileLoader(string _sourcefile, Dictionary<string, string> _destDict, string _ci) {
            _destDict.Clear();
            bool defaultDuplicates = false;
            string defaultFile = Directory.GetCurrentDirectory() + @"\lang\Default\" + _sourcefile;

            try {
                string[] tempFile = File.ReadAllLines(defaultFile);

                for (int i = 0; i < tempFile.Length; i++) {
                    if (tempFile[i] != "" && !tempFile[i].StartsWith("[")) {
                        string[] tmp = new string[2];
                        try {
                            tmp = tempFile[i].Split(new char[] { ';' }, 2);
                        } catch { }

                        if (tmp[0] != "") {
                            if (!_destDict.ContainsKey(tmp[0]))
                                _destDict.Add(tmp[0], tmp[1]);
                            else
                                defaultDuplicates = true;
                        }
                    }
                }
            } catch {
                IO_Utilities.LogWriter(_sourcefile + " file is missing");
            }

            if (defaultDuplicates) {
                var txtToWrite = _destDict.Select(x => string.Join(";", new string[] { x.Key, x.Value })).ToList();
                txtToWrite.Insert(0, "[Default]");

                File.WriteAllLines(defaultFile, txtToWrite, Encoding.UTF8);
            }

            string languageFile = "";

            if (_ci != "Default")
                languageFile = _ci += "\\" + _sourcefile;

            if (!File.Exists(Directory.GetCurrentDirectory() + @"\lang\" + languageFile))
                return;

            try {
                string[] tempFile = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\lang\" + languageFile);

                for (int i = 0; i < tempFile.Length; i++) {
                    if (tempFile[i] != "" && !tempFile[i].StartsWith("[")) {
                        string[] tmp = new string[2];
                        try {
                            tmp = tempFile[i].Split(new char[] { ';' }, 2);
                        } catch { }

                        if (tmp[0] != null && tmp[0] != "") {
                            if (_destDict.ContainsKey(tmp[0]))
                                _destDict[tmp[0]] = tmp[1];
                            else
                                _destDict.Add(tmp[0], tmp[1]);
                        }
                    }
                }
            } catch {
                IO_Utilities.LogWriter(_sourcefile + " file is missing");
            }
        }

        private void LoadTruckBrandsLng() {
            TruckBrandsLngDict.Clear();

            try {
                string[] tempFile = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\lang\Default\truck_brands.txt");

                for (int i = 0; i < tempFile.Length; i++) {
                    if (tempFile[i].StartsWith("#"))
                        continue;

                    string[] tmp = tempFile[i].Split(new char[] { ';' });
                    TruckBrandsLngDict.Add(tmp[0], tmp[1]);
                }
            } catch {
                IO_Utilities.LogWriter("truck_brands.txt file is missing");
            }
        }

        private void LoadDriverNamesLng() {
            DriverNames.Clear();

            try {
                string[] tempFile = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\lang\Default\" + SelectedGame.Type + "\\driver_names.csv");

                for (int i = 0; i < tempFile.Length; i++) {
                    string[] tmp = tempFile[i].Split(new char[] { ';' });
                    DriverNames.Add(tmp[0], tmp[1]);
                }
            } catch {
                IO_Utilities.LogWriter("truck_brands.txt file is missing");
            }
        }

        private void LoadExtImages() {
            string[] imgNames, imgPaths;

            //=== UI images

            imgNames = new string[] { "Language", "github", "SCS", "TMP", "PDF", "YouTube",
                                        "ProgramSettings", "Settings", "Cross", "Info", "Download",
                                        "Question", "NetworkCloud", "Reload", "EditList",
                                        "Extract"};

            imgPaths = new string[] { @"img\UI\globe.png", @"img\UI\github.png", @"img\UI\SCS.png", @"img\UI\TMP.png", @"img\UI\PDF.png", @"img\UI\YouTube.png",
                                        @"img\UI\pSettings.png", @"img\UI\cogwheel.png", @"img\UI\quit.png", @"img\UI\info.png", @"img\UI\download.png",
                                        @"img\UI\question.png", @"img\UI\networkCloud.png", @"img\UI\reload.png", @"img\UI\edit.png",
                                        @"img\UI\extract.png"};

            Image[] tmpArray = Graphics_TSSET.ImgFromFileLoader(imgPaths);

            for (int i = 0; i < imgPaths.Length; i++)
                ProgUIImgsDict.Add(imgNames[i], tmpArray[i]);

            imgNames = new string[] { "plus", "minus" };
            imgPaths = new string[] { @"img\UI\add.dds", @"img\UI\remove.dds" };

            tmpArray = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            for (int i = 0; i < imgPaths.Length; i++)
                ProgUIImgsDict.Add(imgNames[i], tmpArray[i]);


            //=== Game Icons

            imgPaths = new string[] { @"img\ETS2\game_n.dds", @"img\ATS\game_n.dds" };
            GameIconeImg = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            //=== Main icons

            imgPaths = new string[] { @"img\UI\unknown.dds" };
            MainIcons = Graphics_TSSET.ddsImgLoader(imgPaths, 95, 95).images;

            //=== Tab page icons

            imgPaths = new string[] { @"img\UI\MainTabs\profiles.dds", @"img\UI\MainTabs\comp_man.dds", @"img\UI\MainTabs\truck_service.dds", @"img\UI\MainTabs\trailers.dds",
                                      @"img\UI\MainTabs\company_job.dds", @"img\UI\MainTabs\cargo_market.dds", @"img\UI\MainTabs\maps.dds" };
            TabpagesImages.Images.AddRange(Graphics_TSSET.ddsImgLoader(imgPaths, 64, 64, 64, 0).images);

            //=== Profile

            // skill icons
            imgPaths = new string[] { @"img\UI\Profile\skill_adr.dds", @"img\UI\Profile\skill_distance.dds", @"img\UI\Profile\skill_heavy.dds", @"img\UI\Profile\skill_fragile.dds",
                                      @"img\UI\Profile\skill_jit.dds", @"img\UI\Profile\skill_mechanical.dds" };
            SkillImgS = Graphics_TSSET.ddsImgLoader(imgPaths, 64, 64).images;

            // ADR icons
            imgPaths = new string[] { @"img\" + SelectedGame.Type + @"\adr_1.dds", @"img\" + SelectedGame.Type + @"\adr_2.dds", @"img\" + SelectedGame.Type + @"\adr_3.dds",
                                      @"img\" + SelectedGame.Type + @"\adr_4.dds", @"img\" + SelectedGame.Type + @"\adr_6.dds", @"img\" + SelectedGame.Type + @"\adr_8.dds" };
            ADRImgS = Graphics_TSSET.ddsImgLoader(imgPaths, 46, 46, 9, 9, 32, 32).images;

            imgPaths = new string[] { @"img\" + SelectedGame.Type + @"\adr_1_grey.dds", @"img\" + SelectedGame.Type + @"\adr_2_grey.dds", @"img\" + SelectedGame.Type + @"\adr_3_grey.dds",
                                      @"img\" + SelectedGame.Type + @"\adr_4_grey.dds", @"img\" + SelectedGame.Type + @"\adr_6_grey.dds", @"img\" + SelectedGame.Type + @"\adr_8_grey.dds" };
            ADRImgSGrey = Graphics_TSSET.ddsImgLoader(imgPaths, 46, 46, 9, 9, 32, 32).images;

            // skill level select
            imgPaths = new string[] { @"img\UI\Profile\skill_bar_s.dds", @"img\UI\Profile\skill_bar_s2.dds", @"img\UI\Profile\skill_bar1.dds", @"img\UI\Profile\skill_bar2.dds", @"img\UI\Profile\skill_bar3.dds" };

            int y = 9;
            for (int i = 0; i < imgPaths.Count(); i++) {
                if (i == 2) y = 8;

                SkillImgSBG[i] = Graphics_TSSET.ddsImgLoader(new[] { imgPaths[i] }, 46, 46, 9, y).images[0];
            }

            //=== Company

            // garages
            imgPaths = new string[] { @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\garage_small_ico.dds", @"img\UI\Company\Garages\garage_large_ico.dds",
                                      @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\garage_tiny_ico.dds" };
            GaragesImg = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            // hq
            imgPaths = new string[] { @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\hq_garage_ico_small_n.dds", @"img\UI\Company\Garages\hq_garage_ico_big_n.dds",
                                      @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\garage_free_ico.dds", @"img\UI\Company\Garages\hq_garage_ico_tiny_n.dds" };
            GaragesHQImg = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            // visited cities
            imgPaths = new string[] { @"img\UI\Company\VisitedCities\city_pin_0.dds", @"img\UI\Company\VisitedCities\city_pin_1.dds", @"img\UI\Company\VisitedCities\city_pin_2.dds", @"img\UI\notice_star.dds" };
            CitiesImg = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;


            //=== Truck & Trailer tab

            // truck parts
            imgPaths = new string[] { @"img\" + SelectedGame.Type + @"\engine.dds", @"img\" + SelectedGame.Type + @"\transmission.dds", @"img\" + SelectedGame.Type + @"\chassis.dds",
                                      @"img\" + SelectedGame.Type + @"\cabin.dds", @"img\" + SelectedGame.Type + @"\tyres.dds" };
            TruckPartsImg = Graphics_TSSET.ddsImgLoader(imgPaths, 64, 64).images;

            // trailer parts
            imgPaths = new string[] { @"img\" + SelectedGame.Type + @"\cargo.dds", @"img\" + SelectedGame.Type + @"\trailer_body.dds",
                                      @"img\" + SelectedGame.Type + @"\trailer_chassis.dds", @"img\" + SelectedGame.Type + @"\tyres.dds" };
            TrailerPartsImg = Graphics_TSSET.ddsImgLoader(imgPaths, 64, 64).images;

            // integrity progress bar

            imgPaths = new string[] { @"img\UI\wrench-pattern.png", @"img\UI\swap-pattern.png", @"img\UI\skull-pattern.png" };

            VehicleIntegrityPBImg = Graphics_TSSET.ImgFromFileLoader(imgPaths);

            // buttons
            imgPaths = new string[] { @"img\UI\Trucks&Trailers\service_ico.dds", @"img\UI\Trucks&Trailers\gas_ico.dds", @"img\UI\customize_p.dds" };

            Image[] imgArray = Graphics_TSSET.ddsImgLoader(imgPaths).images;

            RepairImg = imgArray[0];
            RefuelImg = imgArray[1];
            CustomizeImg = imgArray[2];

            //=== Freight market

            // urgency
            imgPaths = new string[] { @"img\UI\FreightMarket\JobUrgency\easy.dds", @"img\UI\FreightMarket\JobUrgency\normal.dds", @"img\UI\FreightMarket\JobUrgency\hard.dds" };
            UrgencyImg = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            // trailer type
            imgPaths = new string[] { @"img\UI\FreightMarket\CargoTypes\none_32.dds", @"img\UI\FreightMarket\CargoTypes\heavy.dds", @"img\UI\FreightMarket\CargoTypes\articulated.dds" };
            CargoTypeImg = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            // cargo type
            imgPaths = new string[] { @"img\UI\FreightMarket\CargoTypes\fragile.dds", @"img\UI\FreightMarket\CargoTypes\valuable.dds" };
            CargoType2Img = Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32).images;

            // Accessories icons
            List<Image> tmpIMGlist = new List<Image>();

            imgPaths = new string[] { @"img\UI\Trucks&Trailers\Accessories\truck_config.dds", @"img\UI\Trucks&Trailers\Accessories\upgrades.dds" };
            tmpIMGlist.AddRange(Graphics_TSSET.ddsImgLoader(imgPaths, 40, 40, 0, 0, 32, 32).images);

            imgPaths = new string[] { @"img\" + SelectedGame.Type + @"\tyres.dds" };
            tmpIMGlist.AddRange(Graphics_TSSET.ddsImgLoader(imgPaths, 42, 42, 5, 5, 32, 32).images);

            imgPaths = new string[] { @"img\UI\Trucks&Trailers\Accessories\use_preset.dds" };
            tmpIMGlist.AddRange(Graphics_TSSET.ddsImgLoader(imgPaths, 24, 24, 16, 16, 32, 32).images);

            imgPaths = new string[] { @"img\UI\Trucks&Trailers\Accessories\mute_checkbox_2.dds" };
            tmpIMGlist.AddRange(Graphics_TSSET.ddsImgLoader(imgPaths, 32, 32, 0, 0, 32, 32, 4).images);

            imgPaths = new string[] { @"img\UI\Trucks&Trailers\Accessories\plate_number.dds" };
            tmpIMGlist.AddRange(Graphics_TSSET.ddsImgLoader(imgPaths, 80, 54, 0, 0, 32, 32).images);

            AccessoriesImg = tmpIMGlist.ToArray();

        }

        //Save new language strings
        private void SaveCompaniesLng() {
            CompaniesList = CompaniesList.Distinct().OrderBy(x => x).ToList();

            List<string> newEntries = new List<string>();

            foreach (string tempitem in CompaniesList) {
                if (!CompaniesLngDict.TryGetValue(tempitem, out string value)) {
                    newEntries.Add(tempitem);
                }
            }

            SaveLngFilesWriter(newEntries, "companies_translate");
        }

        private void SaveCitiesLng() {
            CitiesList = CitiesList.Distinct().OrderBy(x => x.CityName).ToList();

            List<string> newEntries = new List<string>();

            foreach (City tempitem in CitiesList) {
                if (!CitiesLngDict.TryGetValue(tempitem.CityName, out string value)) {
                    newEntries.Add(tempitem.CityName);
                }
            }

            SaveLngFilesWriter(newEntries, "cities_translate");
        }

        private void SaveCargoLng() {
            CargoesList = CargoesList.Distinct().OrderBy(x => x.CargoName).ToList();

            List<string> newEntries = new List<string>();

            foreach (Cargo tempitem in CargoesList) {
                if (!CargoLngDict.TryGetValue(tempitem.CargoName, out string value)) {
                    newEntries.Add(tempitem.CargoName);
                }
            }

            SaveLngFilesWriter(newEntries, "cargo_translate");
        }

        private void SaveLngFilesWriter(List<string> newEntries, string outputFile) {
            if (newEntries.Count > 0) {
                newEntries = newEntries.Distinct().ToList();

                try {
                    using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\lang\Default\" + outputFile + ".txt", true)) {
                        foreach (string str in newEntries) {
                            writer.WriteLine();
                            writer.Write(str + ";");
                        }
                    }
                } catch {
                    IO_Utilities.LogWriter(outputFile + ".txt file is missing");
                }
            }
        }
        //

        private void ExportFormControlstoLanguageFile() {
            string filename = Directory.GetCurrentDirectory() + @"\lang\base_lngfile.txt";

            if (!File.Exists(filename)) {
                File.CreateText(filename);
            }

            try {
                using (StreamWriter writer = new StreamWriter(filename, false)) {
                    foreach (ToolStripDropDownItem temp in menuStripMain.Items) {
                        writer.WriteLine(temp.Name + "=" + temp.Text);

                        foreach (var temp2 in temp.DropDownItems) {
                            if (temp2.GetType() == typeof(ToolStripMenuItem)) {
                                ToolStripDropDownItem temp3 = (ToolStripDropDownItem)temp2;
                                if (!temp3.Name.Contains("Translation"))
                                    writer.WriteLine(temp3.Name + "=" + temp3.Text);
                            }

                        }
                    }

                    foreach (Control x in Controls) {
                        if (x.Text != "" && (x is Label || x is CheckBox || x is Button || x is GroupBox)) {
                            if (x.Text.Any(z => char.IsLetter(z)))
                                writer.WriteLine(x.Name + "=" + x.Text);
                        }

                        if (x is GroupBox) {
                            foreach (Control xc in x.Controls) {
                                if (xc.Text != "" && (xc is Label || xc is CheckBox || xc is Button || xc is GroupBox)) {
                                    if (xc.Text.Any(z => char.IsLetter(z)))
                                        writer.WriteLine(xc.Name + "=" + xc.Text);
                                }
                            }
                        }

                        if (x is TabControl) {
                            TabControl pages = x as TabControl;

                            foreach (TabPage page in pages.TabPages) {
                                writer.WriteLine(page.Name + "=" + page.Text);

                                foreach (Control y in page.Controls) {
                                    if (y.Text != "" && (y is Label || y is CheckBox || y is Button || y is GroupBox)) {
                                        if (y.Text.Any(z => char.IsLetter(z)))
                                            writer.WriteLine(y.Name + "=" + y.Text);
                                    }

                                    if (y is GroupBox) {
                                        foreach (Control yc in y.Controls) {
                                            if (yc.Text != "" && (yc is Label || yc is CheckBox || yc is Button || yc is GroupBox)) {
                                                if (yc.Text.Any(z => char.IsLetter(z)))
                                                    writer.WriteLine(yc.Name + "=" + yc.Text);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            } catch {
                IO_Utilities.LogWriter("base_lngfile file is missing");
            }
        }

        private byte[] LoadFileToMemory(string _filePath) {
            byte[] _buffer;
            try {
                _buffer = File.ReadAllBytes(_filePath);
            } catch {
                IO_Utilities.LogWriter("Could not find file in: " + _filePath);
                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_find_file");

                FileDecoded = false;
                return null;
            }
            return _buffer;
        }

        private void LoadSaveFile(object sender, DoWorkEventArgs e) {
            UpdateStatusBarMessage.MainForm = Application.OpenForms.OfType<FormMain>().Single();

            // Status
            UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Info, "message_decoding_save_file");
            IO_Utilities.LogWriter("Working on " + Globals.SelectedSavePath + " save file");

            // Variables
            string SiiProfilePath = Globals.SelectedProfilePath + @"\profile.sii";
            string SiiInfoPath = Globals.SelectedSavePath + @"\info.sii";
            string SiiSavePath = Globals.SelectedSavePath + @"\game.sii";

            (bool valid, string[] fileArray) resulCheck;

            if (File.Exists(SiiSavePath)) {
                string dbPath = "dbs/" + SelectedGame.Type + "." + Path.GetFileName(Globals.SelectedProfilePath) + ".sdf";
                DBconnection = new SqlCeConnection("Data Source = " + dbPath);

                CreateDatabase(dbPath);
            } else {
                e.Cancel = true;

                DialogResult DR = UpdateStatusBarMessage.ShowMessageBox(this, "Save file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            //=== Profile Info
            resulCheck = preProcessFile(SiiProfilePath, "Profile file");

            if (resulCheck.valid) {
                tempProfileFileInMemory = resulCheck.fileArray;

                MainSaveFileProfileData = new SaveFileProfileData(SelectedGame);
                MainSaveFileProfileData.ProcessData(tempProfileFileInMemory);

                tempProfileFileInMemory = null;
            } else {
                e.Cancel = true;

                DialogResult DR = UpdateStatusBarMessage.ShowMessageBox(this, "Error occured during preprocessing Profile file." + Environment.NewLine +
                    "Probably New \\ Updated format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            //=== End Profile Info

            //=== Save info
            resulCheck = preProcessFile(SiiInfoPath, "Info file");

            if (resulCheck.valid) {
                tempInfoFileInMemory = resulCheck.fileArray;

                CheckSaveInfoData();
                tempInfoFileInMemory = null;
            } else {
                e.Cancel = true;

                DialogResult DR = UpdateStatusBarMessage.ShowMessageBox(this, "Error occured during preprocessing Info file." + Environment.NewLine +
                    "Probably New \\ Updated format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            //=== End Save Info

            //=== Check for Dependencies conflict
            if (!InfoDepContinue) {
                e.Cancel = true;
                return;
            }
            //=== End

            //=== Save file
            resulCheck = preProcessFile(SiiSavePath, "Save file");

            if (resulCheck.valid) {
                tempSavefileInMemory = resulCheck.fileArray;

                LastModifiedTimestamp = File.GetLastWriteTime(SiiSavePath);

                if (!NewPrepareData()) {
                    e.Cancel = true;

                    DialogResult DR = UpdateStatusBarMessage.ShowMessageBox(this, "Error occured during preparing Save file." + Environment.NewLine +
                        "Probably New \\ Updated Save format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
            } else {
                e.Cancel = true;

                DialogResult DR = UpdateStatusBarMessage.ShowMessageBox(this, "Error occured during preprocessing Save file." + Environment.NewLine +
                    "Probably New \\ Updated Save format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            // End Save file

            //===
            (bool valid, string[] fileArray) preProcessFile(string _filePath, string _type) {
                string[] _inputArray;

                if (!File.Exists(_filePath)) {
                    IO_Utilities.LogWriter("File does not exist in " + _filePath);
                    UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_find_file");

                    return (false, null);
                } else {
                    FileDecoded = false;
                    _inputArray = decodeFile(_filePath);
                    bool checkResult = checkDecodedFile(_inputArray, _type);

                    return (checkResult, _inputArray);
                }
            }
            //===
            string[] decodeFile(string _filePath) {
                string[] fileArray = null;

                try {
                    int decodeAttempt = 0;

                    while (decodeAttempt < 5) {
                        fileArray = NewDecodeFile(_filePath);

                        if (FileDecoded) {
                            break;
                        }

                        decodeAttempt++;
                    }

                    if (decodeAttempt == 5) {
                        IO_Utilities.LogWriter("Could not decrypt after 5 attempts");
                        UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_decode_file");
                    }
                } catch {
                    IO_Utilities.LogWriter("Could not read: " + _filePath);
                }

                return fileArray;
            }
            //===
            bool checkDecodedFile(string[] _input, string _type) {
                if (_input == null || _input[0] != "SiiNunit") {
                    IO_Utilities.LogWriter("Wrongly decoded " + _type + " or wrong file format");
                    UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_file_not_decoded");

                    _input = null;

                    return false;
                } else
                    return true;
            }
            //===
        }

        private void LoadProfileDataFile(string SiiProfilePath) {
            //Profile Info
            if (!File.Exists(SiiProfilePath)) {
                IO_Utilities.LogWriter("File does not exist in " + SiiProfilePath);
                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_find_file");
            } else {
                FileDecoded = false;
                try {
                    int decodeAttempt = 0;
                    while (decodeAttempt < 5) {
                        tempProfileFileInMemory = NewDecodeFile(SiiProfilePath, false);

                        if (FileDecoded) {
                            break;
                        }

                        decodeAttempt++;
                    }

                    if (decodeAttempt == 5) {
                        IO_Utilities.LogWriter("Could not decrypt after 5 attempts");
                        //UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_decode_file");
                    }
                } catch {
                    IO_Utilities.LogWriter("Could not read: " + SiiProfilePath);
                }

                if ((tempProfileFileInMemory == null) || (tempProfileFileInMemory[0] != "SiiNunit")) {
                    IO_Utilities.LogWriter("Wrongly decoded Profile file or wrong file format");
                    //UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_file_not_decoded");

                    tempProfileFileInMemory = null;

                    SetDefaultValues(false);
                    ToggleMainControlsAccess(true);
                    ToggleControlsAccess(false);
                } else if (tempProfileFileInMemory != null) {
                    //UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);
                    MainSaveFileProfileData = new SaveFileProfileData(SelectedGame);
                    MainSaveFileProfileData.ProcessData(tempProfileFileInMemory);
                }
            }

            tempProfileFileInMemory = null; //clearmemory
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            toolStripProgressBarMain.Value = e.ProgressPercentage;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled == true) {
                SetDefaultValues(false);
                ToggleMainControlsAccess(true);
                ToggleControlsAccess(false);

                //UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);

                return;
            }

            if (SiiNunitData is null) {
                MessageBox.Show("SiiNunitData is null!", "?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Globals.SupportedGames.Get("ETS2").Installed) radioButtonMainGameSwitchETS.Enabled = true;
                if (Globals.SupportedGames.Get("ATS").Installed) radioButtonMainGameSwitchATS.Enabled = true;
                return;
            }

            if (SiiNunitData.UnidentifiedBlocks.Count > 0) {
                MessageBox.Show("Some of the blocks in save file was not recognized and it may affect Program behavior." + Environment.NewLine + Environment.NewLine +
                    "Please contact Developer via e-mail <" + Utilities.Web_Utilities.External.linkMailDeveloper + ">" + Environment.NewLine + Environment.NewLine +
                    "Information can be found in error.log file.",
                    "Unidentified blocks in save file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            SiiNunitData.NamelessControlList.Sort();
            SiiNunitData.NamelessIgnoreList.Sort();
            SiiNunitData.UnidentifiedBlocks.Sort();

            toolStripProgressBarMain.Value = 0;
            //ClearFormControls(false);

            ToggleMainControlsAccess(true);
            ToggleControlsAccess(true);

            PopulateFormControls();

            IO_Utilities.LogWriter("Successfully completed work with " + Globals.SelectedSavePath + " save file");

            //GC
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void workerWrite_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                ToggleMainControlsAccess(true);
                ToggleControlsAccess(true);

                IO_Utilities.ErrorLogWriter("Error during Writing save file" + Environment.NewLine + e.Error.Message + Environment.NewLine + e.Error.StackTrace);
                MessageBox.Show("Something went wrong during Writing Save file", "Error during Writing save file", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            toolStripProgressBarMain.Value = 0;

            ClearFormControls(false);

            ToggleMainControlsAccess(true);
            ToggleControlsAccess(false);

            //GC
            GC.Collect();
            GC.WaitForPendingFinalizers();

            IO_Utilities.LogWriter("Save game successfully writen in " + Globals.SelectedSavePath);
            MessageBox.Show("File saved", "Saving", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void PrintAddedJobs() {
            foreach (JobAdded tempJobData in AddedJobsList) {
                string SourceCityName = CitiesList.Find(x => x.CityName == tempJobData.SourceCity).CityNameTranslated;
                string SourceCompanyName = tempJobData.SourceCompany;
                CompaniesLngDict.TryGetValue(SourceCompanyName, out SourceCompanyName);

                string DestinationCityName = CitiesList.Find(x => x.CityName == tempJobData.DestinationCity).CityNameTranslated;
                string DestinationCompanyName = tempJobData.DestinationCompany;
                CompaniesLngDict.TryGetValue(DestinationCompanyName, out DestinationCompanyName);

                #region WriteLog
                //Write log
                string jobdata = "", tempStr = "";

                jobdata += "\r\nLoad of " + tempJobData.Cargo;
                jobdata += " of " + tempJobData.UnitsCount + " units";

                if (tempJobData.Type == 0)
                    tempStr = "Normal";
                else if (tempJobData.Type == 1)
                    tempStr = "Heavy";
                else if (tempJobData.Type == 2)
                    tempStr = "Double";

                jobdata += "\r\nIn " + tempStr + " trailer ";
                jobdata += tempJobData.TrailerDefinition;
                jobdata += " with " + tempJobData.TrailerVariant + " appearance";

                tempStr = tempJobData.Urgency.ToString();

                if (UrgencyLngDict.TryGetValue(tempStr, out string value))
                    if (value != null && value != "")
                        tempStr = value;

                jobdata += "\r\nUrgency - " + tempStr;
                jobdata += "\r\nMinimum travel distance of " + tempJobData.Distance + " km ";
                jobdata += "in " + tempJobData.CompanyTruck;
                jobdata += "\r\nJob valid for " + (tempJobData.ExpirationTime - SiiNunitData.Economy.game_time) + " minutes";

                if (tempJobData.Ferrytime > 0 || tempJobData.Ferryprice > 0) {
                    jobdata += "\r\nExtra time on ferry - " + tempJobData.Ferrytime;
                    jobdata += "and it will cost " + tempJobData.Ferryprice;
                }

                IO_Utilities.LogWriter("Job Added" + Environment.NewLine +
                    "From -> " + SourceCityName + " | " + SourceCompanyName + " -> To -> " + DestinationCityName + " | " + DestinationCompanyName +
                    "\r\n-----------" + jobdata + "\r\n-----------");

                #endregion
            }
        }

        //button_save_file
        private void NewWrireSaveFile(object sender, DoWorkEventArgs e) {
            string ProfileFolderPath = Globals.SelectedProfilePath + "\\profile.sii";
            string SiiInfoPath = Globals.SelectedSavePath + "\\info.sii";
            string SiiSavePath = Globals.SelectedSavePath + "\\game.sii";

            UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Info, "message_saving_file");

            if (File.GetLastWriteTime(SiiSavePath) > LastModifiedTimestamp) {
                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_file_was_modified");
                IO_Utilities.LogWriter("Save game was modified - reload file to prevent progress loss");
            } else {
                //Prepare
                PrepareEvents();

                PrepareGarages();

                PrepareGaragesWrite();
                PrepareCompaniesJobWrite();
                PrepareDriversTrucksWrite();
                PrepareVisitedCitiesWrite();

                PrepareUserColors();

                PrintAddedJobs();

                //Backup
                string ProfileFolderPathBackup = Globals.SelectedProfilePath + "\\profile_backup.sii";
                string SiiInfoPathBackup = Globals.SelectedSavePath + "\\info_backup.sii";
                string SiiSavePathBackup = Globals.SelectedSavePath + "\\game_backup.sii";

                File.Copy(ProfileFolderPath, ProfileFolderPathBackup, true);
                File.Copy(SiiInfoPath, SiiInfoPathBackup, true);
                File.Copy(SiiSavePath, SiiSavePathBackup, true);

                //Write Profile data
                if (MainSaveFileProfileData.isEdited)
                    using (StreamWriter writer = new StreamWriter(ProfileFolderPath, false)) {
                        writer.Write(MainSaveFileProfileData.PrintOut());
                    }

                //Write Info data
                if (MainSaveFileInfoData.isEdited)
                    using (StreamWriter writer = new StreamWriter(SiiInfoPath, false)) {
                        writer.Write(MainSaveFileInfoData.PrintOut());
                    }

                //Write Save data
                using (StreamWriter writer = new StreamWriter(SiiSavePath, false)) {
                    writer.Write(SiiNunitData.PrintOut(MainSaveFileInfoData.Version));
                }

                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Info, "message_file_saved");
            }
        }

        private void GetTranslationFiles() {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\lang")) {
                string[] langfolders = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\lang", "??-??", SearchOption.TopDirectoryOnly);
                string langTag;

                string flagpath;

                ArrayList tempTS_AList = new ArrayList();

                foreach (string folder in langfolders) {
                    string lngfile = folder + @"\lngfile.txt";

                    if (File.Exists(lngfile)) {
                        langTag = File.ReadAllLines(lngfile, Encoding.UTF8)[0].Split(new char[] { '[', ']' })[1];

                        //check
                        Regex rgx = new Regex(@"^[a-zA-Z]{2}-[a-zA-Z]{2}$");

                        if (!rgx.IsMatch(langTag))
                            continue;
                        //

                        CultureInfo ci = new CultureInfo(langTag, false);

                        char[] a = ci.NativeName.ToCharArray();
                        a[0] = char.ToUpper(a[0]);

                        string CorrectedNativeName = new string(a);

                        ToolStripItem TSitem = new ToolStripMenuItem();

                        TSitem.Name = langTag.Replace('-', '_') + "_ToolStripMenuItemTranslation";
                        TSitem.Text = CorrectedNativeName;
                        TSitem.Click += new EventHandler(toolstripChangeLanguage);

                        flagpath = folder + @"\flag.png";

                        if (File.Exists(flagpath)) {
                            TSitem.Image = new Bitmap(flagpath);
                            TSitem.ImageScaling = ToolStripItemImageScaling.None;
                        }

                        tempTS_AList.Add(TSitem);
                    }
                }

                IComparer myComparer = new TSSETtoolstripLanguage();
                tempTS_AList.Sort(myComparer);

                foreach (ToolStripItem TSitem in tempTS_AList) {
                    toolStripMenuItemLanguage.DropDownItems.Add(TSitem);
                }
            } else {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\lang");
            }

            //Button to Make blank folder for current Culture
            ToolStripItem TSitemBlank = new ToolStripMenuItem();

            TSitemBlank.Name = "toolStripMenuItemTranslationCreateBlankFolder";
            TSitemBlank.Text = "Make Blank translation for system language (" + CultureInfo.InstalledUICulture.NativeName + ")";
            TSitemBlank.Click += new EventHandler(makeToolStripMenuItem_Click);

            toolStripMenuItemLanguage.DropDownItems.Insert(0, TSitemBlank);

            //Separator
            toolStripMenuItemLanguage.DropDownItems.Insert(1, new ToolStripSeparator());
        }

        public class TSSETtoolstripLanguage : IComparer {
            int IComparer.Compare(Object x, Object y) {
                ToolStripItem oItem1 = x as ToolStripItem;
                ToolStripItem oItem2 = y as ToolStripItem;

                return ((new CaseInsensitiveComparer()).Compare(oItem1.Text, oItem2.Text));
            }
        }

        //Caching
        private void CacheGameData() {
            generalWorker = new BackgroundWorker();
            generalWorker.WorkerReportsProgress = false;
            generalWorker.DoWork += CacheExternalGameData;
            generalWorker.RunWorkerAsync();
        }

        private void CacheExternalGameData(object sender, DoWorkEventArgs e) {
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\gameref")) {
                string[] gameFolders = { "ETS2", "ATS" };

                foreach (string gamename in gameFolders) {
                    string gamefolder = Directory.GetCurrentDirectory() + @"\gameref\" + gamename;

                    if (Directory.Exists(gamefolder)) {
                        string[] dlcFolders = Directory.GetDirectories(gamefolder);

                        foreach (string dlcFolder in dlcFolders) {
                            string dbfilepath = Directory.GetCurrentDirectory() + @"\gameref\cache\" + gamename + "\\" + new DirectoryInfo(dlcFolder).Name + ".sdf";

                            if (!File.Exists(dbfilepath) || (new FileInfo(dbfilepath).LastWriteTime < new FileInfo(dlcFolder).LastWriteTime)) {
                                string cargoFolder = dlcFolder + @"\def\cargo";
                                //Scan cargo files
                                if (Directory.Exists(cargoFolder)) {
                                    if (!File.Exists(dbfilepath))
                                        ExtDataCreateDatabase(dbfilepath);

                                    string[] cargoFiles = Directory.GetFiles(cargoFolder, "*.sii");

                                    List<ExtCargo> tExtCargoList = new List<ExtCargo>();

                                    foreach (string cargo in cargoFiles) {
                                        ExtCargo tempExtCargo = null;
                                        string[] tempCargoFile = File.ReadAllLines(cargo);

                                        foreach (string line in tempCargoFile) {
                                            if (line.StartsWith("cargo_data:")) {
                                                tempExtCargo = new ExtCargo(line.Split(new char[] { '.' })[1]);
                                                continue;
                                            }
                                            if (line.StartsWith("	fragility:")) {
                                                tempExtCargo.Fragility = decimal.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty), CultureInfo.InvariantCulture);
                                                continue;
                                            }
                                            if (line.StartsWith("	adr_class:")) {
                                                tempExtCargo.ADRclass = int.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty));
                                                continue;
                                            }
                                            if (line.StartsWith("	mass:")) {
                                                tempExtCargo.Mass = decimal.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty), CultureInfo.InvariantCulture);
                                                continue;
                                            }
                                            if (line.StartsWith("	unit_reward_per_km:")) {
                                                tempExtCargo.UnitRewardpPerKM = decimal.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty), CultureInfo.InvariantCulture);
                                                continue;
                                            }
                                            if (line.StartsWith("	group[]:")) {
                                                tempExtCargo.Groups.Add(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty));
                                                continue;
                                            }
                                            if (line.StartsWith("	body_types[]:")) {
                                                tempExtCargo.BodyTypes.Add(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty));
                                                continue;
                                            }
                                            if (line.StartsWith("	maximum_distance:")) {
                                                tempExtCargo.MaxDistance = int.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty));
                                                continue;
                                            }
                                            if (line.StartsWith("	volume:")) {
                                                tempExtCargo.Volume = decimal.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty), CultureInfo.InvariantCulture);
                                                continue;
                                            }
                                            if (line.StartsWith("	valuable:")) {
                                                tempExtCargo.Valuable = bool.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty));
                                                continue;
                                            }
                                            if (line.StartsWith("	overweight:")) {
                                                tempExtCargo.Overweight = bool.Parse(line.Split(new char[] { ':' })[1].Replace(" ", String.Empty));
                                                continue;
                                            }
                                        }

                                        tExtCargoList.Add(tempExtCargo);
                                    }

                                    ExtDataInsertDataIntoDatabase(dbfilepath, "CargoesTable", tExtCargoList);
                                }

                                //Companies

                                string gcompanyFolder = dlcFolder + @"\def\company";
                                //Scan cargo files
                                if (Directory.Exists(gcompanyFolder)) {
                                    if (!File.Exists(dbfilepath))
                                        ExtDataCreateDatabase(dbfilepath);

                                    string[] companyFolders = Directory.GetDirectories(gcompanyFolder);

                                    List<ExtCompany> tempExternalCompanies = new List<ExtCompany>();

                                    companyFolders.AsParallel().ForAll(companyFolder => {
                                        if (Directory.Exists(companyFolder + @"\out")) {
                                            string company = companyFolder.Split(new string[] { "\\" }, StringSplitOptions.None).Last();

                                            string[] cargoes = Directory.GetFiles(companyFolder + @"\out", "*.sii");
                                            List<string> tempOutCargo = new List<string>();

                                            foreach (string cargo in cargoes) {
                                                string tempcargo = cargo.Split(new string[] { "\\" }, StringSplitOptions.None).Last().Split(new char[] { '.' })[0];

                                                tempOutCargo.Add(tempcargo);
                                            }

                                            if (!tempExternalCompanies.Exists(x => x.CompanyName == company)) {
                                                tempExternalCompanies.Add(new ExtCompany(company));
                                            }

                                            tempExternalCompanies.Find(x => x.CompanyName == company).AddCargoOut(tempOutCargo);
                                        }

                                        if (Directory.Exists(companyFolder + @"\in")) {
                                            string company = companyFolder.Split(new string[] { "\\" }, StringSplitOptions.None).Last();

                                            string[] cargoes = Directory.GetFiles(companyFolder + @"\in", "*.sii");
                                            List<string> tempInCargo = new List<string>();

                                            foreach (string cargo in cargoes) {
                                                string tempcargo = cargo.Split(new string[] { "\\" }, StringSplitOptions.None).Last().Split(new char[] { '.' })[0];

                                                tempInCargo.Add(tempcargo);
                                            }

                                            if (!tempExternalCompanies.Exists(x => x.CompanyName == company)) {
                                                tempExternalCompanies.Add(new ExtCompany(company));
                                            }

                                            tempExternalCompanies.Find(x => x.CompanyName == company).AddCargoIn(tempInCargo);
                                        }
                                    }
                                        );

                                    ExtDataInsertDataIntoDatabase(dbfilepath, "CompaniesTable", tempExternalCompanies);
                                }

                            }
                        }
                    }
                }
            } else {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\gameref");
            }
        }

    }
}