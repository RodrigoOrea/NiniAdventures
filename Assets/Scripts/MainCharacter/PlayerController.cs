using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float health = 100f; // Salud del personaje
    private Material originalMaterial; // Material original del personaje
    private Material flashMaterial; // Material para el efecto de impacto
    private SpriteRenderer spriteRenderer; // Renderer del personaje

    // Lógica de animación
    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;


    void Start()
    {
        // Lógica de animación
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();

        // Obtener el SpriteRenderer del personaje
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Guardar el material original
        originalMaterial = spriteRenderer.material;

        // Crear un material blanco para el efecto de impacto
        flashMaterial = new Material(Shader.Find("Sprites/Default"));
        flashMaterial.color = Color.white;
    }

    void Update()
    {
        // Verificar si el jugador está en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (isGrounded) animator.SetBool("IsJumping", false);
        else animator.SetBool("IsJumping", true);

        // Movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Mirar a la derecha
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Mirar a la izquierda
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto colisionado tiene la etiqueta "enemyBullet"
        if (collision.gameObject.CompareTag("enemyBullet"))
        {
            // Aplicar efecto visual de impacto
            FlashWhite(0.1f);

            GameManager.Instance.RegisterBulletHit(15.0f);

        }
    }

    public void FlashWhite(float duration)
    {
        // Cambiar el material al material blanco
        spriteRenderer.material = flashMaterial;

        // Restaurar el material original después de un breve momento
        Invoke("RestoreMaterial", duration);
    }

    private void RestoreMaterial()
    {
        // Restaurar el material original
        spriteRenderer.material = originalMaterial;
    }

    //private void Die()
    //{
        // Lógica para cuando el personaje muere
        //Debug.Log("Player has died!");
        //Destroy(gameObject);
    //}

    
}