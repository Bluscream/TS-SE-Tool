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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

using TS_SE_Tool.Global;
using TS_SE_Tool.Utilities;
using System.Threading;
using TS_SE_Tool.Save;

namespace TS_SE_Tool {
    public partial class FormMain {
        //User Trailer tab
        private void CreateTrailerPanelControls() {
            CreateTrailerPanelMainButtons();
            CreateTrailerPanelPartsControls();
        }

        private void CreateTrailerPanelMainButtons() {
            int pHeight = RepairImg.Height, pOffset = 5, tOffset = comboBoxUserTrailerCompanyTrailers.Location.Y;
            int topbutoffset = comboBoxUserTrailerCompanyTrailers.Location.X + comboBoxUserTrailerCompanyTrailers.Width + pOffset;

            Button buttonR = new Button();
            tableLayoutPanelUserTrailerControls.Controls.Add(buttonR, 1, 0);
            buttonR.FlatStyle = FlatStyle.Flat;
            buttonR.Size = new Size(RepairImg.Height, RepairImg.Height);
            buttonR.Name = "buttonTrailerRepair";
            buttonR.BackgroundImage = RepairImg;
            buttonR.BackgroundImageLayout = ImageLayout.Zoom;
            buttonR.Text = "";
            buttonR.FlatAppearance.BorderSize = 0;
            buttonR.Dock = DockStyle.Fill;
            buttonR.Click += new EventHandler(buttonTrailerRepair_Click);
            buttonR.EnabledChanged += new EventHandler(buttonTrailerElRepair_EnabledChanged);

            Button buttonInfo = new Button();
            tableLayoutPanelUserTrailerControls.Controls.Add(buttonInfo, 3, 0);
            buttonInfo.FlatStyle = FlatStyle.Flat;
            buttonInfo.Size = new Size(CustomizeImg.Width, CustomizeImg.Height);
            buttonInfo.Name = "buttonTrailerInfo";
            buttonInfo.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(CustomizeImg);
            buttonInfo.BackgroundImageLayout = ImageLayout.Zoom;
            buttonInfo.Text = "";
            buttonInfo.FlatAppearance.BorderSize = 0;
            buttonInfo.Enabled = false;
            buttonInfo.Dock = DockStyle.Fill;
        }

