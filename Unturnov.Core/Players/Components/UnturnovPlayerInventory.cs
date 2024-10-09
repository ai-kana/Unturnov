using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerInventory
{
    private PlayerInventory _Inventory => Owner.Player.inventory;
    private readonly UnturnovPlayer Owner;

    public UnturnovPlayerInventory(UnturnovPlayer owner)
    {
        Owner = owner;
    }
    
    public void GiveItem(ushort id)
    {
        _Inventory.forceAddItem(new Item(id, EItemOrigin.ADMIN), true);
    }
    
    public void GiveItem(ushort id, byte amount, byte quality)
    {
        _Inventory.forceAddItem(new Item(id, amount, quality), true);
    }
    
    public void GiveItem(ushort id, byte amount, byte quality, byte[] state)
    {
        _Inventory.forceAddItem(new Item(id, amount, quality, state), true);
    }
    
    public void GiveItems(ushort id, ushort count)
    {
        for (int i = 0; i < count; i++)
        {
            GiveItem(id);
        }
    }
    
    public void GiveItems(ushort id, byte amount, byte quality, ushort count)
    {
        for (int i = 0; i < count; i++)
        {
            GiveItem(id, amount, quality);
        }
    }
    
    public void GiveItems(ushort id, byte amount, byte quality, byte[] state, ushort count)
    {
        for (int i = 0; i < count; i++)
        {
            GiveItem(id, amount, quality, state);
        }
    }
}
