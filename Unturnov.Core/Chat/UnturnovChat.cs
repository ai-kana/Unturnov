using Microsoft.Extensions.Logging;
using SDG.Unturned;
using UnityEngine;
using Unturnov.Core.Formatting;
using Unturnov.Core.Players;
using Unturnov.Core.Logging;
using Unturnov.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Roles;
using Unturnov.Core.Translations;

namespace Unturnov.Core.Chat;

public class UnturnovChat
{
    private static readonly ILogger _Logger;
    private static float _LocalChatDistance;

    static UnturnovChat()
    {
        _Logger = LoggerProvider.CreateLogger<UnturnovChat>();
        ChatManager.onChatted += OnChatted;

        OnConfigurationReloaded();
        ConfigurationEvents.OnConfigurationReloaded += OnConfigurationReloaded;
    }

    private static void OnConfigurationReloaded()
    {
        float distance = UnturnovHost.Configuration.GetValue<float>("LocalChatDistance");
        _LocalChatDistance = distance * distance;
    }

    private static void SendLocal(UnturnovPlayer sender, string text)
    {
        string message = $"[{Formatter.RedColor.ColorText("L")}] {sender.Name}: {text}";
        _Logger.LogInformation($"[Local] {sender.LogName}: {text}");

        foreach (UnturnovPlayer player in UnturnovPlayerManager.Players.Values)
        {
            if (Vector3.SqrMagnitude(player.Movement.Position - sender.Movement.Position) > _LocalChatDistance)
            {
                continue;
            }

            ChatManager.serverSendMessage(message, Color.white, sender.SteamPlayer, player.SteamPlayer, EChatMode.GROUP, null, true);
        }
    }

    private static void SendGroup(UnturnovPlayer sender, string text)
    {
        string message = $"[{Formatter.RedColor.ColorText("G")}] {sender.Name}: {text}";
        _Logger.LogInformation($"[Group] {sender.LogName}: {text}");
        foreach (UnturnovPlayer player in UnturnovPlayerManager.Players.Values)
        {
            if (!player.SteamPlayer.isMemberOfSameGroupAs(sender.SteamPlayer))
            {
                continue;
            }

            ChatManager.serverSendMessage(message, Color.white, sender.SteamPlayer, player.SteamPlayer, EChatMode.GROUP, null, true);
        }
    }

    private static void SendGlobal(UnturnovPlayer sender, string text)
    {
        string message = $"{GetChatTag(sender)}{sender.Name}: {text}";
        _Logger.LogInformation($"{sender.LogName}: {text}");
        foreach (UnturnovPlayer player in UnturnovPlayerManager.Players.Values)
        {
            ChatManager.serverSendMessage(message, Color.white, sender.SteamPlayer, player.SteamPlayer, EChatMode.GROUP, null, true);
        }
    }

    private static string GetChatTag(UnturnovPlayer player)
    {
        IEnumerable<string> roles = RoleManager.GetRoles(player.Roles.Roles)
            .Where(x => x.DutyOnly ? player.OnDuty : true && x.ChatTag != string.Empty)
            .Select(x => x.ChatTag);

        if (roles.Count() == 0)
        {
            return "";
        }

        return $"[{Formatter.FormatList(roles, " | ")}] ";
    }

    private static void OnChatted(SteamPlayer steamPlayer, EChatMode mode, ref Color chatted, ref bool isRich, string text, ref bool isVisible)
    {
        isVisible = false;

        UnturnovPlayer player = UnturnovPlayerManager.Players[steamPlayer.playerID.steamID];

        if (text.StartsWith("/"))
        {
            CommandManager.ExecuteCommand(text, player);
            return;
        }

        if (player.Moderation.IsMuted && mode != EChatMode.GROUP)
        {
            return;
        }

        string message = text.Replace("<", "< ");

        switch (mode)
        {
            case EChatMode.SAY:
            case EChatMode.GLOBAL:
            case EChatMode.WELCOME:
                SendGlobal(player, message);
                return;
            case EChatMode.GROUP:
                SendGroup(player, message);
                return;
            case EChatMode.LOCAL:
                SendLocal(player, message);
                return;
        }
    }

    public static void BroadcastMessage(UnturnovPlayer player, string message, params object[] args)
    {
        Broadcast(player, Formatter.Format(message, args));
    }

    public static void BroadcastMessage(IEnumerable<UnturnovPlayer> players, string message, params object[] args)
    {
        Broadcast(players, Formatter.Format(message, args));
    }

    public static void BroadcastMessage(string message, params object[] args)
    {
        IEnumerable<UnturnovPlayer> players = UnturnovPlayerManager.Players.Values;
        Broadcast(players, Formatter.Format(message, args));
    }

    public static void BroadcastMessage(UnturnovPlayer player, Translation translation, params object[] args)
    {
        Broadcast(player, translation, args);
    }

    public static void BroadcastMessage(IEnumerable<UnturnovPlayer> players, Translation translation, params object[] args)
    {
        Broadcast(players, translation, args);
    }

    public static void BroadcastMessage(Translation translation, params object[] args)
    {
        IEnumerable<UnturnovPlayer> players = UnturnovPlayerManager.Players.Values;
        Broadcast(players, translation, args);
    }

    private static void Broadcast(IEnumerable<UnturnovPlayer> players, Translation translation, params object[] args)
    {
        foreach (UnturnovPlayer player in players)
        {
            Broadcast(player, translation, args);
        }
    }

    private static void Broadcast(UnturnovPlayer player, Translation translation, params object[] args)
    {
        _Logger.LogInformation(translation.Translate(player, args));
        string message = translation.Translate(player, args);
        ChatManager.serverSendMessage("<b>" + message, Color.white, null, player.SteamPlayer, EChatMode.GLOBAL, Formatter.ChatIconUrl, true);
    }

    private static void Broadcast(IEnumerable<UnturnovPlayer> players, string message)
    {
        foreach (UnturnovPlayer player in players)
        {
            Broadcast(player, message);
        }
    }

    private static void Broadcast(UnturnovPlayer player, string message)
    {
        _Logger.LogInformation(message);
        ChatManager.serverSendMessage("<b>" + message, Color.white, null, player.SteamPlayer, EChatMode.GLOBAL, Formatter.ChatIconUrl, true);
    }
    
    public static void SendPrivateMessage(UnturnovPlayer sender, UnturnovPlayer receiver, string text)
    {
        text = text.Replace("<", "< ");
        string message = $"[{Formatter.RedColor.ColorText("PM")}] {sender.Name} -> {receiver.Name}: {text}";
        _Logger.LogInformation(message);

        ChatManager.serverSendMessage(message, Color.white, sender.SteamPlayer, receiver.SteamPlayer, useRichTextFormatting:true);
        ChatManager.serverSendMessage(message, Color.white, sender.SteamPlayer, sender.SteamPlayer, useRichTextFormatting:true);
    }
}
