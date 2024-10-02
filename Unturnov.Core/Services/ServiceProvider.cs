using Microsoft.Extensions.Logging;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Services;

// Possibly a questionable design choice but I see this as a more functional way to use this concept
public static class ServiceProvider
{
    private static ILoggerProvider? _Provider;

    public static bool AddLogging(ILoggerProvider provider)
    {
        if (_Provider != null)
        {
            return false;
        }

        _Provider = provider;
        return true;
    }

    public static ILogger CreateLogger<T>()
    {
        return _Provider?.CreateLogger<T>() ?? throw new();
    }

    public static ILogger CreateLogger(string name)
    {
        return _Provider?.CreateLogger(name) ?? throw new();
    }

    static ServiceProvider()
    {
        ServerManager.OnPreShutdown += OnPreShutdown;
    }

    private static void OnPreShutdown()
    {
        _Provider?.Dispose();
    }
}
