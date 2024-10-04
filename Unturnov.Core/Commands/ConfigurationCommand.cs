using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Configuration;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Commands;

[CommandData("configuration", "config")]
public class ConfigurationCommand : Command
{
    private readonly ILogger _Logger;

    public ConfigurationCommand(CommandContext context) : base(context)
    {
        _Logger = LoggerProvider.CreateLogger<ConfigurationCommand>();
    }

    public override UniTask Execute()
    {
        IConfigurationRoot root = ((IConfigurationRoot)UnturnovHost.Configuration);

        root.Reload();
        ConfigurationEvents.OnConfigurationReloaded?.Invoke();

        throw Context.Reply("Reloaded configuration");
    }
}
