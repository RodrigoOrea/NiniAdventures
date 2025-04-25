using System.Collections;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;
    public float telegraphDuration = 2f; // Pre-attack delay
    public float attackSpeed = 5f;
    public float detectionRange = 2f;
    public float groundPauseDuration = 1f;
    public Transform player;
    [SerializeField] private int restoVida = 20; 

    private Vector3 startPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isFlying = true;
    private bool isAttacking = false;
    private bool isReturning = false;

    private float previousX;

    void Start()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        previousX = transform.position.x;
    }

    void Update()
    {
        if (isAttacking || isReturning) return;

        if (isFlying)
        {
            if (player != null)
            {
                float horizontalThreshold = 1.0f;
                bool isPlayerBelow = player.position.y < transform.position.y;
                bool isPlayerAligned = Mathf.Abs(player.position.x - transform.position.x) < horizontalThreshold;
                bool isInRange = Mathf.Abs(player.position.y - transform.position.y) <= detectionRange;

                if (isPlayerBelow && isPlayerAligned && isInRange)
                {
                    StartCoroutine(PrepareAttack());
                    return;
                }
            }

            // Flying movement (idle)
            float offsetX = Mathf.Sin(Time.time * speed) * moveDistance;
            Vector3 newPosition = new Vector3(startPosition.x + offsetX, startPosition.y, startPosition.z);
            transform.position = newPosition;

            // ✅ Corrected flip direction
            if (spriteRenderer != null)
                spriteRenderer.flipX = (newPosition.x > previousX);

            previousX = newPosition.x;
        }
    }

    private IEnumerator PrepareAttack()
    {
        isFlying = false;
        yield return new WaitForSeconds(telegraphDuration);

        if (player != null)
        {
            Vector2 target = player.position;
            StartCoroutine(DoAttack(target));
        }
    }

    private IEnumerator DoAttack(Vector2 target)
    {
        isAttacking = true;

        FacePlayer();
        animator.SetBool("isAttacking", true);

        // Move to player
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, attackSpeed * Time.deltaTime);
            yield return null;
        }

        // Make sure GameManager exists and restoVida is not zero
        if (GameManager.Instance != null && restoVida > 0)
        {
            GameManager.Instance.RegisterBulletHit(restoVida);
        }

        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(groundPauseDuration);

        // Return
        StartCoroutine(ReturnToStart());
    }

    private IEnumerator ReturnToStart()
    {
        isReturning = true;

        transform.rotation = Quaternion.identity;

        // Flip direction properly again
        if (spriteRenderer != null)
            spriteRenderer.flipX = (transform.position.x > previousX);

        while (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, attackSpeed * Time.deltaTime);
            yield return null;
        }

        // Reset everything
        isReturning = false;
        isAttacking = false;
        isFlying = true;

        // Flip logic restored
        previousX = transform.position.x;
    }

    private void FacePlayer()
    {
        if (player != null)
        {
            Vector3 dir = player.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);

            // Important! Turn off sprite flipping during rotation-based facing
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
        }
    }

}
