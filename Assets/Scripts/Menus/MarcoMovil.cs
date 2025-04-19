using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarcoMovil : MonoBehaviour
{
    public float speed = 100f; // Velocidad de movimiento
    public float leftLimit = -300f; // Límite izquierdo
    public float rightLimit = 300f; // Límite derecho
    private int direction = 1; // 1 = derecha, -1 = izquierda

    void Update()
    {
        // Mueve el marco
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        // Cambia de dirección al llegar a los límites
        if (transform.localPosition.x >= rightLimit) direction = -1;
        else if (transform.localPosition.x <= leftLimit) direction = 1;
    }
}
