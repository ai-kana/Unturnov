using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Unturnov.Core.Chat;
using Unturnov.Core.Logging;
using Unturnov.Core.Roles;

namespace Unturnov.Core.Players;

public class UnturnovPlayer : IPlayer, IFormattable
{
    public SteamPlayer SteamPlayer {get; private set;}
    public Player Player => SteamPlayer.player;

    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public Vector3 Position => Player.transform.position;

    public PlayerData SaveData {get; private set;}

    public HashSet<string> Permissions => SaveData.Permissions;
    public HashSet<string> Roles => SaveData.Roles;

    public bool OnDuty {get; set;} = false;

    private readonly ILogger _Logger;

    public static async UniTask<UnturnovPlayer> Create(SteamPlayer player)
    {
        return new(player, await PlayerDataManager.LoadDataAsync(player.playerID.steamID));
    }

    private UnturnovPlayer(SteamPlayer player, PlayerData data)
    {
        SaveData = data;
        SteamPlayer = player;
        _Logger = LoggerProvider.CreateLogger($"{typeof(UnturnovPlayer).FullName}.{Name}");
    }

    public bool AddRole(string id)
    {
        return SaveData.Roles.Add(id);
    }

    public bool RemoveRole(string id)
    {
        return SaveData.Roles.Remove(id);
    }

    public bool HasRole(string id)
    {
        return SaveData.Roles.Contains(id);
    }

    public void AddPermission(string permission)
    {
        Permissions.Add(permission.ToLower());
    }

    public void RemovePermission(string permission)
    {
        Permissions.Remove(permission.ToLower());
    }

    public bool HasPermission(string permission)
    {
        if (Permissions.Contains(permission.ToLower()))
        {
            return true;
        }

        HashSet<Role> roles = RoleManager.GetRoles(Roles);
        foreach (Role role in roles)
        {
            if (role.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
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
}
