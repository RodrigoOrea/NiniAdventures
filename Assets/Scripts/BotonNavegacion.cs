using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonNavegacion : MonoBehaviour
{
    public void CargarPrimerNivel()
    {
        SceneManager.LoadScene("LevelForest-PrimerNivel");
    }


    public void CargarSegundoNivel()
    {
        SceneManager.LoadScene("LevelDesert-SecondLevel");
    }
}
