using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("position", "pos")]
[CommandSyntax("<[player?]>")]
public class PositionCommand : Command
{
    public PositionCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("position");

        string x,y,z;
        
        if (Context.HasExactArguments(0))
        {
            Context.AssertPlayer(out UnturnovPlayer self);
            
            x = self.Position.x.ToString("F1");
            y = self.Position.y.ToString("F1");
            z = self.Position.z.ToString("F1");
            
            throw Context.Reply("You are at X: {0} | Y: {1} | Z: {2}", x, y, z);
        }
        
        Context.AssertArguments(1);
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        x = target.Position.x.ToString("F1");
        y = target.Position.y.ToString("F1");
        z = target.Position.z.ToString("F1");
        
        throw Context.Reply("{0} is at X: {1} | Y: {2} | Z: {3}", target.Name, x, y, z);
    }
}