using UnityEngine;

public class GranadeBehavior : MonoBehaviour
{
    public float lifetime = 3f;  // Tiempo antes de destruir la bala

    void Start()
    {
        Destroy(gameObject, lifetime);  // Destruye la bala después de 'lifetime' segundos
    }
}
