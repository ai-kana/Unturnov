using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;

namespace Unturnov.Core.Commands;

[CommandData("shutdown")]
public class ShutdownCommand : Command
{
    public ShutdownCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("shutdown");
        ServerManager.Shutdown();
        throw Context.Reply("Shutting server down");
    }
}
