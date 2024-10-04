using Microsoft.Extensions.Configuration;
using UnityEngine;
using Unturnov.Core.Configuration;

namespace Unturnov.Core.Formatting;

public static class Formatter
{
    private static Color32 MainColor = new(0xF5, 0xA9, 0xB8, 0);
    private static Color32 FormatColor = new(0x5B, 0xCE, 0xFE, 0);

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
}