        private void CreateTrailerPanelPartsControls() {
            int pSkillsNameHeight = 32, pSkillsNameWidth = 32;

            string[] toolskillimgtooltip = new string[] { "Cargo", "Body", "Chassis", "Wheels" };
            Label partLabel, partnameLabel;
            Panel pbPanel;

            for (int i = 0; i < 4; i++) {
                //Create table layout
                TableLayoutPanel tbllPanel = new TableLayoutPanel();
                tableLayoutPanelTrailerDetails.Controls.Add(tbllPanel, 0, i);
                tbllPanel.Dock = DockStyle.Fill;
                tbllPanel.Margin = new Padding(0);
                //
                tbllPanel.Name = "tableLayoutPanelTrailerDetails" + toolskillimgtooltip[i];

                tbllPanel.ColumnCount = 3;
                tbllPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
                tbllPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                tbllPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
                tbllPanel.RowCount = 2;
                tbllPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
                tbllPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                //

                //FlowLayoutPanel
                FlowLayoutPanel flowPanel = new FlowLayoutPanel();
                flowPanel.FlowDirection = FlowDirection.LeftToRight;
                flowPanel.Margin = new Padding(0);
                flowPanel.Dock = DockStyle.Fill;
                flowPanel.WrapContents = false;

                tbllPanel.Controls.Add(flowPanel, 0, 0);
                tbllPanel.SetColumnSpan(flowPanel, 2);
                //
                //Part type
                partLabel = new Label();
                partLabel.Name = "labelTrailerPartName" + toolskillimgtooltip[i];
                partLabel.Text = toolskillimgtooltip[i];
                partLabel.AutoSize = true;
                partLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                partLabel.MinimumSize = new Size(36, partLabel.Height);

                flowPanel.Controls.Add(partLabel);

                //Part name
                partnameLabel = new Label();
                partnameLabel.Name = "labelTrailerPartDataName" + i;
                partnameLabel.Text = "";
                partnameLabel.AutoSize = true;
                partnameLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

                flowPanel.Controls.Add(partnameLabel);

                //Part type image
                Panel imgpanel = new Panel();
                imgpanel.BorderStyle = BorderStyle.None;
                imgpanel.Size = new Size(pSkillsNameWidth, pSkillsNameHeight);
                imgpanel.Margin = new Padding(1);
                imgpanel.Name = "TrailerPartImg" + i.ToString();

                Bitmap bgimg = new Bitmap(TrailerPartsImg[i], pSkillsNameHeight, pSkillsNameWidth);
                imgpanel.BackgroundImage = bgimg;
                tbllPanel.Controls.Add(imgpanel, 0, 1);

                //Progress bar panel 
                pbPanel = new Panel();
                pbPanel.BorderStyle = BorderStyle.FixedSingle;
                pbPanel.Name = "progressbarTrailerPart" + i.ToString();
                pbPanel.Dock = DockStyle.Fill;
                tbllPanel.Controls.Add(pbPanel, 1, 1);

                //Repair button
                Button button = new Button();
                button.FlatStyle = FlatStyle.Flat;
                button.Dock = DockStyle.Fill;
                button.Margin = new Padding(1);

                button.Name = "buttonTrailerElRepair" + i.ToString();
                button.BackgroundImage = RepairImg;
                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.Text = "";
                button.FlatAppearance.BorderSize = 0;
                button.Click += new EventHandler(buttonTrailerElRepair_Click);
                button.EnabledChanged += new EventHandler(buttonTrailerElRepair_EnabledChanged);

                tbllPanel.Controls.Add(button, 2, 1);
            }

            //License plate
            //label
            Label labelPlate = new Label();
            labelPlate.Name = "labelUserTrailerLicensePlate";
            labelPlate.Text = "License plate";
            labelPlate.Margin = new Padding(0);
            labelPlate.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            labelPlate.TextAlign = ContentAlignment.MiddleCenter;

            tableLayoutPanelTrailerLP.Controls.Add(labelPlate, 0, 0);

            //text
            Label lcPlate = new Label();
            lcPlate.Name = "labelLicensePlateTrailer";
            lcPlate.Text = "A 000 AA";
            lcPlate.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            lcPlate.Dock = DockStyle.Fill;
            lcPlate.TextAlign = ContentAlignment.MiddleLeft;

            tableLayoutPanelTrailerLP.Controls.Add(lcPlate, 1, 0);

            //button Edit
            Button buttonLPEdit = new Button();
            buttonLPEdit.Size = new Size(CustomizeImg.Width, CustomizeImg.Height);
            buttonLPEdit.Name = "buttonTrailerLicensePlateEdit";
            buttonLPEdit.BackgroundImage = CustomizeImg;
            buttonLPEdit.BackgroundImageLayout = ImageLayout.Zoom;
            buttonLPEdit.Text = "";
            buttonLPEdit.Margin = new Padding(3, 0, 3, 0);
            buttonLPEdit.Enabled = true;
            buttonLPEdit.Dock = DockStyle.Fill;
            buttonLPEdit.Click += new EventHandler(buttonTrailerLicensePlateEdit_Click);

            tableLayoutPanelTrailerLP.Controls.Add(buttonLPEdit, 2, 0);

            //image
            Panel LPpanel = new Panel();
            LPpanel.Dock = DockStyle.Fill;
            LPpanel.Margin = new Padding(0);
            LPpanel.Name = "TrailerLicensePlateIMG";
            LPpanel.BackgroundImageLayout = ImageLayout.Center;
            LPpanel.BorderStyle = BorderStyle.None;

            tableLayoutPanelTrailerLP.Controls.Add(LPpanel, 3, 0);
        }

        public void buttonTrailerLicensePlateEdit_Click(object sender, EventArgs e) {
            UserTrailerDictionary.TryGetValue(comboBoxUserTrailerCompanyTrailers.SelectedValue.ToString(), out UserCompanyTrailerData SelectedUserCompanyTrailer);
            string LicensePlateText = SelectedUserCompanyTrailer.TrailerMainData.license_plate.Value;

            FormLicensePlateEdit frm = new FormLicensePlateEdit(LicensePlateText);
            frm.StartPosition = FormStartPosition.CenterParent;

            if (frm.ShowDialog() == DialogResult.OK) {
                //Find label control
                Label lpText = groupBoxUserTrailerTrailerDetails.Controls.Find("labelLicensePlateTrailer", true).FirstOrDefault() as Label;

                SelectedUserCompanyTrailer.TrailerMainData.license_plate = new Save.DataFormat.SCS_String(frm.licenseplatetext);

                UpdateTrailerPanelLicensePlate();
            }
        }

