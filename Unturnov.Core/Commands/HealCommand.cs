using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

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
            UnturnovPlayer other = Context.Parse<UnturnovPlayer>();
            other.Life.Heal();
            throw Context.Reply("Successfully healed {0}", other.Name);
        }
        
        Context.AssertPlayer(out UnturnovPlayer callerPlayer);
        callerPlayer.Life.Heal();
        throw Context.Reply("Successfully healed yourself");
    }
}
