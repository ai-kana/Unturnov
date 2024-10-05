using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Unturnov.Core.Chat;
using Unturnov.Core.Permissions;

namespace Unturnov.Core.Players;

public class UnturnovPlayer : IPlayer, IFormattable
{
    public SteamPlayer SteamPlayer {get; private set;}
    public Player Player => SteamPlayer.player;

    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public Vector3 Position => Player.transform.position;

    public HashSet<string> Permissions {get; private set;}

    public static async UniTask<UnturnovPlayer> Create(SteamPlayer player)
    {
        return new(player, await PermissionManager.LoadPermissions(player.playerID.steamID));
    }

    private UnturnovPlayer(SteamPlayer player, HashSet<string> permissions)
    {
        SteamPlayer = player;
        Permissions = permissions;
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
        return Permissions.Contains(permission.ToLower());
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
