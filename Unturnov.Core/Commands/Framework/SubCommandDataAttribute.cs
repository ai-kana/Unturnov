namespace Unturnov.Core.Commands.Framework;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SubCommandDataAttribute : Attribute
{
    public readonly string Name; 
    public readonly string[] Aliases; 

    public SubCommandDataAttribute(string name, params string[] aliases)
    {
        Name = name;
        Aliases = aliases;
    }
}
