using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("giveitem", "gi")]
[CommandSyntax("<[player] [id | name] [amount?]>")]
public class GiveItemCommand : Command
{
    public GiveItemCommand(CommandContext context) : base(context)
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
        Context.AssertPermission("giveitem");
        Context.AssertOnDuty();
        Context.AssertArguments(2);
        
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();

        if (!GetItemAsset(Context.Current, out ItemAsset? itemAsset))
        {
            throw Context.Reply("Item not found");
        }
        
        if (Context.HasExactArguments(3))
        {
            Context.MoveNext();

            if (!Context.TryParse(out ushort count))
            {
                throw Context.Reply("Invalid amount");
            }
                
            target.Inventory.GiveItems(itemAsset!.id, count);
            throw Context.Reply("You gave {0} {1}x {2} ({3})", target.Name, count, itemAsset.FriendlyName, itemAsset.id);
        }
            
        target.Inventory.GiveItem(itemAsset!.id);
        throw Context.Reply("You gave {0} {1} ({2})", target.Name, itemAsset.FriendlyName, itemAsset.id);
    }
}