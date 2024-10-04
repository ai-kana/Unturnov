using System.Reflection;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Logging;

namespace Unturnov.Core;

public sealed class UnturnovHost 
{
    private ILogger? _Logger;
    private Harmony? _Harmony;

    private GameObject? _Owner;

    public static string WorkingDirectory {get; private set;} = "";
    public static IConfiguration Configuration {get; private set;} = null!;

    private async UniTask CreateFile()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        const string path = "Unturnov.Core.Configuration.json";
        using StreamReader reader = new(assembly.GetManifestResourceStream(path));

        string content = await reader.ReadToEndAsync();

        await using StreamWriter writer = new(WorkingDirectory + "/Configuration.json");
        await writer.WriteAsync(content);
    }

    private async UniTask<IConfiguration> CreateConfiguration()
    {
        if (!File.Exists(WorkingDirectory + "/Configuration.json"))
        {
            await CreateFile();
        }

        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.SetBasePath(WorkingDirectory);
        configurationBuilder.AddJsonFile("Configuration.json");
        return configurationBuilder.Build();
    }

    public async UniTask LoadAsync()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        WorkingDirectory = Directory.GetCurrentDirectory() + "/Unturnov";

        Configuration = await CreateConfiguration();

        string level = UnturnovHost.Configuration.GetValue<string>("LoggingLevel") ?? "None";
        LogLevel allowedLevel = Enum.Parse<LogLevel>(level);

        LoggerProvider.AddLogging(new UnturnovLoggerProvider($"{WorkingDirectory}/Logs/Log.log"));
        _Logger = LoggerProvider.CreateLogger<UnturnovHost>()!;
        _Logger.LogInformation("Starting Unturnov...");
        _Logger.LogInformation(WorkingDirectory);

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
