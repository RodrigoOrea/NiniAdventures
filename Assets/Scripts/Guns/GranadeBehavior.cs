using UnityEngine;

public class GranadeBehavior : MonoBehaviour
{
    public float lifetime = 3f;  // Tiempo antes de la explosión
    public float explosionRadius = 2f;  // Radio de la explosión
    public int damage = 50;  // Daño de la explosión
    public LayerMask damageLayer;  // Capa de objetos que pueden recibir daño

    private bool hasExploded = false;

    void Start()
    {
        // Programa la explosión después de 'lifetime' segundos
        Invoke("Explode", lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Explota al colisionar con cualquier objeto que no sea el jugador
        if (!hasExploded && !collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;  // Evita múltiples explosiones
        hasExploded = true;

        // Reproduce la animación de explosión
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Explosion");  // Nombre de la animación de explosión
            Debug.Log("Explode");
        }

        // Aplica daño a los objetos dentro del radio de explosión
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayer);
        foreach (Collider2D hit in colliders)
        {
            if (hit.TryGetComponent<EnemyHealth>(out var health))
            {
                health.TakeDamage(damage);
            }
        }
    }

    void DestroyGranade()
    {
        Destroy(gameObject);
    }

    // Opcional: Dibuja el radio de explosión en el editor para debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}