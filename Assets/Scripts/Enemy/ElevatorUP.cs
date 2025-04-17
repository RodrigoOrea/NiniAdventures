using System.Collections;
using UnityEngine;

public class ElevatorMovementUP : MonoBehaviour
{
    public float speed = 2f;
    public float topY = 10f; // Altura a la que subirá la plataforma

    private Vector3 startPos;
    private bool isMoving = false;
    private bool isWaiting = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToTop();
        }
    }

    private void MoveToTop()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, topY, transform.position.z),
            speed * Time.deltaTime
        );

        if (Mathf.Approximately(transform.position.y, topY))
        {
            isMoving = false; // Detener el movimiento al llegar
            Debug.Log("Elevador llegó arriba.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isMoving && !isWaiting)
        {
            Debug.Log("Jugador entró en la plataforma.");
            StartCoroutine(WaitAndRise());
        }
    }

    private IEnumerator WaitAndRise()
    {
        isWaiting = true;

        yield return new WaitForSeconds(3f); // Espera de 3 segundos antes de subir

        isWaiting = false;
        isMoving = true;
    }
}
