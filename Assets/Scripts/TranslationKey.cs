using TMPro;
using UnityEngine;

public class TranslationKey : MonoBehaviour
{
    public string key;
    public bool isDynamic; // Si usa placeholders (ej: puntuaciones)
    public TMP_Text textComponent;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        UpdateText();
    }

    public void UpdateText(params object[] args)
    {

        if (isDynamic)
        {
            textComponent.text = LanguageManager.Instance.GetText(key, args);
        }
        else
        {
            textComponent.text = LanguageManager.Instance.GetText(key);
        }
        Debug.Log(textComponent.text);
    }
}