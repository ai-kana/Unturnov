using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
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
    public static ConcurrentDictionary<CSteamID, UnturnovPlayer> Players {get; private set;}

    public static event PlayerConnected? OnPlayerConnected;
    public static event PlayerDisconnected? OnPlayerDisconnected;

    private static readonly ILogger _Logger;

    static UnturnovPlayerManager()
    {
        _Logger = LoggerProvider.CreateLogger<UnturnovPlayerManager>();
        Players = new();

        Provider.onServerConnected += OnServerConnected;
        Provider.onServerDisconnected += OnServerDisconnected;

        // God mode
        DamageTool.damagePlayerRequested += OnDamageRequested;
        PlayerLife.OnTellHealth_Global += GodModeHandler;
        PlayerLife.OnTellFood_Global += GodModeHandler;
        PlayerLife.OnTellWater_Global += GodModeHandler;
        PlayerLife.OnTellVirus_Global += GodModeHandler;
        PlayerLife.OnTellBroken_Global += GodModeHandler;
        PlayerLife.OnTellBleeding_Global += GodModeHandler;

        _Logger.LogInformation("Created manager");
    }

    private static void GodModeHandler(PlayerLife life)
    {
        // Do this with a patch later
        TryGetPlayer(life.player, out UnturnovPlayer player);
        if (player.Life.GodMode)
        {
            life.sendRevive();
        }
    }

    private static void OnDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
    {
        TryGetPlayer(parameters.player, out UnturnovPlayer player);
        shouldAllow = !player?.Life.GodMode ?? true;
    }

    public static void KickAll(string reason)
    {
        while (Players.Count > 0)
        {
            Players.TryRemove(Players.Keys.First(), out UnturnovPlayer player);
            player.Kick(reason);
        }
    }

    public static bool TryGetPlayer(Player inPlayer, out UnturnovPlayer player)
    {
        return Players.TryGetValue(inPlayer.channel.owner.playerID.steamID, out player);
    }

    public static bool TryGetPlayer(SteamPlayer inPlayer, out UnturnovPlayer player)
    {
        return Players.TryGetValue(inPlayer.playerID.steamID, out player);
    }

    public static bool TryGetPlayer(CSteamID id, out UnturnovPlayer player)
    {
        return Players.TryGetValue(id, out player);
    }
    
    public static bool IsOnline(CSteamID steamID)
    {
        return Players.TryGetValue(steamID, out _);
    }

    public static bool TryFindPlayer(string search, out UnturnovPlayer player)
    {
        if (PlayerTool.tryGetSteamID(search, out CSteamID steamID))
        {
            return UnturnovPlayerManager.Players.TryGetValue(steamID, out player);
        }

        player = UnturnovPlayerManager.Players.Values.FirstOrDefault(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        if (player == null)
        {
            return false;
        }

        return true;
    }

    private static async void OnServerConnected(CSteamID steamID)
    {
        SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID.steamID == steamID);
        UnturnovPlayer player = await UnturnovPlayer.CreateAsync(steamPlayer);
        Players.TryAdd(player.SteamID, player);

        UnturnovChat.BroadcastMessage("{0} has joined the server", player.Name);
        _Logger.LogInformation($"{player.LogName} has joined the server");
        OnPlayerConnected?.BeginInvoke(player, null, null);
    }

    private static async void OnServerDisconnected(CSteamID steamID)
    {
        Players.Remove(steamID, out UnturnovPlayer player);
        await PlayerDataManager.SaveDataAsync(player);

        OnPlayerDisconnected?.Invoke(player);

        UnturnovChat.BroadcastMessage("{0} has left the server", player.Name);
        _Logger.LogInformation($"{player.LogName} has left the server");
    }
}
