using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float damage = 1f;
    public float flashDuration = 0.1f; // Duración del efecto de impacto (en segundos)

    private Vector2 direccion;

    private Rigidbody2D rb;

    public float velocidad = 10f;

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

    public void DestroyBullet()
{
    Destroy(gameObject); // Destruye el objeto que tiene este script
}
}
