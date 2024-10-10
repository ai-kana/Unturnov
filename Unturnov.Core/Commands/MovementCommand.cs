using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("movement", "move")]
public class MovementCommand : Command
{
    public MovementCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("movement");
        Context.AssertOnDuty();
        
        throw Context.Reply("<[player?]> <[speed,s | jump,j | gravity,g | all,a]> <[value] | reset, r>");
    }
}

[CommandParent(typeof(MovementCommand))]
[CommandData("speed", "s")]
public class MovementSpeedCommand : Command
{
    public MovementSpeedCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("movement");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        UnturnovPlayer target;
        if (Context.HasArguments(2))
        {
            target = Context.Parse<UnturnovPlayer>();
            Context.MoveNext();
        }
        else
        {
            Context.AssertPlayer(out target);
        }

        float multiplier = Context.MatchParameter("reset", "r") ? 1f : Context.Parse<float>();
        target.Movement.SetSpeed(multiplier);
        throw Context.HasArguments(2) ?
            Context.Reply("Set {0}'s movement speed to {1}", target.Name, multiplier)
            : Context.Reply("Set your movement speed to {0}", multiplier);
    }
}

[CommandParent(typeof(MovementCommand))]
[CommandData("jump", "j")]
public class MovementJumpCommand : Command
{
    public MovementJumpCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("movement");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        UnturnovPlayer target;
        if (Context.HasArguments(2))
        {
            target = Context.Parse<UnturnovPlayer>();
            Context.MoveNext();
        }
        else
        {
            Context.AssertPlayer(out target);
        }

        float multiplier = Context.MatchParameter("reset", "r") ? 1f : Context.Parse<float>();
        target.Movement.SetJump(multiplier);
        throw Context.HasArguments(2) ?
            Context.Reply("Set {0}'s jump height to {1}", target.Name, multiplier)
            : Context.Reply("Set your jump height to {0}", multiplier);
    }
}

[CommandParent(typeof(MovementCommand))]
[CommandData("gravity", "g")]
public class MovementGravityCommand : Command
{
    public MovementGravityCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("movement");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        UnturnovPlayer target;
        if (Context.HasArguments(2))
        {
            target = Context.Parse<UnturnovPlayer>();
            Context.MoveNext();
        }
        else
        {
            Context.AssertPlayer(out target);
        }

        float multiplier = Context.MatchParameter("reset", "r") ? 1f : Context.Parse<float>();
        target.Movement.SetGravity(multiplier);
        throw Context.HasArguments(2) ?
            Context.Reply("Set {0}'s gravity to {1}", target.Name, multiplier)
            : Context.Reply("Set your gravity to {0}", multiplier);
    }
}
