using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("item", "i")]
[CommandSyntax("<[id | name] [amount?]>")]
public class ItemCommand : Command
{
    public ItemCommand(CommandContext context) : base(context)
    {
    }

    public bool GetItemAsset(string input, out ItemAsset? itemAsset)
    {
        input = input.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            itemAsset = null;
            return false;
        }

        List<ItemAsset> itemAssetsList = new();
        Assets.find(itemAssetsList);

        if (ushort.TryParse(input, out ushort id))
        {
            if (id == 0)
            {
                itemAsset = null;
                return false;
            }

            itemAsset = itemAssetsList.FirstOrDefault(i => i.id == id && !i.isPro);
            return itemAsset != null;
        }

        itemAsset = itemAssetsList.FirstOrDefault(i =>
            i.itemName.Contains(input, StringComparison.InvariantCultureIgnoreCase) ||
            i.name.Contains(input, StringComparison.InvariantCultureIgnoreCase) && !i.isPro);

        return itemAsset != null;
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("item");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        Context.AssertPlayer(out UnturnovPlayer self);

        if (!GetItemAsset(Context.Current, out ItemAsset? itemAsset))
        {
            throw Context.Reply("Item not found");
        }
        
        if (Context.HasExactArguments(2))
        {
            Context.MoveNext();

            if (!Context.TryParse(out ushort count))
            {
                throw Context.Reply("Invalid amount");
            }
                
            self.Inventory.GiveItems(itemAsset!.id, count);
            throw Context.Reply("You have received {0}x {1} ({2})", count, itemAsset.FriendlyName, itemAsset.id);

        }
            
        self.Inventory.GiveItem(itemAsset!.id);
        throw Context.Reply("You have received {0} ({1})", itemAsset.FriendlyName, itemAsset.id);
    }
}