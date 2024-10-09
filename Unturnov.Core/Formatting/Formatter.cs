using Microsoft.Extensions.Configuration;
using UnityEngine;
using Unturnov.Core.Configuration;

namespace Unturnov.Core.Formatting;

public static class Formatter
{
    public static Color32 MainColor = new(0xF5, 0xA9, 0xB8, 0);
    public static Color32 FormatColor = new(0x5B, 0xCE, 0xFE, 0);
    public static Color32 RedColor = new(0xFF, 0x00, 0x00, 0);

    static Formatter()
    {
        ConfigurationEvents.OnConfigurationReloaded += OnConfigurationReloaded;
        SetIcon();
    }

    private static void SetIcon()
    {
        ChatIconUrl = UnturnovHost.Configuration.GetValue<string>("ChatIconUrl");
    }

    private static void OnConfigurationReloaded()
    {
        SetIcon();
    }

    public static string? ChatIconUrl {get; private set;}

    // Will do more prettying up later
    // Later is complete
    public static string Format(string format, params object[] args)
    {
        string[] strings = new string[args.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            strings[i] = FormatColor.ColorText(args[i].ToString());
        }

        return string.Format(MainColor.ColorText(format), strings);
    }

    public static string FormatNoColor(string format, params object[] args)
    {
        return string.Format(format, args);
    }

    public static string FormatList(IEnumerable<string> strings, string seperator)
    {
        return String.Join(seperator, strings);
    }

    private static IEnumerable<string> GetTime(long seconds)
    {
        TimeSpan span = new(seconds * TimeSpan.TicksPerSecond);
        if (span.Days > 0)
        {
            yield return $"{span.Days} {(span.Days == 1 ? "Day" : "Days")}";
        }
        if (span.Hours > 0)
        {
            yield return $"{span.Hours} {(span.Hours == 1 ? "Hour" : "Hours")}";
        }
        if (span.Minutes > 0)
        {
            yield return $"{span.Minutes} {(span.Minutes == 1 ? "Minute" : "Minutes")}";
        }
        if (span.Seconds > 0)
        {
            yield return $"{span.Seconds} {(span.Seconds == 1 ? "Second" : "Seconds")}";
        }
    }

    public static string FormatTime(long seconds)
    {
        return FormatList(GetTime(seconds), " ");
    }
}
