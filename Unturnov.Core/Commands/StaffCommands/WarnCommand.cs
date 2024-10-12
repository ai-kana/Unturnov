using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("warn")]
[CommandSyntax("<[add | remove | list]>")]
public class WarnCommand : Command
{
    public WarnCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("warn");
        Context.AssertOnDuty();
        
        throw Context.Reply("<add | remove | list>");
    }
}

public class WarnAddCommand : Command
{
    public WarnAddCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("warn.add");
        Context.AssertOnDuty();
        Context.AssertArguments(2);
        Context.AssertPlayer(out UnturnovPlayer self);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        string reason = Context.Form();
        
        //target.AddWarning(reason, self.SteamID);
        
        throw Context.Reply("Warned {0} for {1}", target.Name, reason);
    }
}

public class WarnRemoveCommand : Command
{
    public WarnRemoveCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("warn.remove");
        Context.AssertOnDuty();
        
        Context.AssertArguments(2);
        Context.AssertPlayer(out UnturnovPlayer self);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        int id = Context.Parse<int>(); //@0x5bc2 - this is subject to change, depending how you/kana stores the Ids.
        
        //if(!target.HasWarning(id)) 
            //throw Context.Reply("{0} has no warning with id {1}", target.Name, id);
        
        //target.RemoveWarning(id, self.SteamID);
        
        throw Context.Reply("Removed warning #{0} from {1}", id, target.Name);
    }
}

public class WarnListCommand : Command
{
    public WarnListCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("warn.list");
        Context.AssertOnDuty();
        
        throw Context.Reply("Not Implemented Yet.");
    }
}
