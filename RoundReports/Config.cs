﻿using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EBroadcast = Exiled.API.Features.Broadcast;

namespace RoundReports
{
    public enum StatType
    {
        // Start Stats
        StartTime,
        StartClassD,
        StartScientist,
        StartSCP,
        StartFacilityGuard,
        StartPlayers,

        // MVP Stats
        HumanMVP,
        SCPMVP,
        HumanPoints,
        SCPPoints,
        PointLogs,

        // Final Stats
        WinningTeam,
        EndTime,
        RoundTime,
        TotalDeaths,
        TotalKills,
        SCPKills,
        DClassKills,
        ScientistKills,
        MTFKills,
        ChaosKills,
        SerpentsHandKills,
        UIUKills,
        TutorialKills,
        SurvivingPlayers,

        //-- Warhead
        ButtonUnlocked,
        ButtonUnlocker,
        FirstWarheadActivator,
        Detonated,
        DetonationTime,

        //-- Doors
        DoorsOpened,
        DoorsClosed,
        DoorsDestroyed,
        PlayerDoorsOpened,
        PlayerDoorsClosed,

        // Respawn Stats
        SpawnWaves,
        TotalRespawnedPlayers,
        Respawns,

        // Item Stats
        TotalDrops,
        Drops,
        PlayerDrops,
        KeycardScans,
        PainkillersConsumed,
        MedkitsConsumed,
        AdrenalinesConsumed,
        SCP500sConsumed,

        //-- Firearm
        TotalShotsFired,
        TotalReloads,

        // SCP Stats
        Scp096Charges,
        Scp096Enrages,
        Scp106Teleports,
        Scp173Blinks,
        Scp018sThrown,
        Scp207sDrank,
        Scp268Uses,
        Scp1853Uses,

        //-- SCP-330
        First330Use,
        First330User,
        TotalCandiesTaken,
        SeveredHands,
        CandiesTaken,
        CandiesByPlayer,

        //-- SCP-914
        First914Activation,
        First914Activator,
        Total914Activations,
        TotalItemUpgrades,
        KeycardUpgrades,
        FirearmUpgrades,
        AllActivations,
        AllUpgrades,

        // Kill Stats
        KillsByPlayer,
        KillsByType,
        PlayerKills,

        // Damage Stats
        TotalDamage,
        PlayerDamage,
        DamageByPlayer,
        DamageByType,
    }

    public class Config : IConfig
    {
        [Description("Whether or not the plugin is active.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not to show debug logs.")]
        public bool Debug { get; set; } = false;

        [Description("Your Paste.ee key. Get this from https://paste.ee/account/api after creating a paste.ee account. The plugin cannot function without a valid Pastee key!")]
        public string PasteKey { get; set; } = "";

        [Description("Time until reports expire. Valid values: Never, 1H, 12H, 1D, 3D, 7D, 14D, 1M, 2M, 3M, 6M, 1Y, 2Y")]
        public string ExpiryTime { get; set; } = "1M";

        [Description("Provide a Discord webhook to send reports to.")]
        public string DiscordWebhook { get; set; } = string.Empty;
        [Description("Send report links in server console when compiled?")]
        public bool SendInConsole { get; set; } = true;
        [Description("If set to true, users with Do Not Track enabled will be excluded from all stats entirely. If set to false, they will be included with a removed name (including in round remarks). See the plugin's GitHub page for more information.")]
        public bool ExcludeDNTUsers { get; set; } = false;
        [Description("If set to true, stats from tutorial users will be entirely ignored. Does not affect Serpent's Hand (if installed).")]
        public bool ExcludeTutorials { get; set; } = true;
        [Description("Broadcast(s) to show at the end of the round. A full list of arguments are available on the plugin's GitHub page. Set to [] (or set broadcast's show value to false) to disable. Additional broadcasts can be added and removed.")]
        public List<EBroadcast> EndingBroadcasts { get; set; } = new List<EBroadcast> { new("<b>{HUMANMVP}</b> was the human MVP of this round!", duration: 3), new("<b>{SCPMVP}</b> was the SCP MVP of this round!", duration: 3), new("View the end-of-round report on our Discord server!", duration: 3) };

        [Description("Name of the server, without any formatting tags, as it will be shown in the report.")]
        public string ServerName { get; set; } = string.Empty;

        [Description("Determines the footer text, as shown on the Discord embed. A full list of arguments are available on the plugin's GitHub page.")]
        public string FooterText { get; set; } = "{PLAYERCOUNT} players connected";

        [Description("Determines the format of timestamps.")]
        public string FullTimeFormat { get; set; } = "MMMM dd, yyyy hh:mm:ss tt";
        public string ShortTimeFormat { get; set; } = "HH:mm:ss";
        [Description("Determine which statistics are NOT included in the report. Some stats will only be shown if applicable (eg. Serpent's Hand kills will only show if the server has Serpent's Hand installed).")]
        public List<StatType> IgnoredStats { get; set; } = new();
        [Description("A list of players (by user id) who will be ignored from stats, regardless of their do not track setting. For single-player stats (eg. warhead button opener) and kill logs, they will be shown as a do not track player.")]
        public List<string> IgnoredUsers { get; set; } = new() { "12345@steam" };
    }
}
