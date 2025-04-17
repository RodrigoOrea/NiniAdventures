using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public Transform weaponHolder; // Punto de anclaje para el arma equipada
    private Weapon currentWeapon; // Arma actualmente equipada

    void Update()
    {
        // Detectar si el jugador está cerca de un arma y presiona una tecla para recogerla
        if (Input.GetKeyDown(KeyCode.E)) // Por ejemplo, la tecla "E" para recoger
        {
            TryPickUpWeapon();
        }

        // Disparar si se hace clic y hay un arma equipada
        if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire1")) && currentWeapon != null)
        {
            Shoot();
        }
    }

    void TryPickUpWeapon()
    {
        // Buscar todos los objetos con el tag "Weapon" cerca del jugador
        Collider2D[] nearbyWeapons = Physics2D.OverlapCircleAll(transform.position, 2.0f); // Radio de detección
        foreach (Collider2D weaponCollider in nearbyWeapons)
        {
            if (weaponCollider.CompareTag("Weapon"))
            {
                Debug.Log("Se intenta coger arma");
                EquipWeapon(weaponCollider.gameObject);
                break; // Equipar solo un arma a la vez
            }
        }
    }

    void EquipWeapon(GameObject newWeapon)
    {
        // Si ya hay un arma equipada, destruirla
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // Obtener el componente Weapon del nuevo arma
        Weapon weapon = newWeapon.GetComponent<Weapon>();
        if (weapon != null)
        {
            currentWeapon = weapon;

            // Hacer que el arma sea hija del weaponHolder
            currentWeapon.transform.SetParent(weaponHolder);
            currentWeapon.transform.localPosition = Vector3.zero; // Centrar en el punto de anclaje
            currentWeapon.transform.localRotation = Quaternion.identity; // Sin rotación adicional

            // Desactivar el collider del arma para evitar que se recoja de nuevo
            Collider2D weaponCollider = newWeapon.GetComponent<Collider2D>();
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;
            }

            // Desactivar la física del arma (si tiene Rigidbody2D)
            Rigidbody2D weaponRb = newWeapon.GetComponent<Rigidbody2D>();
            if (weaponRb != null)
            {
                weaponRb.simulated = false;
            }
        }
        else
        {
            Debug.LogError("El objeto no tiene un componente Weapon.");
        }
    }

    void Shoot()
    {
        // Llamar al método Shoot() del arma equipada
        currentWeapon.Shoot();
    }
}