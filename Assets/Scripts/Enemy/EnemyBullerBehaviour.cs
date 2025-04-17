using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    public float damage = 1f;
    public float flashDuration = 0.1f; // Duración del efecto de impacto (en segundos)
    public float velocidad = 10f;

    private Vector2 direccion;
    private Rigidbody2D rb;
    private GameObject player; // Referencia al jugador

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Buscar el GameObject con la etiqueta "Player"
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Calcular la dirección hacia el jugador
            direccion = (player.transform.position - transform.position).normalized;

            // Aplicar velocidad al Rigidbody2D
            rb.velocity = direccion * velocidad;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player'.");
            Destroy(gameObject); // Destruye la bala si no hay jugador
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);  // Destruye la bala si sale de la pantalla
    }

    public void DestroyBullet()
    {
        Destroy(gameObject); // Destruye el objeto que tiene este script
    }
}