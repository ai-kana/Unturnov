using Cysharp.Threading.Tasks;
using Unturnov.Core.Commands.Framework;

namespace Unturnov.Core.Commands;

[CommandData("test")]
public class TestCommand : Command
{
    public TestCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask Execute()
    {
        throw Context.Reply("Normal:C");
    }

    [SubCommandData("waow")]
    private class SubCommand : Command
    {
        public SubCommand(CommandContext context) : base(context)
        {
        }

        public override UniTask Execute()
        {
            throw Context.Reply("Sub command reply!");
        }
    }
}
