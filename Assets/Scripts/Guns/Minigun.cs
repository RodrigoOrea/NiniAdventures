using UnityEngine;
using System.Collections;

public class Minigun : Weapon
{
    public GameObject bulletPrefab; // Prefab de la bala
    public Transform firePoint; // Punto de origen del disparo
    public float spreadAngle = 30f; // Ángulo de dispersión (en grados)
    public int numberOfBullets = 8; // Número de balas por disparo
    public float bulletSpeed = 10f; // Velocidad de las balas
    public float range = 10f; // Alcance de las balas
    public float delayBetweenBullets = 0.05f; // Retraso entre cada bala
    public float fireRate = 0.5f; // Tiempo entre cada disparo continuo

    private float fireTimer = 0f; // Temporizador para controlar el fireRate

    void Update()
    {
        
    }

    public override void Shoot() {
        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
{
    if(currentAmmo > 0) {
                // Obtener la posición del ratón en el mundo
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Asegurarse de que la posición Z sea 0 (2D)

            // Calcular la dirección base hacia el ratón
            Vector2 baseDirection = (mousePosition - firePoint.position).normalized;

            for (int i = 0; i < numberOfBullets; i++)
            {
                // Calcular un ángulo aleatorio dentro del rango de dispersión
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);

                // Rotar la dirección base según el ángulo aleatorio
                Quaternion spreadRotation = Quaternion.Euler(0, 0, randomAngle);
                Vector2 bulletDirection = spreadRotation * baseDirection;

                // Calcular la rotación de la bala basada en la dirección
                float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
                Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

                // Instanciar la bala
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

                // Aplicar velocidad a la bala
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = bulletDirection * bulletSpeed;
                }

                // Destruir la bala después de alcanzar el rango máximo
                Destroy(bullet, range / bulletSpeed);

                // Esperar un pequeño retraso antes de disparar la siguiente bala
                yield return new WaitForSeconds(delayBetweenBullets);

                currentAmmo--;
            }
    }
}
}