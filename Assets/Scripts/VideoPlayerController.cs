using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips; // Array con los videos

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer == null || videoClips.Length == 0)
        {
            Debug.LogError("VideoPlayer o VideoClips no asignados");
            return;
        }

        // Suscribirse al evento cuando el video termine
        videoPlayer.loopPointReached += OnVideoEnd;

        // Reproducir un video aleatorio al inicio
        PlayRandomVideo();
    }

    void PlayRandomVideo()
    {
        int index = Random.Range(0, videoClips.Length); // Elegir un video aleatorio
        videoPlayer.clip = videoClips[index];
        videoPlayer.Play();
        Debug.Log("Reproduciendo video: " + videoClips[index].name);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("El video ha terminado.");

        // Reproducir otro video aleatorio cuando termine
        PlayRandomVideo();
    }
}
