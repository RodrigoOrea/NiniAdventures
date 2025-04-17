using UnityEngine;

public class AngrySun : MonoBehaviour
{
    [Header("Movement Settings")]
    public float pendulumAmplitude = 5f; // Rango horizontal del movimiento pendular
    public float pendulumSpeed = 1f; // Velocidad del movimiento pendular
    public float attackHeightOffset = 2f; // Altura sobre el jugador para iniciar ataque
    public float attackSpeed = 5f; // Velocidad del ataque
    public float timeBetweenAttacks = 5f; // Tiempo entre ataques
    public float returnSpeed = 3f; // Velocidad para volver a posición pendular

    [Header("References")]
    public Transform player;
    public LayerMask obstacleLayer; // Capa para detectar obstáculos

    private Vector2 pendulumCenter;
    private float pendulumAngle = 0f;
    private bool isAttacking = false;
    private Vector2 attackTargetPosition;
    private float timeSinceLastAttack = 0f;
    private bool isReturning = false;
    private Vector2 returnStartPosition;
    private float returnJourneyLength;
    private float returnStartTime;

    private float originalY; // Añade esta variable al inicio de la clase


    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        pendulumCenter = transform.position;
        originalY = transform.position.y;

    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (!isAttacking && !isReturning && timeSinceLastAttack >= timeBetweenAttacks)
        {
            InitiateAttack();
        }

        if (isAttacking)
        {
            PerformAttack();
        }
        else if (isReturning)
        {
            ReturnToPendulum();
        }
        else
        {
            PerformPendulumMovement();
        }
    }

    private void InitiateAttack()
    {
        // Toma la altura actual del jugador pero no la cambia durante el ataque
        attackTargetPosition = new Vector2(player.position.x, originalY - attackHeightOffset);
        isAttacking = true;
        timeSinceLastAttack = 0f;
    }

    private void PerformAttack()
    {
        transform.position = Vector2.MoveTowards(transform.position, attackTargetPosition, attackSpeed * Time.deltaTime);

        // Comprobar si hemos alcanzado la posición objetivo
        if (Vector2.Distance(transform.position, attackTargetPosition) < 0.1f)
        {
            StartReturning();
        }
    }

    private void StartReturning()
    {
        isAttacking = false;
        isReturning = true;
        returnStartPosition = transform.position;
        returnStartTime = Time.time;
        
        // Calculamos un punto seguro para volver (centro del péndulo)
        Vector2 safeReturnPoint = new Vector2(
            pendulumCenter.x + Mathf.Sin(pendulumAngle) * pendulumAmplitude,
            pendulumCenter.y
        );
        
        returnJourneyLength = Vector2.Distance(returnStartPosition, safeReturnPoint);
    }

    private void ReturnToPendulum()
    {
        float distCovered = (Time.time - returnStartTime) * returnSpeed;
        float fractionOfJourney = distCovered / returnJourneyLength;

        // Calculamos la posición pendular actual mientras volvemos
        Vector2 currentPendulumPosition = new Vector2(
            pendulumCenter.x + Mathf.Sin(pendulumAngle) * pendulumAmplitude,
            pendulumCenter.y
        );

        // Interpolamos suavemente hacia la posición pendular
        transform.position = Vector2.Lerp(returnStartPosition, currentPendulumPosition, fractionOfJourney);

        if (fractionOfJourney >= 1f)
        {
            isReturning = false;
            pendulumAngle = Mathf.Asin((transform.position.x - pendulumCenter.x) / pendulumAmplitude);
        }
    }

    private void PerformPendulumMovement()
    {
        float baseX = player.position.x;
    
        // Oscilación independiente
        pendulumAngle += pendulumSpeed * Time.deltaTime;
        float offsetX = Mathf.Sin(pendulumAngle) * pendulumAmplitude;
        
        // Mantén tu posición Y original
        float newY = transform.position.y;
        
        // Aplica el movimiento (posición del jugador + oscilación)
        transform.position = new Vector2(baseX + offsetX, newY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // El sol ha golpeado al jugador
            GameManager.Instance.RegisterBulletHit(20);
            
            // Opcional: hacer que el sol rebote o retroceda
            if (isAttacking)
            {
                StartReturning();
            }
        }
    }

    // Dibujar gizmos para visualizar el rango de movimiento en el editor
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector2(transform.position.x - pendulumAmplitude, transform.position.y),
                new Vector2(transform.position.x + pendulumAmplitude, transform.position.y)
            );
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector2(pendulumCenter.x - pendulumAmplitude, pendulumCenter.y),
                new Vector2(pendulumCenter.x + pendulumAmplitude, pendulumCenter.y)
            );
            
            if (isAttacking)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, attackTargetPosition);
            }
        }
    }
}