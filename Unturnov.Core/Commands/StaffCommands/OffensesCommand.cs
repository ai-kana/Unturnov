using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Offenses;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("offenses")]
[CommandSyntax("<[player] [bans,b | warns,w | mutes,m?]>")]
public class OffensesCommand : Command
{
    public OffensesCommand(CommandContext context) : base(context)
    {
    }

    public override async UniTask ExecuteAsync()
    {
        Context.AssertPermission("offenses");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        if (Context.HasExactArguments(1))
        {
            IEnumerable<Offense> offensesI = await OffenseManager.GetOffenses(target.SteamID);
            List<Offense> offenses =  offensesI.Take(5).ToList();
            
            if (offenses.Count == 0)
            {
                throw Context.Reply(TranslationList.NoOffenses, target.Name);
            }
            
            Context.Reply(TranslationList.OffenseList, target.Name);
            foreach (Offense offense in offenses)
            {
                Context.Reply(TranslationList.Offense, offense.Id, offense.Reason);
            }

            throw Context.Exit;
        }
        
        Context.MoveNext();
        string type = Context.Form().ToLower();

        List<Offense> offensesList;
        IEnumerable<Offense> enumerableOffenses;
        Translation offenseType;

        switch (type)
        {
            case "warn": case "w": case "warns": 
                enumerableOffenses = await OffenseManager.GetWarnOffenses(target.SteamID);
                offenseType = TranslationList.Warns;
                break;
            case "ban": case "b": case "bans":
                enumerableOffenses = await OffenseManager.GetBanOffenses(target.SteamID);
                offenseType = TranslationList.Bans;
                break;
            case "mute": case "m": case "mutes":
                enumerableOffenses = await OffenseManager.GetMuteOffenses(target.SteamID);
                offenseType = TranslationList.Mutes;
                break;
            default:
                throw Context.Reply("<[player] [bans,b | warns,w | mutes,m]>");
        }
        
        offensesList = enumerableOffenses.OrderByDescending(w => w.Id).Take(5).ToList();
        
        if(offensesList.Count == 0)
        {
            throw Context.Reply(TranslationList.NoOffenses, target.Name);
        }
        
        Context.Reply(TranslationList.OffenseType, new TranslationPackage(offenseType), target.Name);
        
        offensesList.ForEach(offense => 
            Context.Reply(TranslationList.Offense, offense.Id, offense.Reason)
        );

        throw Context.Exit;
    }
}