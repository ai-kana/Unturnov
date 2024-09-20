using Unturnov.Core.Players;
using Unturnov.Core.Services;

namespace Unturnov.Core.Commands.Framework;

public sealed class CommandContext
{
    public IPlayer Caller {get; private set;}
    private readonly IEnumerator<string> _Enumerator; 
    private readonly IEnumerable<string> _Arguments; 
    private readonly CommandParser _Parser;

    public CommandExitedException Reply(object text)
    {
        Caller.SendMessage(text);
        return new();
    }

    public CommandExitedException Reply(string format, params object[] args)
    {
        Caller.SendMessage(format, args);
        return new();
    }

    public CommandExitedException Exit => new();

    public bool HasArguments(int count)
    {
        return count < _Arguments.Count();
    }

    public bool HasExactArguments(int count)
    {
        return count == _Arguments.Count();
    }

    public void AssertArguments(int count)
    {
        if (count < _Arguments.Count())
        {
            throw Reply("This command requires {0} arguments", count);
        }
    }

    private const string AssertPlayerFailed = "This command can only be execute by players";
    public void AssertPlayer()
    {
        if (Caller is not UnturnovPlayer)
        {
            throw Reply(AssertPlayerFailed);
        }
    }

    public void AssertPlayer(out UnturnovPlayer caller)
    {
        if (Caller is not UnturnovPlayer player)
        {
            throw Reply(AssertPlayerFailed);
        }

        caller = player;
    }

    public T Parse<T>()
    {
        return _Parser.Parse<T>(_Enumerator.Current);
    }

    public bool TryParse<T>(out T result)
    {
        return _Parser.TryParse<T>(Current, out result);
    }

    public string Current => _Enumerator.Current;

    public bool MoveNext()
    {
        return _Enumerator.MoveNext();
    }

    public void Reset()
    {
        _Enumerator.Reset();
    }

    internal CommandContext(IEnumerable<string> arguments, IPlayer caller)
    {
        _Enumerator = _Arguments.GetEnumerator() ;
        _Parser = ServiceProvider.GetRequiredService<CommandParser>();
        _Enumerator.MoveNext();
        Caller = caller;
    }
}
