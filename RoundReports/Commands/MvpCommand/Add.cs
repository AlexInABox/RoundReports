﻿using CommandSystem;
using System;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;
using System.Linq;

namespace RoundReports.Commands.MvpCommand
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Add : ICommand, IUsageProvider
    {
        public string Command => "add";

        public string[] Aliases => new[] { "a" };

        public string Description => "Adds points to the provided player.";

        public string[] Usage => new[] { "players", "SCP/Human", "amount", "reason" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("rr.mvp.add"))
            {
                response = "Missing permission: rr.mvp.add";
                return false;
            }

            if (!MainPlugin.Configs.MvpSettings.MvpEnabled)
            {
                response = "MVP system is disabled in server configs.";
                return false;
            }

            if (!Round.InProgress)
            {
                response = "Points can only be added during a round.";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "Invalid command usage. Required parameters: players, scp/human, amount";
                return false;
            }

            string team = arguments.At(1).ToLower();
            if (team is not "scp" && team is not "human")
            {
                response = "Invalid team provided. Team must be 'scp' or 'human'.";
                return false;
            }

            PointTeam pt = team is "scp" ? PointTeam.SCP : PointTeam.Human;

            if (!int.TryParse(arguments.At(2), out int amount))
            {
                response = "Invalid command usage. Amount of points must be an integer.";
                return false;
            }

            Player player = Player.Get(arguments.At(0));

            if (player is null)
            {
                response = "Invalid player provided.";
                return false;
            }

            string reason = string.Join(" ", arguments.Skip(3));
            MainPlugin.Handlers.AddPoints(player, amount, reason, pt);

            response = $"Added {amount} MVP points to {player.Nickname}!";
            return true;
        }
    }
}
