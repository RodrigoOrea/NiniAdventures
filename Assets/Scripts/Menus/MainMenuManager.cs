using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame() //BOTON PLAY
    {
        SceneManager.LoadScene("Forest-PrimerNivel");  // Cambia "Level1" por tu escena.
    }

    public void ShowLevelSelector() //BOTON NIVELES
    {
        SceneManager.LoadScene("Levels"); 
    }

    public void ShowCredits() //BOTON CR�DITOS
    {
        SceneManager.LoadScene("Credits");
    }
    

    public void Settings() //BOT�N AJUSTES (falta por acabar)
    {

    }

    public void QuitGame() //BOT�N SALIR
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}