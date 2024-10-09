using Steamworks;

namespace Unturnov.Core.Players;

public interface IPlayer
{
    public string Name {get;}
    public string LogName {get;}
    public CSteamID SteamID {get;}
    public void SendMessage(string format, params object[] args);
    public string Language {get;}
}
