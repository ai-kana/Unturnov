using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Extensions;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("give", "item", "i")]
[CommandSyntax("<[id | name] [amount?]>")]
public class GiveCommand : Command
{
    public GiveCommand(CommandContext context) : base(context)
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
        Context.AssertPermission("give");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        Context.AssertPlayer(out UnturnovPlayer self);

        if (!GetItemAsset(Context.Current, out ItemAsset? itemAsset))
        {
            throw Context.Reply(TranslationList.ItemNotFound);
        }
        
        if (Context.HasExactArguments(2))
        {
            Context.MoveNext();

            if (!Context.TryParse(out ushort count))
            {
                throw Context.Reply(TranslationList.BadNumber);
            }
                
            self.Inventory.GiveItems(itemAsset!.id, count);
            throw Context.Reply(TranslationList.ItemSelfAmount, count, itemAsset.FriendlyName, itemAsset.id);
        }
            
        self.Inventory.GiveItem(itemAsset!.id);
        throw Context.Reply(TranslationList.ItemSelf, itemAsset.FriendlyName, itemAsset.id);
    }
}
