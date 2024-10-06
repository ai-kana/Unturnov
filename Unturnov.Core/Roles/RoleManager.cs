using System.Reflection;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Unturnov.Core.Logging;

namespace Unturnov.Core.Roles;

public class RoleManager
{
    private static HashSet<Role>? _Roles;

    private static async UniTask CreateFile()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        const string path = "Unturnov.Core.Roles.json";
        using StreamReader reader = new(assembly.GetManifestResourceStream(path));

        string content = await reader.ReadToEndAsync();

        await using StreamWriter writer = new(File.Create("Roles.json"));
        await writer.WriteAsync(content);
    }

    public static async UniTask RegisterRoles()
    {
        string path = "Roles.json";

        if (!File.Exists(path))
        {
            await CreateFile();
        }

        using StreamReader stream = new(File.Open(path, FileMode.Open, FileAccess.Read));
        string text = await stream.ReadToEndAsync();

        _Roles = JsonConvert.DeserializeObject<HashSet<Role>>(text) ?? new();

        ILogger logger = LoggerProvider.CreateLogger<RoleManager>();
        foreach (Role role in _Roles)
        {
            logger.LogInformation(role.Id);
        }
    }

    public static HashSet<Role> GetRoles(HashSet<string> ids)
    {
        HashSet<Role> roles = new();
        foreach (string id in ids)
        {
            Role? role = _Roles.Where(x => string.Compare(x.Id, id, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
            if (role == null)
            {
                continue;
            }

            roles.Add(role);
        }

        return roles;
    }

    public static Role? GetRole(string id)
    {
        Role? role = _Roles.Where(x => string.Compare(x.Id, id, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
        if (role == null)
        {
            return null;
        }

        return role;
    }
}
