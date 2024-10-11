using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("reputation", "rep")]
[CommandSyntax("<[get,g | set,s | reset,r | add,a | take,t]>")]
public class ReputationCommand : Command
{
    public ReputationCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reputation");
        Context.AssertOnDuty();
        
        throw Context.Reply("<[get,g | set,s | reset,r | add,a | take,t]>");
    }
}

[CommandParent(typeof(ReputationCommand))]
[CommandData("get", "g")]
public class ReputationGetCommand : Command
{
    public ReputationGetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reputation");
        Context.AssertOnDuty();
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();

        throw Context.Reply(TranslationList.CheckedReputation, target.Name, target.Quests.Reputation);
    }
}

[CommandParent(typeof(ReputationCommand))]
[CommandData("set", "s")]
[CommandSyntax("<player> <reputation>")]
public class ReputationSetCommand : Command
{
    public ReputationSetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reputation");
        Context.AssertOnDuty();
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        int reputation = Context.Parse<int>();

        target.Quests.SetReputation(reputation);
        
        throw Context.Reply(TranslationList.SetReputation, target.Name, reputation);
    }
}

[CommandParent(typeof(ReputationCommand))]
[CommandData("reset", "r")]
[CommandSyntax("<player>")]
public class ReputationResetCommand : Command
{
    public ReputationResetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reputation");
        Context.AssertOnDuty();
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();

        target.Quests.SetReputation(0);
        
        throw Context.Reply(TranslationList.ResetReputation, target.Name);
    }
}

[CommandParent(typeof(ReputationCommand))]
[CommandData("add", "a")]
[CommandSyntax("<player> <reputation>")]
public class ReputationAddCommand : Command
{
    public ReputationAddCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reputation");
        Context.AssertOnDuty();
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        int reputation = Context.Parse<int>();

        target.Quests.GiveReputation(reputation);
        
        throw Context.Reply(TranslationList.AddedReputation, reputation, target.Name);
    }
}

[CommandParent(typeof(ReputationCommand))]
[CommandData("take", "t")]
[CommandSyntax("<player> <reputation>")]
public class ReputationTakeCommand : Command
{
    public ReputationTakeCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("reputation");
        Context.AssertOnDuty();
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        int reputation = Context.Parse<int>();

        target.Quests.RemoveReputation(reputation);
        
        throw Context.Reply(TranslationList.TookReputation, reputation, target.Name);
    }
}
