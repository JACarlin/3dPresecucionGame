using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string itemName; // Mantenemos el nombre original como backup
    public Sprite itemIcon;

    [Header("Localization")]
    public string localizationKey;

    // Propiedad modificada para mayor seguridad
    public string LocalizedName
    {
        get
        {
            if (Application.isPlaying && LanguageManager.Instance != null)
            {
                return LanguageManager.Instance.GetText(localizationKey);
            }
            return itemName; // Fallback durante edición
        }
    }
}
