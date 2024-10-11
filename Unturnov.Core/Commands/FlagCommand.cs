using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("flag")]
[CommandSyntax("<[get | set | unset]>")]
public class FlagCommand : Command
{
    public FlagCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertOnDuty();
        throw Context.Reply("<[get | set | unset]>");
    }
}

[CommandParent(typeof(FlagCommand))]
[CommandData("get")]
[CommandSyntax("<[player] [flag]>")]
public class FlagGetCommand : Command
{
    public FlagGetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertOnDuty();
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        ushort flag = Context.Parse<ushort>();

        if (flag <= 0)
        {
            throw Context.Reply(TranslationList.GreaterThanZero);
        }

        if (!player.Quests.FlagExists(flag))
        {
            throw Context.Reply(TranslationList.FlagDoesNotExist, player.Name, flag);
        }
        
        player.Quests.TryGetFlag(flag, out short value);
        throw Context.Reply(TranslationList.FlagGet, flag, player.Name, value);
    }
}

[CommandParent(typeof(FlagCommand))]
[CommandData("set")]
[CommandSyntax("<[player] [flag] [value]>")]
public class FlagSetCommand : Command
{
    public FlagSetCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertOnDuty();
        Context.AssertArguments(3);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        ushort flag = Context.Parse<ushort>();
        Context.MoveNext();
        short value = Context.Parse<short>();

        player.Quests.SetFlag(flag, value);
        throw Context.Reply(TranslationList.FlagSet, flag, player.Name, value);
    }
}

[CommandParent(typeof(FlagCommand))]
[CommandData("unset")]
[CommandSyntax("<[player] [flag]>")]
public class FlagUnsetCommand : Command
{
    public FlagUnsetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("flag");
        Context.AssertOnDuty();
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        ushort flag = Context.Parse<ushort>();

        if (!player.Quests.FlagExists(flag))
        {
            throw Context.Reply(TranslationList.FlagDoesNotExist, player.Name, flag);
        }
        
        player.Quests.RemoveFlag(flag);
        throw Context.Reply(TranslationList.FlagUnset, flag, player.Name);
    }
}
