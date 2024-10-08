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
        Context.AssertCooldown(); //@0x5bc2 - Not sure if you'd like the cooldown here or not lol.

        if (Context.HasExactArguments(1))
        {
            Context.AssertPermission("heal.other");
            UnturnovPlayer other = Context.Parse<UnturnovPlayer>();
            other.Heal();
            Context.AddCooldown(60);
            throw Context.Reply("Successfully healed {0}", other.Name);
        }
        
        Context.AssertPlayer(out UnturnovPlayer callerPlayer);
        callerPlayer.Heal();
        Context.AddCooldown(60);
        throw Context.Reply("Successfully healed yourself");
    }
}