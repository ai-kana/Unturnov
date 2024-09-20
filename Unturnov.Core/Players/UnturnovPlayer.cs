using SDG.Unturned;
using Steamworks;

namespace Unturnov.Core.Players;

public class UnturnovPlayer : IPlayer
{
    public SteamPlayer SteamPlayer {get; private set;}
    public Player Player => SteamPlayer.player;

    public string Name => SteamPlayer.playerID.characterName;
    public string LogName => $"{Name} ({SteamID})";
    public CSteamID SteamID => SteamPlayer.playerID.steamID;

    public void SendMessage(object message)
    {
    }

    public void SendMessage(string format, params object[] args)
    {
    }

    public UnturnovPlayer(SteamPlayer player)
    {
        SteamPlayer = player;
    }
}
