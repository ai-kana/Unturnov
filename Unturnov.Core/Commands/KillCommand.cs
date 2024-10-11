using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("kill")]
[CommandSyntax("<[player]>")]
public class KillCommand : Command
{
    public KillCommand(CommandContext context) : base(context)
    {
        
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("kill");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        target.Life.Kill();
        throw Context.Reply(TranslationList.Killed, target.Name);
    }
}