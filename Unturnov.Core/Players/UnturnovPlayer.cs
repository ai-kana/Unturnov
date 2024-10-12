using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using Unturnov.Core.Chat;
using Unturnov.Core.Logging;
using Unturnov.Core.Players.Components;
using Unturnov.Core.Roles;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Players;

public class UnturnovPlayer : IPlayer, IFormattable
{
    public SteamPlayer SteamPlayer {get; private set;}
    public Player Player => SteamPlayer.player;
    
    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public PlayerData SaveData {get; private set;}

    private readonly ILogger _Logger;

    public readonly UnturnovPlayerMovement Movement; 
    public readonly UnturnovPlayerRoles Roles; 
    public readonly UnturnovPlayerPermissions Permissions; 
    public readonly UnturnovPlayerLife Life; 
    public readonly UnturnovPlayerQuests Quests;
    public readonly UnturnovPlayerCooldowns Cooldowns;
    public readonly UnturnovPlayerSkills Skills;
    public readonly UnturnovPlayerInventory Inventory;
    public readonly UnturnovPlayerClothing Clothing;
    public readonly UnturnovPlayerModeration Moderation;
    public readonly UnturnovPlayerAdministration Administration;

    public CSteamID? LastPrivateMessage {get; set;} = null;

    public string Language => SaveData.Language;

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
        Inventory = new(this);
        Clothing = new(this);
        Moderation = new(this);
        Administration = new(this);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Name;
    }

    public void Exit()
    {
        //@0x5bc2 - Made this a separate method, so in case Kick gets some sort of logs, Exit kicks dont get logged.
        Provider.kick(SteamID, "You exited the server");
    }
    
    public void Spy(CSteamID caller)
    {
        Player.sendScreenshot(caller, null);
    }

    public void SendMessage(string format, params object[] args)
    {
        UnturnovChat.BroadcastMessage(this, format, args);
    }

    public void SendMessage(Translation translation, params object[] args)
    {
        UnturnovChat.BroadcastMessage(translation.Translate(Language, args));
    }
}
