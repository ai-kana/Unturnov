using SDG.Unturned;

namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerSkills
{
    public readonly UnturnovPlayer Owner; 
    private PlayerSkills _Skills => Owner.Player.skills; 

    public UnturnovPlayerSkills(UnturnovPlayer owner)
    {
        Owner = owner;
    }

    public uint Experience => _Skills.experience;

    public void GiveExperience(uint xp)
    {
        _Skills.ServerSetExperience(_Skills.experience + xp);
    }

    public void RemoveExperience(uint xp)
    {
        _Skills.ServerSetExperience(_Skills.experience - xp);
    }

    public void SetExperience(uint xp)
    {
        _Skills.ServerSetExperience(xp);
    }
}
