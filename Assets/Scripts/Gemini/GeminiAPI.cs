using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class GeminiAPI : MonoBehaviour
{
    //-------------------------------------------Variables y Configuración Inicial de la API---------------------------------------
    [SerializeField] private TMP_Text responseText; // Referencia al TMP_Text en la UI
    private string apiKey = "AIzaSyACQdPPLW3Pw1w562V6NEjmjFWAs1MIaWk"; // Reemplaza con tu clave de API
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    public GameObject portrait; // Referencia al retrato del personaje

    //---------------------------------------Variables para control de fases y puntuación-----------------------------------------
    private int preguntaCount = 0; // Contador de preguntas realizadas
    private int currentPhase = 1;  // Fase actual (1-4)
    private bool juegoTerminado = false; // Controla si el juego ha terminado
    private int totalScore = 0; // Puntuación acumulada

    //------------------------------------------------------------------------------------------------------------------------------

    public static GeminiAPI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendRequest(string prompt, string personajeText)
    {
        if (!juegoTerminado)
        {
            StartCoroutine(PostRequest(prompt, personajeText));
        }
    }

    IEnumerator PostRequest(string prompt, string personajeText)
    {
        preguntaCount++;
        UpdateInterviewPhase();

        string interviewPhase = GetInterviewPhaseDescription();
        string examples = GetDialogExamples();

        // Construcción del JSON en una sola línea
        string jsonData = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"Actúas como Naronei, el viejo sabio del desierto, una sarcástica pero autoritaria leyenda del desierto que debe pasarle una entrevista de trabajo al personaje para concederle o no el legendario cargo de barredor del desierto. Cargo que solo unos pocos elegidos pueden ostentar. A veces te metes de forma sarcástica con personaje y a veces valoras su determinación y audacia. No toleras las faltas de respeto, suponen un 0 siempre. Tu nivel de exigencia es bajo, és fácil convencerte y te encanta ver una buena actitud en el personaje al que le pasas la entrevista. EVALÚA ESTA RESPUESTA ENTRE -10 Y 10 BASADO EN: 1) Relevancia (-3 a 3), 2) Creatividad (-3 a 3), 3) Actitud (-4 a 4). FORMATO OBLIGATORIO: [PUNTUACIÓN:X] 1) Relevancia (), 2) Creatividad (), 3) Actitud () al final. CONTEXTO ACTUAL: {interviewPhase} REGLAS: Responde en 30 palabras máximo. Incluye tu evaluación. HISTORIAL: {personajeText}. RESPUESTA DEL CANDIDATO: {prompt}. {(preguntaCount >= 13 ? "ESTE ES EL FINAL: Despídete y muestra puntuación final. " : "")}\"}}]}}]}}";  //EJEMPLOS: {examples}

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
            string rawResponse = request.downloadHandler.text;
            string parsedResponse = ParseResponse(rawResponse);

            int score = ExtractScoreFromResponse(parsedResponse);
            totalScore += score;

            GameManager.Instance.ShowMessage(parsedResponse, 40f, portrait);
            Debug.Log($"Respuesta (Puntuación: {score}): " + parsedResponse);

            if (preguntaCount >= 13)
            {
                juegoTerminado = true;

                if (totalScore >= 50)
                {
                    GameManager.Instance.ShowMessage($"<size=36>¡FELICIDADES! HAS DEMOSTRADO SER VALEDOR DE \nESTE ANSIADO PUESTO.EL CARGO ES TUYO.\n¡PRUEBA SUPERADA!</size>\n\nPuntuación final: {totalScore}/100", 7f, portrait);
                    yield return new WaitForSeconds(7f);
                    SceneManager.LoadScene("Credits");
                }
                else
                {
                    GameManager.Instance.ShowMessage($"<size=36>VAYA... PARECE QUE LA ARENA NO \nES LO MAS SECO DE ESTE MILENARIO LUGAR.\nVUELVE CUANDO FLUYA UN POCO \nMAS DE SANGRE POR ESE CEREBRO.</size>\n\nPuntuación final: {totalScore}/100", 7f, portrait);
                    yield return new WaitForSeconds(7f);
                    SceneManager.LoadScene("Levels");
                }
            }
        }
    }

    private void UpdateInterviewPhase()
    {
        if (preguntaCount >= 12) currentPhase = 4;
        else if (preguntaCount >= 6) currentPhase = 3;
        else if (preguntaCount >= 1) currentPhase = 2;
        else currentPhase = 1;
    }

    private string GetInterviewPhaseDescription()
    {
        switch (currentPhase)
        {
            case 1: return "FASE 1: Presentación.";
            case 2: return "FASE 2: Evaluación técnica. Pregunta sobre experiencia en ambientes extremos habilidades específicas";
            case 3: return "FASE 3: Prueba ética. Cuestiona sobre perseverancia";
            default: return "FASE 4: Cierre. Evalúa si el candidato es digno";
        }
    }

    private int ExtractScoreFromResponse(string response)
    {
        int startIndex = response.IndexOf("[PUNTUACIÓN:") + "[PUNTUACIÓN:".Length;
        int endIndex = response.IndexOf("]", startIndex);

        if (startIndex >= 0 && endIndex >= 0)
        {
            string scoreStr = response.Substring(startIndex, endIndex - startIndex);
            if (int.TryParse(scoreStr, out int score))
            {
                return Mathf.Clamp(score, -10, 10);
            }
        }
        return 0;
    }

    private string GetDialogExamples()
    {
        return "EJEMPLOS: 1) '¿Barriendo arenas... o excusas? [PUNTUACIÓN:5]' 2) 'Te pagaré en granos de arena... [PUNTUACIÓN:2]' 3) 'Usaría el viento como aliado... [PUNTUACIÓN:8]'";
    }

    private string ParseResponse(string jsonResponse)
    {
        try
        {
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(jsonResponse);
            if (response.candidates != null && response.candidates.Length > 0)
            {
                string rawText = response.candidates[0].content.parts[0].text;
                return FormatTextByWords(rawText, 7);
            }
            return "No se encontró una respuesta válida.";
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al parsear: " + e.Message);
            return "Error al procesar la respuesta.";
        }
    }

    private string FormatTextByWords(string input, int wordsPerLine)
    {
        string[] words = input.Split(' ');
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < words.Length; i++)
        {
            sb.Append(words[i]);
            if ((i + 1) % wordsPerLine == 0) sb.AppendLine();
            else sb.Append(" ");
        }
        return sb.ToString().Trim();
    }

    [System.Serializable] private class GeminiResponse { public Candidate[] candidates; }
    [System.Serializable] private class Candidate { public Content content; }
    [System.Serializable] private class Content { public Part[] parts; }
    [System.Serializable] private class Part { public string text; }
}