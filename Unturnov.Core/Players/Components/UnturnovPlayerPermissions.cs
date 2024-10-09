using Unturnov.Core.Roles;

namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerPermissions
{
    public HashSet<string> Permissions => Owner.SaveData.Permissions;
    public readonly UnturnovPlayer Owner; 

    public UnturnovPlayerPermissions(UnturnovPlayer owner)
    {
        Owner = owner;
    }

    public void AddPermission(string permission)
    {
        Permissions.Add(permission.ToLower());
    }

    public void RemovePermission(string permission)
    {
        Permissions.Remove(permission.ToLower());
    }

    public bool HasPermission(string permission)
    {
        if (Permissions.Contains("all"))
        {
            return true;
        }

        if (Permissions.Contains(permission.ToLower()))
        {
            return true;
        }

        HashSet<Role> roles = RoleManager.GetRoles(Owner.Roles);
        foreach (Role role in roles)
        {
            if (role.Permissions.Contains("all"))
            {
                return true;
            }

            if (role.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

}
