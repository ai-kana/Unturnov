using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("experience", "exp", "xp")]
[CommandSyntax("<[add,a | remove,r | set,s | reset | check,c]>")]
public class ExperienceCommand : Command
{
    public ExperienceCommand (CommandContext context) : base(context)
    {
    }
    
    public static bool IsXpValid(uint xp)
    {
        return xp is > 0 and < uint.MaxValue;
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("experience");
        Context.AssertOnDuty();
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
        Context.AssertOnDuty();
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        uint amount = Context.Parse<uint>();

        if (!ExperienceCommand.IsXpValid(amount))
        {
            throw Context.Reply(TranslationList.BadNumber);
        }
        
        player.Skills.GiveExperience(amount);
        
        throw Context.Reply(TranslationList.AddedExperience, amount, player.Name);
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
        Context.AssertOnDuty();
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        uint amount = Context.Parse<uint>();

        if (!ExperienceCommand.IsXpValid(amount))
        {
            throw Context.Reply(TranslationList.BadNumber);
        }
        
        player.Skills.RemoveExperience(amount);
        
        throw Context.Reply(TranslationList.RemovedExperience, amount, player.Name);
    }
}

[CommandParent(typeof(ExperienceCommand))]
[CommandData("reset")]
public class ExperienceResetCommand : Command
{
    public ExperienceResetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("experience");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        
        player.Skills.SetExperience(0);
        
        throw Context.Reply(TranslationList.ResetExperience, player.Name);
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
        Context.AssertOnDuty();
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        uint amount = Context.Parse<uint>();

        if (!ExperienceCommand.IsXpValid(amount))
        {
            throw Context.Reply(TranslationList.BadNumber);
        }
        
        player.Skills.SetExperience(amount);
        
        throw Context.Reply(TranslationList.SetExperience, player.Name, amount);
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
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        
        throw Context.Reply(TranslationList.CheckedExperience, player.Name, player.Skills.Experience);
    }
}
