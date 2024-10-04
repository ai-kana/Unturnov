using Microsoft.Extensions.Logging;
using SDG.Unturned;
using Steamworks;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Players;

public class ConsolePlayer : IPlayer
{
    public string Name => "Console";
    public string LogName => "Console";
    public CSteamID SteamID => Provider.server;

    private readonly ILogger _Logger;

    public void SendMessage(string format, params object[] args)
    {
        _Logger.LogInformation(format, args);
    }

    public ConsolePlayer()
    {
        _Logger = LoggerProvider.CreateLogger<ConsolePlayer>();
    }
}
