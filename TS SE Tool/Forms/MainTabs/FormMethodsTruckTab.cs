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

using TS_SE_Tool.Utilities;
using TS_SE_Tool.Global;
using System.Threading;
using S16.Drawing;
using TS_SE_Tool.Save;

namespace TS_SE_Tool {
    public partial class FormMain {
        //User Trucks tab
        private void CreateTruckPanelControls() {
            CreateTruckPanelMainButtons();
            CreateTruckPanelPartsControls();
        }

        private void CreateTruckPanelMainButtons() {
            int pHeight = RepairImg.Height, pOffset = 5, tOffset = comboBoxUserTruckCompanyTrucks.Location.Y;
            int topbutoffset = comboBoxUserTruckCompanyTrucks.Location.X + comboBoxUserTruckCompanyTrucks.Width + pOffset;

            Button buttonInfo = new Button();
            tableLayoutPanelUserTruckControls.Controls.Add(buttonInfo, 3, 0);
            buttonInfo.FlatStyle = FlatStyle.Flat;
            buttonInfo.Size = new Size(CustomizeImg.Width, CustomizeImg.Height);
            buttonInfo.Name = "buttonTruckVehicleEditor";
            buttonInfo.BackgroundImage = CustomizeImg;
            buttonInfo.BackgroundImageLayout = ImageLayout.Zoom;
            buttonInfo.Text = "";
            buttonInfo.FlatAppearance.BorderSize = 0;
            buttonInfo.Dock = DockStyle.Fill;
            buttonInfo.Click += new EventHandler(buttonUserTruckVehicleEditor_Click);

            Button buttonR = new Button();
            tableLayoutPanelUserTruckControls.Controls.Add(buttonR, 1, 0);
            buttonR.FlatStyle = FlatStyle.Flat;
            buttonR.Size = new Size(RepairImg.Height, RepairImg.Height);
            buttonR.Name = "buttonTruckRepair";
            buttonR.BackgroundImage = RepairImg;
            buttonR.BackgroundImageLayout = ImageLayout.Zoom;
            buttonR.Text = "";
            buttonR.FlatAppearance.BorderSize = 0;
            buttonR.Click += new EventHandler(buttonTruckRepair_Click);
            buttonR.EnabledChanged += new EventHandler(buttonElRepair_EnabledChanged);
            buttonR.Dock = DockStyle.Fill;

            Button buttonF = new Button();
            tableLayoutPanelUserTruckControls.Controls.Add(buttonF, 2, 0);
            buttonF.FlatStyle = FlatStyle.Flat;
            buttonF.Size = new Size(RepairImg.Height, RepairImg.Height);
            buttonF.Name = "buttonTruckReFuel";
            buttonF.BackgroundImage = RefuelImg;
            buttonF.BackgroundImageLayout = ImageLayout.Zoom;
            buttonF.Text = "";
            buttonF.FlatAppearance.BorderSize = 0;
            buttonF.Click += new EventHandler(buttonTruckReFuel_Click);
            buttonF.EnabledChanged += new EventHandler(buttonRefuel_EnabledChanged);
            buttonF.Dock = DockStyle.Fill;
        }

