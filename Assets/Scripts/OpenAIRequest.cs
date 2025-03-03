using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class OpenAIRequest : MonoBehaviour
{
    private string apiKey = "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";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    void Start()
    {
        StartCoroutine(SendChatRequest());
    }

    private IEnumerator SendChatRequest()
    {
        // JSON hardcoded con el mensaje a enviar
        string jsonPayload = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"Hola, ¿cómo estás?\"}]}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Respuesta de ChatGPT: " + responseText);
            }
            else
            {
                Debug.LogError("Error en la solicitud: " + request.error);
            }
        }
    }
}
