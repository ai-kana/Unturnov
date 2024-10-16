using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Unturnov.Core.Configuration;

namespace Unturnov.Core.Logging;

public sealed class UnturnovLogger : ILogger
{
    private string _Name;
    private LoggerQueue _Queue;
    private LogLevel AllowedLevel;

    internal UnturnovLogger(string name, LoggerQueue queue) 
    {
        _Name = name;
        _Queue = queue;

        SetLogLevel();
        ConfigurationEvents.OnConfigurationReloaded += OnConfigurationReloaded;
    }

    ~UnturnovLogger()
    {
        ConfigurationEvents.OnConfigurationReloaded -= OnConfigurationReloaded;
    }

    private void SetLogLevel()
    {
        string level = UnturnovHost.Configuration.GetValue<string>("LoggingLevel") ?? "None";
        AllowedLevel = (LogLevel)Enum.Parse(typeof(LogLevel), level);
    }

    private void OnConfigurationReloaded()
    {
        SetLogLevel();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel > AllowedLevel;
    }

    private string GetLevelTag(LogLevel level) => level switch 
    {
        LogLevel.Trace => "TRC",
        LogLevel.Debug => "DBG",
        LogLevel.Information => "INF",
        LogLevel.Warning => "WRN",
        LogLevel.Error => "ERR",
        LogLevel.Critical => "CRT",
        _ => "IDK IT BROKE"
    };

    private const string Red = "\u001b[31m";
    private const string Yellow = "\u001b[33m";
    private const string White = "\u001b[37m";
    private string GetLevelColor(LogLevel level) => level switch
    {
        LogLevel.Error => Red,
        LogLevel.Critical => Red,
        LogLevel.Warning => Yellow,
        _ => White
    };

    private const string MessageFormat = "[{0}] [{1}] [{2}] -> {3}";
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        Exception? current = exception;
        StringBuilder builder = new();
        while (current != null)
        {
            builder.Append(current);
            current = current.InnerException;
        }
    
        string formatted = state + builder.ToString();
        string color = GetLevelColor(logLevel);
        string tag = GetLevelTag(logLevel);
        string fileMessage = string.Format(MessageFormat, DateTime.Now, tag, _Name, formatted);
        string consoleMessage = string.Format(MessageFormat, DateTime.Now, color + tag + White, _Name, formatted);

        _Queue.Enqueue(new(consoleMessage, fileMessage));
    }
}
