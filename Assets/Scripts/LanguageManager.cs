// LanguageManager.cs
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class SerializableKeyValuePair
{
    public string key;
    public string value;
}

[System.Serializable]
public class LanguageData
{
    public SerializableKeyValuePair[] strings;
}


public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;
    public Dictionary<string, string> CurrentLanguage { get; private set; }
    public static event Action OnLanguageChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
            LoadSavedLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadSavedLanguage()
    {
        string savedLang = PlayerPrefs.GetString("language", "es");
        LoadLanguage(savedLang);
    }

    public void LoadLanguage(string langCode)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"Localization/{langCode}");
        if (jsonFile != null)
        {
            LanguageData langData = JsonUtility.FromJson<LanguageData>(jsonFile.text);
            CurrentLanguage = new Dictionary<string, string>();

            foreach (var entry in langData.strings)
            {
                CurrentLanguage[entry.key] = entry.value;
            }

            PlayerPrefs.SetString("language", langCode);
            UpdateAllTexts();
        }
        else
        {
            Debug.LogError($"Archivo de idioma {langCode} no encontrado.");
        }
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key, params object[] args)
    {
        if (CurrentLanguage.TryGetValue(key, out string value))
        {
            // Maneja el caso donde no hay argumentos
            return args == null || args.Length == 0
                ? value
                : string.Format(value, args);
        }
        return $"MISSING: {key}";
    }

    // Llama a este método cada vez que se cambia el idioma
    private void UpdateAllTexts()
    {
        TranslationKey[] allTexts = FindObjectsOfType<TranslationKey>(true); // Busca incluso objetos inactivos
        foreach (TranslationKey text in allTexts)
        {
            text.UpdateText();
        }
    }
}