using System.Collections;
using UnityEngine;
public class ElevatorMovement : MonoBehaviour
{
    public float speed = 2f;
    public float bottomY = -37f; // Altura inferior (nueva variable)
    public float waitTime = 20f;
    
    private Vector3 startPos;
    private bool isMoving = false;
    private bool isWaiting = false;
    private float timeAtStop;
    private float targetY; // Destino actual (alterna entre startPos.y y bottomY)

    private void Start()
    {
        startPos = transform.position;
        targetY = bottomY; // Primer destino: bajar
    }

    private void Update()
    {
        if (isWaiting)
        {
            timeAtStop += Time.deltaTime;
            if (timeAtStop >= waitTime)
            {
                isWaiting = false;
                timeAtStop = 0f;
                isMoving = true;
                
                // Alternar destino
                targetY = Mathf.Approximately(targetY, startPos.y) ? bottomY : startPos.y;
            }
        }

        if (isMoving && !isWaiting)
        {
            MoveToTarget();
        }
    }

        private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, targetY, transform.position.z),
            speed * Time.deltaTime
        );

        if (Mathf.Approximately(transform.position.y, targetY))
        {
            isMoving = false;

            // Si ha llegado al fondo, espera 5 segundos antes de volver a subir
            if (Mathf.Approximately(targetY, bottomY))
            {
                StartCoroutine(WaitAtBottom());
            }
            else
            {
                isWaiting = true;
            }

            Debug.Log($"Llegó a {targetY} en {Time.time}");
        }
    }

    private IEnumerator WaitAtBottom()
    {
        isWaiting = true;
        yield return new WaitForSeconds(10f);
        isWaiting = false;
        isMoving = true;
        targetY = startPos.y; // Ahora sube
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isMoving && !isWaiting)
        {
            Debug.Log("Player");
            StartCoroutine(WaitAndMove());
        }
    }

    private IEnumerator WaitAndMove()
    {
        isWaiting = true;

        yield return new WaitForSeconds(4f); // Espera 4 segundos

        isWaiting = false;
        isMoving = true;
        targetY = bottomY; // Ahora sí inicia el movimiento hacia abajo
    }
}