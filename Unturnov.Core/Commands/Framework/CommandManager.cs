using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Unturnov.Core.Logging;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands.Framework;

public class CommandManager
{
    private static ConcurrentDictionary<string, CommandTypeData> _CommandTypes;
    private static readonly ILogger _Logger;

    static CommandManager()
    {
        _Logger = LoggerProvider.CreateLogger<CommandManager>();
        ThreadConsole.OnInputCommitted += OnInput;
        _CommandTypes = new();
    }

    private static void TryRegisterCommand(string name, CommandTypeData type)
    {
        string fixedName = name.ToLower();
        if (_CommandTypes.TryAdd(fixedName, type))
        {
            _Logger.LogInformation($"Registered command {fixedName}");
            return;
        }

        if (_CommandTypes.TryGetValue(name, out CommandTypeData owner))
        {
            _Logger.LogWarning($"Tried to register duplicate command name from {type.OwnerType.FullName}; originally registered from {owner.OwnerType.FullName}");
            return;
        }

        _Logger.LogWarning($"Something failed trying to register command alias");
    }

    public static void RegisterCommandTypes(Assembly assembly)
    {
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            if (type.BaseType != typeof(Command))
            {
                continue;
            }

            CommandDataAttribute? commandData = type.GetCustomAttribute<CommandDataAttribute>();
            if (commandData == null)
            {
                continue;
            }

            CommandParentAttribute? parent = type.GetCustomAttribute<CommandParentAttribute>();
            if (parent != null)
            {
                continue;
            }

            CommandTypeData data = new(type, assembly);
            TryRegisterCommand(commandData.Name, data);
            foreach (string name in commandData.Aliases)
            {
                TryRegisterCommand(name, data);
            }
        }
    }

    public static async void ExecuteCommand(string commandText, IPlayer caller)
    {
        CommandTokenizer parser = new(commandText);
        IEnumerable<string> arguments = parser.Parse();

        string name = arguments.First();
        if (!_CommandTypes.TryGetValue(name, out CommandTypeData typeData))
        {
            caller.SendMessage($"There is no command called {name}");
            return;
        }

        Type commandType = typeData.GetCommand(arguments, out int depth);
        arguments = arguments.Skip(1 + depth);

        CommandContext context = new(commandType, arguments, caller);
        Command command = (Command)Activator.CreateInstance(commandType, args: context);
        _Logger.LogInformation($"Executing command [{caller.LogName}]: {commandText}");
        try
        {
            await command.Execute();
        }
        catch (CommandExitedException)
        {
        }
        catch (UserMessageException message)
        {
            caller.SendMessage(message.PlayerMessage);
        }
        catch (Exception exception)
        {
            _Logger.LogError(exception, "Failed to execute command");
            _Logger.LogError(exception.ToString());
        }
    }

    private static void OnInput(string message)
    {
        ExecuteCommand(message, new ConsolePlayer());
    }
}
