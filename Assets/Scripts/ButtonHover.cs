using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float scaleFactor = 1.1f;

    void Awake()
    {
        // Inicializa en Awake para capturar la escala incluso si el objeto está inactivo
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        // Asegura que la escala se reinicie al activar el objeto
        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}