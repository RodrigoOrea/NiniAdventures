using System.Collections;
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

    // Variables para el respawn
    public Transform player; // Referencia al transform del jugador
    public Transform initialRespawnPoint; // Punto de respawn inicial
    private Transform currentRespawnPoint; // Punto de respawn actual

    public TMP_Text textoVida;

    private float health = 100;

    private void Awake()
    {
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
    }

    // Método que se llama cuando el usuario termina de editar el InputField
    private void OnInputEndEdit(string userInput)
    {
        if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && !isWaitingForResponse)
        {
            isWaitingForResponse = true;
            Debug.Log("Envio prompt una vez");
            string userInput1 = userInput.Replace("\n", "").Trim();
            Gemini.SendRequest(userInput1);
            playerInput.text = "";
            playerInput.interactable = false; // Desactiva el InputField
        }
    }

    // Método para cambiar el punto de respawn
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

    // Método para manejar la muerte del jugador
    public void RespawnPlayer()
    {
        if (player != null && currentRespawnPoint != null)
        {
            // Mueve al jugador al punto de respawn
            player.position = currentRespawnPoint.position;
            Debug.Log("Jugador respawneado en: " + currentRespawnPoint.position);
            health = 100;
            textoVida.text = "VIDA: " + 100;
        }
        else
        {
            Debug.LogError("El jugador o el punto de respawn no están asignados.");
        }
    }
    public void RegisterBulletHit(float hit)
    {
        health -= hit; // Incrementar el contador
        textoVida.text = "VIDA: " + health; // Actualizar el texto en la UI
        if(health <= 0) RespawnPlayer();
    }

}