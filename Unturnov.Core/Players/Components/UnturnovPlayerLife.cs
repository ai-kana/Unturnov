using SDG.Unturned;
using Steamworks;
using UnityEngine;

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

    public void Kill()
    {
        _Life.askDamage(101, Vector3.up, EDeathCause.KILL, ELimb.SKULL, new CSteamID(0UL), out _, false, ERagdollEffect.GOLD, true, true);
    }
}
