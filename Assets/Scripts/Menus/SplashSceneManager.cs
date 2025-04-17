using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneManager : MonoBehaviour
{
    //GESTIONAMOS EL COMPORTAMIENTO DE LA PANTALLA DE SPLASH
    public float waitTime = 5f; //5 seg de espera antes de cambiar de escena
    private float startedAt; //momento en que la escena comienza

    void Start()
    {
        startedAt = Time.time;
    }

    void Update()
    {
        //Al presionar cualquier tecla o tras 5 seg carga el menú principal
        if (Input.anyKey || Time.time - startedAt > waitTime)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }


    //TRAS EL LETRERO APARECERÁ CAMINANDO EL PERSONAJE
    //CRUZANDO LA PANTALLA DE UN LADO A OTRO

    //INTRODCIR MEJORAS PARA QUE SUENE MUSICA RELAJANTE DESDE QUE SE EJEUTA EL JUEGO
    //(ES DECIR, EN SPLASH Y MAINMENU. EN LOS NIVELES YA CAMBIA) Y QUE TRAS EL TIEMPO
    //O AL TOCAR LA PANTALLA, EL TITULO SE MARCHE PRIMERO HACIA ARRIBA DE
    //VUELTA Y DESPUÉS YA CAMBIA DE ESCENA (LOS BOTONES DEL MAIN MENÚ
    //ENTRAN DESDE ARRIBA TAMBIÉN).  


}
