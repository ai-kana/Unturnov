using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("flag")]
[CommandSyntax("<get | set | unset>")]
public class FlagCommand : Command
{
    public FlagCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("flag");
        throw Context.Reply("<get | set | unset>");
    }
}

[CommandParent(typeof(FlagCommand))]
[CommandData("get")]
[CommandSyntax("<[player]> <[flag]>")]
public class FlagGetCommand : Command
{
    public FlagGetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        ushort flag = Context.Parse<ushort>();

        if (flag == 0)
        {
            throw Context.Reply("Flag value must be greater than 0");
        }

        PlayerQuests quests = player.Quests;

        if (!quests.flagsList.Exists(f => f.id == flag))
        {
            throw Context.Reply("Player {0} does not have flag {1}", player.Name, flag);
        }
        
        quests.getFlag(flag, out short value);
        
        throw Context.Reply("Flag {0} for {1} is {2}", flag, player.Name, value);
    }
}

[CommandParent(typeof(FlagCommand))]
[CommandData("set")]
[CommandSyntax("<[player]> <[flag]> <[value]>")]
public class FlagSetCommand : Command
{
    public FlagSetCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertArguments(3);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        ushort flag = Context.Parse<ushort>();
        Context.MoveNext();
        short value = Context.Parse<short>();

        if (flag == 0)
        {
            throw Context.Reply("Flag value must be greater than 0");
        }

        PlayerQuests quests = player.Quests;
        
        quests.setFlag(flag, value);
        
        throw Context.Reply("Set flag {0} for {1} to {2}", flag, player.Name, value);
    }
}

[CommandParent(typeof(FlagCommand))]
[CommandData("unset")]
[CommandSyntax("<[player]> <[flag]>")]
public class FlagUnsetCommand : Command
{
    public FlagUnsetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        ushort flag = Context.Parse<ushort>();

        if (flag == 0)
        {
            throw Context.Reply("Flag value must be greater than 0");
        }

        PlayerQuests quests = player.Quests;

        if (!quests.flagsList.Exists(f => f.id == flag))
        {
            throw Context.Reply("Player {0} does not have flag {1}", player.Name, flag);
        }
        
        quests.removeFlag(flag);
        
        throw Context.Reply("Unset flag {0} for {1}", flag, player.Name);
    }
}