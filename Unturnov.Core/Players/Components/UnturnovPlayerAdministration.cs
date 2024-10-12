namespace Unturnov.Core.Players;

public class UnturnovPlayerAdministration
{
    private readonly UnturnovPlayer Owner;
    public bool OnDuty {get; private set;} = false;
    public bool GodMode {get; private set;} = false;
    
    public UnturnovPlayerAdministration(UnturnovPlayer owner)
    {
        Owner = owner;
    }

    private bool _VanishMode = false;
    public bool VanishMode
    {
        get => _VanishMode;
        set
        {
            _VanishMode = value;
            Owner.Player.movement.canAddSimulationResultsToUpdates = !_VanishMode;
            Owner.Movement.Teleport(Owner);
        }
    }

    public bool ToggleGod()
    {
        Owner.Life.Heal();
        GodMode = !GodMode;
        return GodMode;
    }

    public bool ToggleDuty()
    {
        OnDuty = !OnDuty;
        GodMode = OnDuty;
        VanishMode = VanishMode || !OnDuty;

        if (Owner.Permissions.HasPermission("spectator"))
        {
            Owner.Player.look.sendFreecamAllowed(OnDuty);
            Owner.Player.look.sendFreecamAllowed(OnDuty);
            Owner.Player.look.sendFreecamAllowed(OnDuty);
        }

        return OnDuty;
    }
}
