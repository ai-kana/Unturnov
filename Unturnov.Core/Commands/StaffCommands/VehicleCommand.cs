using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Extensions;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("vehicle", "v")]
[CommandSyntax("<[id | name | guid>")]
public class VehicleCommand : Command
{
    public VehicleCommand(CommandContext context) : base(context)
    {
    }

    public bool GetVehicleAsset(string input, out VehicleAsset? vehicleAsset)
    {
        input = input.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            vehicleAsset = null;
            return false;
        }

        List<VehicleAsset> vehicleAssetsList = new();
        Assets.find(vehicleAssetsList);
        
        if (ushort.TryParse(input, out ushort id))
        {
            if (id == 0)
            {
                vehicleAsset = null;
                return false;
            }

            vehicleAsset = vehicleAssetsList.FirstOrDefault(i => i.id == id);
            return vehicleAsset != null;
        }

        vehicleAsset = vehicleAssetsList.FirstOrDefault(i =>
            i.vehicleName.Contains(input, StringComparison.InvariantCultureIgnoreCase) ||
            i.name.Contains(input, StringComparison.InvariantCultureIgnoreCase) ||
            i.FriendlyName.Contains(input, StringComparison.InvariantCultureIgnoreCase));

        return vehicleAsset != null;
    }
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("vehicle");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        Context.AssertPlayer(out UnturnovPlayer self);

        if (!GetVehicleAsset(Context.Current, out VehicleAsset? vehicleAsset))
        {
            throw Context.Reply(TranslationList.VehicleNotFound);
        }
            
        VehicleTool.SpawnVehicleForPlayer(self.Player, vehicleAsset!);
        throw Context.Reply(TranslationList.SpawningVehicle, vehicleAsset!.FriendlyName);
    }
}
