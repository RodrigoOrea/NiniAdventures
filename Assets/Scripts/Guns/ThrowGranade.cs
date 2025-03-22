using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    public GameObject grenadePrefab;  // Prefab de la granada
    public Transform throwPoint;      // Punto desde donde se lanza la granada
    public float throwForceX = 5f;    // Fuerza horizontal del lanzamiento
    public float throwForceY = 8f;    // Fuerza vertical del lanzamiento

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Detecta la tecla "F"
        {
            Throw();
        }
    }

    void Throw()
    {
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = new Vector2(throwForceX * transform.localScale.x, throwForceY);
        }
        GameManager.Instance.DecreaseGranades(1);
    }
}
