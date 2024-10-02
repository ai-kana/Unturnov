using Microsoft.Extensions.Logging;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Formatting;
using Unturnov.Core.Players;
using Unturnov.Core.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Unturnov.Core.Chat;

public class UnturnovChat
{
    private static readonly ILogger _Logger;

    static UnturnovChat()
    {
        _Logger = ServiceProvider.CreateLogger<UnturnovChat>();
    }

    public static void BroadcastMessage(UnturnovPlayer player, string message, params object[] args)
    {
        Broadcast(player, message, args);
    }

    public static void BroadcastMessage(IEnumerable<UnturnovPlayer> players, string message, params object[] args)
    {
        Broadcast(players, message, args);
    }

    public static void BroadcastMessage(string message, params object[] args)
    {
        IEnumerable<UnturnovPlayer> players = UnturnovPlayerManager.Players.Values;
        Broadcast(players, message, args);
    }

    private static void Broadcast(IEnumerable<UnturnovPlayer> players, string message, params object[] args)
    {
        foreach (UnturnovPlayer player in players)
        {
            Broadcast(player, message, args);
        }
    }

    private static void Broadcast(UnturnovPlayer player, string message, params object[] args)
    {
        _Logger.LogInformation($"{Formatter.FormatNoColor(message, args)}");
        ChatManager.serverSendMessage(Formatter.Format(message, args), Color.white, null, player.SteamPlayer, EChatMode.GLOBAL, Formatter.ChatImageUrl, true);
    }
}
