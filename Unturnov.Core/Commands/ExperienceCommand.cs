using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("experience", "exp", "xp")]
[CommandSyntax("<add | remove | set | check>")]
public class ExperienceCommand : Command
{
    public ExperienceCommand (CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("experience");
        throw Context.Reply("<add | remove | set | check>");
    }
}

[CommandParent(typeof(ExperienceCommand))]
[CommandData("add", "a")]
[CommandSyntax("<[player]> <[Amount]>")]
public class ExperienceAddCommand : Command 
{
    public ExperienceAddCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("experience");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        uint amount = Context.Parse<uint>();

        if (amount == 0)
        {
            throw Context.Reply("Amount must be greater than 0");
        }
        
        if(amount >= uint.MaxValue)
        {
            throw Context.Reply("Amount must be less than {0}", uint.MaxValue);
        }
        
        player.Skills.askAward(amount);
        
        throw Context.Reply("Added {0} experience to {1}", amount, player.Name);
    }
}

[CommandParent(typeof(ExperienceCommand))]
[CommandData("remove", "r")]
[CommandSyntax("<[player]> <[Amount]>")]
public class ExperienceRemoveCommand : Command
{
    public ExperienceRemoveCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("experience");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        uint amount = Context.Parse<uint>();

        if (amount <= 0)
        {
            throw Context.Reply("Amount must be greater than 0");
        }
        
        if(amount >= uint.MaxValue)
        {
            throw Context.Reply("Amount must be less than {0}", uint.MaxValue);
        }
        
        player.Skills.askSpend(amount);
        
        throw Context.Reply("Removed {0} experience from {1}", amount, player.Name);
    }
}

[CommandParent(typeof(ExperienceCommand))]
[CommandData("set", "s")]
[CommandSyntax("<[player]> <[Amount]>")]
public class ExperienceSetCommand : Command
{
    public ExperienceSetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("experience");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        uint amount = Context.Parse<uint>();

        if (amount <= 0)
        {
            throw Context.Reply("Amount must be greater than 0");
        }
        
        if (amount >= uint.MaxValue)
        {
            throw Context.Reply("Amount must be less than {0}", uint.MaxValue);
        }
        
        player.Skills.ReceiveExperience(amount);
        
        throw Context.Reply("Set {0}'s experience to {1}", player.Name, amount);
    }
}

[CommandParent(typeof(ExperienceCommand))]
[CommandData("check", "c")]
[CommandSyntax("<[player]>")]
public class ExperienceCheckCommand : Command
{
    public ExperienceCheckCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("experience");
        Context.AssertArguments(1);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        
        throw Context.Reply("{0} has {1} experience", player.Name, player.Skills.experience);
    }
}
