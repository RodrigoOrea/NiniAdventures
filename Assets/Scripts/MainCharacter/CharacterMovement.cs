using TMPro;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 7f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public LayerMask movingPlatformLayer;
    public LayerMask elevatorLayer; // Nueva capa para el elevador
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Health and Impact")]
    public float health = 100f;
    private Material originalMaterial;
    private Material flashMaterial;
    private SpriteRenderer spriteRenderer;

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // States
    private bool isGrounded;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;

    private int jumpCount = 0;
    private int dashCount = 0;

    private Collider2D currentPlatform;
    private Vector2 platformVelocity;
    private Transform previousParent; // Para guardar el padre anterior

    public TMP_InputField inputField;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalMaterial = spriteRenderer.material;
        flashMaterial = new Material(Shader.Find("Sprites/Default"));
        flashMaterial.color = Color.white;
        
        previousParent = transform.parent; // Guardar el padre inicial
    }

    void Update()
    {
        // Si el input está enfocado, bloqueamos el control del jugador
        if (inputField != null && inputField.isFocused)
            return;
        if (!isDashing)
        {
            HandleGroundDetection();
            HandleJump();
            HandleDash();
            HandleAnimationAndDirection();
        }
        else
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0) StopDash();
        }
    }

    private void FixedUpdate()
    {
        if (inputField != null && inputField.isFocused)
            return;
        if (!isDashing)
        {
            HandleHorizontalMovement();
            // Eliminamos ApplyPlatformMovement() ya que lo manejaremos diferente
        }
    }

    void HandleGroundDetection()
    {
        bool wasGrounded = isGrounded;

        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Collider2D platformCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, movingPlatformLayer);
        Collider2D elevatorCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, elevatorLayer);

        isGrounded = groundCollider != null || platformCollider != null || elevatorCollider != null;

        if (isGrounded)
        {
            // Solo nos hacemos hijos del elevador, no de otras plataformas
            if (elevatorCollider != null)
            {
                currentPlatform = elevatorCollider;
                if (transform.parent != elevatorCollider.transform)
                {
                    previousParent = transform.parent;
                    transform.SetParent(elevatorCollider.transform);
                    rb.interpolation = RigidbodyInterpolation2D.None;
                }
            }
            else
            {
                currentPlatform = platformCollider ?? groundCollider;
                if (transform.parent != previousParent)
                {
                    transform.SetParent(previousParent);
                    rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                }
            }

            if (!wasGrounded) platformVelocity = Vector2.zero;
        }
        else
        {
            currentPlatform = null;
            if (transform.parent != previousParent)
            {
                transform.SetParent(previousParent);
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }

        if (isGrounded)
        {
            jumpCount = 0;
            dashCount = 0;
        }
    }

    void HandleHorizontalMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        float targetVelocityX = moveInput * speed;
        
        // Si estamos en un elevador, mantenemos nuestra velocidad relativa
        if (currentPlatform != null && currentPlatform.gameObject.layer == LayerMask.NameToLayer("Elevator"))
        {
            Rigidbody2D platformRb = currentPlatform.GetComponent<Rigidbody2D>();
            if (platformRb != null)
            {
                // Calculamos velocidad relativa al ascensor
                float relativeVelocityX = targetVelocityX - platformRb.velocity.x;
                rb.velocity = new Vector2(relativeVelocityX, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
        }
    }

    void ApplyPlatformMovement()
    {
        if (currentPlatform != null && isGrounded)
        {
            Rigidbody2D platformRB = currentPlatform.GetComponent<Rigidbody2D>();
            Vector2 newPlatformVelocity = platformRB != null ? platformRB.velocity : Vector2.zero;

            rb.velocity = new Vector2(rb.velocity.x, newPlatformVelocity.y);
            platformVelocity = Vector2.Lerp(platformVelocity, newPlatformVelocity, Time.fixedDeltaTime * 10f);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < 1))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            currentPlatform = null;
            transform.SetParent(previousParent); // Dejar de ser hijo al saltar
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Q) && dashCount < 1)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCount++;

        float dashDirection = Mathf.Sign(Input.GetAxis("Horizontal"));
        if (dashDirection == 0) dashDirection = Mathf.Sign(transform.localScale.x);

        rb.velocity = new Vector2(dashDirection * dashForce, 0);
        currentPlatform = null;
        transform.SetParent(previousParent); // Dejar de ser hijo al hacer dash
    }

    void StopDash()
    {
        isDashing = false;
        rb.velocity = new Vector2(rb.velocity.x * 0.5f, 0);
    }

    void HandleAnimationAndDirection()
    {
        float moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsJumping", !isGrounded);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("enemyBullet"))
        {
            FlashWhite(0.1f);
            GameManager.Instance.RegisterBulletHit(15.0f);
        }
    }

    public void FlashWhite(float duration)
    {
        spriteRenderer.material = flashMaterial;
        Invoke("RestoreMaterial", duration);
    }

    private void RestoreMaterial()
    {
        spriteRenderer.material = originalMaterial;
    }
}