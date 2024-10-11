using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("freeze")]
[CommandSyntax("<[player]>")]
public class FreezeCommand : Command
{
    public FreezeCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("freeze");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        
        if(player.Movement.IsFrozen)
            throw Context.Reply(TranslationList.PlayerAlreadyFrozen, player.Name);
        
        player.Movement.Freeze();
        throw Context.Reply(TranslationList.PlayerFrozen, player.Name);
    }
}
