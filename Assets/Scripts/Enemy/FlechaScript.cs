using UnityEngine;

public class FlechaScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;  // Aseg�rate de asignar un valor en el Inspector
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destruir autom�ticamente despu�s de 3 segundos si no choca con nada
        Destroy(gameObject, 3f);
    }

    private void FixedUpdate()
    {
        if (direction != Vector3.zero)
        {
            rb.velocity = direction * speed;
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitar colisi�n con el propio arquero
        if (collision.gameObject.CompareTag("Player")) {
            GameManager.Instance.RegisterBulletHit(20);
            Destroy(gameObject);}

    }


}