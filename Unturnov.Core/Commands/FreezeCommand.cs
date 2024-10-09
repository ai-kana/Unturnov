using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

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
        Context.AssertOnDuty();
        Context.AssertPermission("freeze");
        Context.AssertArguments(1);
        
        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        
        player.Movement.Freeze();
        throw Context.Reply("Successfully froze {0}", player.Name);
    }
}
