using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Formatting;
using Unturnov.Core.Players;
using Unturnov.Core.Roles;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("role")]
[CommandSyntax("<add, a | remove, r | list>")]
public class RoleCommand : Command
{
    public RoleCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("role");
        throw Context.Reply("<add, a | remove, r | list>");
    }
}

[CommandParent(typeof(RoleCommand))]
[CommandData("add", "a")]
[CommandSyntax("<player> <role>")]
public class RoleAddCommand : Command
{
    public RoleAddCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("role");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        Role role = Context.Parse<Role>();

        player.Roles.AddRole(role.Id);
        throw Context.Reply(TranslationList.AddedRole, player.Name, role.Id);
    }
}

[CommandParent(typeof(RoleCommand))]
[CommandData("remove", "r")]
[CommandSyntax("<player> <role>")]
public class RoleRemoveCommand : Command
{
    public RoleRemoveCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("role");
        Context.AssertArguments(2);

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        Role role = Context.Parse<Role>();

        if (!player.Roles.HasRole(role.Id))
        {
            throw Context.Reply(TranslationList.DoesNotHaveRole, player.Name, role.Id);
        }

        player.Roles.AddRole(role.Id);
        throw Context.Reply(TranslationList.RemovedRole, player.Name, role.Id);
    }
}

[CommandParent(typeof(RoleCommand))]
[CommandData("list")]
[CommandSyntax("<player?>")]
public class RoleListCommand : Command
{
    public RoleListCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("role");

        if (Context.HasExactArguments(0))
        {
            throw Context.Reply(TranslationList.RoleList, Formatter.FormatList(RoleManager.Roles.Select(x => x.Id), ", "));
        }

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();

        HashSet<Role> roles = RoleManager.GetRoles(player.Roles.Roles);
        throw Context.Reply(TranslationList.RoleHasRole, player.Name, Formatter.FormatList(roles.Select(x => x.Id), ", "));
    }
}
