using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntDisparar : MonoBehaviour
{

    private int health = 3;
    public GameObject BulletPrefab;
    private float lastShoot; 

    public GameObject John;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (John == null) { return; }

        Vector3 direction = John.transform.position - transform.position; //Al restar la posición de John menos nuestra dirección obtdenemos el vector dirección que va desde nuestra posición hasta John
        if (direction.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }


        float distance = Mathf.Abs(John.transform.position.x - transform.position.x);

        if (distance < 1f && Time.time > lastShoot + 0.25f)
        {
            Shoot();
            lastShoot = Time.time;
        }
    }



    private void Shoot()
    {

        //Debug.Log("Shoot"); //Esto escribe por consola "Shoot" cada vex que se invoque la funación. Nos sirve para encontrar bugs o para cuando aún no hemos vamos a implemntar la función pero queremos ver si el codigo en el que aparezca funciona


        Vector3 direccBala;
        if (transform.localScale.x == 1.0f)
        {
            direccBala = Vector3.right; //Si la dirección a la que vamos (transform.localScale) es derecha (1), la dirección de la bala será derecha también
        }
        else { direccBala = Vector3.left; }

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direccBala * 0.5f, Quaternion.identity); //Instantiate lo que hace es coger una instancia, es decir, un duplicado, de un objeto, en este caso la bala que le hemos pasado, y lo duplica en el sitio que le digamos (transform.position) (donde estemos con John) con determinada rotación (Quaternion.identity) (rotacion 0)
        bullet.GetComponent<BulletScript>().SetDirection(direccBala); //Tomamos de la bala que se va a instaciar el componente/metodo SetDirection y le añadimos la direcc en que se ha determinado que se desplaze la bala


    }


    public void Hit()
    {

        health = health - 1;

        if (health == 0)
        {

            Destroy(gameObject);

        }


    }



}
