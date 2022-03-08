using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localization;
using UnityEngine;


/// <summary>
/// Localization manager singleton interface to easy and safe access.
/// </summary>
public interface ILocalizationManager
{
    string ChosenLanguage { get;}
    string GetLocalizedValue(string key);
    void SwapLanguage();
    void SetLocalization(string localization);
}

/// <summary>
/// Localization manager singleton.
/// </summary>
public sealed class LocalizationManager : Singleton<LocalizationManager, ILocalizationManager>, ILocalizationManager
{
    private const string LOCALIZATION_FILES_PREFIX = "localizedText_"; 
    private const string SELECTED_LANGUAGE_KEY = "LocalizationManager_SELECTED_LANGUAGE_KEY";

    private Dictionary<string, string> selectedLocalization = null;
    private Dictionary<string, string> defaultLocalization = null;

    // TODO: Make it's UnityEvent.
    public delegate void LocalizationManagerMethod();
    public static event LocalizationManagerMethod onLanguageChange;

    public string ChosenLanguage { get; private set; }

    public override void Initialize()
    {
        var defaultLang = Localizations.ENGLISH;
        if (PlayerPrefs.HasKey(SELECTED_LANGUAGE_KEY))
        {
            defaultLang = PlayerPrefs.GetString(SELECTED_LANGUAGE_KEY);
        }
        else
        {
            var systemLang = Application.systemLanguage;
            if (systemLang == SystemLanguage.Russian || systemLang == SystemLanguage.Ukrainian || systemLang == SystemLanguage.Belarusian)
            {
                defaultLang = Localizations.RUSSIAN;
            }
        }
        ChosenLanguage = defaultLang;
        LoadDefaultLocalization();
        SetLanguage(defaultLang);

        Debug.Log(Application.systemLanguage + " is system language");
        Debug.Log(string.Format("Selected {0} language", defaultLang));
    }

    public override void Dispose()
    {
    }
    
    public string GetLocalizedValue(string key)
    {
        var result = key;
        if (ChosenLanguage != Localizations.ENGLISH && selectedLocalization.ContainsKey(key))
        {
            result = selectedLocalization[key];
        }
        else if(defaultLocalization.ContainsKey(key))
        {
            result = defaultLocalization[key];
        }
        return result;
    }

    public void SwapLanguage()
    {
        SetLanguage(ChosenLanguage == Localizations.RUSSIAN ? Localizations.ENGLISH : Localizations.RUSSIAN);
    }

    public void SetLocalization(string localization)
    {
        if (localization == Localizations.RUSSIAN || localization == Localizations.ENGLISH)
        {
            SetLanguage(localization);
        }
        else
        {
            throw new ArgumentException("localization");
        }
    }

    /// <summary>
    /// Load some localization.
    /// </summary>
    /// <param name="language"></param>
    private void LoadLocalization(string language)
    {
        if (language != Localizations.ENGLISH)
        {
            selectedLocalization = new Dictionary<string, string>();
            string fileName = LOCALIZATION_FILES_PREFIX + language + ".json";
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
                selectedLocalization = loadedData.items.ToDictionary(x => x.key, y => y.value);
            }
        }
    }

    /// <summary>
    /// Load default localization.
    /// NOTE: Strong caching without chance to update localization in cache.
    /// </summary>
    private void LoadDefaultLocalization()
    {
        if (defaultLocalization == null)
        {
            defaultLocalization = new Dictionary<string, string>();
            string fileName = LOCALIZATION_FILES_PREFIX + Localizations.ENGLISH + ".json";
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
                defaultLocalization = loadedData.items.ToDictionary(x => x.key, y => y.value);
            }
        }
    }

    private void SetLanguage(string language)
    {
        LoadLocalization(language);
        SaveChosenLanguage(language);
        if (onLanguageChange != null)
        {
            onLanguageChange();
        }

        // TODO: Bad stuff.
        var button = FindObjectOfType<SwapLanguageButton>();
        if (button != null)
        {
            button.SetSprite(language);
        }
    }

    private void SaveChosenLanguage(string language)
    {
        ChosenLanguage = language;
        PlayerPrefs.SetString(SELECTED_LANGUAGE_KEY, language);
    }
}
