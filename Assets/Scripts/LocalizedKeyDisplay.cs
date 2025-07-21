using UnityEngine;
using TMPro;

public class LocalizedKeyDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct KeyBinding
    {
        public string actionKey;  // Clave de localización de la acción
        public KeyCode key;       // Tecla física
    }

    [SerializeField] private KeyBinding[] bindings;
    [SerializeField] private TranslationKey translationKey;

    private void Start()
    {
        UpdateKeyDisplay();
        LanguageManager.OnLanguageChanged += UpdateKeyDisplay;
    }

    private void OnDestroy()
    {
        LanguageManager.OnLanguageChanged -= UpdateKeyDisplay;
    }

    public void UpdateKeyDisplay()
    {
        if (translationKey == null) return;

        object[] keyNames = new object[bindings.Length];

        for (int i = 0; i < bindings.Length; i++)
        {
            // Obtiene el nombre localizado de la tecla
            keyNames[i] = LanguageManager.Instance.GetText($"key_{bindings[i].key}");
        }

        translationKey.UpdateText(keyNames);
    }
}

