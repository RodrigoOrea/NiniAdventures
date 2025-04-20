using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Instancia singleton

    public GeminiAPI Gemini;
    public TMP_InputField playerInput; // Referencia al InputField
    private bool isWaitingForResponse = false;

    // ------------------------------------------
    // Sistema de diálogo mejorado
    // ------------------------------------------
    [Tooltip("Número máximo de mensajes a recordar para contexto")]
    [SerializeField] private int maxDialogueHistory = 3;
    private List<string> dialogueHistory = new List<string>();
    private string personajeText = ""; // Compatibilidad con código existente

    // Variables para el respawn
    public Transform player; // Referencia al transform del jugador
    public Transform initialRespawnPoint; // Punto de respawn inicial
    private Transform currentRespawnPoint; // Punto de respawn actual

    // UI Elements
    public TMP_Text textoHEALTH;
    private float health = 100;
    public TMP_Text textoAMMO;
    public int ammo = 50;
    public TMP_Text textoGRANADE;
    public int granades = 10;

    // Constants
    private static int MAX_AMMO = 50;
    private static int MAX_GRANADES = 10;
    private static int MAX_HEALTH = 100;

    // Stats
    public int dañoRealizado;
    public int dañoRecibido;
    public int enemiesKilled;

    // Dialogue System
    public TMP_Text dialogueText;
    private string currentMessage = "";
    private GameObject currentPortrait = null;

    private void Awake()
    {
        textoAMMO.text = "AMMO: 50";
        textoHEALTH.text = "HEALTH: 100";
        textoGRANADE.text = "GRANADE: 10";

        // Configura el singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: Mantener el GameManager entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruye cualquier duplicado
        }
    }

    void Start()
    {
        // Asigna el evento onEndEdit del InputField
        playerInput.onEndEdit.AddListener(OnInputEndEdit);

        // Inicializa el punto de respawn
        if (initialRespawnPoint != null)
        {
            currentRespawnPoint = initialRespawnPoint;
        }
        else
        {
            Debug.LogError("No se ha asignado un punto de respawn inicial.");
        }

        // Mensaje inicial para comenzar el diálogo
        AddToDialogueHistory("(El cactus te mira con desinterés)");
    }

    // ------------------------------------------
    // Métodos mejorados de diálogo
    // ------------------------------------------
    private void OnInputEndEdit(string userInput)
    {
        if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && !isWaitingForResponse)
        {
            string cleanInput = userInput.Replace("\n", "").Trim();

            if (!string.IsNullOrEmpty(cleanInput))
            {
                isWaitingForResponse = true;
                AddToDialogueHistory($"Jugador: {cleanInput}");

                // Construye contexto con los últimos mensajes
                string context = GetDialogueContext();
                Gemini.SendRequest(cleanInput, context);

                playerInput.text = "";
                playerInput.interactable = false; // Feedback visual
            }
        }
    }

    private void AddToDialogueHistory(string message)
    {
        dialogueHistory.Add(message);

        // Mantiene solo los últimos mensajes
        while (dialogueHistory.Count > maxDialogueHistory)
        {
            dialogueHistory.RemoveAt(0);
        }

        personajeText = message; // Mantiene compatibilidad
    }

    private string GetDialogueContext()
    {
        return string.Join("\n", dialogueHistory);
    }

    public void ShowMessage(string message, float duration, GameObject portraitToShow)
    {
        CancelInvoke(nameof(ClearMessage));

        // Añade la respuesta al historial
        AddToDialogueHistory($"Cactus: {message}");

        // Actualiza UI
        currentMessage = message;
        dialogueText.text = message;

        // Manejo de retratos
        if (currentPortrait != null && currentPortrait != portraitToShow)
            currentPortrait.SetActive(false);

        currentPortrait = portraitToShow;
        if (currentPortrait != null)
            currentPortrait.SetActive(true);

        // Restaura interacción
        isWaitingForResponse = false;
        playerInput.interactable = true;

        Invoke(nameof(ClearMessage), duration);
    }

    private void ClearMessage()
    {
        if (dialogueText.text == currentMessage)
        {
            dialogueText.text = "";

            if (currentPortrait != null)
                currentPortrait.SetActive(false);

            currentPortrait = null;
        }
    }

    // ------------------------------------------
    // Métodos existentes de gameplay
    // ------------------------------------------
    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint != null)
        {
            currentRespawnPoint = newRespawnPoint;
            Debug.Log("Punto de respawn actualizado a: " + newRespawnPoint.position);
        }
        else
        {
            Debug.LogError("El nuevo punto de respawn es nulo.");
        }
    }

    public void RespawnPlayer()
    {
        if (player != null && currentRespawnPoint != null)
        {
            player.position = currentRespawnPoint.position;
            Debug.Log("Jugador respawneado en: " + currentRespawnPoint.position);
            health = 100;
            textoHEALTH.text = "HEALTH: " + 100;
        }
        else
        {
            Debug.LogError("El jugador o el punto de respawn no están asignados.");
        }
    }

    public void RegisterBulletHit(float hit)
    {
        health -= hit;
        textoHEALTH.text = "HEALTH: " + health;
        if (health <= 0) RespawnPlayer();
        IncreaseDamageTaken((int)hit);
    }

    public void IncreaseAmmo(int amount)
    {
        ammo = Mathf.Min(ammo + amount, MAX_AMMO);
        granades = Mathf.Min(granades + amount, MAX_GRANADES);
        textoAMMO.text = "AMMO: " + ammo;
        textoGRANADE.text = "GRANADES: " + granades;
    }

    public void DecreaseAmmo(int amount)
    {
        ammo -= 1;
        textoAMMO.text = "AMMO: " + ammo;
    }

    public void DecreaseGranades(int amount)
    {
        granades -= 1;
        textoGRANADE.text = "GRANADES: " + granades;
    }

    public void IncreaseHealth(int amount)
    {
        health = Mathf.Min(health + amount, MAX_HEALTH);
        textoHEALTH.text = "HEALTH: " + health;
    }

    public void IncreaseDamageDone(int amount)
    {
        dañoRealizado += amount;
    }

    public void IncreaseDamageTaken(int amount)
    {
        dañoRecibido += amount;
    }

    public void IncreaseEnemiesKilled(int amount)
    {
        enemiesKilled += amount;
    }
}