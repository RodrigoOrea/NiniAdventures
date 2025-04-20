using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Necesario para usar TMP_Text
using System.Collections;

public class GeminiAPI : MonoBehaviour
{


    //-------------------------------------------Variables y Configuración Inicial de la API---------------------------------------
    [SerializeField] private TMP_Text responseText; // Referencia al TMP_Text en la UI. Campo para mostrar respuestas en UI (TextMeshPro)
    private string apiKey = "AIzaSyACQdPPLW3Pw1w562V6NEjmjFWAs1MIaWk"; // Reemplaza con tu clave de API
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public GameObject portrait; // Referencia al retrato del personaje que "habla"
    //------------------------------------------------------------------------------------------------------------------------------



    //-----------------------------------------Método Principal: SendRequest()------------------------------------------------------
    public void SendRequest(string prompt, string personajeText)
    {
        StartCoroutine(PostRequest(prompt, personajeText));
    }

    /*
     Propósito: Inicia la corrutina para enviar la petición a la API.
     Parámetros:
            prompt: Texto enviado por el jugador.
            personajeText: Última cosa dicha por del personaje. Se emplea para dar contexto y continuidad a la conversación y que Gemini no conteste de forma aislada.
     */
    //------------------------------------------------------------------------------------------------------------------------------




    //---------------------------------------Corrutina PostRequest() (Conexión con la API)------------------------------------------
    IEnumerator PostRequest(string prompt, string personajeText)
    {
        // Construye el JSON (el mensaje) para la API (: Indica a Gemini que actúe como un "cactus aburrido" y limita la respuesta a 15 palabras.)
        string jsonData = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{"Estas siendo usada para representar a un personaje dentro de un videojuego. Ahora mismo representas a un cactus aburrido del desierto, con personalidad lamentable e insistente. Responde en un maximo de 15 palabras. Acabas de decir: " + personajeText + ". Mensaje del jugador: " + prompt}\"}}]}}]}}"; //LINEA IMPORTANTE!!! ENVÍA MENSAJE A LA API
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);


        // Configura la petición HTTP
        UnityWebRequest request = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest(); //Envía la petición


        // Manejo de errores (Muestra mensajes en consola y UI si falla.)
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            responseText.text = "Error en la conexión.";
        }
        else
        {
            // Procesa la respuesta exitosa (Llama a ParseResponse() y muestra el texto con GameManager.Instance.ShowMessage().)
            string responseTextValue = ParseResponse(request.downloadHandler.text);
            GameManager.Instance.ShowMessage(responseTextValue, 3f, portrait); //LINEA IMPORTANTE!!! DEVUELVE RESPUESTA DE LA API POR PANTALLA
            Debug.Log("Respuesta de Gemini: " + responseTextValue);

            // Inicia una corrutina para borrar el texto después de 5 segundos
            //StartCoroutine(ClearTextAfterDelay(5f));
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------



    //-----------------------------------------Borrado Automático (Opcional)-------------------------------------------------
    private IEnumerator ClearTextAfterDelay(float delay)
     {
        yield return new WaitForSeconds(delay); // Espera el tiempo especificado
        responseText.text = ""; // Borra el texto
     }
    //-----------------------------------------------------------------------------------------------------------------------




    //-------------------------------------------Procesamiento de la Respuesta (ParseResponse)-------------------------------
    private string ParseResponse(string jsonResponse)
    {
        try
        {
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(jsonResponse);

            if (response.candidates != null && response.candidates.Length > 0) //Extrae el texto de la respuesta
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
    //---------------------------------------------------------------------------------------------------------------------------


    //--------------------------------------------Clases Auxiliares para el JSON--------------------------------------------------
    [System.Serializable] private class GeminiResponse { public Candidate[] candidates; }
    [System.Serializable] private class Candidate { public Content content; }
    [System.Serializable] private class Content { public Part[] parts; }
    [System.Serializable] private class Part { public string text; }
    //----------------------------------------------------------------------------------------------------------------------------
}
