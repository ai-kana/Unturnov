using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands.StaffCommands;

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
        Context.AssertPlayer(out UnturnovPlayer self);
        Context.AssertPermission("clear");
        Context.AssertOnDuty();
        
        float radius = Context.Parse<float>();

        ItemManager.ServerClearItemsInSphere(self.Movement.Position, radius);
        throw Context.Reply(TranslationList.ClearedGroundDistance, radius);
    }
}

[CommandParent(typeof(ClearGroundCommand))]
[CommandData("all", "a")]
public class ClearGroundAllCommand : Command
{
    public ClearGroundAllCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPlayer(out UnturnovPlayer self);
        Context.AssertPermission("clear");
        Context.AssertOnDuty();

        ItemManager.askClearAllItems();
        throw Context.Reply(TranslationList.ClearedGround);
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
                throw Context.Reply(TranslationList.ClearedInventoryOther, target.Name);

            throw Context.Reply(TranslationList.FailedToClearInventoryOther, target.Name);
        }
        
        Context.AssertPlayer(out UnturnovPlayer self);
        
        if(self.Inventory.ClearInventory() && self.Clothing.ClearClothes())
            throw Context.Reply(TranslationList.ClearedInventorySelf);
        
        throw Context.Reply(TranslationList.FailedToClearInventorySelf);
    }
}
