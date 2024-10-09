using SDG.Unturned;

namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerLife
{
    public bool GodMode {get; set;} = false;

    private PlayerLife _Life => Owner.Player.life;
    private readonly UnturnovPlayer Owner;

    public UnturnovPlayerLife(UnturnovPlayer owner)
    {
        Owner = owner;
    }

    public void Heal()
    {
        _Life.sendRevive();
    }
}
