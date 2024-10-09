using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

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
            i.name.Contains(input, StringComparison.InvariantCultureIgnoreCase));

        return vehicleAsset != null;
    }
    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("vehicle");
        Context.AssertArguments(1);
        Context.AssertPlayer(out UnturnovPlayer self);

        if (!GetVehicleAsset(Context.Current, out VehicleAsset? vehicleAsset))
        {
            throw Context.Reply("Vehicle not found");
        }
            
        VehicleTool.SpawnVehicleForPlayer(self.Player, vehicleAsset!);
        throw Context.Reply("Spawning {0}", vehicleAsset!.FriendlyName);
    }
}