using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [TextArea]
    public string message;

    public float displayTime = 3.0f;

    public GameObject portrait; // ← Este es el retrato específico para este evento/personaje

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Diálogo");
            GameManager.Instance.ShowMessage(message, displayTime, portrait);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
