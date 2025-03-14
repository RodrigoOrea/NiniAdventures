using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeminiAPI : MonoBehaviour
{
    private string apiKey = "AIzaSyACQdPPLW3Pw1w562V6NEjmjFWAs1MIaWk"; // Reemplaza con tu clave de API
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
    public void SendRequest(string prompt)
    {
        StartCoroutine(PostRequest(prompt));
    }

    IEnumerator PostRequest(string prompt)
    {
        // Crea el cuerpo de la solicitud en formato JSON
        string jsonData = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{prompt}\"}}]}}]}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Configura la solicitud HTTP
        UnityWebRequest request = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Envía la solicitud y espera la respuesta
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Procesa la respuesta
            string responseText = request.downloadHandler.text;
            string parsedResponse = ParseResponse(responseText);
            Debug.Log("Respuesta de Gemini: " + parsedResponse);
            // Aquí puedes parsear la respuesta y usarla en tu juego
        }
    }

    private string ParseResponse(string jsonResponse)
    {
        try
        {
            // Parsea la respuesta JSON
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(jsonResponse);

            // Extrae el texto de la respuesta
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
            return "Error al parsear la respuesta.";
        }
    }

    // Clases para parsear la respuesta JSON
    [System.Serializable]
    private class GeminiResponse
    {
        public Candidate[] candidates;
        public UsageMetadata usageMetadata;
        public string modelVersion;
    }

    [System.Serializable]
    private class Candidate
    {
        public Content content;
        public string finishReason;
        public float avgLogprobs;
    }

    [System.Serializable]
    private class Content
    {
        public Part[] parts;
        public string role;
    }

    [System.Serializable]
    private class Part
    {
        public string text;
    }

    [System.Serializable]
    private class UsageMetadata
    {
        public int promptTokenCount;
        public int candidatesTokenCount;
        public int totalTokenCount;
        public TokenDetails[] promptTokensDetails;
        public TokenDetails[] candidatesTokensDetails;
    }

    [System.Serializable]
    private class TokenDetails
    {
        public string modality;
        public int tokenCount;
    }
}