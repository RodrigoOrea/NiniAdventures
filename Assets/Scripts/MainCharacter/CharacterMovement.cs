using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpCount = 0;
    private int dashCount = 0; // Contador de dashes
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float originalGravityScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale; // Guardar la escala de gravedad original
    }

    void Update()
    {
        if (!isDashing)
        {
            // Movimiento horizontal
            float moveX = Input.GetAxis("Horizontal") * speed;
            rb.velocity = new Vector2(moveX, rb.velocity.y);

            // Detectar si está en el suelo
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer);

            // Reiniciar los contadores de saltos y dashes si está en el suelo
            if (isGrounded)
            {
                jumpCount = 0;
                dashCount = 0; // Reiniciar el contador de dashes
            }

            // Saltar cuando se presiona la barra espaciadora
            if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < 1))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;
            }

            // Dash cuando se presiona la tecla Q y no se ha usado el dash en este salto
            if (Input.GetKeyDown(KeyCode.Q) && dashCount < 1)
            {
                StartDash();
            }
        }
        else
        {
            // Manejar la duración del dash
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                StopDash();
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCount++; // Incrementar el contador de dashes

        // Desactivar la gravedad durante el dash
        rb.gravityScale = 0;

        // Determinar la dirección del dash basado en la entrada horizontal actual
        float dashDirection = Mathf.Sign(Input.GetAxis("Horizontal"));

        // Si no hay entrada horizontal, usar la dirección en la que el personaje está mirando
        if (dashDirection == 0)
        {
            dashDirection = Mathf.Sign(transform.localScale.x);
        }

        // Aplicar el dash en la dirección determinada
        rb.velocity = new Vector2(dashDirection * dashForce, 0); // Mantener la altura actual
    }

    void StopDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravityScale; // Restaurar la gravedad original
    }

}