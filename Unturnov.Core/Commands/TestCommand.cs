using System.Reflection;
using Cysharp.Threading.Tasks;
using SDG.NetPak;
using Unturnov.Core.Commands.Framework;

namespace Unturnov.Core.Commands;

[CommandData("test")]
public class TestCommand : Command
{
    public NetPakReader Reader = 
        (NetPakReader)
        Assembly.GetAssembly(typeof(SDG.Unturned.NetId)).GetType("NetMessages").GetField("reader", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

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

        [SubCommandData("amzing", "meow")]
        private class SubCommand2 : Command
        {
            public SubCommand2(CommandContext context) : base(context)
            {
            }

            public override UniTask Execute()
            {
                Context.AssertArguments(1);
                throw Context.Reply(Context.Current);
            }
        }
    }
}
