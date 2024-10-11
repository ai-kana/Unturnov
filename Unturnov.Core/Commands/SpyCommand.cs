using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("spy")]
[CommandSyntax("<[player]>")]
public class SpyCommand : Command
{
    public SpyCommand(CommandContext context) : base(context)
    {
        
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("spy");
        Context.AssertOnDuty();
        Context.AssertArguments(1);
        
        Context.AssertPlayer(out UnturnovPlayer self);
        UnturnovPlayer target = Context.Parse<UnturnovPlayer>();
        
        target.Spy(self.SteamID);
        throw Context.Reply(TranslationList.SpyingOn, target.Name);
    }
}