using UnityEngine;
using TMPro;
using System.Collections;

public class EnableTMPInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField; // Referencia al TMP_InputField
    [SerializeField] private float disableTime = 7f; // Tiempo antes de ocultarlo

    private void Start()
    {
        inputField.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inputField.gameObject.SetActive(true);
            StartCoroutine(DisableAfterTime());
        }
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(disableTime);
        inputField.gameObject.SetActive(false);
    }
}
