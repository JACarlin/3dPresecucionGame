using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource ambientMusic;
    public AudioSource actionMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Hace que el AudioManager no se destruya al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayActionMusic()
    {
        if (ambientMusic.isPlaying)
            ambientMusic.Stop();

        if (!actionMusic.isPlaying)
            actionMusic.Play();
    }

    public void PlayAmbientMusic()
    {
        if (actionMusic.isPlaying)
            actionMusic.Stop();

        if (!ambientMusic.isPlaying)
            ambientMusic.Play();
    }
}
