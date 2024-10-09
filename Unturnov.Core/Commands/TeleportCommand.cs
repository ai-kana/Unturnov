using Cysharp.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;
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
        Context.AssertPermission("teleport");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        if (Context.HasExactArguments(1))
        {
            Context.AssertPlayer(out UnturnovPlayer caller);
            UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
            caller.Movement.Teleport(player);
            throw Context.Reply("Teleported you to {0}", player.Name);
        }

        if (Context.HasExactArguments(2))
        {
            UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
            Context.MoveNext();
            UnturnovPlayer target = Context.Parse<UnturnovPlayer>();

            target.Movement.Teleport(player);
            throw Context.Reply("Teleported {0} to {1}", player.Name, target.Name);
        }

        if (Context.HasExactArguments(3))
        {
            Context.AssertPlayer(out UnturnovPlayer caller);

            Vector3 position = Context.Parse<Vector3>();
            caller.Movement.Teleport(position);
            throw Context.Reply("Teleported you to {0}, {1}, {2}", position.x, position.y, position.z);
        }

        {
            UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
            Context.MoveNext();
            Vector3 position = Context.Parse<Vector3>();
            player.Movement.Teleport(position);
            throw Context.Reply("Teleporting {0} to {1}, {2}, {3}", player.Name, position.x, position.y, position.z);
        }
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
        Context.AssertPermission("teleport");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        string location = Context.Form();
        
        Context.AssertPlayer(out UnturnovPlayer self);

        if (TryFindLocation(location, out LocationDevkitNode node))
        {
            self.Movement.Teleport(node.inspectablePosition);
            throw Context.Reply("Teleporting to {0}", node.locationName);
        }

        throw Context.Reply("Failed to find location called {0}", location);
    }

    private bool TryFindLocation(string name, out LocationDevkitNode node)
    {
        IEnumerable<LocationDevkitNode> nodes = LocationDevkitNodeSystem.Get().GetAllNodes();
        node = nodes.FirstOrDefault(x => x.locationName.Contains(name, StringComparison.OrdinalIgnoreCase));
        if (node == null)
        {
            return false;
        }

        return true;
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
        Context.AssertPermission("teleport");
        Context.AssertOnDuty();
        Context.AssertPlayer(out UnturnovPlayer self);
        
        if (!self.Quests.TryGetMarkerPosition(out Vector3 position))
        {
            throw Context.Reply("You have not placed a marker");
        }
        
        self.Movement.Teleport(position);
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
        Context.AssertPermission("teleport");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        Context.AssertPlayer(out UnturnovPlayer self);

        UnturnovPlayer toTp = Context.Parse<UnturnovPlayer>();
        
        toTp.Movement.Teleport(self);
        throw Context.Reply("Teleporting player {0} to you", toTp.Name);
    }
}
