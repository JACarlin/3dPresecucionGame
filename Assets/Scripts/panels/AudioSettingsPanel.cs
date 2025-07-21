using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsPanel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button applyButton;

    [SerializeField] TMP_Text volumeMasterPercentageText;
    [SerializeField] TMP_Text volumeMusicPercentageText;
    [SerializeField] TMP_Text volumeSFXPercentageText;

    private const float MinVolume = -30f;
    private const float MaxVolume = 0f;

    void Start()
    {
        // Configurar valores iniciales
        LoadSavedValues();
        ConfigureListeners();
        UpdateAllPercentageDisplays();
    }
    private void ConfigureListeners()
    {
        masterSlider.onValueChanged.AddListener(value => {
            SetMasterVolume(value);
            UpdatePercentageDisplay(value, volumeMasterPercentageText);
        });

        musicSlider.onValueChanged.AddListener(value => {
            SetMusicVolume(value);
            UpdatePercentageDisplay(value, volumeMusicPercentageText);
        });

        sfxSlider.onValueChanged.AddListener(value => {
            SetSFXVolume(value);
            UpdatePercentageDisplay(value, volumeSFXPercentageText);
        });

        applyButton.onClick.AddListener(SaveSettings);
    }

    private void LoadSavedValues()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Forzar actualización inicial
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    private void SetMasterVolume(float value)
    {
        SetVolume("MasterVol", value);
    }

    private void SetMusicVolume(float value)
    {
        SetVolume("MusicVol", value);
    }

    private void SetSFXVolume(float value)
    {
        SetVolume("SFXVol", value);
    }

    private void SetVolume(string parameterName, float normalizedValue)
    {
        // Convertir valor lineal (0-1) a logarítmico (dB)
        float dB = MinVolume + (MaxVolume - MinVolume) * Mathf.Clamp01(normalizedValue);
        audioMixer.SetFloat(parameterName, dB);
    }

    private void UpdateAllPercentageDisplays()
    {
        UpdatePercentageDisplay(masterSlider.value, volumeMasterPercentageText);
        UpdatePercentageDisplay(musicSlider.value, volumeMusicPercentageText);
        UpdatePercentageDisplay(sfxSlider.value, volumeSFXPercentageText);
    }

    private void UpdatePercentageDisplay(float value, TMP_Text textComponent)
    {
        textComponent.text = $"{Mathf.RoundToInt(value * 100)}%";
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    // Opcional: Resetear a valores por defecto
    public void ResetToDefaults()
    {
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;
        UpdateAllPercentageDisplays();
        SaveSettings();
    }
}