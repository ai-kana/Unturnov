using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using Steamworks;
using Unturnov.Core.Chat;
using Unturnov.Core.Extensions;
using Unturnov.Core.Formatting;
using Unturnov.Core.Logging;
using Unturnov.Core.Offenses;
using Unturnov.Core.Translations;

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

    private static IEnumerable<UnturnovPlayer> GetPlayerListCopy()
    {
        foreach (UnturnovPlayer player in Players.Values)
        {
            yield return player;
        }
    }

    public static void KickAll(string reason)
    {
        foreach (UnturnovPlayer player in GetPlayerListCopy())
        {
            player.Kick(reason);
        }
    
        while (Players.Count != 0);
    }

    public static void KickAll(Translation translation, params object[] args)
    {
        foreach (UnturnovPlayer player in GetPlayerListCopy())
        {
            player.Kick(translation, args);
        }
    
        while (Players.Count != 0);
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

    private static async UniTask OnConnected(CSteamID steamID)
    {
        SteamPlayer steamPlayer = Provider.clients.Find(x => x.playerID.steamID == steamID);
        UnturnovPlayer player = await UnturnovPlayer.CreateAsync(steamPlayer);
        Players.TryAdd(player.SteamID, player);

        IEnumerable<Offense> offenses = await PlayerIdManager.GetOffenses(player);
        Offense? permBan = offenses.FirstOrDefault(x => x.OffenseType == OffenseType.Ban && x.Duration == 0 && !x.Pardoned);

        string discord = UnturnovHost.Configuration.GetValue<string>("DiscordInviteLink") ?? "Failed to get discord link";

        if (permBan != null)
        {
            player.Kick(TranslationList.BanPermanent, permBan.Reason, discord);
            return;
        }

        long now = DateTimeOffset.Now.ToUnixTimeSeconds();
        Offense? nonPermBan = 
            offenses.Where(x => x.OffenseType == OffenseType.Ban && x.Duration > 0 && !x.Pardoned)
            .FirstOrDefault(x => (x.Duration + x.Issued) > now);

        if (nonPermBan != null)
        {
            long time = (nonPermBan.Issued + nonPermBan.Duration) - now;
            player.Kick(TranslationList.BanTemporary, nonPermBan.Reason, Formatter.FormatTime(time), discord);
            return;
        }

        UnturnovChat.BroadcastMessage(TranslationList.PlayerConnected, player.Name);
        _Logger.LogInformation($"{player.LogName} has joined the server");
        OnPlayerConnected?.Invoke(player);
    }

    private static async void OnServerConnected(CSteamID steamID)
    {
        try
        {
            await OnConnected(steamID);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Exception while player connection; Kicking...");
            string discord = UnturnovHost.Configuration.GetValue<string>("DiscordInviteLink") ?? "Failed to get discord link";
            Provider.kick(steamID, $"Something failed while connecting; Please contact staff; {discord ?? "Failed to get link :C"}");
        }
    }

    private static async void OnServerDisconnected(CSteamID steamID)
    {
        Players.TryRemove(steamID, out UnturnovPlayer player);
        await PlayerDataManager.SaveDataAsync(player);

        OnPlayerDisconnected?.Invoke(player);

        UnturnovChat.BroadcastMessage(TranslationList.PlayerDisconnected, player.Name);
        _Logger.LogInformation($"{player.LogName} has left the server");
    }
}
