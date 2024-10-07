using SDG.Unturned;

namespace Unturnov.Core;

public delegate void PreShutdown();
public delegate void ServerSave();

public static class ServerManager
{
    public static PreShutdown? OnPreShutdown;
    public static ServerSave? OnServerSave;

    private static void DoSave()
    {
        OnServerSave?.Invoke();
        SaveManager.save();
    }

    public static void Save()
    {
        MainThreadWorker.EnqueueSync(DoSave);
    }

    private static void DoShutdown()
    {
        Provider.shutdown();
    }

    public static void Shutdown()
    {
        Save();
        OnPreShutdown?.Invoke();
        Thread.Sleep(1000);
        MainThreadWorker.Enqueue(DoShutdown);
    }
}
