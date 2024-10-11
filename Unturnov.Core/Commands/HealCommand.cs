using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("heal")]
[CommandSyntax("<[player?]>")]
public class HealCommand : Command
{
    public HealCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("heal");

        if (Context.HasExactArguments(1))
        {
            Context.AssertPermission("heal.other");
            UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
            target.Life.Heal();
            throw Context.Reply(TranslationList.HealedOther, target.Name);
        }
        
        Context.AssertPlayer(out UnturnovPlayer self);
        self.Life.Heal();
        throw Context.Reply(TranslationList.HealedSelf);
    }
}
