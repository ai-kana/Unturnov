using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("teleport", "tp")]
[CommandSyntax("<[xyz | player,p | location,loc,l | waypoint,wp | here,h]>")]
public class TeleportCommand : Command
{
    public TeleportCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("teleport");

        if (Context.HasExactArguments(0))
        {
            throw Context.Reply("<[xyz | player,p | location,loc,l | waypoint,wp | here,h]>");
        }

        if (!Context.TryParse(out float x))
            throw Context.Reply("<[xyz | player,p | location,loc,l | waypoint,wp | here,h]>");
        Context.MoveNext();
        
        if (!Context.TryParse(out float y))
            throw Context.Reply("<[xyz | player,p | location,loc,l | waypoint,wp | here,h]>");
        Context.MoveNext();
        
        if (!Context.TryParse(out float z))
            throw Context.Reply("<[xyz | player,p | location,loc,l | waypoint,wp | here,h]>");
        
        Context.AssertPlayer(out UnturnovPlayer self);
        
        self.Teleport(x, y, z);
        throw Context.Reply("Teleporting to {0} | {1} | {2}", x, y, z);
    }
}

//if u type a random input, it errors out; fig out tmrw; - 0x5bc2
[CommandParent(typeof(TeleportCommand))]
[CommandData("location", "loc", "l")]
[CommandSyntax("<[name]>")]
public class TeleportLocationCommand : Command
{
    public TeleportLocationCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("teleport.location");
        Context.AssertArguments(1);
        
        string location = Context.Form();
        
        Context.AssertPlayer(out UnturnovPlayer self);

        if (self.Teleport(location, out var name))
        {
            throw Context.Reply("Teleporting to location {0}", name);
        }

        throw Context.Reply("Failed to teleport to {0}", location);
    }
}

[CommandParent(typeof(TeleportCommand))]
[CommandData("player", "p")]
[CommandSyntax("<[player]>")]
public class TeleportPlayerCommand : Command
{
    public TeleportPlayerCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("teleport.player");
        Context.AssertArguments(1);

        if (Context.HasExactArguments(2))
        {
            //add fucking xyz here by checking if first arg is float or not
            
            UnturnovPlayer target1 = Context.Parse<UnturnovPlayer>();
            Context.MoveNext();
            UnturnovPlayer target2 = Context.Parse<UnturnovPlayer>();
            
            target1.TeleportToPlayer(target2);
            throw Context.Reply("Teleporting player {0} to player {1}", target1.Name, target2.Name);
        }
        
        Context.AssertPlayer(out UnturnovPlayer self);
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        self.TeleportToPlayer(target);
        
        throw Context.Reply("Teleporting to player {0}", target.Name);
    }
}

[CommandParent(typeof(TeleportCommand))]
[CommandData("waypoint", "wp")]
public class TeleportWaypointCommand : Command
{
    public TeleportWaypointCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("teleport.waypoint");
        
        Context.AssertPlayer(out UnturnovPlayer self);
        
        self.TeleportToWaypoint();
        
        throw Context.Reply("Teleporting to waypoint!");
    }
}

[CommandParent(typeof(TeleportCommand))]
[CommandData("here", "h")]
[CommandSyntax("<[player]>")]
public class TeleportHereCommand : Command
{
    public TeleportHereCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("teleport.here");
        Context.AssertArguments(1);
        
        Context.AssertPlayer(out UnturnovPlayer self);
        Context.AssertPlayer(out UnturnovPlayer target);
        
        self.TeleportHere(target);
        
        throw Context.Reply("Teleporting player {0} to you", target.Name);
    }
}

/*
tp x y z
tp location
tp player
tp waypoint
tp player player
tp player x y z
tp here player

*/