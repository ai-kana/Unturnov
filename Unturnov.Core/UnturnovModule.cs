using SDG.Framework.Modules;

namespace Unturnov.Core;

public sealed class UnturnovModule : IModuleNexus
{
    private UnturnovHost? _Host;

    public async void initialize()
    {
        try
        {
            _Host = new();
            await _Host.LoadAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async void shutdown()
    {
        if (_Host == null) 
        {
            return;
        }

        await _Host.UnloadAsync();
    }
}
