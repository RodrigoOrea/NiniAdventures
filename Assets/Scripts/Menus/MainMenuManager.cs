using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame() //BOTON PLAY
    {
        SceneManager.LoadScene("LevelForest-PrimerNivel");  // Cambia "Level1" por tu escena.
    }

    public void ShowLevelSelector() //BOTON NIVELES
    {
        SceneManager.LoadScene("Levels"); 
    }

    public void ShowCredits() //BOTON CRÉDITOS
    {
        SceneManager.LoadScene("Credits");
    }
    

    public void Settings() //BOTÓN AJUSTES 
    {
        SceneManager.LoadScene("Settings");

    }

    public void QuitGame() //BOTÓN SALIR
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}