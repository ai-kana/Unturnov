using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("spy")]
[CommandSyntax("<[player]>")]
public class SpyCommand : Command
{
    public SpyCommand(CommandContext context) : base(context)
    {
        
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("spy");
        Context.AssertArguments(1);
        
        Context.AssertPlayer(out UnturnovPlayer self);
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        target.Spy(self.SteamID);
        throw Context.Reply("Spying on {0}", target.Name);
    }
}