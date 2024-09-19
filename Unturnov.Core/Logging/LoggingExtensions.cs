using Microsoft.Extensions.Logging;

namespace Unturnov.Core.Logging;

public static class LoggingExtensions
{
    public static ILogger CreateLogger<T>(this ILoggerProvider provider)
    {
        return provider.CreateLogger(typeof(T).FullName);
    }
}
