using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("position", "pos")]
[CommandSyntax("<[player?]>")]
public class PositionCommand : Command
{
    public PositionCommand(CommandContext context) : base(context)
    {
    }
    
    private static readonly Translation SelfPosition = new("PositionSelf", "You are at: {0} | {1} | {2}");
    private static readonly Translation TargetPosition = new("PositionTarget", "{0} is at: {1} | {2} | {3}");
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("position");

        string x,y,z;
        
        if (Context.HasExactArguments(0))
        {
            Context.AssertPlayer(out UnturnovPlayer self);
            
            x = self.Movement.Position.x.ToString("F1");
            y = self.Movement.Position.y.ToString("F1");
            z = self.Movement.Position.z.ToString("F1");
            
            throw Context.Reply(SelfPosition, x, y, z);
        }
        
        Context.AssertArguments(1);
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        x = target.Movement.Position.x.ToString("F1");
        y = target.Movement.Position.y.ToString("F1");
        z = target.Movement.Position.z.ToString("F1");
        
        throw Context.Reply(TargetPosition, target.Name, x, y, z);
    }
}
