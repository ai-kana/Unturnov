namespace Unturnov.Core.Translations;

public class TranslationPackage
{
    private readonly Translation _Translation;
    private readonly object[] _Arguments;

    public TranslationPackage(Translation translation, params object[] args)
    {
        _Translation = translation;
        _Arguments = args;
    }

    public string Translate(string language)
    {
        return _Translation.Translate(language, _Arguments);
    }
}