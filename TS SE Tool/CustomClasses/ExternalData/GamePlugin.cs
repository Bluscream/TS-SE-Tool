﻿/*
   Copyright 2016-2020 LIPtoH <liptoh.codebase@gmail.com>

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
using System.IO;
using TS_SE_Tool.Utilities;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TS_SE_Tool {
    public class GamePlugin {
        public bool Enabled {
            get => !File32bit.IsDisabled() && !File64bit.IsDisabled();
            set {
                if (x86) File32bit.Toggle(value);
                if (x64) File64bit.Toggle(value);
            }
        }
        public string Name { get => File32bit?.FileNameWithoutExtension().RemoveAll("_win32", "_x86") ?? File64bit?.FileNameWithoutExtension().RemoveAll("_win64", "_x64"); }
        public DateTime? InstallDate {
            get {
                if (x86) return File32bit.LastWriteTime;
                else if (x64) return File64bit.LastWriteTime;
                return null;
            }
        }

        public bool x86 { get => File32bit != null && File32bit.Exists; }
        public bool x64 { get => File64bit != null && File64bit.Exists; }

        [JsonIgnore]
        [Browsable(false)]
        public FileInfo File32bit { get; set; }

        [Browsable(false)]
        public string Path32bit { get => File32bit?.FullName; }

        [JsonIgnore]
        [Browsable(false)]
        public FileInfo File64bit { get; set; }

        [Browsable(false)]
        public string Path64bit { get => File64bit?.FullName; }

        [JsonIgnore]
        [Browsable(false)]
        public List<FileInfo> Files {
            get {
                var list = new List<FileInfo>();
                if (x86) list.Add(File32bit);
                if (x64) list.Add(File64bit);
                return list;
            }
        }

        public bool Delete() {
            try {
                if (x86) File32bit.Delete();
            } catch (Exception ex) {
                IO_Utilities.LogWriter($"Failed to delete {File32bit.FullString()}: {ex}");
                return false;
            }
            try {
                if (x64) File64bit.Delete();
            } catch (Exception ex) {
                IO_Utilities.LogWriter($"Failed to delete {File64bit.FullString()}: {ex}");
                return false;
            }
            return true;
        }
        //public string ToJson() {
        //    try {
        //        var json = JsonSerializer.Serialize(new {
        //            Enabled,
        //            Name,
        //            File32bit = File32bit.FullName,
        //            InstallDate32bit = File32bit.LastWriteTime,
        //            File64bit = File64bit.FullName,
        //            InstallDate64bit = File64bit.LastWriteTime
        //        });

        //        return json;
        //    } catch (Exception ex) {
        //        Console.WriteLine($"Error serializing GamePlugin object: {ex.Message}");
        //        return "{}";
        //    }
        //}
    }
    public static class GamePluginExtensions {
        public static IEnumerable<GamePlugin> AddSafe(this IEnumerable<GamePlugin> plugins, GamePlugin plugin) {

        }
    }
}
