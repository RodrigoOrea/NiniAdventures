using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Necesario para usar TMP_Text
using System.Collections;

public class GeminiAPI : MonoBehaviour
{
    [SerializeField] private TMP_Text responseText; // Referencia al TMP_Text en la UI
    private string apiKey = "AIzaSyACQdPPLW3Pw1w562V6NEjmjFWAs1MIaWk"; // Reemplaza con tu clave de API
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public GameObject portrait;

    public void SendRequest(string prompt, string personajeText)
    {
        StartCoroutine(PostRequest(prompt, personajeText));
    }

    IEnumerator PostRequest(string prompt, string personajeText)
    {
        string jsonData = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{"Estas siendo usada para representar a un personaje dentro de un videojuego. Ahora mismo representas a un cactus aburrido del desierto, con personalidad lamentable e insistente. Responde en un maximo de 15 palabras. Acabas de decir: " + personajeText + ". Mensaje del jugador: " + prompt}\"}}]}}]}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            responseText.text = "Error en la conexión.";
        }
        else
        {
            string responseTextValue = ParseResponse(request.downloadHandler.text);
            GameManager.Instance.ShowMessage(responseTextValue, 3f, portrait);
            Debug.Log("Respuesta de Gemini: " + responseTextValue);

            // Inicia una corrutina para borrar el texto después de 5 segundos
            //StartCoroutine(ClearTextAfterDelay(5f));
        }
    }

     private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera el tiempo especificado
        responseText.text = ""; // Borra el texto
    }

    private string ParseResponse(string jsonResponse)
    {
        try
        {
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(jsonResponse);

            if (response.candidates != null && response.candidates.Length > 0)
            {
                return response.candidates[0].content.parts[0].text;
            }
            else
            {
                return "No se encontró una respuesta válida.";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al parsear la respuesta: " + e.Message);
            return "Error al procesar la respuesta.";
        }
    }

    [System.Serializable] private class GeminiResponse { public Candidate[] candidates; }
    [System.Serializable] private class Candidate { public Content content; }
    [System.Serializable] private class Content { public Part[] parts; }
    [System.Serializable] private class Part { public string text; }
}
