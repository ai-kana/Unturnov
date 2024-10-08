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
    
    public PlayerSkills Skills => Player.skills;
    public PlayerQuests Quests => Player.quests;
    public PlayerLife Life => Player.life;
    public PlayerMovement Movement => Player.movement;

    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public Vector3 Position => Player.transform.position;

    public PlayerData SaveData {get; private set;}

    public HashSet<string> Permissions => SaveData.Permissions;
    public HashSet<string> Roles => SaveData.Roles;

    public bool OnDuty {get; set;} = false;
    public bool GodMode {get; set;} = false; //@0x5bc2 - Does Nothing For Now. I need to ask you how do you wanna handle it lol.

    private readonly ILogger _Logger;

    public static async UniTask<UnturnovPlayer> CreateAsync(SteamPlayer player)
    {
        return new(player, await PlayerDataManager.LoadDataAsync(player.playerID.steamID));
    }

    private UnturnovPlayer(SteamPlayer player, PlayerData data)
    {
        SaveData = data;
        SteamPlayer = player;
        _Logger = LoggerProvider.CreateLogger($"{typeof(UnturnovPlayer).FullName}.{Name}");
    }

    public long GetCooldown(string id)
    {
        long now = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (!SaveData.Cooldowns.ContainsKey(id))
        {
            return 0;
        }

        long remaining = SaveData.Cooldowns[id] - now;
        if (remaining < 0)
        {
            SaveData.Cooldowns.Remove(id);
            return 0;
        }

        return remaining;
    }

    public void AddCooldown(string id, long length)
    {
        long end = DateTimeOffset.Now.ToUnixTimeSeconds() + length;
        if (SaveData.Cooldowns.ContainsKey(id))
        {
            SaveData.Cooldowns[id] = end;
            return;
        }

        SaveData.Cooldowns.Add(id, end);
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
        if (Permissions.Contains("all"))
        {
            return true;
        }

        if (Permissions.Contains(permission.ToLower()))
        {
            return true;
        }

        HashSet<Role> roles = RoleManager.GetRoles(Roles);
        foreach (Role role in roles)
        {
            if (role.Permissions.Contains("all"))
            {
                return true;
            }

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

    public void Heal()
    {
        Life.askHeal(100, true, true);
        Life.askEat(100);
        Life.askDrink(100);
        Life.askRest(100);
        Life.askDisinfect(100);
        Life.askBreath(100);
    }
    
    public void SetGod(bool state)
    { 
        if(state)
            Heal();
        GodMode = state;
    }
}
