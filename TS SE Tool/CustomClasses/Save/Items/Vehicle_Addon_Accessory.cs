﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TS_SE_Tool.Utilities;
using TS_SE_Tool.Save.DataFormat;

namespace TS_SE_Tool.Save.Items
{
    class Vehicle_Addon_Accessory
    {
        internal List<string> slot_name { get; set; } = new List<string>();
        internal List<string> slot_hookup { get; set; } = new List<string>();

        internal string data_path { get; set; } = "";
        internal uint refund { get; set; } = 0;


        internal Vehicle_Addon_Accessory()
        { }

        internal Vehicle_Addon_Accessory(string[] _input)
        {
            string tagLine = "", dataLine = "";

            foreach (string currentLine in _input)
            {
                if (currentLine.Contains(':'))
                {
                    string[] splittedLine = currentLine.Split(new char[] { ':' }, 2);

                    tagLine = splittedLine[0].Trim();
                    dataLine = splittedLine[1].Trim();
                }
                else
                {
                    tagLine = currentLine.Trim();
                    dataLine = "";
                }

                try
                {
                    switch (tagLine)
                    {
                        case "":
                            {
                                break;
                            }

                        case "slot_name":
                            {
                                slot_name.Capacity = int.Parse(dataLine);
                                break;
                            }

                        case var s when s.StartsWith("slot_name["):
                            {
                                slot_name.Add(dataLine);
                                break;
                            }

                        case "slot_hookup":
                            {
                                slot_hookup.Capacity = int.Parse(dataLine);
                                break;
                            }

                        case var s when s.StartsWith("slot_hookup["):
                            {
                                slot_hookup.Add(dataLine);
                                break;
                            }

                        case "data_path":
                            {
                                data_path = dataLine;
                                break;
                            }

                        case "refund":
                            {
                                refund = uint.Parse(dataLine);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.IO_Utilities.ErrorLogWriter(ex.Message + Environment.NewLine + this.GetType().Name.ToLower() + " | " + tagLine + " = " + dataLine);
                    break;
                }
            }
        }

        internal string PrintOut(uint _version, string _nameless)
        {
            string returnString = "";

            StringBuilder returnSB = new StringBuilder();

            returnSB.AppendLine("vehicle_addon_accessory : " + _nameless + " {");

            returnSB.AppendLine(" slot_name: " + slot_name.ToString());
            returnSB.AppendLine(" slot_hookup: " + slot_hookup.ToString());

            returnSB.AppendLine(" data_path: " + data_path);
            returnSB.AppendLine(" refund: " + refund.ToString());

            returnSB.AppendLine("}");

            returnString = returnSB.ToString();

            return returnString;
        }
    }
}