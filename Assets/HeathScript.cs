using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{

    private GameManager gameController;
    // Start is called before the first frame update
    void Start()
    {
        // Busca el GameController en la escena
        gameController = FindObjectOfType<GameManager>();
        if (gameController == null)
        {
            Debug.LogError("No se encontró el GameController en la escena.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si colisionó con un objeto con la etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HealthBox");
            if (gameController != null)
            {
                // Aumenta la munición en el GameController
                gameController.IncreaseHealth(100);
            }

            // Destruye la granada después de la colisión (opcional)
            Destroy(gameObject);
        }
    }
}
