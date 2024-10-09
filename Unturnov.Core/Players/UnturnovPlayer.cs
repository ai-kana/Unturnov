using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using Unturnov.Core.Chat;
using Unturnov.Core.Logging;
using Unturnov.Core.Players.Components;
using Unturnov.Core.Roles;

namespace Unturnov.Core.Players;

public class UnturnovPlayer : IPlayer, IFormattable
{
    public SteamPlayer SteamPlayer {get; private set;}
    public Player Player => SteamPlayer.player;
    
    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public PlayerData SaveData {get; private set;}

    public bool OnDuty {get; set;} = false;
    
    private readonly ILogger _Logger;

    public readonly UnturnovPlayerMovement Movement; 
    public readonly UnturnovPlayerRoles Roles; 
    public readonly UnturnovPlayerPermissions Permissions; 
    public readonly UnturnovPlayerLife Life; 
    public readonly UnturnovPlayerQuests Quests;
    public readonly UnturnovPlayerCooldowns Cooldowns;
    public readonly UnturnovPlayerSkills Skills;

    public CSteamID? LastPrivateMessage {get; set;} = null;

    public static async UniTask<UnturnovPlayer> CreateAsync(SteamPlayer player)
    {
        return new(player, await PlayerDataManager.LoadDataAsync(player.playerID.steamID));
    }

    private UnturnovPlayer(SteamPlayer player, PlayerData data)
    {
        SaveData = data;
        SteamPlayer = player;
        _Logger = LoggerProvider.CreateLogger($"{typeof(UnturnovPlayer).FullName}.{Name}");

        Movement = new(this);
        Roles = new(this);
        Permissions = new(this);
        Life = new(this);
        Quests = new(this);
        Cooldowns = new(this);
        Skills = new(this);
    }

    public void SendMessage(string format, params object[] args)
    {
        UnturnovChat.BroadcastMessage(this, format, args);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Name;
    }

    public void Kick()
    {
        Provider.kick(SteamID, "No reason provided");
    }

    public void Kick(string reason)
    {
        Provider.kick(SteamID, reason);
    }

    public void Exit()
    {
        //@0x5bc2 - Made this a separate method, so in case Kick gets some sort of logs, Exit kicks dont get logged.
        Provider.kick(SteamID, "You exited the server");
    }
}
