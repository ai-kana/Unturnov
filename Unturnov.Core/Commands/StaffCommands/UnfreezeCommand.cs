using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("unfreeze")]
[CommandSyntax("<[player]>")]
public class UnfreezeCommand : Command
{
    public UnfreezeCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("unfreeze");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        
        if(!player.Movement.IsFrozen)
            throw Context.Reply(TranslationList.PlayerNotFrozen, player.Name);
        
        player.Movement.Unfreeze();
        throw Context.Reply(TranslationList.PlayerUnfrozen, player.Name);
    }
}
