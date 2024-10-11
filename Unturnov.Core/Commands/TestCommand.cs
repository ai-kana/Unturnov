using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Formatting;

namespace Unturnov.Core.Commands;

[CommandData("test")]
public class TestCommand : Command
{
    public TestCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("all");
        throw Context.Reply(Formatter.FormatTime(100).Translate("English"));
    }
}
