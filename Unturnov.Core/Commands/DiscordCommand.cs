using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("discord")]
public class DiscordCommand : Command
{
    public DiscordCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPlayer(out UnturnovPlayer caller);

        string? link = UnturnovHost.Configuration.GetValue<string>("DiscordInviteLink");
        if (link == null)
        {
            throw Context.Reply("Discord Link has not been setup!");
        }

        caller.Player.sendBrowserRequest("Click to join our discord!", link);

        throw Context.Exit;
    }
}
