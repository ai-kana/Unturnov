using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Chat;
using Unturnov.Core.Formatting;
using Unturnov.Core.Players;
using Unturnov.Core.Translations;

namespace Unturnov.Core;

public delegate void PreShutdown();
public delegate void ServerSave();

public static class ServerManager
{
    public static PreShutdown? OnPreShutdown;
    public static ServerSave? OnServerSave;

    private static CancellationTokenSource? _Source;

    private static void DoSave()
    {
        OnServerSave?.Invoke();
        MainThreadWorker.EnqueueSync(SaveManager.save);
    }

    public static void Shutdown()
    {
        DoSave();
        UnturnovPlayerManager.KickAll("Server shutting down"); //Perhaps translate this message too?
        OnPreShutdown?.Invoke();
        Provider.shutdown(0);
    }

    public static bool CancelShutdown()
    {
        if (_Source == null || _Source.IsCancellationRequested)
        {
            return false;
        }

        _Source.Cancel();
        return true;
    }

    public static void QueueShutdown(uint delay)
    {
        _Source = new();
        _ = DoShutdown(delay, _Source.Token);
    }

    private static async Task DoShutdown(uint delay, CancellationToken token)
    {
        bool first = false;
        UnturnovChat.BroadcastMessage(TranslationList.Shutdown, Formatter.FormatTime(delay));
        for (; delay > 0; delay--)
        {
            if (first)
            {
                switch (delay)
                {
                    case 60:
                    case 30:
                    case 10:
                    case < 5:
                        UnturnovChat.BroadcastMessage(TranslationList.Shutdown, Formatter.FormatTime(delay));
                        break;
                }
            }

            await Task.Delay(1000);
            first = true;

            if (_Source?.IsCancellationRequested ?? true)
            {
                return;
            }
        }


        //Provider.shutdown();
        Shutdown();
    }
}
