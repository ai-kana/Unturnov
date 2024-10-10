using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

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
        
        player.Movement.Unfreeze();
        throw Context.Reply("Successfully unfroze {0}", player.Name);
    }
}
