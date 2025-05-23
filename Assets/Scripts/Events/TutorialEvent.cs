using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
    public string message; // Mensaje que dirá el personaje
    public TMP_Text dialogueText; // Referencia al texto de la UI

    public float displayTime = 3.0f;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Muestra el mensaje en la UI
            this.dialogueText.text = message;

            GetComponent<Collider2D>().enabled = false;

            // Opcional: Desaparece el mensaje después de un tiempo
            Invoke("ClearMessage", displayTime);

        }
    }

    private void ClearMessage()
    {
        dialogueText.text = ""; // Limpia el mensaje
    }
}