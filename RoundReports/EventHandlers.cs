﻿using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthwoodLib.Pools;
using Exiled.API.Enums;

namespace RoundReports
{
    public class EventHandlers
    {
        public List<IReportStat> Holding { get; set; }

        public void Hold<T>(T stat)
            where T: class, IReportStat
        {
            Holding.RemoveAll(r => r.GetType() == typeof(T));
            Holding.Add(stat);
        }

        public bool TryGetStat<T>(out T value)
            where T: class, IReportStat
        {
            value = Holding.FirstOrDefault(r => r.GetType() == typeof(T)) as T;
            return value != null;
        }

        public T GetStat<T>()
            where T: class, IReportStat, new()
        {
            var value = Holding.FirstOrDefault(r => r.GetType() == typeof(T)) as T;
            if (value is null)
            {
                value = new T();
            }
            return value;
        }

        public void OnWaitingForPlayers()
        {
            Holding = ListPool<IReportStat>.Shared.Rent();
            MainPlugin.Reporter = new Reporter(MainPlugin.Singleton.Config.DiscordWebhook);
        }

        public void OnRestarting()
        {
            foreach (var stat in Holding)
            {
                MainPlugin.Reporter.SetStat(stat);
            }
            if (MainPlugin.Reporter is not null && !MainPlugin.Reporter.HasSent)
            {
                MainPlugin.Reporter.SendReport();
            }
            ListPool<IReportStat>.Shared.Return(Holding);
        }

        public void OnRoundStarted()
        {
            Timing.CallDelayed(.5f, () =>
            {
                StartingStats stats = new()
                {
                    ClassDPersonnel = Player.Get(RoleType.ClassD).Count(),
                    SCPs = Player.Get(Team.SCP).Count(),
                    FacilityGuards = Player.Get(RoleType.FacilityGuard).Count(),
                    Scientists = Player.Get(RoleType.Scientist).Count(),
                    StartTime = DateTime.Now,
                    PlayersAtStart = Player.List.Where(r => !r.IsDead).Count(),
                    Players = new()
                };
                foreach (var player in Player.List)
                {
                    if (player.DoNotTrack)
                    {
                        stats.Players.Add($"{Reporter.DoNotTrackText} [{player.Role}]");
                    }
                    else
                    {
                        stats.Players.Add($"{player.Nickname} [{player.Role}]");
                    }
                }
                Hold(stats);
            });
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            var stats = GetStat<FinalStats>();
            stats.EndTime = DateTime.Now;
            stats.WinningTeam = ev.LeadingTeam switch
            {
                LeadingTeam.Anomalies => "SCPs",
                LeadingTeam.ChaosInsurgency => "Insurgency",
                LeadingTeam.FacilityForces => "Mobile Task Force",
                LeadingTeam.Draw => "Stalemate",
                _ => "Unknown"
            };
            stats.RoundTime = Round.ElapsedTime;
            Hold(stats);
        }

        public void OnLeft(LeftEventArgs ev)
        {
            if (!Reporter.NameStore.ContainsKey(ev.Player) && !ev.Player.DoNotTrack)
            {
                Reporter.NameStore.Add(ev.Player, ev.Player.Nickname);
            }
        }

        public void OnDied(DiedEventArgs ev)
        {
            var stats = GetStat<FinalStats>();
            var killStats = GetStat<OrganizedKillsStats>();
            stats.TotalDeaths++;
            if (ev.Killer is not null)
            {
                // Kill logs
                killStats.PlayerKills.Insert(0, $"[{DateTime.Now.ToString("hh:mm:ss tt")}] {(ev.Killer.DoNotTrack ? Reporter.DoNotTrackText : ev.Killer.Nickname)} killed {(ev.Target.DoNotTrack ? Reporter.DoNotTrackText : ev.Target.Nickname)}");
                // Kill by player
                if (!killStats.KillsByPlayer.ContainsKey(ev.Killer))
                {
                    killStats.KillsByPlayer.Add(ev.Killer, 1);
                }
                else
                {
                    killStats.KillsByPlayer[ev.Killer]++;
                }
                stats.TotalKills++;
                switch (ev.Killer.Role.Team)
                {
                    case Team.SCP:
                        stats.MTFKills++;
                        break;
                    case Team.CDP:
                        stats.DClassKills++;
                        break;
                    case Team.RSC:
                        stats.ScientistKills++;
                        break;
                    case Team.MTF:
                        stats.MTFKills++;
                        break;
                    case Team.CHI:
                        stats.ChaosKills++;
                        break;
                }
            }
            // Kill by type
            if (!killStats.KillsByType.ContainsKey(ev.Handler.Type))
            {
                killStats.KillsByType.Add(ev.Handler.Type, 1);
            }
            else
            {
                killStats.KillsByType[ev.Handler.Type]++;
            }
            Hold(killStats);
            Hold(stats);
        }

        public void OnUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.IsScp)
            {
                if (!TryGetStat<SCPItemStats>(out SCPItemStats stats))
                {
                    stats = new();
                }
                switch (ev.Item.Type)
                {
                    case ItemType.SCP207:
                        stats.Scp207Drank++;
                        break;
                    case ItemType.SCP268:
                        stats.Scp268Uses++;
                        break;
                    case ItemType.SCP1853:
                        stats.Scp1853Uses++;
                        break;
                }
                Hold(stats);
            }
            else
            {
                if (!TryGetStat<MedicalStats>(out MedicalStats stats))
                {
                    stats = new();
                }
                switch (ev.Item.Type)
                {
                    case ItemType.Painkillers:
                        stats.PainkillersConsumed++;
                        break;
                    case ItemType.Medkit:
                        stats.MedkitsConsumed++;
                        break;
                    case ItemType.Adrenaline:
                        stats.AdrenalinesConsumed++;
                        break;
                    case ItemType.SCP500:
                        stats.SCP500sConsumed++;
                        break;
                }
                Hold(stats);
            }
        }

        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Player is null) return;
            var stats = GetStat<DoorStats>();
            if (ev.Door.IsOpen)
            {
                stats.DoorsClosed++;
                if (stats.PlayerDoorsClosed.ContainsKey(ev.Player))
                {
                    stats.PlayerDoorsClosed[ev.Player]++;
                }
                else
                {
                    stats.PlayerDoorsClosed.Add(ev.Player, 1);
                }
            }
            else
            {
                stats.DoorsOpened++;
                if (stats.PlayerDoorsOpened.ContainsKey(ev.Player))
                {
                    stats.PlayerDoorsOpened[ev.Player]++;
                }
                else
                {
                    stats.PlayerDoorsOpened.Add(ev.Player, 1);
                }
            }
            Hold(stats);
        }
    }
}
