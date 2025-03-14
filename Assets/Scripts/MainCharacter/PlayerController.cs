using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float health = 100f; // Salud del personaje
    private Material originalMaterial; // Material original del personaje
    private Material flashMaterial; // Material para el efecto de impacto
    private SpriteRenderer spriteRenderer; // Renderer del personaje



    //logica de animacion
    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    public Transform groundCheck;

    public LayerMask groundLayer;

    void Start()
    {
        //lógica de animcion
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
        if(isGrounded) animator.SetBool("IsJumping", false);
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

    public void TakeDamage(float damage)
    {
        // Reducir la salud del personaje
        health -= damage;

        // Verificar si el personaje ha muerto
        if (health <= 0)
        {
            Die();
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

    private void Die()
    {
        // Lógica para cuando el personaje muere
        Debug.Log("Player has died!");
        Destroy(gameObject);
    }
}