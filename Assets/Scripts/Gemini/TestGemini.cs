using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System;

public class TestGemini : MonoBehaviour
    {
        public GeminiAPI geminiAPI;

        void Start()
        {
            geminiAPI.SendRequest("Hola, ¿cómo estás?");
        }
    }