        private void FillUserCompanyTrailerList() {
            if (UserTrailerDictionary is null)
                return;

            DataTable combDT = new DataTable();
            DataColumn dc = new DataColumn("UserTrailerNameless", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("TrailerType", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("TrailerName", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("GarageName", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("DriverName", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("TrailerState", typeof(string));
            combDT.Columns.Add(dc);

            CultureInfo ci = Thread.CurrentThread.CurrentUICulture;

            string stringQT = ResourceManagerMain.GetPlainString("QuickJobTruckShort", ci),
                   stringUT = ResourceManagerMain.GetPlainString("UsersTruckShort", ci),
                   stringIU = ResourceManagerMain.GetPlainString("InUse", ci),
                   stringIM = ResourceManagerMain.GetPlainString("ItemMissing", ci),
                   stringBT = ResourceManagerMain.GetPlainString("BodyType", ci),
                   stringAC = ResourceManagerMain.GetPlainString("AxlesCount", ci),
                   stringCT = ResourceManagerMain.GetPlainString("ChainType", ci);

            DataColumn dcDisplay = new DataColumn("DisplayMember");
            dcDisplay.Expression = string.Format(
                "IIF(UserTrailerNameless <> 'null', " +
                "'[' + IIF(TrailerState <> '3', IIF(TrailerType = '0', '" + stringQT + "' ,'" + stringUT + "') ,'S') +'] ' + " +
                "IIF(GarageName <> '', {1} +' || ','') + " +
                "{2} + IIF(DriverName <> 'null', ' || " + stringIU + " - ' + {3},''), '" + stringIM + "')",
                "TrailerType", "GarageName", "TrailerName", "DriverName", "TrailerState");
            combDT.Columns.Add(dcDisplay);
            //

            combDT.Rows.Add("null"); // -- NONE --
            //

            foreach (KeyValuePair<string, UserCompanyTrailerData> UserTrailer in UserTrailerDictionary) {
                if (String.IsNullOrEmpty(UserTrailer.Key))
                    continue;

                if (UserTrailer.Value is null)
                    continue;

                if (UserTrailer.Value.TrailerMainData == null)
                    continue;

                if (UserTrailer.Value.Main) {
                    string trailerNameless = UserTrailer.Key; //link
                    string tmpTrailerName = "", tmpGarageName = "", tmpDriverName = "";
                    byte tmpTrailerType = 0, tmpTruckState = 1;

                    //Quick job or Bought
                    if (UserTrailer.Value.Users) {
                        tmpTrailerType = 1;

                        tmpTruckState = 2;

                        Garages tmpGrg = GaragesList.Find(x => x.Trailers.Contains(trailerNameless));

                        if (tmpGrg is null)
                            continue;

                        tmpGarageName = tmpGrg.GarageNameTranslated;
                    }

                    //Trailer type
                    Save.Items.Trailer tmpTrailerData = UserTrailer.Value.TrailerMainData;

                    string trailerdef = tmpTrailerData.trailer_definition;

                    if (UserTrailerDefDictionary.Count > 0) {
                        if (UserTrailerDefDictionary.ContainsKey(trailerdef)) {
                            string[] trailerDefExtra = { stringBT, stringAC, stringCT };
                            string trailername = "";
                            int lCounter = 0;

                            Save.Items.Trailer_Def CurTrailerDef = UserTrailerDefDictionary[trailerdef];

                            addToString(CurTrailerDef.body_type.ToString());
                            addToString(CurTrailerDef.axles.ToString());
                            addToString(CurTrailerDef.chain_type.ToString());

                            void addToString(string _input) {
                                if (!string.IsNullOrEmpty(_input)) {
                                    if (!string.IsNullOrEmpty(trailername))
                                        trailername += " | ";

                                    trailername += String.Format(trailerDefExtra[lCounter], CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_input));
                                }

                                lCounter++;
                            }

                            tmpTrailerName = trailername;
                        } else {
                            tmpTrailerName = String.Join(" ", trailerdef.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x != "trailer_def"));
                        }
                    } else {
                        tmpTrailerName = trailerdef;
                    }

                    //Driver
                    tmpDriverName = UserDriverDictionary.Where(x => x.Value.AssignedTrailer == trailerNameless)?.SingleOrDefault().Key ?? "null";

                    if (!String.IsNullOrEmpty(tmpDriverName) && tmpDriverName != "null") {
                        if (SiiNunitData.Player.drivers[0] == tmpDriverName) {
                            tmpDriverName = "> " + Utilities.TextUtilities.FromHexToString(Globals.SelectedProfile);
                        } else {
                            if (DriverNames.TryGetValue(tmpDriverName, out string _resultvalue)) {
                                if (!string.IsNullOrEmpty(_resultvalue)) {
                                    tmpDriverName = _resultvalue.TrimStart(new char[] { '+' });
                                }
                            }
                        }
                    }

                    //
                    combDT.Rows.Add(trailerNameless, tmpTrailerType, tmpTrailerName, tmpGarageName, tmpDriverName, tmpTruckState);
                }
            }

            combDT.DefaultView.Sort = "TrailerState, GarageName, TrailerName";

            comboBoxUserTrailerCompanyTrailers.ValueMember = "UserTrailerNameless";
            comboBoxUserTrailerCompanyTrailers.DisplayMember = "DisplayMember";
            comboBoxUserTrailerCompanyTrailers.DataSource = combDT;

            if (combDT.Rows.Count > 1) {
                comboBoxUserTrailerCompanyTrailers.Enabled = true;
            } else {
                comboBoxUserTrailerCompanyTrailers.Enabled = false;
            }

            comboBoxUserTrailerCompanyTrailers.SelectedValue = SiiNunitData.Player.assigned_trailer;
        }
        //
        private void UpdateTrailerPanelDetails() {
            for (byte i = 0; i < 4; i++)
                UpdateTrailerPanelProgressBar(i);

            CheckTrailerRepair();

            UpdateTrailerPanelLicensePlate();
        }

        private void UpdateTrailerPanelProgressBar(byte _number) {
            string trailerNameless = comboBoxUserTrailerCompanyTrailers.SelectedValue.ToString();

            UserTrailerDictionary.TryGetValue(trailerNameless, out UserCompanyTrailerData SelectedUserCompanyTrailer);

            if (SelectedUserCompanyTrailer == null)
                return;

            //Progres bar
            Panel progressBarPanel = groupBoxUserTrailerTrailerDetails.Controls.Find("progressbarTrailerPart" + _number, true).FirstOrDefault() as Panel;

            //Part name
            Label partLabel = groupBoxUserTrailerTrailerDetails.Controls.Find("labelTrailerPartDataName" + _number, true).FirstOrDefault() as Label;

            //Repair button
            Button repairButton = groupBoxUserTrailerTrailerDetails.Controls.Find("buttonTrailerElRepair" + _number, true).FirstOrDefault() as Button;

            if (progressBarPanel != null) {
                float _wear = 0,
                      _unfixableWear = 0,
                      _permanentWear = 0;
                string partType = "";

                // Get part type
                try {
                    switch (_number) {
                        case 0:
                            partType = "cargo";
                            _wear = SelectedUserCompanyTrailer.TrailerMainData.cargo_damage;
                            break;

                        case 1:
                            partType = "body";
                            _wear = SelectedUserCompanyTrailer.TrailerMainData.trailer_body_wear;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                _unfixableWear = SelectedUserCompanyTrailer.TrailerMainData.trailer_body_wear_unfixable;
                            }

                            break;

                        case 2:
                            partType = "chassis";
                            _wear = SelectedUserCompanyTrailer.TrailerMainData.chassis_wear;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                _unfixableWear = SelectedUserCompanyTrailer.TrailerMainData.chassis_wear_unfixable;
                            }

                            break;

                        case 3:
                            partType = "tire";
                            if (SelectedUserCompanyTrailer.TrailerMainData.wheels_wear.Count > 0)
                                _wear = SelectedUserCompanyTrailer.TrailerMainData.wheels_wear.Sum() / SelectedUserCompanyTrailer.TrailerMainData.wheels_wear.Count;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                if (SelectedUserCompanyTrailer.TrailerMainData.wheels_wear_unfixable.Count > 0)
                                    _unfixableWear = SelectedUserCompanyTrailer.TrailerMainData.wheels_wear_unfixable.Sum() /
                                                     SelectedUserCompanyTrailer.TrailerMainData.wheels_wear_unfixable.Count;
                            }

                            break;
                    }
                } catch {
                    repairButton.Enabled = false;
                    return;
                }

                // 1.49 

                _permanentWear = (float)SelectedUserCompanyTrailer.TrailerMainData.integrity_odometer / 5000000;

                //

                // Part name

                if (partLabel != null) {
                    string pnlText = SetTrailerPartLabelText(trailerNameless, SelectedUserCompanyTrailer.TrailerMainData, partType);

                    if (pnlText == null) {
                        repairButton.Enabled = false;
                        progressBarPanel.BackgroundImage = null;
                        partLabel.Text = "";

                        return;
                    } else {
                        partLabel.Text = pnlText;
                        toolTipMain.SetToolTip(partLabel, pnlText);
                    }
                }
                //===

                //=== Repair button

                if (_wear == 0 && _unfixableWear == 0 && _permanentWear == 0)
                    repairButton.Enabled = false;
                else
                    repairButton.Enabled = true;

                //===

                float totalWear = _wear + _unfixableWear + _permanentWear;

                //=== Progress bar

                SolidBrush ppen = new SolidBrush(Graphics_TSSET.GetProgressbarColor(totalWear));

                int x = 0, y = 0,
                    pnlWidth = (int)(progressBarPanel.Width * (1 - (totalWear)));

                Bitmap progress = new Bitmap(progressBarPanel.Width, progressBarPanel.Height);

                using (Graphics g = Graphics.FromImage(progress)) {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    g.FillRectangle(ppen, x, y, pnlWidth, progressBarPanel.Height);

                    int pnlWidthFixable = (int)(progressBarPanel.Width * _wear);

                    using (TextureBrush brush = new TextureBrush(VehicleIntegrityPBImg[0], WrapMode.Tile)) {
                        SolidBrush wearPen = new SolidBrush(Color.Yellow);
                        g.FillRectangle(wearPen, pnlWidth, 0, pnlWidthFixable, progressBarPanel.Height);

                        brush.TranslateTransform(pnlWidth, -4);
                        g.FillRectangle(brush, pnlWidth, 0, pnlWidthFixable, progressBarPanel.Height);
                    }

                    if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                        //1.49

                        int pnlWidthUnfixable = (int)(progressBarPanel.Width * _unfixableWear);

                        using (TextureBrush brush = new TextureBrush(VehicleIntegrityPBImg[1], WrapMode.Tile)) {
                            SolidBrush wearPen = new SolidBrush(Color.Orange);
                            g.FillRectangle(wearPen, pnlWidth + pnlWidthFixable, 0, pnlWidthUnfixable, progressBarPanel.Height);

                            brush.TranslateTransform(pnlWidth + pnlWidthFixable, 0);
                            g.FillRectangle(brush, pnlWidth + pnlWidthFixable, 0, pnlWidthUnfixable, progressBarPanel.Height);
                        }


                        int pnlWidthPermanent = (int)(progressBarPanel.Width * _permanentWear);

                        using (TextureBrush brush = new TextureBrush(VehicleIntegrityPBImg[2], WrapMode.Tile)) {
                            SolidBrush wearPen = new SolidBrush(Color.Red);
                            g.FillRectangle(wearPen, pnlWidth + pnlWidthFixable + pnlWidthUnfixable, 0, pnlWidthPermanent, progressBarPanel.Height);

                            brush.TranslateTransform(progressBarPanel.Width - (pnlWidthPermanent), 0);
                            g.FillRectangle(brush, progressBarPanel.Width - (pnlWidthPermanent), 0, pnlWidthPermanent, progressBarPanel.Height);
                        }

                        //1.49
                    }

                    int fontSize = 14;

                    string textPercent = ((int)((1 - totalWear) * 100)).ToString() + " %";
                    Font percentFont = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);

                    var textSize = g.MeasureString(textPercent, percentFont);

                    // Percent background

                    SolidBrush pbPen = new SolidBrush(Color.FromArgb(200, Color.White));
                    g.FillRectangle(pbPen, (progressBarPanel.ClientRectangle.Width - textSize.Width) / 2,
                                           (progressBarPanel.ClientRectangle.Height - textSize.Height) / 2, textSize.Width, textSize.Height);

                    //

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    GraphicsPath p = new GraphicsPath();

                    p.AddString(
                        textPercent,                    // text to draw
                        percentFont.FontFamily,         // or any other font family
                        (int)percentFont.Style,         // font style (bold, italic, etc.)
                        fontSize,                       // em size
                        new Rectangle(0, 0, progressBarPanel.Width, progressBarPanel.Height),     // location where to draw text
                        sf);                            // set options here (e.g. center alignment)

                    g.FillPath(Brushes.Black, p);
                    g.DrawPath(Pens.Black, p);
                }

