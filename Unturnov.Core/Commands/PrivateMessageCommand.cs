using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("pm", "dm", "msg")]
[CommandSyntax("<[player] [message]>")]
public class PrivateMessageCommand : Command
{
    public PrivateMessageCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("pm");
        Context.AssertArguments(2);
        Context.AssertPlayer(out UnturnovPlayer self);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        string message = Context.Form();
        
        if (target.SteamID == self.SteamID)
        {
            throw Context.Reply(TranslationList.PrivateMessageSelf);
        }
        
        target.LastPrivateMessage = self.SteamID;

        UnturnovChat.SendPrivateMessage(self, target, message);
        throw Context.Exit;
    }
}
