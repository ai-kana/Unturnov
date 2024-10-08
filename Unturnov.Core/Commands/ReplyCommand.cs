using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("reply")]
[CommandSyntax("<[message]>")]
public class ReplyCommand : Command
{
    public ReplyCommand(CommandContext context) : base(context)
    {
    }

    public override async UniTask ExecuteAsync()
    {
        Context.AssertPermission("reply");
        Context.AssertArguments(1);

        string message = Context.Form();

        Context.AssertPlayer(out UnturnovPlayer self);
        
        if(self.LatestPrivateMessagePlayerSteamID == null) 
        {
            throw Context.Reply("You have no one to reply to.");
        }

        if (!UnturnovPlayerManager.IsOnline(self.LatestPrivateMessagePlayerSteamID!.Value, out UnturnovPlayer target))
        {
            throw Context.Reply("The player you are trying to reply to is not online.");
        }
        
        target.LatestPrivateMessagePlayerSteamID = self.SteamID;
        
        UnturnovChat.SendPrivateMessage(self, target, message);
        throw Context.Exit;
    }
}