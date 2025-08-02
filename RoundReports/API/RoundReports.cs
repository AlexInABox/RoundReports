using System.Collections.Generic;
using Exiled.API.Features;

namespace RoundReports.API;

public static class RoundReports
{
    public static int GetPointsOfPlayer(int playerId)
    {
        if (!Player.TryGet(playerId, out Player player)) return 0;

        int points = 0;

        if (MainPlugin.Handlers.Points.TryGetValue(PointTeam.Human, out Dictionary<Player, int> dictHuman))
            points += dictHuman.TryGetValue(player, out int pointsHuman) ? pointsHuman : 0;

        if (MainPlugin.Handlers.Points.TryGetValue(PointTeam.None, out Dictionary<Player, int> dictNone))
            points += dictNone.TryGetValue(player, out int pointsNone) ? pointsNone : 0;

        if (MainPlugin.Handlers.Points.TryGetValue(PointTeam.SCP, out Dictionary<Player, int> dictScp))
            points += dictScp.TryGetValue(player, out int pointsScp) ? pointsScp : 0;


        return points;
    }
}