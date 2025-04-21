using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;

public class CurriculumEvent : MonoBehaviour
{
    [Header("Configuración de Diálogo")]
    [TextArea] public string initialMessage;
    public GameObject portrait;

    [Header("Configuración de Input")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Opciones de Conversación")]
    [SerializeField] private bool isReactivatable = true;
    [SerializeField] private float reactivationCooldown = 5f;
    [SerializeField] private string[] farewellWords = { "adiós", "adios", "Adiós", "Adios", "hasta luego", "Hasta luego", "nos vemos", "Nos vemos", "chao", "bye" };

    private bool hasBeenActivated = false;
    private bool isConversationPaused = false;
    public string lastPlayerMessage { get; private set; } = "";
    public string lastCharacterMessage { get; private set; } = "";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (!hasBeenActivated || isReactivatable))
        {
            StartConversation();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isConversationPaused)
            {
                ResumeConversation();
            }
            else
            {
                // Pausa el juego completo cuando se presiona ESC
                GameManager.Instance.TogglePauseGame();

                // Si estamos en diálogo, también pausamos la conversación
                if (inputField.gameObject.activeSelf)
                {
                    PauseConversation();
                }
            }
        }
    }

    private void StartConversation()
    {
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        inputField.onEndEdit.AddListener(ProcessPlayerInput);

        if (string.IsNullOrEmpty(lastCharacterMessage))
        {
            ShowCharacterMessage(initialMessage);
        }
        else
        {
            ShowCharacterMessage("¿Sí? ¿Decías algo?");
        }
    }

    private void ProcessPlayerInput(string userInput)
    {
        string cleanInput = userInput.ToLower().Trim();
        lastPlayerMessage = userInput;

        // Verifica palabras de despedida
        foreach (string word in farewellWords)
        {
            if (cleanInput.Contains(word.ToLower()))
            {
                EndConversation(true);
                return;
            }
        }

        ContinueConversation();
    }

    private void PauseConversation()
    {
        isConversationPaused = true;
        inputField.gameObject.SetActive(false);
        ShowCharacterMessage("(La conversación está en pausa...)");
    }

    private void ResumeConversation()
    {
        isConversationPaused = false;
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        ShowCharacterMessage("¿Dónde estábamos...?");
    }

    private void ContinueConversation()
    {
        GeminiAPI.Instance.SendRequest(lastPlayerMessage, lastCharacterMessage);
        inputField.text = "";
    }

    public void OnGeminiResponseReceived(string response)
    {
        ShowCharacterMessage(response);
    }

    private void EndConversation(bool saveContext)
    {
        if (!saveContext)
        {
            lastPlayerMessage = "";
            lastCharacterMessage = "";
        }

        inputField.gameObject.SetActive(false);
        GameManager.Instance.ShowMessage("", 0f, null);

        if (isReactivatable)
        {
            StartCoroutine(ReactivateCooldown());
        }
    }

    private void ShowCharacterMessage(string message)
    {
        lastCharacterMessage = message;
        GameManager.Instance.ShowMessage(message, float.MaxValue, portrait);
    }

    private IEnumerator ReactivateCooldown()
    {
        GetComponent<Collider2D>().enabled = false;
        hasBeenActivated = true;
        yield return new WaitForSeconds(reactivationCooldown);
        GetComponent<Collider2D>().enabled = true;
        hasBeenActivated = false;
    }

    public void TriggerEventManually()
    {
        if (!hasBeenActivated || isReactivatable)
        {
            StartConversation();
        }
    }
}