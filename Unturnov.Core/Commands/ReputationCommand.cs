using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

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

        throw Context.Reply("{0}'s reputation is {1}.", target.Name, target.Quests.Reputation);
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
        
        throw Context.Reply("Set {0}'s reputation to {1}.", target.Name, reputation);
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
        
        throw Context.Reply("Reset {0}'s reputation.", target.Name);
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
        
        throw Context.Reply("Added {0} reputation to {1}.", reputation, target.Name);
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
        
        throw Context.Reply("Took {0} reputation from {1}.", reputation, target.Name);
    }
}
