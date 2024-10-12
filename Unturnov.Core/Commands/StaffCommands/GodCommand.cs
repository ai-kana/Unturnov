using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("god")]
[CommandSyntax("<[off | on] [player?]>")]
public class GodCommand : Command
{
    public GodCommand(CommandContext context) : base(context)
    {
    }
    
    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("god");
        Context.AssertOnDuty();

        bool state;
        if (Context.HasArguments(1))
        {
            Context.AssertPermission("god.other");
            UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
            state = player.Administration.ToggleGod();
            
            throw Context.Reply(TranslationList.GodModeOther, player.Name, state ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
        }

        Context.AssertPlayer(out UnturnovPlayer self);
        state = self.Administration.ToggleGod();

        throw Context.Reply(TranslationList.GodModeSelf, state ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
    }
}
