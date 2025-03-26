using UnityEngine;

public class GolemScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float patrolRange = 5f;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform edgeCheck;

    [Header("Attack Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Size Settings")]
    [SerializeField] private float originalSize = 5.4f;
    [SerializeField] private float cloneSizeReduction = 0.8f;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 startingPosition;
    private bool isFacingRight = true;
    private bool isDead = false;
    private bool canAttack = true;
    private float nextAttackTime = 0f;
    private bool isChasing = false;
    private float currentSize;

    private enum State { Idle, Walk, Attack, Hurt, Die }
    private State currentState;

    [Header("Death Settings")]
    [SerializeField] private GameObject miniGolemPrefab; // Asignar desde inspector
    [SerializeField] private float spawnOffsetX = 0.5f; // Distancia en X para spawnear miniGolems
    [SerializeField] private bool spawnMiniGolemsOnDeath = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        currentSize = transform.localScale.x / originalSize;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetState(State.Idle);
    }

    private void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Comportamiento de ataque
        if (distanceToPlayer <= attackRange && canAttack && PlayerInSight())
        {
            Attack();
        }
        // Comportamiento de persecución
        else if (distanceToPlayer <= detectionRange && PlayerInSight())
        {
            isChasing = true;
            ChasePlayer();
        }
        // Comportamiento de patrulla
        else
        {
            isChasing = false;
            Patrol();
        }

        UpdateAnimationState();
        CheckGroundAndEdges();
    }

    private void Patrol()
    {
        // Si está fuera del rango de patrulla, regresa
        if (Mathf.Abs(transform.position.x - startingPosition.x) > patrolRange)
        {
            isFacingRight = (transform.position.x < startingPosition.x);
        }

        // Movimiento de patrulla
        float moveDirection = isFacingRight ? 1 : -1;
        rb.velocity = new Vector2(moveDirection * patrolSpeed, rb.velocity.y);
        SetState(State.Walk);
    }

    private void ChasePlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }

        // Mover hacia el jugador pero mantener distancia de ataque
        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            float moveDirection = isFacingRight ? 1 : -1;
            rb.velocity = new Vector2(moveDirection * chaseSpeed, rb.velocity.y);
            SetState(State.Walk);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            SetState(State.Idle);
        }
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            SetState(State.Attack);
            GameManager.Instance.RegisterBulletHit(damage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;

        SetState(State.Hurt);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        SetState(State.Die);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        if (spawnMiniGolemsOnDeath && miniGolemPrefab != null)
        {
            Invoke("SpawnMiniGolems", 1f);
        }

        Destroy(gameObject, 1f);
    }

    private void SpawnMiniGolems()
    {
        // Instanciar miniGolem a la izquierda
        Instantiate(miniGolemPrefab, 
                  transform.position + new Vector3(-spawnOffsetX, 0, 0), 
                  Quaternion.identity);

        // Instanciar miniGolem a la derecha
        Instantiate(miniGolemPrefab, 
                  transform.position + new Vector3(spawnOffsetX, 0, 0), 
                  Quaternion.identity);
    }

    private void CheckGroundAndEdges()
    {
        // Verificar si hay suelo delante o si está en un borde
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        bool isEdge = !Physics2D.OverlapCircle(edgeCheck.position, 0.1f, groundLayer);

        if (!isGrounded || isEdge)
        {
            Flip();
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                                            (player.position - transform.position).normalized, 
                                            detectionRange, 
                                            groundLayer | playerLayer);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void SetState(State newState)
    {
        currentState = newState;

        // Reset all animation triggers
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Die");

        // Set the appropriate trigger
        switch (currentState)
        {
            case State.Walk:
                animator.SetTrigger("Walk");
                break;
            case State.Idle:
                animator.SetTrigger("Idle");
                break;
            case State.Attack:
                animator.SetTrigger("Attack");
                break;
            case State.Hurt:
                animator.SetTrigger("Hurt");
                break;
            case State.Die:
                animator.SetTrigger("Die");
                break;
        }
    }

    private void UpdateAnimationState()
    {
        if (currentState == State.Attack || currentState == State.Hurt || currentState == State.Die)
            return;

        if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            SetState(State.Walk);
        }
        else
        {
            SetState(State.Idle);
        }
    }

    // Llamado desde eventos de animación
    public void OnAttackAnimationEnd()
    {
        SetState(State.Idle);
    }

    public void OnHurtAnimationEnd()
    {
        SetState(State.Idle);
    }

    // Dibujar gizmos para visualización en el editor
    private void OnDrawGizmosSelected()
    {
        // Rango de patrulla
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startingPosition, patrolRange);

        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}