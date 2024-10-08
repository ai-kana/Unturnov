using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("exit")]
[CommandSyntax("")]
public class ExitCommand : Command
{
    public ExitCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("exit");
        Context.AssertPlayer(out UnturnovPlayer self);
        self.Exit();
        throw Context.Exit;
    }
}