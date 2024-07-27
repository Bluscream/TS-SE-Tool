using System;
using System.Collections.Generic;
using System.IO;
using TS_SE_Tool.Utilities;

namespace TS_SE_Tool.CustomClasses.Program {
    public class SupportedGame {
        public long SteamAppId { get; internal set; }
        public string Type { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }

        public List<Version> SupportedGameVersions { get; internal set; } = new();
        public List<long> SupportedSaveFileVersions { get; internal set; } = new();

        public bool Installed {
            get {
                try { var _ = Globals.SteamGameLocator.getGameInfoByFolder(Name).steamGameID; return true; } catch { return false; }
            }
        }
        public DirectoryInfo GameDir { get => new DirectoryInfo(Globals.SteamGameLocator.getGameInfoByFolder(Name).steamGameLocation); }
        public DirectoryInfo SteamUserDataDir { get => Globals.GetLatestSteamUserDataDir().Combine(SteamAppId.ToString()); }
        public DirectoryInfo DocumentsDir { get => Globals.DocumentsDir.Combine(Name); }
        public DirectoryInfo ModsDir { get => DocumentsDir.Combine("mod"); }

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
