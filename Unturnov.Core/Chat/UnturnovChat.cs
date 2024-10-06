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
            if (Vector3.SqrMagnitude(player.Position - sender.Position) > _LocalChatDistance)
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
        IEnumerable<string> roles = RoleManager.GetRoles(player.Roles)
            .Where(x => x.DutyOnly ? player.OnDuty : true)
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

        if (text.StartsWith('/'))
        {
            CommandManager.ExecuteCommand(text, player);
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
        ChatManager.serverSendMessage(message, Color.white, null, player.SteamPlayer, EChatMode.GLOBAL, Formatter.ChatIconUrl, true);
    }
}
