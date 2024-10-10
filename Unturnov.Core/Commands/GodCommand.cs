using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("god")]
[CommandSyntax("<[off | on] [player?]>")]
public class GodCommand : Command
{
    public GodCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("god");
        Context.AssertOnDuty();
        Context.AssertPlayer(out UnturnovPlayer caller);

        if (Context.HasArguments(1))
        {
            Context.AssertPermission("god.other");
            UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
            player.Life.GodMode = !player.Life.GodMode;
            throw Context.Reply(player.Life.GodMode ? "{0} is now in god mode" : "{0} is now off god mode", player.Name);
        }

        caller.Life.GodMode = !caller.Life.GodMode;
        throw Context.Reply(caller.Life.GodMode ? "You are now in god mode" : "You are now off god mode");
    }
}
