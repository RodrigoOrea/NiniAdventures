using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject weaponHolder; // Referencia al WeaponHolder
    public TMP_Text ammoText; // Referencia al Text de la UI

    void Update()
    {
        // Obtener el arma actual (hijo de WeaponHolder)
        Weapon currentWeapon = weaponHolder.GetComponentInChildren<Weapon>();

        if (currentWeapon != null)
        {
            // Mostrar la cantidad de balas
            ammoText.text = "Balas: " + currentWeapon.currentAmmo.ToString();
        }
        else
        {
            // No hay arma equipada
            ammoText.text = "No gun";
        }
    }
}