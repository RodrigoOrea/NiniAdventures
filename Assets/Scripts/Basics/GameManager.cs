using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración del Jugador")]
    public Transform player;
    public Transform initialRespawnPoint;
    private Transform currentRespawnPoint;

    [Header("UI Elements")]
    public TMP_Text textoHEALTH;
    public TMP_Text textoAMMO;
    public TMP_Text textoGRANADE;
    public TMP_Text dialogueText;

    [Header("Configuración del Juego")]
    private float health = 100;
    public int ammo = 50;
    public int granades = 10;
    private static int MAX_AMMO = 50;
    private static int MAX_GRANADES = 10;
    private static int MAX_HEALTH = 100;

    [Header("Estadísticas")]
    public int dañoRealizado;
    public int dañoRecibido;
    public int enemiesKilled;

    private string currentMessage = "";
    private GameObject currentPortrait = null;
    private bool isDialogueActive = false;
    private bool isGamePaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        textoAMMO.text = "AMMO: 50";
        textoHEALTH.text = "HEALTH: 100";
        textoGRANADE.text = "GRANADE: 10";
    }

    void Start()
    {
        if (initialRespawnPoint != null)
        {
            currentRespawnPoint = initialRespawnPoint;
        }
        else
        {
            Debug.LogError("No se ha asignado un punto de respawn inicial.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }
    }

    public void SetDialogueActive(bool active)
    {
        isDialogueActive = active;
        UpdateGameState();
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        if (isDialogueActive || isGamePaused)
        {
            //Time.timeScale = 1;
            Cursor.visible = true;
        }
        else
        {
            //Time.timeScale = 1;
            Cursor.visible = false;
        }
    }

    public void ShowMessage(string message, float duration, GameObject portrait)
    {
        CancelInvoke(nameof(ClearMessage));
        currentMessage = message;
        dialogueText.text = message;

        if (currentPortrait != null && currentPortrait != portrait)
            currentPortrait.SetActive(false);

        currentPortrait = portrait;
        if (currentPortrait != null)
            currentPortrait.SetActive(true);

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

    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint != null)
        {
            currentRespawnPoint = newRespawnPoint;
        }
    }

    public void RespawnPlayer()
    {
        if (player != null && currentRespawnPoint != null)
        {
            player.position = currentRespawnPoint.position;
            health = 100;
            textoHEALTH.text = "HEALTH: 100";
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

