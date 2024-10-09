using System.Reflection;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Logging;
using Unturnov.Core.Roles;
using Unturnov.Core.Players;
using System.Runtime.InteropServices;

namespace Unturnov.Core;

public sealed class UnturnovHost 
{
    private ILogger? _Logger;
    private Harmony? _Harmony;

    private GameObject? _Owner;

    public static IConfiguration Configuration {get; private set;} = null!;

    private async UniTask CreateFileAsync()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        const string path = "Unturnov.Core.Configuration.json";
        using StreamReader reader = new(assembly.GetManifestResourceStream(path));

        string content = await reader.ReadToEndAsync();

        await using StreamWriter writer = new("Configuration.json");
        await writer.WriteAsync(content);
    }

    private async UniTask<IConfiguration> CreateConfigurationAsync()
    {
        if (!File.Exists("Configuration.json"))
        {
            await CreateFileAsync();
        }

        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("Configuration.json");
        return configurationBuilder.Build();
    }

    public async UniTask LoadAsync()
    {
        Directory.CreateDirectory(AppContext.BaseDirectory + "/Unturnov");
        Directory.SetCurrentDirectory(AppContext.BaseDirectory + "/Unturnov");
        Configuration = await CreateConfigurationAsync();

        string level = UnturnovHost.Configuration.GetValue<string>("LoggingLevel") ?? "None";
        LogLevel allowedLevel = Enum.Parse<LogLevel>(level);

        LoggerProvider.AddLogging(new UnturnovLoggerProvider($"./Logs/Log.log"));
        _Logger = LoggerProvider.CreateLogger<UnturnovHost>()!;
        _Logger.LogInformation("Starting Unturnov...");

        ThreadConsole console = new();
        Dedicator.commandWindow?.removeDefaultIOHandler();
        Dedicator.commandWindow?.addIOHandler(console);

        // Static ctor moment
        UnturnovPlayerManager.Players.Count();

        _Harmony = new("Unturnov.Core");
        _Harmony.PatchAll();

        _Owner = new("Unturnov");
        _Owner.AddComponent<MainThreadWorker>();

        CommandManager.RegisterCommandTypes(Assembly.GetExecutingAssembly());
        await RoleManager.RegisterRoles();

        _Logger.LogInformation("Started Unturnov!");
    }

    public async UniTask UnloadAsync()
    { 
        _Harmony?.UnpatchAll();
    }
}
