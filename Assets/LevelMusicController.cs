using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusicController : MonoBehaviour
{
    public AudioClip levelMusic;

    void Start()
    {
        // Detener música global si existe
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        // Reproducir música del nivel
        AudioSource.PlayClipAtPoint(levelMusic, Camera.main.transform.position);
    }

    void OnDestroy()
    {
        // Reanudar música global al salir (si no es un nivel)
        if (AudioManager.Instance != null && !IsLevelScene(SceneManager.GetActiveScene().name))
        {
            AudioManager.Instance.PlayMusic();
        }
    }

    bool IsLevelScene(string sceneName)
    {
        return sceneName.StartsWith("Level");
    }
}