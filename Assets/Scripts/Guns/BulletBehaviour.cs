using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float damage = 1f;
    public float flashDuration = 0.1f; // Duración del efecto de impacto (en segundos)

    private Vector2 direccion;

    private Rigidbody2D rb;

    public float velocidad = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si la bala colisiona con el personaje
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtener el componente del personaje
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                // Aplicar daño al personaje (si es necesario)
                player.TakeDamage(damage);

                // Cambiar el color del personaje momentáneamente
                player.FlashWhite(flashDuration);
            }

            // Destruir la bala después del impacto
            Destroy(gameObject);
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);  // Destruye la bala si sale de la pantalla
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener la posición del ratón en el mundo
        Vector3 posicionRaton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posicionRaton.z = 0;

        // Calcular la dirección
        direccion = (posicionRaton - transform.position).normalized;

        // Aplicar velocidad al Rigidbody2D
        rb.velocity = direccion * velocidad;
    }
}
