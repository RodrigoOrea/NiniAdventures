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

    public static GeminiAPI Instance { get; private set; }

    private void Awake()
    {
        // Implementación del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional para persistencia entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

    }


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
        string interviewPhase = GetInterviewPhase(personajeText); // Nueva función de control
        string examples = GetDialogExamples(); // Ejemplos embebidos

        // Construye el JSON (el mensaje) para la API (: Indica a Gemini que actúe como un "cactus aburrido" y limita la respuesta a 15 palabras.)
        string jsonData = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{"Actúas como Naronei, el viejo sabio del desierto, que lleva vigilando esas tierras áridas desde antes que el sol. Tu actitud es recta pero con toques de sarcasmo, y tu misión es evaluar al jugador en una entrevista para el puesto de 'Barredor del Desierto', un cargo que lleva vacante milenios. Eres exigente pero no imposible de convencer." + $"CONTEXTO ACTUAL: {interviewPhase} " + $"REGLAS: Mantén el control de la entrevista siempre. Responde en un máximo de 30 palabras mezclando sabiduría ancestral y comentarios mordaces. Haz 1 pregunta clara o comentario evaluativo por turno. Usa sarcasmo refinado (nunca grosero)." + $"HISTORIAL: Acabas de decir: " + personajeText + ". Mensaje del jugador: " + prompt + $". EJEMPLOS DE ESTILO: {examples} "}\"}}]}}]}}"; //LINEA IMPORTANTE!!! ENVÍA MENSAJE A LA API


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
            GameManager.Instance.ShowMessage(responseTextValue, 40f, portrait); //LINEA IMPORTANTE!!! DEVUELVE RESPUESTA DE LA API POR PANTALLA
            Debug.Log("Respuesta de Gemini: " + responseTextValue);

            // Inicia una corrutina para borrar el texto después de 5 segundos
            //StartCoroutine(ClearTextAfterDelay(5f));
        }
    }




    private string GetInterviewPhase(string lastMessage)
    {
        if (string.IsNullOrEmpty(lastMessage))
            return "FASE 1: Presentación. Debes iniciar con una pregunta sobre experiencia laboral en ambientes extremos";

        if (lastMessage.Contains("experiencia"))
            return "FASE 2: Evaluación técnica. Pregunta sobre habilidades específicas para barrer arena infinita";

        if (lastMessage.Contains("habilidad"))
            return "FASE 3: Prueba ética. Cuestiona sobre qué haría si el viento deshace su trabajo";

        return "FASE 4: Cierre. Evalúa si el candidato es digno del puesto milenario";
    }

    private string GetDialogExamples()
    {
        return @"
    EJEMPLOS:
    1. Candidato dice 'Tengo experiencia':
    '¿Barriendo arenas... o excusas? Demuéstrame lo primero.'

    2. Candidato pregunta '¿Salario?':
    'Te pagaré en granos de arena sabia... como a mis últimos 100 empleados.'

    3. Candidato duda:
    'La indecisión es como arena en el viento... ¿Sigues interesado en el puesto?'
    ";
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
                string rawText = response.candidates[0].content.parts[0].text;
                return FormatTextByWords(rawText, 7); // Aquí aplicamos el salto cada 10 palabras
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



    //-------------------------------------------Formateo de Texto con Saltos de Línea--------------------------------------------
    private string FormatTextByWords(string input, int wordsPerLine)
    {
        string[] words = input.Split(' ');
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < words.Length; i++)
        {
            sb.Append(words[i]);

            if ((i + 1) % wordsPerLine == 0)
            {
                sb.AppendLine(); // salto de línea cada 10 palabras
            }
            else
            {
                sb.Append(" ");
            }
        }

        return sb.ToString().Trim(); // Elimina espacios/saltos finales innecesarios
    }
    //----------------------------------------------------------------------------------------------------------------------------



    //--------------------------------------------Clases Auxiliares para el JSON--------------------------------------------------
    [System.Serializable] private class GeminiResponse { public Candidate[] candidates; }
    [System.Serializable] private class Candidate { public Content content; }
    [System.Serializable] private class Content { public Part[] parts; }
    [System.Serializable] private class Part { public string text; }
    //----------------------------------------------------------------------------------------------------------------------------
}
