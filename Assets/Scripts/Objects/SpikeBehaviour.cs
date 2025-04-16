using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colision detectada");
        // Verificar si el objeto colisionado tiene la etiqueta "enemyBullet"
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.RegisterBulletHit(15.0f);

        }
    }
}
