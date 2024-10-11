using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("kick")]
[CommandSyntax("<player> <reason?>")]
public class KickCommand : Command
{
    public KickCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("kick");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();

        if (Context.HasExactArguments(1))
        {
            target.Kick();
            throw Context.Reply(TranslationList.Kicked, target.Name);
        }
        
        Context.MoveNext();
        string reason = Context.Form();
        
        target.Kick(reason);
        throw Context.Reply(TranslationList.KickedReason, target.Name, reason);
    }
}
