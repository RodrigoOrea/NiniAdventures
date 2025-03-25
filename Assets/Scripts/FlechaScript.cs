using UnityEngine;

public class FlechaScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;  // Asegúrate de asignar un valor en el Inspector
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destruir automáticamente después de 3 segundos si no choca con nada
        Destroy(gameObject, 3f);
    }

    private void FixedUpdate()
    {
        if (direction != Vector3.zero)
        {
            rb.velocity = direction * speed;
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitar colisión con el propio arquero
        if (collision.gameObject.CompareTag("Enemy")) return;

        CharacterMovement jugador = collision.GetComponent<CharacterMovement>();
        if (jugador != null)
        {
            jugador.Hit();
        }
        Destroy(gameObject);
    }


}