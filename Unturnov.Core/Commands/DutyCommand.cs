using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("duty", "d")]
[CommandSyntax("<silent, s | check, c>")]
public class DutyCommand : Command
{
    public DutyCommand(CommandContext context) : base(context)
    {
    }

    private static readonly Translation DutyState = new("DutyState", "{0} is now {1} duty");
    private static readonly Translation On = new("On", "on");
    private static readonly Translation Off = new("Off", "off");

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("duty");
        Context.AssertPlayer(out UnturnovPlayer caller);

        caller.OnDuty = !caller.OnDuty;
        UnturnovChat.BroadcastMessage(DutyState, caller.Name, caller.OnDuty ? new TranslationPackage(On) : new TranslationPackage(Off));
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
        Context.AssertPermission("sduty");
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
        Context.AssertPermission("duty");
        Context.AssertPlayer(out UnturnovPlayer caller);

        throw Context.Reply("You are {0} duty", caller.OnDuty ? "on" : "off");
    }
}
