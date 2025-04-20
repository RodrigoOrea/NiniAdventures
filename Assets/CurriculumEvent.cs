using UnityEngine;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;

public class CurriculumEvent : MonoBehaviour
{
    [Header("Configuración de Diálogo")]
    [TextArea] public string initialMessage;
    public float displayTime = 3.0f;
    public GameObject portrait;

    [Header("Configuración de Input")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Opciones de Conversación")]
    [SerializeField] private bool isReactivatable = true;
    [SerializeField] private float reactivationCooldown = 5f;
    [SerializeField] private string[] farewellWords = { "adiós", "hasta luego", "nos vemos", "chao", "bye" };

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
        if (isConversationPaused && Input.GetKeyDown(pauseKey))
        {
            ResumeConversation();
        }
    }

    private void StartConversation()
    {
        GameManager.Instance.SetDialogueActive(true);
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

        foreach (string word in farewellWords)
        {
            if (cleanInput.Contains(word))
            {
                EndConversation(true);
                return;
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            PauseConversation();
            return;
        }

        ContinueConversation();
    }

    private void PauseConversation()
    {
        isConversationPaused = true;
        inputField.gameObject.SetActive(false);
        GameManager.Instance.SetDialogueActive(false);
        ShowCharacterMessage("(La conversación está en pausa...)");
    }

    private void ResumeConversation()
    {
        isConversationPaused = false;
        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        GameManager.Instance.SetDialogueActive(true);
        ShowCharacterMessage("¿Dónde estábamos...?");
    }

    private void ContinueConversation()
    {
        GeminiAPI.Instance.SendRequest(lastPlayerMessage, lastCharacterMessage);
        inputField.text = "";
    }

    private void EndConversation(bool saveContext)
    {
        if (!saveContext)
        {
            lastPlayerMessage = "";
            lastCharacterMessage = "";
        }

        inputField.gameObject.SetActive(false);
        GameManager.Instance.SetDialogueActive(false);
        GameManager.Instance.ShowMessage("", 0f, null);

        if (isReactivatable)
        {
            StartCoroutine(ReactivateCooldown());
        }
    }

    private void ShowCharacterMessage(string message)
    {
        lastCharacterMessage = message;
        GameManager.Instance.ShowMessage(message, displayTime, portrait);
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