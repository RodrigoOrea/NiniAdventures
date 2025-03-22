using UnityEngine;

public class Gun : Weapon
{
    public GameObject bulletPrefab;  // Prefab de la bala (círculo)
    public Transform firePoint;      // Punto desde donde se dispara
    public float bulletSpeed = 10f;  // Velocidad de la bala
    public float fireRate = 0.2f;    // Tiempo entre disparos

    private float nextFireTime = 0f; // Control del tiempo de disparo

    void Update()
    {
/*         if(!transform.parent.CompareTag("Player")) return;
        // Disparar si el botón de disparo está presionado (continuo) o se hace clic una vez
        if ((Input.GetButton("Fire1") || Input.GetButtonDown("Fire1")) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Actualizar el tiempo del próximo disparo
        } */
    }

    public override void Shoot()
    {
        if(currentAmmo > 0){
        
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Aplicar velocidad a la bala
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = transform.right * bulletSpeed;  // Dispara en la dirección de la pistola
            }
            currentAmmo--;
            GameManager.Instance.DecreaseAmmo(1);
        }
    }
}