using Unturnov.Core.Formatting;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Commands.Framework;

public sealed class CommandContext
{

    public IPlayer Caller {get; private set;}
    private readonly IEnumerator<string> _Enumerator; 
    private readonly IEnumerable<string> _Arguments; 
    private readonly Type _Type;

    internal CommandContext(Type type, IEnumerable<string> arguments, IPlayer caller)
    {
        _Type = type;
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

    public CommandExitedException Reply(Translation translation, params object[] args)
    {
        Caller.SendMessage(translation.Translate(Caller.Language), args);
        return new();
    }

    public CommandExitedException Exit => new();

    public bool HasArguments(int count)
    {
        return count <= _Arguments.Count();
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

        if (!player.Permissions.HasPermission(permission))
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

    public void AssertCooldown()
    {
        if (Caller is not UnturnovPlayer player)
        {
            return;
        }

        long time = player.Cooldowns.GetCooldown(_Type.FullName);
        if (time == 0)
        {
            return;
        }

        throw Reply("You cannot use this command for {0}", Formatter.FormatTime(time));
    }

    public void AssertOnDuty()
    {
        if (Caller is not UnturnovPlayer player)
        {
            return;
        }

        if (!player.OnDuty)
        {
            throw Reply("You must be on duty to use this command");
        }
    }

    public void AddCooldown(long length)
    {
        if (Caller is not UnturnovPlayer player)
        {
            return;
        }

        player.Cooldowns.AddCooldown(_Type.FullName, length);
    }

    public bool MatchParameter(params string[] matches)
    {
        return matches.Contains(Current, StringComparer.OrdinalIgnoreCase);
    }

    public T Parse<T>()
    {
        return CommandParser.Parse<T>(_Enumerator);
    }

    public bool TryParse<T>(out T result)
    {
        return CommandParser.TryParse<T>(_Enumerator, out result);
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

    public string Form()
    {
        if (Current == null)
        {
            return string.Empty;
        }
        List<string> args = new();
        args.Add(Current);
        while (MoveNext())
        {
            args.Add(Current);
        }
        return string.Join(" ", args);
    }
}
