using System.Reflection;
using Microsoft.Extensions.Logging;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Commands.Framework;

public class CommandTypeData
{
    public readonly Type OwnerType;
    private readonly SubCommandData _SubCommands;
    private readonly ILogger _Logger;

    internal CommandTypeData(Type type)
    {
        OwnerType = type;
        _Logger = LoggerProvider.CreateLogger<CommandTypeData>();
        _SubCommands = BuildSubCommandTree(type);
    }

    private SubCommandData GetSubCommand(IEnumerator<string> enumerator, ref int depth)
    {
        SubCommandData current = _SubCommands;
        do 
        {
            SubCommandData? result = current.Children.FirstOrDefault(x => x.Switches.Contains(enumerator.Current.ToLower(), StringComparer.OrdinalIgnoreCase));
            if (result == null)
            {
                return current;
            }

            current = result;
            depth++;
        }
        while (enumerator.MoveNext());

        return current;
    }

    public Type GetCommand(IEnumerable<string> arguments, out int depth)
    {
        depth = 0;
        IEnumerator<string> enumerator = arguments.GetEnumerator();
        if (!enumerator.MoveNext() || !enumerator.MoveNext())
        {
            return _SubCommands.Type;
        }

        return GetSubCommand(enumerator, ref depth).Type;
    }

    private SubCommandData BuildSubCommandTree(Type type)
    {
        SubCommandData root = new(type);
        Type[] nestedTypes = type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.NonPublic);

        foreach (Type nestedType in nestedTypes)
        {
            SubCommandDataAttribute attribute = nestedType.GetCustomAttribute<SubCommandDataAttribute>();
            if (nestedType.BaseType != typeof(Command) || attribute == null)
            {
                continue;
            }

            _Logger.LogInformation($"Added subcommand {nestedType.FullName}");
            SubCommandData child = BuildSubCommandTree(nestedType);
            child.Switches.Add(attribute.Name);
            child.Switches.AddRange(attribute.Aliases);
            root.Children.Add(child);
        }

        return root;
    }

    private class SubCommandData
    {
        public List<string> Switches;
        public Type Type;
        public List<SubCommandData> Children {get; private set;}

        public SubCommandData(Type type)
        {
            Children = new();
            Switches = new();
            Type = type;
        }
    }
}
