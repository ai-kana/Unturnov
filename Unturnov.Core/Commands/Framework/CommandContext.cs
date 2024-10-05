using Unturnov.Core.Players;

namespace Unturnov.Core.Commands.Framework;

public sealed class CommandContext
{
    public IPlayer Caller {get; private set;}
    private readonly IEnumerator<string> _Enumerator; 
    private readonly IEnumerable<string> _Arguments; 

    internal CommandContext(IEnumerable<string> arguments, IPlayer caller)
    {
        _Arguments = arguments;
        _Enumerator = _Arguments.GetEnumerator();
        _Enumerator.MoveNext();
        Caller = caller;
    }

    public CommandExitedException Reply(object text)
    {
        Caller.SendMessage(text.ToString());
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
        if (count > _Arguments.Count())
        {
            throw Reply("This command requires {0} arguments", count);
        }
    }

    private const string AssertPermissionFailed = "You do not have permission to execute this command";
    public void AssertPermission(string permission)
    {
        if (Caller is not UnturnovPlayer player)
        {
            return;
        }

        if (!player.HasPermission(permission))
        {
            throw Reply(AssertPermissionFailed);
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
        return CommandParser.Parse<T>(_Enumerator.Current);
    }

    public bool TryParse<T>(out T result)
    {
        return CommandParser.TryParse<T>(Current, out result);
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
}
