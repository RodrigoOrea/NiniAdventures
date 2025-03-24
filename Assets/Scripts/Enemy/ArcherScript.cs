using System.Collections;
using UnityEngine;

public class ArcherScript : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 5f;
    private Vector3 startPosition;
    private Vector3 nextPosition;
    private bool isMovingRight = true;

    [Header("Combat")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpDistance = 3f;
    [SerializeField] private float shootRange = 8f;
    [SerializeField] private float jumpRange = 4f;
    [SerializeField] private float timeBetweenShots = 2f;
    private Transform player;
    private bool isJumping = false;
    private bool isPlayerInShootRange = false;
    private float lastShotTime;

    [Header("Projectile")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        startPosition = transform.position;
        nextPosition = startPosition + Vector3.right * patrolDistance;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isPlayerInShootRange = distanceToPlayer <= shootRange;

        // Prioridad 1: Saltar si el jugador está muy cerca (incluso si está disparando)
        if (distanceToPlayer <= jumpRange && !isJumping)
        {
            Jump();
        }
        // Prioridad 2: Disparar si el jugador está en rango
        else if (isPlayerInShootRange)
        {
            StopPatrol();
            LookAtPlayer();
            TryShoot();
        }
        // Prioridad 3: Patrullar si el jugador está fuera de rango
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("runningArcher", true);
        animator.SetBool("shootArcher", false);
        transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextPosition) < 0.1f)
        {
            isMovingRight = !isMovingRight;
            nextPosition = isMovingRight ? startPosition + Vector3.right * patrolDistance : startPosition;
            Flip();
        }
    }

    void StopPatrol()
    {
        animator.SetBool("runningArcher", false);
    }

    void Jump()
    {
        isJumping = true;
        Vector2 jumpDirection = (player.position.x < transform.position.x) ? Vector2.left : Vector2.right;
        rb.velocity = new Vector2(jumpDirection.x * jumpDistance, jumpForce);
        StartCoroutine(ResetJumpFlag());
    }

    void TryShoot()
    {
        if (Time.time - lastShotTime >= timeBetweenShots)
        {
            animator.SetBool("shootingArcher", true);
            lastShotTime = Time.time;
        }
    }

    /*// Llamado desde Animation Event al final de la animación "shootingArcher"
    void ShootArrow()
    {
        Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        animator.SetBool("shootingArcher", false);
    }*/

    void LookAtPlayer()
    {
        if ((player.position.x > transform.position.x && transform.localScale.x < 0) ||
            (player.position.x < transform.position.x && transform.localScale.x > 0))
        {
            Flip();
        }
    }

    IEnumerator ResetJumpFlag()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}