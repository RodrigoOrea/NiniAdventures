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

    [SerializeField] private LayerMask movingPlatformLayer;
    private Collider2D currentPlatform;

    void Update()
    {
        // Detectar si estamos sobre cualquier plataforma (fija o móvil)
        Collider2D groundCheckCollider = Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);
        Collider2D platformCheckCollider = Physics2D.OverlapCircle(groundCheck.position, 0.05f, movingPlatformLayer);
        
        bool wasGrounded = isGrounded;
        isGrounded = groundCheckCollider != null || platformCheckCollider != null;
        
        // Si acabamos de aterrizar en una nueva plataforma
        if (!wasGrounded && isGrounded)
        {
            currentPlatform = platformCheckCollider ?? groundCheckCollider;
        }
        
        // Si estamos en el aire pero todavía conectados a una plataforma móvil (ascensor)
        if (!isGrounded && currentPlatform != null && 
            currentPlatform.OverlapPoint(groundCheck.position))
        {
            isGrounded = true;
        }
        
        animator.SetBool("IsJumping", !isGrounded);
        
        // Resto del código...
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

    void LateUpdate()
    {
        // Moverse con la plataforma si es móvil
        if (isGrounded && currentPlatform != null && 
            ((1 << currentPlatform.gameObject.layer) & movingPlatformLayer) != 0)
        {
            transform.parent = currentPlatform.transform;
        }
        else
        {
            transform.parent = null;
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