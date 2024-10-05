using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Unturnov.Core.Chat;
using Unturnov.Core.Formatting;

namespace Unturnov.Core.Players;

public class UnturnovPlayer : IPlayer, IFormattable
{
    public SteamPlayer SteamPlayer {get; private set;}
    public Player Player => SteamPlayer.player;

    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public Vector3 Position => Player.transform.position;

    public UnturnovPlayer(SteamPlayer player)
    {
        SteamPlayer = player;
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
