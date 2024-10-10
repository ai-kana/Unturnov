using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
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
        
        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();

        if (Context.HasExactArguments(1))
        {
            player.Kick();
            throw Context.Reply("Kicked {0}", player.Name);
        }
        
        Context.MoveNext();
        string reason = Context.Form();
        
        player.Kick(reason);
        throw Context.Reply("Kicked {0} for {1}", player.Name, reason);
    }
}
