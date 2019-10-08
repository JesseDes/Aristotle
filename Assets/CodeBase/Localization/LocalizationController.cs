using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationController
{
    private LocalizationData _locData;
    private LanguageData _langData;

    public LocalizationController(LocalizationData locData)
    {
        _locData = locData;
    }

    public void setLanguage(string code)
    {
        foreach(var language in _locData.LanguageData)
        {
            if (language.LanguageCode == code)
            {
                _langData = language;
                return;
            }
        }

        Debug.LogError("No language found matching code: " + code);
    }

    public string getLanguageCode()
    {
        return _langData?.LanguageCode;
    }

    public string getLocalization(string key)
    {
        if (_langData ==  null)
        {
            Debug.LogError("Localization not intialized! Please set a language");
            return string.Empty;
        }

        foreach (var localization in _langData.Data.Translations)
        {
            if (localization.Key == key)
            {
                return localization.Value;
            }
        }

        Debug.LogError("No localization found for key: " + key);
        return string.Empty;
    }
}
