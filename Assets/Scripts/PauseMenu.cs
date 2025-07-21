using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public VideoPlayer videoPlayer; // Asigna el VideoPlayer del quad desde el Inspector
    public ConfigurationManager configManager;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !configManager.configPanel.activeSelf)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Bloquear y ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reanudar los audios
        AudioListener.pause = false;

        // Reanudar el video si fuera el caso
        if (videoPlayer != null)
            videoPlayer.Play();
    }


    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Desbloquear y mostrar el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pausar todos los audios
        AudioListener.pause = true;

        // Si tienes un VideoPlayer y lo necesitas, también pausarlo:
        if (videoPlayer != null)
            videoPlayer.Pause();
    }


    public void Restart()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false; // Reanuda el audio
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Configuracion()
    {
        pauseMenuUI.SetActive(false);
        configManager.OpenConfiguration();
        Debug.Log("Botón Configuración presionado (funcionalidad pendiente).");
    }
}
