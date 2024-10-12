using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Offenses;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("warn")]
[CommandSyntax("<[add,a | remove,r | list,l]>")]
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

[CommandParent(typeof(WarnCommand))]
[CommandData("add", "a")]
[CommandSyntax("<player> <reason>")]
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
        
        _ = target.Moderation.AddWarn(self.SteamID, reason);
        
        throw Context.Reply(TranslationList.WarnedReason, target.Name, reason);
    }
}

[CommandParent(typeof(WarnCommand))]
[CommandData("remove", "r")]
[CommandSyntax("<player> <id>")]
public class WarnRemoveCommand : Command
{
    public WarnRemoveCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("warn.remove");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        int id = Context.Parse<int>();

        _ = OffenseManager.PardonOffense(id);
        
        throw Context.Reply(TranslationList.PardonedWarn, id);
    }
}

[CommandParent(typeof(WarnCommand))]
[CommandData("list", "l")]
[CommandSyntax("<player>")]
public class WarnListCommand : Command
{
    public WarnListCommand(CommandContext context) : base(context)
    {
    }
    
    public override async UniTask ExecuteAsync()
    {
        Context.AssertPermission("warn.list");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();

        IEnumerable<Offense> warns = await target.Moderation.GetWarns();
        List<Offense> warnsList = warns.Where(w => w.Pardoned == false).ToList();
        
        if (!warnsList.Any())
        {
            throw Context.Reply(TranslationList.HasNoWarns, target.Name);
        }

        List<Offense> lastFiveWarns = warnsList.OrderByDescending(w => w.Issued).Take(5).ToList();
        
        foreach (Offense lastFiveWarn in lastFiveWarns)
        {
            Context.Reply(TranslationList.WarningListed, lastFiveWarn.Id, lastFiveWarn.Issued, lastFiveWarn.Reason);
        }

        throw Context.Exit;
    }
}
