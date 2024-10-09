using Unturnov.Core.Players;

namespace Unturnov.Core.Translations;

public class Translation
{
    private readonly string _DefaultValue;
    private readonly string _Key;

    public Translation(string key, string defaultValue)
    {
        _DefaultValue = defaultValue;
        _Key = key;
        TranslationManager.AddTranslation(_Key, _DefaultValue);
    }

    public string Translate()
    {
        return _DefaultValue;
    }

    public string Translate(string language)
    {
        if (language == "English")
        {
            return _DefaultValue;
        }

        if (!TranslationManager.TryGetTranslation(language, _Key, out string value))
        {
            return _DefaultValue;
        }

        return value;
    }

    public string Translate(UnturnovPlayer player)
    {
        return Translate(player.Language);
    }
}
