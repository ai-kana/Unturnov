namespace Unturnov.Core.Commands.Framework;

internal interface IArgumentParser
{
    public Type Type {get;} 
    public bool TryParseArgument(string argument, out object? result);
}
