using System.Collections;
using UnityEngine;

public class SecondElevator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject enemy;

    public GameObject enemy2;
    public Transform spawnPosition;
    public float cooldownTime = 0.5f; // Tiempo entre activaciones

    private bool alreadyTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;
            SpawnObject();
            Debug.Log("Enemigo generado correctamente");
        }
    }

    private void SpawnObject()
    {
        //Instantiate(enemy, spawnPosition.position, Quaternion.identity);
        Instantiate(enemy2, spawnPosition.position, Quaternion.identity);
        Destroy(GetComponent<Collider2D>()); // Solo destruye el collider
    }
}