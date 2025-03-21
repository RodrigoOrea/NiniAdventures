using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntSeguir : MonoBehaviour
{
    private Animator Animator;
    private Rigidbody2D Rigidbody2D;

    [SerializeField] private float speed;
    [SerializeField] private float followRange = 10.0f; // Rango de detección del jugador
    [SerializeField] private float shootRange = 6.76f;   // Rango de disparo
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bulletPrefab;    // Prefab de la bala
    [SerializeField] private Transform firePoint;        // Punto de disparo
    [SerializeField] private float bulletSpeed = 10f;    // Velocidad de la bala
    [SerializeField] private float shootCooldown = 1f;  // Tiempo entre disparos

    private bool isFacingRight = true;
    private float timeSinceLastShot = 0f;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange) // Solo sigue si el jugador está dentro del rango
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            bool isPlayerRight = transform.position.x < player.transform.position.x;
            Flip(isPlayerRight);

            Animator.SetBool("running", speed != 0.0f);

            if (distanceToPlayer <= shootRange) // Si el jugador está dentro del rango de disparo
            {
                if (timeSinceLastShot >= shootCooldown)
                {
                    Shoot();
                    timeSinceLastShot = 0f;
                }
            }
        }
        else
        {
            Animator.SetBool("running", false); // Detiene la animación si el jugador está fuera del rango
        }

        timeSinceLastShot += Time.deltaTime;
    }

    private void Flip(bool isPlayerRight)
    {
        if ((isFacingRight && !isPlayerRight) || (!isFacingRight && isPlayerRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplicar velocidad a la bala
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * bulletSpeed;  // Dispara en la dirección de la pistola
        }
    }
}