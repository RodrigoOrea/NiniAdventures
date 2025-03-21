using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(10);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemigo recibió " + damage + " de daño. Vida restante: " + health);
        
        StartCoroutine(FlashWhite()); // Efecto visual

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.red; // Puedes cambiarlo al color original del sprite
    }

    private void Die()
    {
        Debug.Log("Enemigo ha muerto.");
        Destroy(gameObject);
    }
}
