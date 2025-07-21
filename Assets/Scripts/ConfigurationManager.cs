using UnityEngine;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject configPanel; // Panel padre (contiene los botones)
    public GameObject audioPanel;
    public GameObject graphicsPanel;
    public GameObject controlsPanel;
    public GameObject visionPanel;
    public GameObject languagePanel;

    [Header("Botones")]
    public Button audioButton;
    public Button graphicsButton;
    public Button controlsButton;
    public Button visionButton;
    public Button languageButton;

    private GameObject[] allPanels; // Para manejar todos los subpaneles
    private Color normalColor = new Color(0f, 0f, 0f, 0f);
    private Color selectedColor = new Color(0f, 0f, 0f, 0.5882f);

    void Start()
    {
        allPanels = new GameObject[] { audioPanel, graphicsPanel, controlsPanel, visionPanel, languagePanel };

        // Configura los colores iniciales de los botones
        ResetAllButtonColors();
        audioButton.image.color = selectedColor;

        // Desactiva todos los subpaneles al inicio
        foreach (var panel in allPanels)
        {
            panel.SetActive(false);
        }
        configPanel.SetActive(false);
    }

    // Al abrir la configuración, muestra el panel de audio por defecto
    public void OpenConfiguration()
    {
        configPanel.SetActive(true);
        ShowPanel(audioPanel);
        FindObjectOfType<PauseMenu>().pauseMenuUI.SetActive(false);
    }

    public void CloseConfiguration()
    {
        configPanel.SetActive(false);
        FindObjectOfType<PauseMenu>().pauseMenuUI.SetActive(true);
    }

    // Método genérico para mostrar cualquier panel
    public void ShowPanel(GameObject targetPanel)
    {
        foreach (var panel in allPanels)
        {
            panel.SetActive(panel == targetPanel);
        }
        UpdateButtonColors(targetPanel);
    }

    // Actualiza los colores de los botones según el panel activo
    private void UpdateButtonColors(GameObject activePanel)
    {
        ResetAllButtonColors();

        if (activePanel == audioPanel) audioButton.image.color = selectedColor;
        else if (activePanel == graphicsPanel) graphicsButton.image.color = selectedColor;
        else if (activePanel == controlsPanel) controlsButton.image.color = selectedColor;
        else if (activePanel == visionPanel) visionButton.image.color = selectedColor;
        else if (activePanel == languagePanel) languageButton.image.color = selectedColor;
    }

    private void ResetAllButtonColors()
    {
        audioButton.image.color = normalColor;
        graphicsButton.image.color = normalColor;
        controlsButton.image.color = normalColor;
        visionButton.image.color = normalColor;
        languageButton.image.color = normalColor;
    }

    void Update()
    {
        if (configPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseConfiguration();
        }
    }
}