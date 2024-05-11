using System.Collections.Generic;
using YG;

public class TranslationsManager
{
    private Dictionary<string, Dictionary<string, string>> _phrases = new();

    public TranslationsManager()
    {
        Register("legs", "ru", "Отгадано ножек: ");
        Register("legs", "en", "LEGS GUESSED: ");
        Register("legs", "tr", "TAHMİN EDİLEN BACAK SAYISI: ");

        Register("rec", "ru", "Рекорд: ");
        Register("rec", "en", "Record: ");
        Register("rec", "tr", "Rekor: ");
    }


    public void Register(string phraseKey, string language, string phrase)
    {
        if (!_phrases.ContainsKey(phraseKey.ToLower()))
            _phrases[phraseKey.ToLower()] = new Dictionary<string, string>();

        _phrases[phraseKey.ToLower()][language.ToLower()] = phrase;
    }

    public string GetPhrase(string phraseKey)
    {
        return _phrases[phraseKey][YandexGame.EnvironmentData.language.ToLower()];
    }
}