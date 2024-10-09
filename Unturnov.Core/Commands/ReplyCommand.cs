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

    public override UniTask ExecuteAsync()
    {
        Context.AssertArguments(1);

        string message = Context.Form();

        Context.AssertPlayer(out UnturnovPlayer self);
        
        if(self.LastPrivateMessage == null) 
        {
            throw Context.Reply("You have no one to reply to");
        }

        if (!UnturnovPlayerManager.TryGetPlayer(self.LastPrivateMessage.Value, out UnturnovPlayer target))
        {
            throw Context.Reply("The player you are trying to reply to is not online");
        }
        
        target.LastPrivateMessage = self.SteamID;
        
        UnturnovChat.SendPrivateMessage(self, target, message);
        throw Context.Exit;
    }
}
