﻿using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundReports
{
    public class MVPStats : IReportStat
    {
        [Translation(nameof(Translation.MVPTitle))]
        public string Title => "MVPs";
        public int Order => 51;
        [Translation(nameof(Translation.HumanMVP))]
        public Player HumanMVP { get; set; }

        [Translation(nameof(Translation.SCPMVP))]
        public Player SCPMVP { get; set; }
        [Translation(nameof(Translation.Players))]
        public Dictionary<Player, int> Players { get; set; }
        public Dictionary<Player, int> SCPPlayers { get; set; }
    }
}