        private void CreateTruckPanelPartsControls() {
            int pSkillsNameHeight = 32, pSkillsNameWidth = 32;

            string[] toolskillimgtooltip = new string[] { "Engine", "Transmission", "Chassis", "Cabin", "Wheels" };
            Label partLabel, partnameLabel;
            Panel pbPanel;

            for (int i = 0; i < 5; i++) {
                //Create table layout
                TableLayoutPanel tbllPanel = new TableLayoutPanel();
                tableLayoutPanelTruckDetails.Controls.Add(tbllPanel, 0, i);
                tbllPanel.Dock = DockStyle.Fill;
                tbllPanel.Margin = new Padding(0);
                //
                tbllPanel.Name = "tableLayoutPanelTruckDetails" + toolskillimgtooltip[i];

                tbllPanel.ColumnCount = 3;
                tbllPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
                tbllPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                tbllPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
                tbllPanel.RowCount = 2;
                tbllPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
                tbllPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                //

                FlowLayoutPanel flowPanel = new FlowLayoutPanel();
                flowPanel.FlowDirection = FlowDirection.LeftToRight;
                flowPanel.Margin = new Padding(0);
                flowPanel.Dock = DockStyle.Fill;
                flowPanel.WrapContents = false;

                tbllPanel.Controls.Add(flowPanel, 0, 0);
                tbllPanel.SetColumnSpan(flowPanel, 2);

                //Part type
                partLabel = new Label();
                partLabel.Name = "labelTruckPartName" + toolskillimgtooltip[i];
                partLabel.Text = toolskillimgtooltip[i];
                partLabel.AutoSize = true;
                partLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                partLabel.MinimumSize = new Size(36, partLabel.Height);

                flowPanel.Controls.Add(partLabel);

                //Part name
                partnameLabel = new Label();
                partnameLabel.Name = "labelTruckPartDataName" + i;
                partnameLabel.Text = "";
                partnameLabel.AutoSize = true;
                partnameLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

                flowPanel.Controls.Add(partnameLabel);

                //Part type image
                Panel imgpanel = new Panel();
                imgpanel.BorderStyle = BorderStyle.None;
                imgpanel.Size = new Size(pSkillsNameWidth, pSkillsNameHeight);
                imgpanel.Margin = new Padding(1);
                imgpanel.Name = "TruckPartImg" + i.ToString();

                Bitmap bgimg = new Bitmap(TruckPartsImg[i], pSkillsNameHeight, pSkillsNameWidth);
                imgpanel.BackgroundImage = bgimg;
                tbllPanel.Controls.Add(imgpanel, 0, 1);

                //Progress bar panel 
                pbPanel = new Panel();
                pbPanel.BorderStyle = BorderStyle.FixedSingle;
                pbPanel.Name = "progressbarTruckPart" + i.ToString();
                pbPanel.Dock = DockStyle.Fill;
                tbllPanel.Controls.Add(pbPanel, 1, 1);

                //Repair button
                Button button = new Button();
                button.FlatStyle = FlatStyle.Flat;
                button.Dock = DockStyle.Fill;
                button.Margin = new Padding(1);

                button.Name = "buttonTruckElRepair" + i.ToString();
                button.BackgroundImage = RepairImg;
                button.BackgroundImageLayout = ImageLayout.Zoom;
                button.Text = "";
                button.FlatAppearance.BorderSize = 0;
                button.Click += new EventHandler(buttonElRepair_Click);
                button.EnabledChanged += new EventHandler(buttonElRepair_EnabledChanged);

                tbllPanel.Controls.Add(button, 2, 1);
            }

            //label - Fuel
            Label labelF = new Label();
            labelF.Name = "labelTruckDetailsFuel";
            labelF.Text = "Fuel";
            labelF.AutoSize = true;
            labelF.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            labelF.TextAlign = ContentAlignment.MiddleCenter;

            tableLayoutPanelTruckFuel.Controls.Add(labelF, 0, 0);

            //Fuel panel
            Panel Ppanelf = new Panel();
            Ppanelf.BorderStyle = BorderStyle.FixedSingle;
            Ppanelf.Dock = DockStyle.Fill;
            Ppanelf.Name = "progressbarTruckFuel";

            tableLayoutPanelTruckFuel.Controls.Add(Ppanelf, 0, 1);

            //License plate
            //label
            Label labelPlate = new Label();
            labelPlate.Name = "labelUserTruckLicensePlate";
            labelPlate.Text = "License plate";
            labelPlate.Margin = new Padding(0);
            labelPlate.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            labelPlate.TextAlign = ContentAlignment.MiddleCenter;

            tableLayoutPanelTruckLP.Controls.Add(labelPlate, 0, 0);

            //text
            Label lcPlate = new Label();
            lcPlate.Name = "labelLicensePlate";
            lcPlate.Text = "A 000 AA";
            lcPlate.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            lcPlate.Dock = DockStyle.Fill;
            lcPlate.TextAlign = ContentAlignment.MiddleLeft;

            tableLayoutPanelTruckLP.Controls.Add(lcPlate, 1, 0);

            //button Edit
            Button buttonLPEdit = new Button();
            buttonLPEdit.Size = new Size(CustomizeImg.Width, CustomizeImg.Height);
            buttonLPEdit.Name = "buttonTruckLicensePlateEdit";
            buttonLPEdit.BackgroundImage = CustomizeImg;
            buttonLPEdit.BackgroundImageLayout = ImageLayout.Zoom;
            buttonLPEdit.Text = "";
            buttonLPEdit.Margin = new Padding(3, 0, 3, 0);
            buttonLPEdit.Enabled = true;
            buttonLPEdit.Dock = DockStyle.Fill;
            buttonLPEdit.Click += new EventHandler(buttonTruckLicensePlateEdit_Click);

            tableLayoutPanelTruckLP.Controls.Add(buttonLPEdit, 2, 0);

            //image
            Panel LPpanel = new Panel();
            LPpanel.Dock = DockStyle.Fill;
            LPpanel.Margin = new Padding(0);
            LPpanel.Name = "TruckLicensePlateIMG";
            LPpanel.BackgroundImageLayout = ImageLayout.Center;
            LPpanel.BorderStyle = BorderStyle.None;

            tableLayoutPanelTruckLP.Controls.Add(LPpanel, 3, 0);
        }

        public void buttonTruckLicensePlateEdit_Click(object sender, EventArgs e) {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);
            string LicensePlateText = SelectedUserCompanyTruck.TruckMainData.license_plate.Value;

            FormLicensePlateEdit frm = new FormLicensePlateEdit(LicensePlateText);
            frm.StartPosition = FormStartPosition.CenterParent;

            if (frm.ShowDialog() == DialogResult.OK) {
                //Find label control
                Label lpText = groupBoxUserTruckTruckDetails.Controls.Find("labelLicensePlate", true).FirstOrDefault() as Label;

                SelectedUserCompanyTruck.TruckMainData.license_plate = new Save.DataFormat.SCS_String(frm.licenseplatetext);

                UpdateTruckPanelLicensePlate();
            }
        }

