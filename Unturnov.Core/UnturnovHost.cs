using System.Reflection;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Logging;
using Unturnov.Core.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Unturnov.Core;

public sealed class UnturnovHost 
{
    private ILogger? _Logger;
    private Harmony? _Harmony;

    private GameObject? _Owner;

    public static string WorkingDirectory {get; private set;} = "";
    public IConfiguration Configuration {get; private set;} = null!;

    private string _ConfigurationPath = ""; 

    private async UniTask CreateFile()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string path = "Unturnov.Core.Configuration.json";
        using StreamReader reader = new(assembly.GetManifestResourceStream(path));

        string content = await reader.ReadToEndAsync();

        await using StreamWriter writer = new(_ConfigurationPath);
        await writer.WriteAsync(content);
    }

    private async UniTask<IConfiguration> CreateConfiguration()
    {
        if (!File.Exists(WorkingDirectory + "/Config.json"))
        {
            await CreateFile();
        }

        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.SetBasePath(WorkingDirectory);
        configurationBuilder.AddJsonFile("Config.json");
        return configurationBuilder.Build();
    }

    public async UniTask LoadAsync()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        WorkingDirectory = Directory.GetCurrentDirectory() + "/Unturnov";
        _ConfigurationPath = WorkingDirectory + "/Config.json";

        ServiceProvider.AddLogging(new UnturnovLoggerProvider($"{WorkingDirectory}/Logs/Log.log"));
        _Logger = ServiceProvider.CreateLogger<UnturnovHost>()!;
        _Logger.LogInformation("Starting Unturnov...");
        _Logger.LogInformation(WorkingDirectory);
        
        Configuration = await CreateConfiguration();

        ThreadConsole console = new();
        Dedicator.commandWindow?.removeDefaultIOHandler();
        Dedicator.commandWindow?.addIOHandler(console);

        _Harmony = new("Unturnov.Core");
        _Harmony.PatchAll();

        _Owner = new("Unturnov");
        _Owner.AddComponent<MainThreadWorker>();

        CommandManager commandManager = new();
        commandManager.RegisterCommandTypes(Assembly.GetExecutingAssembly());

        _Logger.LogInformation("Started Unturnov!");
    }

    public async UniTask UnloadAsync()
    { 
        _Harmony?.UnpatchAll();
    }
}
