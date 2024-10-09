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

    private bool GetVehicleAssetByName(string name, out VehicleAsset? vehicleAsset)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            vehicleAsset = null;
            return false;
        }
        
        List<VehicleAsset> vehicleAssetsList = new();
        Assets.find(vehicleAssetsList);
        vehicleAssetsList = vehicleAssetsList.OrderBy(v => v.id).ToList();
        
        foreach (var vAsset in vehicleAssetsList)
        {
            if(vAsset.vehicleName.Contains(name, StringComparison.InvariantCultureIgnoreCase) ||
               vAsset.FriendlyName.Contains(name, StringComparison.InvariantCultureIgnoreCase))
            {
                vehicleAsset = vAsset;
                return true;
            }
        }

        vehicleAsset = null;
        return false;
    } 

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("vehicle");
        Context.AssertArguments(1);
        Context.AssertPlayer(out UnturnovPlayer self);
        
        Asset vehicleAsset = null;
        var idSuccess = Context.TryParse(out ushort vId);
        
        if (idSuccess && vehicleAsset == null)
        {
            vehicleAsset = Assets.find(EAssetType.VEHICLE, vId);
        }
        
        if (Context.TryParse(out Guid vGuid) && vehicleAsset == null)
        {
            vehicleAsset = Assets.find<VehicleAsset>(vGuid);
        }
        
        if (GetVehicleAssetByName(Context.Current, out VehicleAsset? vAssetByName) && vehicleAsset == null && !idSuccess)
        {
            vehicleAsset = vAssetByName!;
        }
        
        if (vehicleAsset == null)
        {
            throw Context.Reply("Vehicle not found");
        }
        
        VehicleTool.SpawnVehicleForPlayer(self.Player, vehicleAsset);
        throw Context.Reply("Spawning {0}", vehicleAsset.FriendlyName);
    }
}