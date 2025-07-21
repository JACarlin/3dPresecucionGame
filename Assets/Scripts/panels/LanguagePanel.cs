// LanguagePanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LanguagePanel : MonoBehaviour
{
    [System.Serializable]
    public struct LanguageOption
    {
        public string displayName;
        public string code;
    }

    [SerializeField]
    private List<LanguageOption> languageOptions = new List<LanguageOption>
    {
        new LanguageOption { displayName = "Español", code = "es" },
        new LanguageOption { displayName = "English", code = "en" }
    };

    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Button applyButton;

    private void Awake()
    {
        ConfigureDropdown();
        LoadCurrentLanguage();
        applyButton.onClick.AddListener(ApplyLanguage);
    }

    private void ConfigureDropdown()
    {
        languageDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var option in languageOptions)
        {
            options.Add(option.displayName);
        }

        languageDropdown.AddOptions(options);
    }

    private void LoadCurrentLanguage()
    {
        string currentLang = PlayerPrefs.GetString("language", "es");
        int langIndex = languageOptions.FindIndex(l => l.code == currentLang);
        languageDropdown.value = langIndex >= 0 ? langIndex : 0;
    }

    private void ApplyLanguage()
    {
        int selectedIndex = languageDropdown.value;
        string selectedCode = languageOptions[selectedIndex].code;
        LanguageManager.Instance.LoadLanguage(selectedCode);
    }
}