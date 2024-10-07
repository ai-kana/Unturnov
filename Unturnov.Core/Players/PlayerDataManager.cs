using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Steamworks;
using Unturnov.Core.Roles;

namespace Unturnov.Core.Players;

public static class PlayerDataManager
{
    private const string DataDirectory = "PlayerData";

    static PlayerDataManager()
    {
        Directory.CreateDirectory(DataDirectory);
        ServerManager.OnServerSave += OnSave;
    }

    private static void OnSave()
    {
        foreach (UnturnovPlayer player in UnturnovPlayerManager.Players.Values)
        {
            _ = SaveDataAsync(player);
        }
    }

    public static async UniTask<PlayerData> LoadDataAsync(CSteamID steamID)
    {
        string path = $"{DataDirectory}/{steamID}.json";

        if (!File.Exists(path))
        {
            return new();
        }

        using StreamReader reader = new(File.Open(path, FileMode.Open, FileAccess.Read));
        string data = await reader.ReadToEndAsync();

        return JsonConvert.DeserializeObject<PlayerData>(data) ?? new();
    }

    public static async UniTask SaveDataAsync(UnturnovPlayer player)
    {
        string path = $"{DataDirectory}/{player.SteamID}.json";

        string data = JsonConvert.SerializeObject(player.SaveData);

        await using StreamWriter writer = new(File.Open(path, FileMode.Create, FileAccess.Write));
        await writer.WriteAsync(data);
    }
}
