using Cysharp.Threading.Tasks;

namespace Unturnov.Core.Commands.Framework;

public abstract class Command
{
    public CommandContext Context {get;}
    public abstract UniTask ExecuteAsync();

    internal Command(CommandContext context)
    {
        Context = context;
    }
}
