using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Formatting;
using Unturnov.Core.Logging;
using Unturnov.Core.Players;
using Unturnov.Core.Roles;

namespace Unturnov.Core.Commands;

[CommandData("role", "r")]
public class RoleCommand : Command
{
    public RoleCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("role");
        throw Context.Reply("/role <add | remove | list>");
    }
}

[CommandParent(typeof(RoleCommand))]
[CommandData("add", "a")]
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

        player.AddRole(role.Id);
        throw Context.Reply("Added {0} to {1}", player.Name, role.Id);
    }
}

[CommandParent(typeof(RoleCommand))]
[CommandData("remove", "r")]
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

        if (!player.HasRole(role.Id))
        {
            throw Context.Reply("{0} does not have {1}", player.Name, role.Id);
        }

        player.AddRole(role.Id);
        throw Context.Reply("Removed {0} from {1}", player.Name, role.Id);
    }
}

[CommandParent(typeof(RoleCommand))]
[CommandData("list")]
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
            throw Context.Reply("Roles: {0}", Formatter.FormatList(RoleManager.Roles.Select(x => x.Id), ", "));
        }

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();

        HashSet<Role> roles = RoleManager.GetRoles(player.Roles);
        throw Context.Reply("{0} has {1}", player.Name, Formatter.FormatList(roles.Select(x => x.Id), ", "));
    }
}
