using SDG.Unturned;
using Steamworks;
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

    public UnturnovPlayer(SteamPlayer player)
    {
        SteamPlayer = player;
    }

    public void SendMessage(string format, params object[] args)
    {
        UnturnovChat.BroadcastMessage(this, Formatter.Format(format, args));
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return Name;
    }
}
