using Cysharp.Threading.Tasks;
using SDG.NetTransport;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("clear")]
[CommandSyntax("<[ground,g | inventory,i]>")]
public class ClearCommand : Command
{
    public ClearCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("clear");
        Context.AssertOnDuty();
        
        throw Context.Reply("<[ground,g | inventory,i]>");
    }
}

[CommandParent(typeof(ClearCommand))]
[CommandData("ground", "g")]
public class ClearGroundCommand : Command
{
    public ClearGroundCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("clear");
        Context.AssertOnDuty();
        
        if (Context.TryParse(out float distance))
        {
            Context.AssertPlayer(out UnturnovPlayer self);
            ItemManager.ServerClearItemsInSphere(self.Movement.Position, distance);
            //@0x5bc2 - Kana said to leave a comment here so she can make this more efficient :D
            throw Context.Reply("Cleared items within {0} meters", distance);
        }
        
        ItemManager.askClearAllItems();
        throw Context.Reply("Cleared ground");
    }
}

[CommandParent(typeof(ClearCommand))]
[CommandData("inventory", "i")]
[CommandSyntax("<[player?]>")]
public class ClearInventoryCommand : Command
{
    public ClearInventoryCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("clear");
        Context.AssertOnDuty();

        if (Context.HasExactArguments(1) && Context.TryParse(out UnturnovPlayer target))
        {
            Context.AssertPermission("clear.other");
            
            if(target.Inventory.ClearInventory() && target.Clothing.ClearClothes())
                throw Context.Reply("Cleared {0}'s inventory", target.Name);

            throw Context.Reply("Failed to clear {0}'s inventory", target.Name);
        }
        
        Context.AssertPlayer(out UnturnovPlayer self);
        
        if(self.Inventory.ClearInventory() && self.Clothing.ClearClothes())
            throw Context.Reply("Cleared inventory");
        
        throw Context.Reply("Failed to clear inventory");
    }
}