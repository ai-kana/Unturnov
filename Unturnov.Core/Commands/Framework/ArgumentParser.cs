namespace Unturnov.Core.Commands.Framework;

public abstract class ArgumentParser<T> : IArgumentParser
{
    public Type Type => typeof(T);
    public abstract bool TryParse(string argument, out T result);

    public bool TryParseArgument(string argument, out object? result)
    {
        bool ret = TryParse(argument, out T r );
        result = r;
        return ret;
    }
}
