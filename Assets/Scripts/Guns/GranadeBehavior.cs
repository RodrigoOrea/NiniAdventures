using UnityEngine;

public class GranadeBehavior : MonoBehaviour
{
    public float lifetime = 3f;  // Tiempo antes de la explosión
    public float explosionRadius = 5f;  // Radio de la explosión
    public int damage = 50;  // Daño de la explosión
    public LayerMask damageLayer;  // Capa de objetos que pueden recibir daño

    private bool hasExploded = false;

    void Start()
    {
        // Programa la explosión después de 'lifetime' segundos
        Invoke("Explode", lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Explota al colisionar con cualquier objeto
        if (!hasExploded)
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
        }


        // Aplica daño a los objetos dentro del radio de explosión
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        foreach (Collider hit in colliders)
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
}