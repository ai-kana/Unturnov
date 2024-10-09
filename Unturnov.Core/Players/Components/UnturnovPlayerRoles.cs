namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerRoles
{
    public HashSet<string> Roles => Owner.SaveData.Roles;

    public readonly UnturnovPlayer Owner; 

    public UnturnovPlayerRoles(UnturnovPlayer owner)
    {
        Owner = owner;
    }

    public bool AddRole(string id)
    {
        return Roles.Add(id);
    }

    public bool RemoveRole(string id)
    {
        return Roles.Remove(id);
    }

    public bool HasRole(string id)
    {
        return Roles.Contains(id);
    }
}
