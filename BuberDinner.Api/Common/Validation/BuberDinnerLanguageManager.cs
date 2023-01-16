using System.Collections.Concurrent;
using System.Globalization;
using FluentValidation.Resources;

namespace BuberDinner.Api.Common.Validation;

// This class is a copy of the FluentValidation default Language Manager.
// Needed to copy and overwrite the GetTranslation to include a custom default language.
public class BuberDinnerLanguageManager : ILanguageManager
{
    private readonly ConcurrentDictionary<string, string> _languages = new ConcurrentDictionary<string, string>();

    private static string GetTranslation(string culture, string key)
    {
        return culture switch
        {
            CustomEnglishLanguage.AmericanCulture => CustomEnglishLanguage.GetTranslation(key),
            CustomEnglishLanguage.BritishCulture => CustomEnglishLanguage.GetTranslation(key),
            CustomEnglishLanguage.Culture => CustomEnglishLanguage.GetTranslation(key),
            _ => null,
        };
    }


    public bool Enabled { get; set; } = true;


    public CultureInfo Culture { get; set; }


    public void Clear()
    {
        _languages.Clear();
    }

    public virtual string GetString(string key, CultureInfo culture = null)
    {
        string value;

        if (Enabled)
        {
            culture = culture ?? Culture ?? CultureInfo.CurrentUICulture;

            string currentCultureKey = culture.Name + ":" + key;
            value = _languages.GetOrAdd(currentCultureKey, k => GetTranslation(culture.Name, key));

            // If the value couldn't be found, try the parent culture.
            var currentCulture = culture;
            while (value == null && currentCulture.Parent != CultureInfo.InvariantCulture)
            {
                currentCulture = currentCulture.Parent;
                string parentCultureKey = currentCulture.Name + ":" + key;
                value = _languages.GetOrAdd(parentCultureKey, k => GetTranslation(currentCulture.Name, key));
            }

            if (value == null && culture.Name != CustomEnglishLanguage.Culture)
            {
                // If it couldn't be found, try the fallback English (if we haven't tried it already).
                if (!culture.IsNeutralCulture && culture.Parent.Name != CustomEnglishLanguage.Culture)
                {
                    value = _languages.GetOrAdd(CustomEnglishLanguage.Culture + ":" + key,
                        k => CustomEnglishLanguage.GetTranslation(key));
                }
            }
        }
        else
        {
            value = _languages.GetOrAdd(CustomEnglishLanguage.Culture + ":" + key,
                k => CustomEnglishLanguage.GetTranslation(key));
        }

        return value ?? string.Empty;
    }

    public void AddTranslation(string language, string key, string message)
    {
        if (string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

        _languages[language + ":" + key] = message;
    }
}