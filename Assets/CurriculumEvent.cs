using UnityEngine;
using TMPro;

public class CurriculumEvent : MonoBehaviour
{
    [TextArea]
    public string message;

    [SerializeField] private TMP_InputField inputField; // Referencia al TMP_InputField

    public float displayTime = 3.0f;

    public GameObject portrait; // ← Este es el retrato específico para este evento/personaje

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inputField.gameObject.SetActive(true);
            GameManager.Instance.ShowMessage(message, displayTime, portrait);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
