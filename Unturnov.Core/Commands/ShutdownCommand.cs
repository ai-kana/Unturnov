using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands;

[CommandData("shutdown")]
[CommandSyntax("<[delay] | cancel>?")]
public class ShutdownCommand : Command
{
    public ShutdownCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("shutdown");
        Context.AssertOnDuty();

        uint delay = 1;
        if (Context.HasArguments(1))
        {
            delay = Context.Parse<uint>();
        }

        ServerManager.QueueShutdown(delay);
        throw Context.Exit;
    }
}

[CommandParent(typeof(ShutdownCommand))]
[CommandData("cancel")]
public class ShutdownCancelCommand : Command
{
    public ShutdownCancelCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("shutdown");
        Context.AssertOnDuty();

        if (!ServerManager.CancelShutdown())
        {
            throw Context.Reply(TranslationList.ShutdownNotActive);
        }

        UnturnovChat.BroadcastMessage(TranslationList.ShutdownCancelled);
        throw Context.Exit;
    }
}
