using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToSigNivel : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 2f;
    public float delayBeforeFade = 1f;
    public float delayAfterFade = 1f;

    private void Start()
    {
        fadeCanvasGroup.alpha = 0f; // Make sure the panel starts invisible
    }

    public void StartFade()
    {
        // Activate the panel first (if it's deactivated)
        fadeCanvasGroup.gameObject.SetActive(true);

        // Start the fade animation
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(delayAfterFade);

        // Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
