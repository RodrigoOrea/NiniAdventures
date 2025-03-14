using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScriptRaul : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    public float speed;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {

        Rigidbody2D = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
    }


    private void FixedUpdate()
    {
        Rigidbody2D.velocity = this.direction * speed; //La velocidad del cuerpo fisico de la bala (Rigidbody2D.velocity) será el vector dirección de la bala por una determinada velocidad que elijamos

    }


    public void SetDirection(Vector3 direction) //Este método será invocado desde JhonMovement y determinado por la dirección de jhon (izq o drch). El vector dirección determinado, en FixedUpdate se multiplicará por la velocidad que decidamos y ya podremos ver el movimeinto de la bala
    {
        this.direction = direction;  //"this.direction" es la variable privada direction, y "direction" es el argumento de esta función

    }


    public void DestroyBullet() //Esta funcion se invoca en el segundo 0.6 de la animación de la bala cuando se activa el evento que hemos puesto en ese momento. Hace que se destruya el la instancia del objeto en cuestión
    {

        Destroy(gameObject); //Destroy es una función de Unity que destruirá el gameObject, y gameObject hace referencia al objeto que contiene la clase donde lo invoquemos (BulletScript en este caso)

    }




    private void OnTriggerEnter2D(Collider2D collision) //Se llama a esta función cuando el objeto (la bala) ha chocado con algo. Función para objeto en el juego que debe reaccionar cuando colisiona con otro objeto
    {


        CharacterMovement jhon = collision.GetComponent<CharacterMovement>(); //de la variable collision cogemos el collider (aun que de .collider.GetComponent<JohnMovement>(); no sea visible .collider aquí) del otro objeto con el que hemos chocado.
        GruntDisparar grunt = collision.GetComponent<GruntDisparar>();

        if (jhon != null)
        { // Si john es = null es que no hemos chocado con ningún objeto de tipo JohnMovement, y si john != algo que no sea null, es que hemos chocado contra JohnMovement (contra John) 

            jhon.Hit();

        }



        if (grunt != null)
        {

            grunt.Hit();

        }
        DestroyBullet();

    }
}
