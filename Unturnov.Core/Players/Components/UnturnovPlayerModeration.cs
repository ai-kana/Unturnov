using Cysharp.Threading.Tasks;
using Steamworks;
using Unturnov.Core.Offenses;

namespace Unturnov.Core.Players.Components;

public class UnturnovPlayerModeration
{
    public readonly UnturnovPlayer Owner;

    public UnturnovPlayerModeration(UnturnovPlayer owner)
    {
        Owner = owner;
    }

    public async UniTask<IEnumerable<Offense>> GetAllOffenses()
    {
        return await OffenseManager.GetOffenses(Owner.SteamID);
    }

    public async UniTask<IEnumerable<Offense>> GetWarns()
    {
        return await OffenseManager.GetWarnOffenses(Owner.SteamID);
    }

    public void AddWarn(CSteamID issuer, string reason)
    {
        _ = OffenseManager.AddOffense(Offense.Create(OffenseType.Warn, Owner.SteamID, issuer, reason, 0));
    }

    public void AddMute(CSteamID issuer, long duration, string reason)
    {
        _ = OffenseManager.AddOffense(Offense.Create(OffenseType.Mute, Owner.SteamID, issuer, reason, duration));
    }

    public void AddBan(CSteamID issuer, long duration, string reason)
    {
        _ = OffenseManager.AddOffense(Offense.Create(OffenseType.Ban, Owner.SteamID, issuer, reason, duration));
    }
}
