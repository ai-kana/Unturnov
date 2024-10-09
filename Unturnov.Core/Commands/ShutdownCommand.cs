using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;

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
        Context.AssertOnDuty();
        Context.AssertPermission("shutdown");

        uint delay = 0;
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
        Context.AssertOnDuty();
        Context.AssertPermission("shutdown");

        if (!ServerManager.CancelShutdown())
        {
            throw Context.Reply("Server is not shutting down");
        }

        UnturnovChat.BroadcastMessage("Cancelled server shutdown");
        throw Context.Exit;
    }
}
