using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public int currentAmmo = 50;
    public abstract void Shoot(); // Método abstracto para usar el arma
}