using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveInterval = 2f; // Tiempo entre movimientos
    public float moveSpeed = 3f; // Velocidad de movimiento

    [Header("Shooting Settings")]
    public float shootInterval = 1f; // Tiempo entre disparos
    public GameObject bulletPrefab; // Prefab de la bala
    public Transform shootPoint; // Punto de origen del disparo

    [Header("Drop Settings")]
    public GameObject[] dropItems; // Array de items que puede dropear al morir
    [Range(0f, 1f)] public float dropChance = 0.5f; // Probabilidad de dropear un item

    private Transform player; // Referencia al jugador
    private float moveTimer;
    private float shootTimer;
    private bool isAlive = true;

    void Start()
    {
        // Busca al jugador por su tag (asegúrate de que el jugador tenga el tag "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializa los timers
        moveTimer = moveInterval;
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (!isAlive) return;

        // Movimiento hacia el jugador
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0)
        {
            MoveTowardsPlayer();
            moveTimer = moveInterval;
        }

        // Disparo
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        // Calcula la dirección hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;

        // Mueve al enemigo en esa dirección
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Opcional: Rotar al enemigo para que mire hacia el jugador
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null) return;

        // Instancia la bala en el punto de disparo
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
    }

    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        // Dropear un item aleatorio
        DropItem();

        // Destruir al enemigo
        Destroy(gameObject);
    }

    void DropItem()
    {
        if (dropItems.Length == 0) return;

        // Decide si dropear un item basado en la probabilidad
        if (Random.value <= dropChance)
        {
            // Selecciona un item aleatorio del array
            int index = Random.Range(0, dropItems.Length);
            Instantiate(dropItems[index], transform.position, Quaternion.identity);
        }
    }
}