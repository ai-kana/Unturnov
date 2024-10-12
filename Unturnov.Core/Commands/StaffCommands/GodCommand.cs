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

        if (Context.HasArguments(1))
        {
            Context.AssertPermission("god.other");
            UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
            player.Life.GodMode = !player.Life.GodMode;
            
            throw Context.Reply(TranslationList.GodModeOther, player.Name, player.Life.GodMode ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
        }

        Context.AssertPlayer(out UnturnovPlayer self);
        self.Life.GodMode = !self.Life.GodMode;
        
        throw Context.Reply(TranslationList.GodModeSelf, self.Life.GodMode ? new TranslationPackage(TranslationList.On) : new TranslationPackage(TranslationList.Off));
    }
}
