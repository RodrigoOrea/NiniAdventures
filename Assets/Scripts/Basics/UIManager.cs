using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public CanvasGroup pantallaNegra;
    public GameObject panelEstadisticas;
    public TextMeshProUGUI textoEnemigos;
    public TextMeshProUGUI textoDañoHecho;
    public TextMeshProUGUI textoDañoRecibido;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IniciarFinalNivel()
    {
        StartCoroutine(TransicionFinalNivel());
    }

    private IEnumerator TransicionFinalNivel()
    {
        // Fade a negro
        yield return StartCoroutine(FadeNegro(1f, 1f));

        // Mostrar panel
        MostrarDatos();
        panelEstadisticas.SetActive(true);
    }

    private IEnumerator FadeNegro(float targetAlpha, float duration)
    {
        float startAlpha = pantallaNegra.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            pantallaNegra.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        pantallaNegra.alpha = targetAlpha;
    }

    private void MostrarDatos()
    {
        GameManager gm = GameManager.Instance;
        textoEnemigos.text = "Enemigos eliminados: " + gm.enemiesKilled;
        textoDañoHecho.text = "Daño hecho: " + gm.dañoRealizado;
        textoDañoRecibido.text = "Daño recibido: " + gm.dañoRecibido;
    }


    public void VolverAlMenu()
    {
        StartCoroutine(TransicionAlMenu());
    }

    private IEnumerator TransicionAlMenu()
    {
        // Fade a negro en 1 segundo
        yield return StartCoroutine(FadeNegro(1f, 1f));

        // Cargar escena del menú
        SceneManager.LoadScene("Splash");
    }

}
