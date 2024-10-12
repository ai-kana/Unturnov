using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Configuration;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Commands.StaffCommands;

[CommandData("configuration", "config")]
public class ConfigurationCommand : Command
{
    private readonly ILogger _Logger;

    public ConfigurationCommand(CommandContext context) : base(context)
    {
        _Logger = LoggerProvider.CreateLogger<ConfigurationCommand>();
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("staff");
        Context.AssertCooldown();

        IConfigurationRoot root = ((IConfigurationRoot)UnturnovHost.Configuration);

        root.Reload();
        ConfigurationEvents.OnConfigurationReloaded?.Invoke();

        Context.AddCooldown(120);
        throw Context.Reply("Reloaded configuration");
    }
}
