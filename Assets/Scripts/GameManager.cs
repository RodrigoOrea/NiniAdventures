using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Corrected Singleton Pattern

    [Header("Gemini API Integration")]
    public GeminiAPI Gemini;
    public TMP_InputField playerInput;
    private bool isWaitingForResponse = false;

    [Header("Respawn System")]
    public Transform player; // Player transform
    public Transform initialRespawnPoint; // Initial respawn point
    private Transform currentRespawnPoint; // Current respawn point

    [Header("Player Stats")]
    public TMP_Text textoHEALTH;
    private float health = 100;

    public TMP_Text textoAMMO;
    public int ammo = 50;

    public TMP_Text textoGRANADE;
    public int granades = 10;

    private static int MAX_AMMO = 50;
    private static int MAX_GRANADES = 10;
    private static int MAX_HEALTH = 100;

    [Header("Level Completion")]
    private float startTime;
    public int enemiesKilled = 0;
    public GameObject levelCompleteUI;
    public TMP_Text timeText, killText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep GameManager between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        // Initialize variables
        startTime = Time.time;
        playerInput.onEndEdit.AddListener(OnInputEndEdit);

        // Initialize respawn point
        if (initialRespawnPoint != null)
        {
            currentRespawnPoint = initialRespawnPoint;
        }
        else
        {
            Debug.LogError("No initial respawn point assigned.");
        }

        // Initialize UI
        textoAMMO.text = "AMMO: " + ammo;
        textoHEALTH.text = "HEALTH: " + health;
        textoGRANADE.text = "GRANADES: " + granades;
    }

    private void OnInputEndEdit(string userInput)
    {
        if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && !isWaitingForResponse)
        {
            isWaitingForResponse = true;
            string userInput1 = userInput.Replace("\n", "").Trim();
            Gemini.SendRequest(userInput1);
            playerInput.text = "";
            playerInput.interactable = false;
        }
    }

    // Respawn System
    public void RespawnPlayer()
    {
        if (player != null && currentRespawnPoint != null)
        {
            player.position = currentRespawnPoint.position;
            health = MAX_HEALTH;
            textoHEALTH.text = "HEALTH: " + health;
        }
        else
        {
            Debug.LogError("Player or respawn point not assigned.");
        }
    }

    public void RegisterBulletHit(float hit)
    {
        health -= hit;
        textoHEALTH.text = "HEALTH: " + health;
        if (health <= 0) RespawnPlayer();
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
        ammo -= amount;
        textoAMMO.text = "AMMO: " + ammo;
    }

    public void DecreaseGranades(int amount)
    {
        granades -= amount;
        textoGRANADE.text = "GRANADES: " + granades;
    }

    public void IncreaseHealth(int amount)
    {
        health = Mathf.Min(health + amount, MAX_HEALTH);
        textoHEALTH.text = "HEALTH: " + health;
    }

    // Level Completion
    public void EnemyKilled()
    {
        enemiesKilled++;
    }

    public void LevelComplete()
    {
        float timeTaken = Time.time - startTime;
        levelCompleteUI.SetActive(true);
        timeText.text = "Time: " + timeTaken.ToString("F2") + "s";
        killText.text = "Enemies Killed: " + enemiesKilled;
        Time.timeScale = 0f; // Pause game on level complete
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint != null)
        {
            currentRespawnPoint = newRespawnPoint;
            Debug.Log("Respawn point updated to: " + newRespawnPoint.position);
        }
        else
        {
            Debug.LogError("New respawn point is null.");
        }
    }
}
