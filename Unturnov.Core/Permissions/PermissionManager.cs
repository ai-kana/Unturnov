using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Steamworks;
using Unturnov.Core.Players;

namespace Unturnov.Core.Permissions;

public static class PermissionManager
{
    static PermissionManager()
    {
        Directory.CreateDirectory("Permissions");
    }

    public static async UniTask<HashSet<string>> LoadPermissions(CSteamID id)
    {
        string path = $"Permissions/{id}";
        if (!File.Exists(path))
        {
            return new();
        }

        using StreamReader stream = new(File.Open(path, FileMode.Open, FileAccess.Read));
        string text = await stream.ReadToEndAsync();

        return JsonConvert.DeserializeObject<HashSet<string>>(text) ?? new();
    }

    public static async UniTask SavePermissions(UnturnovPlayer player)
    {
        string path = $"Permissions/{player.SteamID}";


        string text = JsonConvert.SerializeObject(player.Permissions);

        await using StreamWriter stream = new(File.Open(path, FileMode.Create, FileAccess.Write), Encoding.UTF8);
        await stream.WriteAsync(text);
    }
}
