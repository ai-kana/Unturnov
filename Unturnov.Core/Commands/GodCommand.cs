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
        Context.AssertArguments(1);
        
        string option = Context.Current.ToLower();
        bool offOrOn = option == "on";
        
        if (Context.HasExactArguments(2))
        {
            Context.MoveNext();
            UnturnovPlayer other = Context.Parse<UnturnovPlayer>();
            other.SetGod(offOrOn);
            throw Context.Reply("Successfully put {0}, {1} God Mode.", other.Name, offOrOn ? "in" : "out of");
        }
        
        Context.AssertPlayer(out UnturnovPlayer callerPlayer);

        callerPlayer.SetGod(offOrOn);

        throw Context.Reply("Successfully put yourself {0} God Mode!", offOrOn ? "in" : "out of");
    }
}