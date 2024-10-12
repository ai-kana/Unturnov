using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerLife
{
    public bool GodMode {get; set;} = false;
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

    public void Kill()
    {
        _Life.askDamage(byte.MaxValue, Vector3.up * 5, EDeathCause.KILL, ELimb.SKULL, new CSteamID(0UL), out _, false, ERagdollEffect.GOLD, true, true);
    }
}
