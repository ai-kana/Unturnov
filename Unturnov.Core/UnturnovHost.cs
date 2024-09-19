using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Logging;
using Unturnov.Core.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Unturnov.Core;

public sealed class UnturnovHost 
{
    private ILogger? _Logger;
    private Harmony? _Harmony;

    private GameObject? _Owner;

    public async UniTask LoadAsync()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        ServiceProvider.AddLogging(new UnturnovLoggerProvider("./Unturnov/Logs/Log.log"));
        _Logger = ServiceProvider.CreateLogger<UnturnovHost>()!;
        _Logger.LogInformation("Starting Unturnov...");

        ThreadConsole console = new();
        Dedicator.commandWindow?.removeDefaultIOHandler();
        Dedicator.commandWindow?.addIOHandler(console);

        _Harmony = new("Unturnov.Core");
        _Harmony.PatchAll();

        _Owner = new("Unturnov");
        _Owner.AddComponent<MainThreadWorker>();

        _Logger.LogInformation("Started Unturnov!");
    }

    public async UniTask UnloadAsync()
    { 
        _Harmony?.UnpatchAll();
    }
}
