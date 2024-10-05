using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using Steamworks;
using Unturnov.Core.Chat;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Players;

public delegate void PlayerConnected(UnturnovPlayer player);
public delegate void PlayerDisconnected(UnturnovPlayer player);

public class UnturnovPlayerManager
{
    public static ConcurrentDictionary<CSteamID, UnturnovPlayer> Players;

    public static event PlayerConnected? OnPlayerConnected;
    public static event PlayerDisconnected? OnPlayerDisconnected;

    private static readonly ILogger _Logger;

    static UnturnovPlayerManager()
    {
        _Logger = LoggerProvider.CreateLogger<UnturnovPlayerManager>();
        Players = new();

        Provider.onServerConnected += OnServerConnected;
        Provider.onServerDisconnected += OnServerDisconnected;

        _Logger.LogInformation("Created manager");
    }

    public static void KickAll(string reason)
    {
        foreach (UnturnovPlayer player in Players.Values)
        {
            player.Kick(reason);
        }
    }

    private static void OnServerConnected(CSteamID steamID)
    {
        SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID.steamID == steamID);
        UnturnovPlayer player = new(steamPlayer);
        Players.TryAdd(player.SteamID, player);

        UnturnovChat.BroadcastMessage("{0} has joined the server", player.Name);
        _Logger.LogInformation($"{player.LogName} has joined the server");
        OnPlayerConnected?.BeginInvoke(player, null, null);
    }

    private static void OnServerDisconnected(CSteamID steamID)
    {
        Players.Remove(steamID, out UnturnovPlayer player);
        OnPlayerDisconnected?.Invoke(player);

        UnturnovChat.BroadcastMessage("{0} has left the server", player.Name);
        _Logger.LogInformation($"{player.LogName} has left the server");
    }
}
