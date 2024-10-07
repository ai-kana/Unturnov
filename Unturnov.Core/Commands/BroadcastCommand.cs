using Cysharp.Threading.Tasks;
using Unturnov.Core.Chat;
using Unturnov.Core.Commands.Framework;

namespace Unturnov.Core.Commands;

[CommandData("broadcast")]
public class BroadcastCommand : Command
{
    public BroadcastCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("broadcast");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        string message = Context.Form();
        UnturnovChat.BroadcastMessage(message);
        throw Context.Exit;
    }
}
