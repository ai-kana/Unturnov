using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Services;

// Possibly a questionable design choice but I see this as a more functional way to use this concept
public static class ServiceProvider
{
    private static ConcurrentDictionary<Type, object> _Services = new();
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

    public static bool RegisterService<T>(T instance) where T : notnull
    {
        return _Services.TryAdd(typeof(T), instance);
    }

    public static T? GetService<T>()
    {
        if (!_Services.TryGetValue(typeof(T), out object value))
        {
            return default(T);
        }

        return (T)value;
    }

    public static T GetRequiredService<T>()
    {
        if (!_Services.TryGetValue(typeof(T), out object value))
        {
            throw new KeyNotFoundException($"Service registed as {typeof(T).FullName} does not exist");
        }

        return (T)value;
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
