using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("duty", "d")]
public class DutyCommand : Command
{
    public DutyCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPlayer(out UnturnovPlayer caller);
        caller.OnDuty = !caller.OnDuty;
        UnturnovChat.BroadcastMessage("{0} is now {1} duty", caller.Name, caller.OnDuty ? "on" : "off");
        throw Context.Exit;
    }
}

[CommandParent(typeof(DutyCommand))]
[CommandData("silent", "s")]
public class DutySlientCommand : Command
{
    public DutySlientCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPlayer(out UnturnovPlayer caller);
        caller.OnDuty = !caller.OnDuty;
        throw Context.Reply("You are now {0} duty", caller.OnDuty ? "on" : "off");
    }
}

[CommandParent(typeof(DutyCommand))]
[CommandData("check", "c")]
public class DutyCheckCommand : Command
{
    public DutyCheckCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPlayer(out UnturnovPlayer caller);
        throw Context.Reply("You are {0} duty", caller.OnDuty ? "on" : "off");
    }
}
