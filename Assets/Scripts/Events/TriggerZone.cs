using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Crossed");
            activado = true;
            UIManager.Instance.IniciarFinalNivel();
        }
    }
}
