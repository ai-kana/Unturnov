using System.Reflection;

namespace Unturnov.Core.Commands.Framework;

public class CommandTypeData
{
    public readonly Type OwnerType;
    private readonly List<SubCommandData> SubCommands;

    private Type? RecurseSubCommands(IEnumerator<string> enumerator, ref int depth)
    {
        foreach (SubCommandData data in SubCommands)
        {
            if (data.Switches.Contains(enumerator.Current.ToLower()))
            {
                if (!enumerator.MoveNext())
                {
                    return data.Type;
                }

                depth++;
                RecurseSubCommands(enumerator, ref depth);
            }
        }

        return null;
    }

    public Type GetCommand(IEnumerable<string> arguments, out int depth)
    {
        depth = 0;

        IEnumerator<string> enumerator = arguments.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            return OwnerType;
        }

        foreach (SubCommandData data in SubCommands)
        {
            Type? type = RecurseSubCommands(enumerator, ref depth);
            if (type == null)
            {
                continue;
            }

            return type;
        }

        return OwnerType;
    }

    private void FillSubCommandData(SubCommandData commandData)
    {
        SubCommandDataAttribute attribute = commandData.Type.GetCustomAttribute<SubCommandDataAttribute>();
        if (attribute == null)
        {
            return;
        }

        if (commandData.Type.BaseType != typeof(Command))
        {
            return;
        }

        commandData.Switches.Add(attribute.Name);
        commandData.Switches.AddRange(attribute.Aliases);

        foreach (Type type in commandData.Type.GetNestedTypes())
        {
            SubCommandData data = new(type);
            FillSubCommandData(data);
            commandData.Children.Add(data);
        }
    }

    internal CommandTypeData(Type type)
    {
        OwnerType = type;

        Type[] nestedTypes = type.GetNestedTypes();
        SubCommands = new(nestedTypes.Count());

        foreach (Type nestedType in nestedTypes)
        {
            SubCommandData data = new(nestedType);
            FillSubCommandData(data);
        }
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
