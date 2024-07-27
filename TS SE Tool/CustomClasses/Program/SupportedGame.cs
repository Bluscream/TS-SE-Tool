﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TS_SE_Tool.Utilities;

namespace TS_SE_Tool.CustomClasses.Program {
    public class SupportedGame {
        public long SteamAppId { get; internal set; }
        public string Type { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string ExecutableName { get; internal set; }

        public List<Version> SupportedGameVersions { get; internal set; } = new();
        public List<long> SupportedSaveFileVersions { get; internal set; } = new();

        public bool Installed {
            get {
                try { var _ = Globals.SteamGameLocator.getGameInfoByFolder(Name).steamGameLocation; return true; } catch { return false; }
            }
        }
        public DirectoryInfo SteamUserDataDir { get => Globals.GetLatestSteamUserDataDir().Combine(SteamAppId.ToString()); }
        public DirectoryInfo SteamRemoteDir { get => SteamUserDataDir.Combine("remote"); }

        public DirectoryInfo GameDir { get => Installed ? new DirectoryInfo(Globals.SteamGameLocator.getGameInfoByFolder(Name).steamGameLocation) : null; }
        public FileInfo Executable { get => Installed ? GameDir.CombineFile("bin", "win_x64", ExecutableName) : null; }
        public List<DirectoryInfo> PluginDirs { get => new() { GameDir.Combine("bin", "win_x64", "plugins"), GameDir.Combine("bin", "win_x86", "plugins") }; }

        public DirectoryInfo DocumentsDir { get => Globals.DocumentsDir.Combine(Name); }
        public DirectoryInfo ModsDir { get => DocumentsDir.Combine("mod"); }

        public List<Process> Processes { get => Installed ? Process.GetProcessesByName(Executable.Name).ToList() : new(); }
        public bool IsRunning { get => Processes.Count > 0; }

        public int LicensePlateWidth { get; internal set; }
        public List<int> PlayerLevelUps { get; internal set; } = new();
        public Dictionary<string, Currency> Currencies { get; internal set; } = new();

        //public SupportedGame(long steamAppId, string type, string name, string description, string supportedGameVersions, List<long> supportedSaveFileVersions) =>
        //    new SupportedGame(steamAppId, type, name, description, supportedGameVersions.ParseVersions(), supportedSaveFileVersions);
        //public SupportedGame(long steamAppId, string type, string name, string description, List<Version> supportedGameVersions, List<long> supportedSaveFileVersions) {
        //    SteamAppId = steamAppId;
        //    Type = type;
        //    Name = name;
        //    Description = description;
        //    SupportedGameVersions = supportedGameVersions;
        //    SupportedSaveFileVersions = supportedSaveFileVersions;
        //}
    }
    public class Currency {
        public string Name { get; set; }
        public double ConversionRate { get; set; }
        public List<string> FormatSymbols { get; set; }

        public Currency(string name, double conversionRate, List<string> formatSymbols) {
            Name = name;
            ConversionRate = conversionRate;
            FormatSymbols = formatSymbols ?? throw new ArgumentNullException(nameof(formatSymbols));
        }
    }

}