        private void FillUserCompanyTrucksList() {
            if (UserTruckDictionary == null)
                return;

            // Creating Data table

            DataTable combDT = new DataTable();
            DataColumn dc = new DataColumn("UserTruckNameless", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("TruckType", typeof(byte));
            combDT.Columns.Add(dc);

            dc = new DataColumn("TruckName", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("GarageName", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("DriverName", typeof(string));
            combDT.Columns.Add(dc);

            dc = new DataColumn("TruckState", typeof(byte));
            combDT.Columns.Add(dc);

            CultureInfo ci = Thread.CurrentThread.CurrentUICulture;

            string stringQT = ResourceManagerMain.GetPlainString("QuickJobTruckShort", ci),
                   stringUT = ResourceManagerMain.GetPlainString("UsersTruckShort", ci),
                   stringIU = ResourceManagerMain.GetPlainString("InUse", ci),
                   stringIM = ResourceManagerMain.GetPlainString("ItemMissing", ci);

            DataColumn dcDisplay = new DataColumn("DisplayMember");
            dcDisplay.Expression = string.Format(
                "IIF(UserTruckNameless <> 'null', " +
                "'[' + IIF(TruckState <> '3', IIF(TruckType = '0', '" + stringQT + "' ,'" + stringUT + "') ,'S') +'] ' + " +
                "IIF(GarageName <> '', {1} +' || ','') + " +
                "{2} + IIF(DriverName <> 'null', ' || " + stringIU + " - ' + {3},''), '" + stringIM + "')",
                "TruckType", "GarageName", "TruckName", "DriverName", "TruckState");
            combDT.Columns.Add(dcDisplay);

            //===

            //Iterate through User trucks
            foreach (KeyValuePair<string, UserCompanyTruckData> UserTruck in UserTruckDictionary) {
                if (String.IsNullOrEmpty(UserTruck.Key))
                    continue;

                if (UserTruck.Value == null)
                    continue;

                if (UserTruck.Value.TruckMainData == null)
                    continue;

                if (UserTruck.Value.TruckMainData.accessories.Count == 0)
                    continue;

                //Setup values
                string truckName = "undetected",
                       truckNameless = UserTruck.Key; //link
                string tmpTruckName = "", tmpGarageName = "", tmpDriverName = "";
                byte tmpTruckType = 0, tmpTruckState = 1;

                //Quick job or Bought
                if (UserTruck.Value.Users) {
                    tmpTruckType = 1;

                    tmpTruckState = 2;

                    //Garage
                    tmpGarageName = GaragesList.Find(x => x.Vehicles.Contains(truckNameless)).GarageNameTranslated;
                }

                //Brand
                foreach (string accLink in UserTruck.Value.TruckMainData.accessories) {
                    if (!String.IsNullOrEmpty(accLink))
                        if (SiiNunitData.SiiNitems[accLink].GetType().Name == "Vehicle_Accessory") {
                            var tmpAcc = (Save.Items.Vehicle_Accessory)SiiNunitData.SiiNitems[accLink];

                            if (tmpAcc.accType == "basepart") {
                                if (!String.IsNullOrEmpty(tmpAcc.data_path)) {
                                    try {
                                        var tmpParts = tmpAcc.data_path.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0]
                                        .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                                        truckName = tmpParts[tmpParts.Length - 2];
                                    } catch { }
                                }

                                break;
                            }
                        }
                }

                if (TruckBrandsLngDict.TryGetValue(truckName, out string truckNameValue)) {
                    if (!String.IsNullOrEmpty(truckNameValue)) {
                        tmpTruckName = truckNameValue;
                    } else {
                        tmpTruckName = truckName;
                    }
                } else {
                    var tmpParts = truckName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in tmpParts) {
                        tmpTruckName += Utilities.TextUtilities.CapitalizeWord(word) + " ";
                    }

                    tmpTruckName = tmpTruckName.TrimEnd(new char[] { ' ' });
                }

                //Driver
                Garages tmpGrg = GaragesList.Where(tX => tX.Vehicles.Contains(truckNameless))?.SingleOrDefault() ?? null;

                if (tmpGrg != null) {
                    tmpDriverName = tmpGrg.Drivers[tmpGrg.Vehicles.IndexOf(truckNameless)];
                } else {
                    tmpDriverName = UserDriverDictionary.Where(tX => tX.Value.AssignedTruck == truckNameless)?.SingleOrDefault().Key ?? "null";
                }

                if (!String.IsNullOrEmpty(tmpDriverName) && tmpDriverName != "null") {
                    if (SiiNunitData.Player.drivers[0] == tmpDriverName || tmpTruckType == 0) {
                        tmpDriverName = "> " + Utilities.TextUtilities.FromHexToString(Globals.SelectedProfile);
                    } else {
                        if (DriverNames.TryGetValue(tmpDriverName, out string _resultvalue)) {
                            if (!String.IsNullOrEmpty(_resultvalue)) {
                                tmpDriverName = _resultvalue.TrimStart(new char[] { '+' });
                            }
                        }
                    }
                }

                // Add row
                combDT.Rows.Add(truckNameless, tmpTruckType, tmpTruckName, tmpGarageName, tmpDriverName, tmpTruckState);
            }

            bool noTrucks = false;

            if (combDT.Rows.Count == 0) {
                combDT.Rows.Add("null"); // -- NONE --
                noTrucks = true;
            }

            combDT.DefaultView.Sort = "TruckState, GarageName, TruckName";

            comboBoxUserTruckCompanyTrucks.ValueMember = "UserTruckNameless";
            comboBoxUserTruckCompanyTrucks.DisplayMember = "DisplayMember";
            comboBoxUserTruckCompanyTrucks.DataSource = combDT;

            comboBoxUserTruckCompanyTrucks.Enabled = !noTrucks;

            if (!noTrucks)
                comboBoxUserTruckCompanyTrucks.SelectedValue = SiiNunitData.Player.assigned_truck;
            else
                comboBoxUserTruckCompanyTrucks.SelectedValue = "null";
        }
        //

        private void UpdateTruckPanelDetails() {
            for (byte i = 0; i < 5; i++)
                UpdateTruckPanelProgressBar(i);

            CheckTruckRepair();

            UpdateTruckPanelFuel();
            UpdateTruckPanelLicensePlate();
        }

        private void UpdateTruckPanelProgressBar(byte _number) {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            if (SelectedUserCompanyTruck == null)
                return;

            string pnlname = "progressbarTruckPart" + _number.ToString(), labelPartName = "labelTruckPartDataName" + _number.ToString();

            //Progres bar
            Panel pbPanel = groupBoxUserTruckTruckDetails.Controls.Find(pnlname, true).FirstOrDefault() as Panel;

            //Part name
            Label pnLabel = groupBoxUserTruckTruckDetails.Controls.Find(labelPartName, true).FirstOrDefault() as Label;

            //Repair button
            Button repairButton = groupBoxUserTruckTruckDetails.Controls.Find("buttonTruckElRepair" + _number, true).FirstOrDefault() as Button;

            if (pbPanel != null) {
                float _wear = 0,
                      _unfixableWear = 0,
                      _permanentWear = 0;
                string partType = "";

                // Part wear

                try {
                    switch (_number) {
                        case 0:
                            partType = "engine";
                            _wear = SelectedUserCompanyTruck.TruckMainData.engine_wear;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                _unfixableWear = SelectedUserCompanyTruck.TruckMainData.engine_wear_unfixable;
                            }

                            break;

                        case 1:
                            partType = "transmission";
                            _wear = SelectedUserCompanyTruck.TruckMainData.transmission_wear;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                _unfixableWear = SelectedUserCompanyTruck.TruckMainData.transmission_wear_unfixable;
                            }

                            break;

                        case 2:
                            partType = "chassis";
                            _wear = SelectedUserCompanyTruck.TruckMainData.chassis_wear;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                _unfixableWear = SelectedUserCompanyTruck.TruckMainData.chassis_wear_unfixable;
                            }

                            break;

                        case 3:
                            partType = "cabin";
                            _wear = SelectedUserCompanyTruck.TruckMainData.cabin_wear;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                _unfixableWear = SelectedUserCompanyTruck.TruckMainData.cabin_wear_unfixable;
                            }

                            break;

                        case 4:
                            partType = "tire";
                            if (SelectedUserCompanyTruck.TruckMainData.wheels_wear.Count > 0)
                                _wear = SelectedUserCompanyTruck.TruckMainData.wheels_wear.Sum() / SelectedUserCompanyTruck.TruckMainData.wheels_wear.Count;

                            if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                                if (SelectedUserCompanyTruck.TruckMainData.wheels_wear_unfixable.Count > 0)
                                    _unfixableWear = SelectedUserCompanyTruck.TruckMainData.wheels_wear_unfixable.Sum() / SelectedUserCompanyTruck.TruckMainData.wheels_wear_unfixable.Count;
                            }

