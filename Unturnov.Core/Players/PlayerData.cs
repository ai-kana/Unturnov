using Newtonsoft.Json;

namespace Unturnov.Core.Roles;

[Serializable]
public class PlayerData
{
    [JsonProperty]
    public HashSet<string> Permissions {get; private set;} = new();
    [JsonProperty]
    public HashSet<string> Roles {get; private set;} = new();
    // String - Cooldown Name
    // long - Unix time stamp for end
    [JsonProperty]
    public Dictionary<string, long> Cooldowns {get; private set;} = new();

    [JsonProperty]
    public string Language = "English";
}
