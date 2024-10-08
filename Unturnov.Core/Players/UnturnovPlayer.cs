using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    public bool IsFrozen {get; set;} = false;
    
    public bool IsMarkedPlaced => Quests.isMarkerPlaced;
    
    private float TeleportRotation => Player.transform.rotation.eulerAngles.y;
    private Vector3 MarkerPosition => Quests.markerPosition;

    public CSteamID? LatestPrivateMessagePlayerSteamID {get; set;} = null;
    
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
    
    public void Freeze()
    {
        IsFrozen = true;
        Movement.sendPluginGravityMultiplier(0);
        Movement.sendPluginSpeedMultiplier(0);
        Movement.sendPluginJumpMultiplier(0);
        
        //To be tested
        Player.animator.captorStrength = 999;
        Player.animator.captorID = new CSteamID(0);
        Player.animator.captorItem = 1195;
        Player.animator.sendGesture(EPlayerGesture.ARREST_START, false);
    }
    
    public void Unfreeze()
    {
        IsFrozen = false;
        Movement.sendPluginGravityMultiplier(1);
        Movement.sendPluginSpeedMultiplier(1);
        Movement.sendPluginJumpMultiplier(1);
        
        //Same as above.
        Player.animator.captorStrength = 0;
        Player.animator.captorID = new CSteamID(0);
        Player.animator.captorItem = 0;
        Player.animator.sendGesture(EPlayerGesture.ARREST_STOP, false);
    }
    
    public void Exit()
    {
        //@0x5bc2 - Made this a separate method, so in case Kick gets some sort of logs, Exit kicks dont get logged.
        Provider.kick(SteamID, "You exited the server");
    }

    public void Teleport(Vector3 position)
    {
        position.y += 0.5f;
        Player.teleportToLocation(position, TeleportRotation);
    }
    
    public void TeleportToPlayer(UnturnovPlayer target)
    {
        Teleport(target.Position);
    }

    public void TeleportHere(UnturnovPlayer target)
    {
        target.Player.teleportToLocation(Position, TeleportRotation);
    }
    
    public void TeleportToWaypoint()
    {
        Player.teleportToLocation(MarkerPosition, TeleportRotation);
    }

    public void Teleport(float x, float y, float z)
    {
        Teleport(new Vector3(x, y, z));
    }
    
    public bool Teleport(string location, out string nodeName)
    {
        //Perhaps move this to a separate class or something?
        IReadOnlyList<LocationDevkitNode> allNodes = LocationDevkitNodeSystem.Get().GetAllNodes().ToList();
        
        if(!allNodes.Any())
        {
            nodeName = " ";
            return false;
        }
        
        foreach (var locationDevkitNode in allNodes)
        {
            _Logger.LogInformation("Node: " + locationDevkitNode.locationName);
        }
        
        LocationDevkitNode? node = allNodes.First(n => n.locationName.Contains(location, StringComparison.OrdinalIgnoreCase));

        if (node == null)
        {
            nodeName = " ";
            return false;
        }
        
        Teleport(node.transform.position);
        nodeName = node.locationName;
        return true;
    }
    
    public void AddWarning(string reason, CSteamID staff)
    {
        //warning should have: id, issuer, offender, type, reason, issued, duration
        //maybe implement some sort of auto-ban system on specific amount of warnings
        //@0x5bc2 - I'm leaving this blank as I'm not sure how'd you'd like to handle the data! :cat_thumbsup:
    }

    public bool HasWarning(int id)
    {
        //@0x5bc2 - leaving this blank for same reason as above.
        return true;
    }
    
    public void RemoveWarning(int id, CSteamID staff)
    {
        //@0x5bc2 - leaving this blank for same reason as above.
    }
    
    //There should be a method to get all warnings, but it's not implemented yet.
}