                            break;
                    }
                } catch {
                    repairButton.Enabled = false;
                    return;
                }

                // 1.49 

                _permanentWear = (float)SelectedUserCompanyTruck.TruckMainData.integrity_odometer / 5000000;

                //

                // Part name

                if (pnLabel != null) {
                    pnLabel.Text = "";
                    string pnlText = "";

                    foreach (string accLink in SelectedUserCompanyTruck.TruckMainData.accessories) {
                        dynamic accessoryDyn = SiiNunitData.SiiNitems[accLink];

                        Type accType = accessoryDyn.GetType();

                        if (accType.Name == "Vehicle_Accessory" && partType != "tire") {
                            Save.Items.Vehicle_Accessory tmp = (Save.Items.Vehicle_Accessory)SiiNunitData.SiiNitems[accLink];

                            if (tmp.accType == partType) {
                                pnlText = tmp.data_path.Split(new char[] { '"' })[1].Split(new char[] { '/' }).Last().Split(new char[] { '.' })[0];
                                break;
                            }
                        } else if (accType.Name == "Vehicle_Wheel_Accessory" && partType == "tire") {
                            Save.Items.Vehicle_Wheel_Accessory tmp = (Save.Items.Vehicle_Wheel_Accessory)accessoryDyn;

                            if (tmp.accType == "tire") {
                                if (pnlText.Length != 0)
                                    pnlText += " | ";

                                pnlText += tmp.data_path.Split(new char[] { '"' })[1].Split(new char[] { '/' }).Last().Split(new char[] { '.' })[0];
                                continue;
                            }
                        }
                    }

                    pnLabel.Text = pnlText;
                    toolTipMain.SetToolTip(pnLabel, pnlText);
                }

                //

                if (_wear == 0 && _unfixableWear == 0 && _permanentWear == 0)
                    repairButton.Enabled = false;
                else
                    repairButton.Enabled = true;

                //

                float totalWear = _wear + _unfixableWear + _permanentWear;

                int x = 0, y = 0,
                    pnlWidth = (int)(pbPanel.Width * (1 - (totalWear)));

                Bitmap progressBar = new Bitmap(pbPanel.Width, pbPanel.Height);

                using (Graphics g = Graphics.FromImage(progressBar)) {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    SolidBrush pPen = new SolidBrush(Graphics_TSSET.GetProgressbarColor(totalWear));
                    g.FillRectangle(pPen, x, y, pnlWidth, pbPanel.Height);

                    int pnlWidthFixable = (int)(pbPanel.Width * _wear);

                    using (TextureBrush brush = new TextureBrush(VehicleIntegrityPBImg[0], WrapMode.Tile)) {
                        SolidBrush wearPen = new SolidBrush(Color.Yellow);
                        g.FillRectangle(wearPen, pnlWidth, 0, pnlWidthFixable, pbPanel.Height);

                        brush.TranslateTransform(pnlWidth, -4);
                        g.FillRectangle(brush, pnlWidth, 0, pnlWidthFixable, pbPanel.Height);
                    }

                    if (MainSaveFileInfoData.Version > (byte)saveVTV.v148) {
                        //1.49

                        int pnlWidthUnfixable = (int)(pbPanel.Width * _unfixableWear);

                        using (TextureBrush brush = new TextureBrush(VehicleIntegrityPBImg[1], WrapMode.Tile)) {
                            SolidBrush wearPen = new SolidBrush(Color.Orange);
                            g.FillRectangle(wearPen, pnlWidth + pnlWidthFixable, 0, pnlWidthUnfixable, pbPanel.Height);

                            brush.TranslateTransform(pnlWidth + pnlWidthFixable, 0);
                            g.FillRectangle(brush, pnlWidth + pnlWidthFixable, 0, pnlWidthUnfixable, pbPanel.Height);
                        }


                        int pnlWidthPermanent = (int)(pbPanel.Width * _permanentWear);

                        using (TextureBrush brush = new TextureBrush(VehicleIntegrityPBImg[2], WrapMode.Tile)) {
                            SolidBrush wearPen = new SolidBrush(Color.Red);
                            g.FillRectangle(wearPen, pnlWidth + pnlWidthFixable + pnlWidthUnfixable, 0, pnlWidthPermanent, pbPanel.Height);

                            brush.TranslateTransform(pbPanel.Width - (pnlWidthPermanent), 0);
                            g.FillRectangle(brush, pbPanel.Width - (pnlWidthPermanent), 0, pnlWidthPermanent, pbPanel.Height);
                        }

                        //1.49
                    }

                    int fontSize = 14;

                    string textPercent = ((int)((1 - totalWear) * 100)).ToString() + " %";
                    Font percentFont = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);

                    var textSize = g.MeasureString(textPercent, percentFont);

                    // Percent background

                    SolidBrush pbPen = new SolidBrush(Color.FromArgb(200, Color.White));
                    g.FillRectangle(pbPen, (pbPanel.ClientRectangle.Width - textSize.Width) / 2,
                                           (pbPanel.ClientRectangle.Height - textSize.Height) / 2, textSize.Width, textSize.Height);

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
                        new Rectangle(0, 0, pbPanel.Width, pbPanel.Height),     // location where to draw text
                        sf);                            // set options here (e.g. center alignment)

                    g.FillPath(Brushes.Black, p);
                    g.DrawPath(Pens.Black, p);
                }

                //===

                pbPanel.BackgroundImage = progressBar;
            }
        }

        private void CheckTruckRepair() {
            bool repairEnabled = false;
            Button repairTruck = tableLayoutPanelUserTruckControls.Controls.Find("buttonTruckRepair", true).FirstOrDefault() as Button;

            for (byte i = 0; i < 5; i++) {
                try {
                    Button tmp = groupBoxUserTruckTruckDetails.Controls.Find("buttonTruckElRepair" + i, true).FirstOrDefault() as Button;
                    if (tmp.Enabled) {
                        repairEnabled = true;
                        break;
                    }
                } catch {
                    continue;
                }
            }

            repairTruck.Enabled = repairEnabled;
        }

        private void UpdateTruckPanelFuel() {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            string pnlnamefuel = "progressbarTruckFuel";
            Panel pnlfuel = groupBoxUserTruckTruckDetails.Controls.Find(pnlnamefuel, true).FirstOrDefault() as Panel;

            Button refuelTruck = tableLayoutPanelUserTruckControls.Controls.Find("buttonTruckReFuel", true).FirstOrDefault() as Button;

            if (pnlfuel != null) {
                float _fuel = SelectedUserCompanyTruck.TruckMainData.fuel_relative;

                if (_fuel == 1)
                    refuelTruck.Enabled = false;
                else
                    refuelTruck.Enabled = true;

                SolidBrush ppen = new SolidBrush(Graphics_TSSET.GetProgressbarColor(1 - _fuel));
                int pnlheight = (int)(pnlfuel.Height * _fuel),
                    x = 0, y = pnlfuel.Height - pnlheight;

                Bitmap progress = new Bitmap(pnlfuel.Width, pnlfuel.Height);

                using (Graphics g = Graphics.FromImage(progress)) {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillRectangle(ppen, x, y, pnlfuel.Width, pnlheight);

                    int fontSize = 14;

                    string textPercent = ((int)(_fuel * 100)).ToString() + " %";
                    Font percentFont = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);

                    var textSize = g.MeasureString(textPercent, percentFont);

                    // Percent background

                    SolidBrush pbPen = new SolidBrush(Color.FromArgb(200, Color.White));
                    g.FillRectangle(pbPen, (pnlfuel.ClientRectangle.Width - textSize.Width) / 2, (pnlfuel.ClientRectangle.Height - textSize.Height) / 2, textSize.Width, textSize.Height);

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
                        new Rectangle(0, 0, pnlfuel.Width, pnlfuel.Height), // location where to draw text
                        sf);                            // set options here (e.g. center alignment)

                    g.FillPath(Brushes.Black, p);
                    g.DrawPath(Pens.Black, p);
                }

                pnlfuel.BackgroundImage = progress;
            }
        }

        private void UpdateTruckPanelLicensePlate() {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            string LicensePlate = SelectedUserCompanyTruck.TruckMainData.license_plate.Value;

            SCS.SCSLicensePlate thisLP = new SCS.SCSLicensePlate(LicensePlate, SCS.SCSLicensePlate.LPtype.Truck);

            //Find label control
            Label lpText = groupBoxUserTruckTruckDetails.Controls.Find("labelLicensePlate", true).FirstOrDefault() as Label;

            if (lpText != null) {
                lpText.Text = thisLP.LicensePlateTXT + " | ";

                string value = null;
                CountriesLngDict.TryGetValue(thisLP.SourceLPCountry, out value);

                if (value != null && value != "")
                    lpText.Text += value;
                else
                    lpText.Text += CultureInfo.InvariantCulture.TextInfo.ToTitleCase(thisLP.SourceLPCountry);
            }

            //
            Panel lpPanel = groupBoxUserTruckTruckDetails.Controls.Find("TruckLicensePlateIMG", true).FirstOrDefault() as Panel;
            if (lpPanel != null) {
                lpPanel.BackgroundImage = Graphics_TSSET.ResizeImage(thisLP.LicensePlateIMG, SelectedGame.LicensePlateWidth, 32); //ETS - 128x32 or ATS - 128x64 | 64x32
            }
        }

        //Events
        private void comboBoxCompanyTrucks_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox cmbbx = sender as ComboBox;

            if (cmbbx.SelectedValue != null && cmbbx.SelectedValue.ToString() != "null") //cmbbx.SelectedIndex != -1 && 
            {
                ToggleTruckPartsCondition(true);

                groupBoxUserTruckTruckDetails.Enabled = true;
                groupBoxUserTruckShareTruckSettings.Enabled = true;

                buttonUserTruckSelectCurrent.Enabled = true;
                tableLayoutPanelUserTruckControls.Enabled = true;

                UpdateTruckPanelDetails();
            } else {
                ToggleTruckPartsCondition(false);

                groupBoxUserTruckTruckDetails.Enabled = false;
                groupBoxUserTruckShareTruckSettings.Enabled = false;

                buttonUserTruckSelectCurrent.Enabled = false;
                tableLayoutPanelUserTruckControls.Enabled = false;
            }
        }

        private void groupBoxUserTruckTruckDetails_EnabledChanged(object sender, EventArgs e) {
            ToggleVisualTruckDetails(groupBoxUserTruckTruckDetails.Enabled);
        }

        private void tableLayoutPanelUserTruckControls_EnabledChanged(object sender, EventArgs e) {
            ToggleVisualTruckControls(tableLayoutPanelUserTruckControls.Enabled);
        }

        private void ToggleVisualTruckDetails(bool _state) {
            for (int i = 0; i < 5; i++) {
                Control tmpButtonRepair = tabControlMain.TabPages["tabPageTruck"].Controls.Find("buttonTruckElRepair" + i.ToString(), true).FirstOrDefault();

                if (tmpButtonRepair == null)
                    continue;

                tmpButtonRepair.Enabled = _state;

                if (_state)
                    tmpButtonRepair.BackgroundImage = RepairImg;
                else
                    tmpButtonRepair.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(RepairImg);
            }
        }

        private void ToggleVisualTruckControls(bool _state) {
            Control tmpControl;

            string[] buttons = { "buttonTruckReFuel", "buttonTruckRepair", "buttonTruckVehicleEditor", "buttonTruckLicensePlateEdit" };
            Image[] images = { RefuelImg, RepairImg, CustomizeImg, CustomizeImg };

            for (int i = 0; i < buttons.Count(); i++) {
                try {
                    tmpControl = tabControlMain.TabPages["tabPageTruck"].Controls.Find(buttons[i], true)[0];
                } catch {
                    continue;
                }

                if (_state && tmpControl.Enabled)
                    tmpControl.BackgroundImage = images[i];
                else
                    tmpControl.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(images[i]);
            }
        }

        private void ToggleTruckPartsCondition(bool _state) {
            if (!_state) {
                string lblname, pnlname;

                for (int i = 0; i < 5; i++) {
                    lblname = "labelTruckPartDataName" + i.ToString();
                    Label pnLabel = groupBoxUserTruckTruckDetails.Controls.Find(lblname, true).FirstOrDefault() as Label;

                    if (pnLabel != null)
                        pnLabel.Text = "";

                    pnlname = "progressbarTruckPart" + i.ToString();
                    Panel pbPanel = groupBoxUserTruckTruckDetails.Controls.Find(pnlname, true).FirstOrDefault() as Panel;

                    if (pbPanel != null)
                        pbPanel.BackgroundImage = null;
                }

                string pnlFname = "progressbarTruckFuel";
                Panel pnlF = groupBoxUserTruckTruckDetails.Controls.Find(pnlFname, true).FirstOrDefault() as Panel;

                if (pnlF != null)
                    pnlF.BackgroundImage = null;

                string lblLCname = "labelLicensePlate";
                Label lblLC = groupBoxUserTruckTruckDetails.Controls.Find(lblLCname, true).FirstOrDefault() as Label;

                if (lblLC != null)
                    lblLC.Text = "A 000 AA";

                pnlname = "TruckLicensePlateIMG";
                Panel LPPanel = groupBoxUserTruckTruckDetails.Controls.Find(pnlname, true).FirstOrDefault() as Panel;

                if (LPPanel != null)
                    LPPanel.BackgroundImage = null;
            }
        }
        //Buttons
        public void buttonTruckReFuel_Click(object sender, EventArgs e) {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            if (SelectedUserCompanyTruck == null)
                return;

            SelectedUserCompanyTruck.TruckMainData.fuel_relative = 1;

            UpdateTruckPanelFuel();
        }

        public void buttonTruckRepair_Click(object sender, EventArgs e) {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            if (SelectedUserCompanyTruck == null)
                return;

            SelectedUserCompanyTruck.TruckMainData.engine_wear = 0;
            SelectedUserCompanyTruck.TruckMainData.transmission_wear = 0;
            SelectedUserCompanyTruck.TruckMainData.chassis_wear = 0;
            SelectedUserCompanyTruck.TruckMainData.cabin_wear = 0;
            SelectedUserCompanyTruck.TruckMainData.wheels_wear = new List<Save.DataFormat.SCS_Float>();

            SelectedUserCompanyTruck.TruckMainData.engine_wear_unfixable = 0;
            SelectedUserCompanyTruck.TruckMainData.transmission_wear_unfixable = 0;
            SelectedUserCompanyTruck.TruckMainData.chassis_wear_unfixable = 0;
            SelectedUserCompanyTruck.TruckMainData.cabin_wear_unfixable = 0;
            SelectedUserCompanyTruck.TruckMainData.wheels_wear_unfixable = new List<Save.DataFormat.SCS_Float>();

            SelectedUserCompanyTruck.TruckMainData.integrity_odometer = 0;
            SelectedUserCompanyTruck.TruckMainData.integrity_odometer_float_part = 0;

            for (byte i = 0; i < 5; i++)
                UpdateTruckPanelProgressBar(i);

            CheckTruckRepair();
        }
        //
        public void buttonElRepair_Click(object sender, EventArgs e) {
            Button curbtn = sender as Button;
            byte bi = Convert.ToByte(curbtn.Name.Substring(19));

            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            if (SelectedUserCompanyTruck == null)
                return;

            switch (bi) {
                case 0:
                    SelectedUserCompanyTruck.TruckMainData.engine_wear = 0;
                    SelectedUserCompanyTruck.TruckMainData.engine_wear_unfixable = 0;

                    break;
                case 1:
                    SelectedUserCompanyTruck.TruckMainData.transmission_wear = 0;
                    SelectedUserCompanyTruck.TruckMainData.transmission_wear_unfixable = 0;
                    break;
                case 2:
                    SelectedUserCompanyTruck.TruckMainData.chassis_wear = 0;
                    SelectedUserCompanyTruck.TruckMainData.chassis_wear_unfixable = 0;
                    break;
                case 3:
                    SelectedUserCompanyTruck.TruckMainData.cabin_wear = 0;
                    SelectedUserCompanyTruck.TruckMainData.cabin_wear_unfixable = 0;
                    break;
                case 4:
                    SelectedUserCompanyTruck.TruckMainData.wheels_wear = new List<Save.DataFormat.SCS_Float>();
                    SelectedUserCompanyTruck.TruckMainData.wheels_wear_unfixable = new List<Save.DataFormat.SCS_Float>();
                    break;
            }

            UpdateTruckPanelProgressBar(bi);
            CheckTruckRepair();
        }
        //
        public void buttonElRepair_EnabledChanged(object sender, EventArgs e) {
            Button tmp = sender as Button;

            if (tmp.Enabled)
                tmp.BackgroundImage = RepairImg;
            else
                tmp.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(RepairImg);
        }

        public void buttonRefuel_EnabledChanged(object sender, EventArgs e) {
            Button tmp = sender as Button;

            if (tmp.Enabled)
                tmp.BackgroundImage = RefuelImg;
            else
                tmp.BackgroundImage = Graphics_TSSET.ConvertBitmapToGrayscale(RefuelImg);
        }
        //
        private void buttonUserTruckSelectCurrent_Click(object sender, EventArgs e) {
            comboBoxUserTruckCompanyTrucks.SelectedValue = SiiNunitData.Player.assigned_truck;
        }

        private void buttonUserTruckSwitchCurrent_Click(object sender, EventArgs e) {
            var SelectedItem = ((DataRowView)comboBoxUserTruckCompanyTrucks.SelectedItem).Row;

            SiiNunitData.Player.assigned_truck = SelectedItem[0].ToString(); // Truck link

            if (SiiNunitData.Player_Job != null) {
                if ((string)SelectedItem[1] == "Q") // check Truck type
                {
                    SiiNunitData.NamelessIgnoreList.Remove(SiiNunitData.Player.assigned_truck);

                    SiiNunitData.Player_Job.company_truck = SiiNunitData.Player.assigned_truck;
                } else {
                    if (SiiNunitData.Player_Job.company_truck != "null")
                        SiiNunitData.NamelessIgnoreList.Add(SiiNunitData.Player_Job.company_truck);

                    SiiNunitData.Player_Job.company_truck = "null";
                }

            }
        }

        private void buttonUserTruckVehicleEditor_Click(object sender, EventArgs e) {
            UserTruckDictionary.TryGetValue(comboBoxUserTruckCompanyTrucks.SelectedValue.ToString(), out UserCompanyTruckData SelectedUserCompanyTruck);

            if (SelectedUserCompanyTruck == null)
                return;

            Dictionary<string, dynamic> partsDict = new Dictionary<string, dynamic>();

            foreach (string acc in SelectedUserCompanyTruck.TruckMainData.accessories) {
                partsDict.Add(acc, SiiNunitData.SiiNitems[acc]);
            }

            FormVehicleEditor frm = new FormVehicleEditor(SelectedUserCompanyTruck.TruckMainData, partsDict);
            frm.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = frm.ShowDialog(this);

            if (dr == DialogResult.OK) {
                List<string> newAccList = new List<string>();

                foreach (KeyValuePair<string, dynamic> item in frm.Accessories) {
                    newAccList.Add(item.Key);
                }

                //Remove acc link
                List<string> removeAcc = new List<string>();

                removeAcc = SelectedUserCompanyTruck.TruckMainData.accessories.Except(newAccList).ToList();

                SiiNunitData.NamelessIgnoreList.AddRange(removeAcc);

                foreach (string acc in removeAcc) {
                    SelectedUserCompanyTruck.TruckMainData.accessories.Remove(acc);
                }

                //Add Acc
                List<string> addAcc = new List<string>();

                addAcc = newAccList.Except(SelectedUserCompanyTruck.TruckMainData.accessories).ToList();

                foreach (string acc in addAcc) {
                    SiiNunitData.SiiNitems.Add(acc, frm.Accessories[acc]);
                }

                SiiNunitData.NamelessControlList.AddRange(addAcc);

                SelectedUserCompanyTruck.TruckMainData.accessories.AddRange(addAcc);
            }
        }
        //
        //Share buttons
        private void buttonTruckPaintCopy_Click(object sender, EventArgs e) {
            string tempPaint = "TruckPaint\r\n";

            List<string> paintstr = new List<string>();
            //List<string> paintstr = UserTruckDictionary[comboBoxUserTruckCompanyTrucks.SelectedValue.ToString()].Parts.Find(xp => xp.PartType == "paintjob").PartData;

            foreach (string temp in paintstr) {
                tempPaint += temp + "\r\n";
            }

            string tmpString = BitConverter.ToString(Utilities.ZipDataUtilities.zipText(tempPaint)).Replace("-", "");
            Clipboard.SetText(tmpString);
            MessageBox.Show("Paint data has been copied.");
        }

        private void buttonTruckPaintPaste_Click(object sender, EventArgs e) {
            try {
                string inputData = Utilities.ZipDataUtilities.unzipText(Clipboard.GetText());
                string[] Lines = inputData.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                if (Lines[0] == "TruckPaint") {
                    List<string> paintstr = new List<string>();
                    for (int i = 1; i < Lines.Length; i++) {
                        paintstr.Add(Lines[i]);
                    }

                    //UserTruckDictionary[comboBoxUserTruckCompanyTrucks.SelectedValue.ToString()].Parts.Find(xp => xp.PartType == "paintjob").PartData = paintstr;

                    MessageBox.Show("Paint data  has been inserted.");
                } else
                    MessageBox.Show("Wrong data. Expected Paint data but\r\n" + Lines[0] + "\r\nwas found.");
            } catch {
                MessageBox.Show("Something gone wrong with decoding.");
            }
        }
        //end Share buttons
        //end User Trucks tab
    }
}