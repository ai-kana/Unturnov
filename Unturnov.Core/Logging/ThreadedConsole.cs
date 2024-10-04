using Microsoft.Extensions.Logging;
using SDG.Unturned;
using Unturnov.Core.Logging;

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
        _Logger = LoggerProvider.CreateLogger("SDG.Unturned");

        // Not sure if this is needed but it dont matter
        CommandWindow.shouldLogChat = false;
        CommandWindow.shouldLogDeaths = false;
        CommandWindow.shouldLogAnticheat = false;
        CommandWindow.shouldLogJoinLeave = false;

        ServerManager.OnPreShutdown += OnPreShutdown;

        Console.CancelKeyPress += OnCancelling;

        _Thread = new(InputThreadLoop);
        _Thread.Start();
    }

    private void OnPreShutdown()
    {
        _IsExiting = true;
        _Thread?.Join();
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
        // Def do something here :D
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
