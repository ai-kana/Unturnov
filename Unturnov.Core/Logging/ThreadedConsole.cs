using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using SDG.Unturned;

namespace Unturnov.Core.Logging;

public delegate void InputCommited(string message);

public class ThreadConsole : ICommandInputOutput
{
    private ILogger? _Logger;

    private Thread? _Thread;
    private bool _IsExiting;

    // OOP moment
#pragma warning disable CS0067
    public event CommandInputHandler? inputCommitted;
#pragma warning restore CS0067
    public static event InputCommited? OnInputCommitted;

    public void initialize(CommandWindow commandWindow)
    {
        // Blatantly ripped from OM
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CommandLineFlag? shouldManageConsole = null;
            bool previousShouldManageConsoleValue = true;

            Type windowsConsole = typeof(Provider).Assembly.GetType("SDG.Unturned.WindowsConsole");
            FieldInfo? shouldManageConsoleField = windowsConsole?.GetField("shouldManageConsole", BindingFlags.Static | BindingFlags.NonPublic);

            if (shouldManageConsoleField != null)
            {
                shouldManageConsole = (CommandLineFlag)shouldManageConsoleField.GetValue(null);
                previousShouldManageConsoleValue = shouldManageConsole.value;
                shouldManageConsole.value = false;
            }
        }

        commandWindow.title = "Unturnov";
        _Logger = LoggerProvider.CreateLogger("SDG.Unturned");

        CommandWindow.shouldLogChat = false;
        CommandWindow.shouldLogDeaths = false;
        CommandWindow.shouldLogAnticheat = false;
        CommandWindow.shouldLogJoinLeave = false;

        ServerManager.OnPreShutdown += OnPreShutdown;

        Console.CancelKeyPress += OnCancelling;

        UTF8Encoding encoding = new UTF8Encoding(true);
        Console.OutputEncoding = encoding;
        Console.InputEncoding = encoding;

        _Thread = new(InputThreadLoop);
        _Thread.Start();
    }

    private void OnPreShutdown()
    {
        shutdown(null!);
    }

    public void outputError(string error)
    {
        _Logger?.LogError(error);
    }

    public void outputInformation(string information)
    {
        _Logger?.LogInformation(information);
    }

    public void outputWarning(string warning)
    {
        _Logger?.LogWarning(warning);
    }

    public void shutdown(CommandWindow commandWindow)
    {
        _IsExiting = true;
        _Thread?.Join();
    }

    public void update()
    {
    }

    private void OnCancelling(object sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        ServerManager.Shutdown();
    }

    private void HandleInput() 
    {
        if (!Console.KeyAvailable)
        {
            return;
        }

        string text = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(text))
        {
            OnInputCommitted?.Invoke(text);
        }
    }

    private void InputThreadLoop() 
    {
        while (true) 
        {
            if (_IsExiting)
            {
                return;
            }

            HandleInput();
            Thread.Sleep(50);
        }
    }
}