                progressBarPanel.BackgroundImage = progress;
                //===
            }
        }

        private string SetTrailerPartLabelText(string _trailerNameless, Save.Items.Trailer _trailerMainData, string _partType) {
            string pnlText = "";
            bool accExist = true;

            if (_partType == "cargo") {
                //player_job
                if (SiiNunitData.Player.assigned_trailer == _trailerNameless) {
                    if (SiiNunitData.Player_Job != null) {
                        string tmpCargo = SiiNunitData.Player_Job.cargo.Split(new char[] { '.' }).Last();

                        if (CargoLngDict.TryGetValue(tmpCargo, out string value)) {
                            if (!string.IsNullOrEmpty(value))
                                pnlText = value;
                            else
                                pnlText = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value);
                        }
                    } else {
                        accExist = false;
                    }
                } else {
                    //drivers job
                    var tmp = UserDriverDictionary.Select(tx => tx.Value).Where(tX => tX.AssignedTrailer == _trailerNameless).ToList();

                    if (tmp != null && tmp.Count > 0) {
                        string tmpCargo = tmp[0].DriverJob.Cargo;

                        if (CargoLngDict.TryGetValue(tmpCargo, out string value)) {
                            if (!string.IsNullOrEmpty(value))
                                pnlText = value;
                            else
                                pnlText = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value);
                        }
                    } else {
                        accExist = false;
                    }
                }
            } else {
                foreach (string accLink in _trailerMainData.accessories) {
                    if (!SiiNunitData.SiiNitems.ContainsKey(accLink))
                        continue;

                    dynamic accessoryDyn = SiiNunitData.SiiNitems[accLink];
                    Type accType = accessoryDyn.GetType();

                    if (accType.Name == "Vehicle_Accessory" && _partType != "tire") {
                        Save.Items.Vehicle_Accessory accessoryThis = (Save.Items.Vehicle_Accessory)accessoryDyn;

                        if (accessoryThis.accType == _partType) {
                            Save.DataFormat.SCS_String tmp = accessoryThis.data_path;
                            pnlText = tmp.Value.Split(new char[] { '/' }).Last().Split(new char[] { '.' }).First();

                            continue;
                        }

                        if (pnlText == "" && accessoryThis.accType == "basepart") {
                            Save.DataFormat.SCS_String tmp = accessoryThis.data_path;
                            var datapathArray = tmp.Value.Split(new char[] { '/' });

                            if (datapathArray.Last() == "data.sii")
                                pnlText = datapathArray[datapathArray.Count() - 2];
                            else
                                pnlText = datapathArray.Last();

                            continue;
                        }
                    }

                    if (accType.Name == "Vehicle_Wheel_Accessory" && _partType == "tire") {
                        Save.Items.Vehicle_Wheel_Accessory accessoryThis = (Save.Items.Vehicle_Wheel_Accessory)accessoryDyn;

                        if (accessoryThis.accType == "tire") {
                            if (pnlText.Length != 0)
                                pnlText += " | ";

                            Save.DataFormat.SCS_String tmpString = accessoryThis.data_path;

                            pnlText += tmpString.Value.Split(new char[] { '/' }).Last().Split(new char[] { '.' }).First();
                            continue;
                        }
                    }
                }
            }

            if (!accExist)
                return null;

            if (pnlText == "")
                pnlText = "none";

            return pnlText;
        }

        private void CheckTrailerRepair() {
            bool repairEnabled = false;
            Button repairTrailer = tableLayoutPanelUserTrailerControls.Controls.Find("buttonTrailerRepair", true).FirstOrDefault() as Button;

            for (byte i = 0; i < 4; i++) {
                try {
                    Button tmp = groupBoxUserTrailerTrailerDetails.Controls.Find("buttonTrailerElRepair" + i, true).FirstOrDefault() as Button;
                    if (tmp.Enabled) {
                        repairEnabled = true;
                        break;
                    }
                } catch {
                    continue;
                }
            }

            repairTrailer.Enabled = repairEnabled;
        }

        private void UpdateTrailerPanelLicensePlate() {
            UserTrailerDictionary.TryGetValue(comboBoxUserTrailerCompanyTrailers.SelectedValue.ToString(), out UserCompanyTrailerData SelectedUserCompanyTrailer);

            string LicensePlate = SelectedUserCompanyTrailer.TrailerMainData.license_plate.Value;

            SCS.SCSLicensePlate thisLP = new SCS.SCSLicensePlate(LicensePlate, SCS.SCSLicensePlate.LPtype.Truck);

            //Find label control
            Label lpText = groupBoxUserTrailerTrailerDetails.Controls.Find("labelLicensePlateTrailer", true).FirstOrDefault() as Label;

            if (lpText != null) {
                lpText.Text = thisLP.LicensePlateTXT + " | ";

                CountriesLngDict.TryGetValue(thisLP.SourceLPCountry, out string value);

                if (!string.IsNullOrEmpty(value))
                    lpText.Text += value;
                else
                    lpText.Text += CultureInfo.InvariantCulture.TextInfo.ToTitleCase(thisLP.SourceLPCountry);
            }

            // ETS - 128x32 or ATS - 128x64 | 64x32
            Panel lpPanel = groupBoxUserTrailerTrailerDetails.Controls.Find("TrailerLicensePlateIMG", true).FirstOrDefault() as Panel;

            if (lpPanel != null)
                lpPanel.BackgroundImage = Graphics_TSSET.ResizeImage(thisLP.LicensePlateIMG, LicensePlateWidth[SelectedGame.Type], 32);
        }

        //Events
        private void comboBoxCompanyTrailers_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox cmbbx = sender as ComboBox;

            if (cmbbx.SelectedIndex != -1 && cmbbx.SelectedValue.ToString() != "null") {
                ToggleTrailerPartsCondition(true);

                groupBoxUserTrailerTrailerDetails.Enabled = true;
                groupBoxUserTrailerShareTrailerSettings.Enabled = true;

                buttonUserTrailerSelectCurrent.Enabled = true;
                tableLayoutPanelUserTrailerControls.Enabled = true;

                UpdateTrailerPanelDetails();
            } else {
                ToggleTrailerPartsCondition(false);

                groupBoxUserTrailerTrailerDetails.Enabled = false;
                groupBoxUserTrailerShareTrailerSettings.Enabled = false;

                if (!comboBoxUserTrailerCompanyTrailers.Enabled)
                    buttonUserTrailerSelectCurrent.Enabled = false;

                tableLayoutPanelUserTrailerControls.Enabled = false;
            }
        }

        private void groupBoxUserTrailerTrailerDetails_EnabledChanged(object sender, EventArgs e) {
            ToggleVisualTrailerDetails(groupBoxUserTrailerTrailerDetails.Enabled);
        }

        private void tableLayoutPanelUserTrailerControls_EnabledChanged(object sender, EventArgs e) {
            ToggleVisualTrailerControls(tableLayoutPanelUserTrailerControls.Enabled);
        }

        private void ToggleVisualTrailerDetails(bool _state) {
            for (int i = 0; i < 4; i++) {
                Control tmpButtonRepair = tabControlMain.TabPages["tabPageTrailer"].Controls.Find("buttonTrailerElRepair" + i.ToString(), true).FirstOrDefault();

                if (tmpButtonRepair == null)
                    continue;

                tmpButtonRepair.Enabled = _state;

                if (_state)
                    tmpButtonRepair.BackgroundImage = RepairImg;
                else
                    tmpButtonRepair.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(RepairImg);
            }
        }

        private void ToggleVisualTrailerControls(bool _state) {
            Control tmpControl;

            string[] buttons = { "buttonTrailerRepair", "buttonTrailerInfo", "buttonTrailerLicensePlateEdit" };
            Image[] images = { RepairImg, CustomizeImg, CustomizeImg };

            for (int i = 0; i < buttons.Count(); i++) {
                try {
                    tmpControl = tabControlMain.TabPages["tabPageTrailer"].Controls.Find(buttons[i], true)[0];
                } catch {
                    continue;
                }

                if (_state && tmpControl.Enabled)
                    tmpControl.BackgroundImage = images[i];
                else
                    tmpControl.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(images[i]);
            }
        }

        private void ToggleTrailerPartsCondition(bool _state) {
            if (!_state) {
                for (int i = 0; i < 4; i++) {
                    Label pnLabel = groupBoxUserTrailerTrailerDetails.Controls.Find("labelTrailerPartDataName" + i.ToString(), true).FirstOrDefault() as Label;

                    if (pnLabel != null)
                        pnLabel.Text = "";

                    Panel pbPanel = groupBoxUserTrailerTrailerDetails.Controls.Find("progressbarTrailerPart" + i.ToString(), true).FirstOrDefault() as Panel;

                    if (pbPanel != null)
                        pbPanel.BackgroundImage = null;

                }

                string lblLCname = "labelLicensePlateTrailer";
                Label lblLC = groupBoxUserTrailerTrailerDetails.Controls.Find(lblLCname, true).FirstOrDefault() as Label;

                if (lblLC != null)
                    lblLC.Text = "A 000 AA";

                string pnlname = "TrailerLicensePlateIMG";
                Panel LPPanel = groupBoxUserTrailerTrailerDetails.Controls.Find(pnlname, true).FirstOrDefault() as Panel;

                if (LPPanel != null)
                    LPPanel.BackgroundImage = null;
            } else {
                for (int i = 0; i < 4; i++) {
                    Button repairButton = groupBoxUserTrailerTrailerDetails.Controls.Find("buttonTrailerElRepair" + i, true).FirstOrDefault() as Button;

                    if (repairButton != null && repairButton.Enabled)
                        repairButton.BackgroundImage = RepairImg;
                }
            }
        }

        // Buttons

        // Main
        public void buttonTrailerRepair_Click(object sender, EventArgs e) {
            string trailerNameless = "", slaveTrailerNameless = "";
            Save.Items.Trailer selectedTrailerData;

            trailerNameless = comboBoxUserTrailerCompanyTrailers.SelectedValue.ToString();
            slaveTrailerNameless = trailerNameless;

            do {
                selectedTrailerData = SiiNunitData.SiiNitems[slaveTrailerNameless];

                selectedTrailerData.cargo_damage = 0;
                selectedTrailerData.trailer_body_wear = 0;
                selectedTrailerData.chassis_wear = 0;
                selectedTrailerData.wheels_wear = new List<Save.DataFormat.SCS_Float>();

                selectedTrailerData.trailer_body_wear_unfixable = 0;
                selectedTrailerData.chassis_wear_unfixable = 0;
                selectedTrailerData.wheels_wear_unfixable = new List<Save.DataFormat.SCS_Float>();

                selectedTrailerData.integrity_odometer = 0;
                selectedTrailerData.integrity_odometer_float_part = 0;

                slaveTrailerNameless = selectedTrailerData.slave_trailer;

                if (slaveTrailerNameless == "null")
                    break;

            } while (true);

            for (byte i = 0; i < 5; i++)
                UpdateTrailerPanelProgressBar(i);

            CheckTrailerRepair();
        }

        //
        private void buttonUserTrailerSelectCurrent_Click(object sender, EventArgs e) {
            comboBoxUserTrailerCompanyTrailers.SelectedValue = SiiNunitData.Player.assigned_trailer;
        }

        private void buttonUserTrailerSwitchCurrent_Click(object sender, EventArgs e) {
            SiiNunitData.Player.assigned_trailer = comboBoxUserTrailerCompanyTrailers.SelectedValue.ToString();
        }
        //Details
        public void buttonTrailerElRepair_Click(object sender, EventArgs e) {
            Button curbtn = sender as Button;

            //Get button index
            Regex reDigits = new Regex(@"\d+");
            Match reMatch = reDigits.Match(curbtn.Name);

            byte buttonIndex = byte.Parse(reMatch.Value);

            //
            string trailerNameless = "", slaveTrailerNameless = "";
            Save.Items.Trailer selectedTrailerData;

            trailerNameless = comboBoxUserTrailerCompanyTrailers.SelectedValue.ToString();
            slaveTrailerNameless = trailerNameless;

            do {
                selectedTrailerData = SiiNunitData.SiiNitems[slaveTrailerNameless];

                switch (buttonIndex) {
                    case 0:
                        selectedTrailerData.cargo_damage = 0;
                        break;

                    case 1:
                        selectedTrailerData.trailer_body_wear = 0;
                        selectedTrailerData.trailer_body_wear_unfixable = 0;

                        break;

                    case 2:
                        selectedTrailerData.chassis_wear = 0;
                        selectedTrailerData.chassis_wear_unfixable = 0;
                        break;

                    case 3:
                        selectedTrailerData.wheels_wear = new List<Save.DataFormat.SCS_Float>();
                        selectedTrailerData.wheels_wear_unfixable = new List<Save.DataFormat.SCS_Float>();
                        break;
                }

                slaveTrailerNameless = selectedTrailerData.slave_trailer;

                if (slaveTrailerNameless == "null")
                    break;

            } while (true);

            UpdateTrailerPanelProgressBar(buttonIndex);

            CheckTrailerRepair();
        }

        public void buttonTrailerElRepair_EnabledChanged(object sender, EventArgs e) {
            Button tmpButton = sender as Button;

            if (tmpButton.Enabled)
                tmpButton.BackgroundImage = RepairImg;
            else
                tmpButton.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(RepairImg);
        }

        //end User Trailer tab
    }
}