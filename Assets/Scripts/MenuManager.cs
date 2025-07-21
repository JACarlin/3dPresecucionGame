using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1); // Cambia "Induccion" por el nombre de tu escena de inducción.
    }
    public void goSandbox()
    {
        SceneManager.LoadScene(0); // Cambia "Induccion" por el nombre de tu escena de inducción.
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("El juego se cerraría aquí, pero estamos en el editor.");
#else
        Application.Quit();
#endif
    }
}
