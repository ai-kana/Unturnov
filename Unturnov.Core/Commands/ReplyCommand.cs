using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("reply", "r")]
[CommandSyntax("<[message]>")]
public class ReplyCommand : Command
{
    public ReplyCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reply");
        Context.AssertArguments(1);

        string message = Context.Form();

        Context.AssertPlayer(out UnturnovPlayer self);
        
        if(self.LastPrivateMessage == null) 
        {
            throw Context.Reply(TranslationList.NoOneToReplyTo);
        }

        if (!UnturnovPlayerManager.TryGetPlayer(self.LastPrivateMessage.Value, out UnturnovPlayer target))
        {
            throw Context.Reply(TranslationList.PlayerNotOnline);
        }
        
        target.LastPrivateMessage = self.SteamID;
        
        UnturnovChat.SendPrivateMessage(self, target, message);
        throw Context.Exit;
    }
}
