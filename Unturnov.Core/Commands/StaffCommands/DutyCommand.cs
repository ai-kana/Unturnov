using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("duty", "d")]
[CommandSyntax("<silent, s | check, c>")]
public class DutyCommand : Command
{
    public DutyCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("duty");
        Context.AssertPlayer(out UnturnovPlayer caller);

        bool state = caller.Administration.ToggleDuty();
        UnturnovChat.BroadcastMessage(TranslationList.DutyStateGlobal, caller.Name, state ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
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

        bool state = caller.Administration.ToggleDuty();
        throw Context.Reply(TranslationList.DutyStateSilent, state ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
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

        throw Context.Reply(TranslationList.DutyStateCheck, caller.Administration.OnDuty ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
    }
}
