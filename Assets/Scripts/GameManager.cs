using System.Collections;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI; // Necesario para usar InputField

public class GameManager : MonoBehaviour
{
    public GeminiAPI Gemini;
    public TMP_InputField playerInput; // Referencia al InputField
    private bool isWaitingForResponse = false;


    void Start()
    {
        // Asigna el evento onEndEdit del InputField
        playerInput.onEndEdit.AddListener(OnInputEndEdit);
    }

    // Método que se llama cuando el usuario termina de editar el InputField
    private void OnInputEndEdit(string userInput)
{
    if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && !isWaitingForResponse)
    {
        isWaitingForResponse = true;
        Debug.Log("Envio prompt una vez");
        string userInput1 = userInput.Replace("\n", "").Trim();
        Gemini.SendRequest(userInput1);
        playerInput.text = "";
        playerInput.interactable = false; // Desactiva el InputField
    }
}
}