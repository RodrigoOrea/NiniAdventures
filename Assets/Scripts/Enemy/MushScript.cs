using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushScript : MonoBehaviour
{
    private Animator Animator;
    private Rigidbody2D Rigidbody2D;

    [SerializeField] private float speed;
    [SerializeField] private float followRange; // Rango de detección del jugador
    [SerializeField] private float attackRange;   // Rango para golpear al jugador
    [SerializeField] private Transform player;
 
    [SerializeField] private float attackCooldown = 0.5f; // Cooldown entre ataques cuerpo a cuerpo

    private bool isFacingRight = true;
    private float timeSinceLastAttack = 0f; // Tiempo desde el último ataque
    private int health = 3;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange && distanceToPlayer > 0) // Sigue al jugador si está dentro del rango y no está en contacto
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            bool isPlayerRight = transform.position.x < player.transform.position.x;
            Flip(isPlayerRight);

            Animator.SetBool("runningMush", speed != 0.0f);

            // Golpeo al jugador si está dentro del rango de ataque
            if (distanceToPlayer <= attackRange)
            {
                CharacterMovement jugador = player.GetComponent<CharacterMovement>();
                Animator.SetBool("attackingMush", true); // Activa la animación de golpeo

                if (timeSinceLastAttack >= attackCooldown) {

                    jugador.Hit(); // Resta una vida al jugador
                    Animator.SetBool("attackingMush", false); // Desactiva la animación de golpeo

                    timeSinceLastAttack = 0f; // Reinicia el cooldown del ataque
                }

            }
            else
            {
                Animator.SetBool("attackingMush", false); // Desactiva la animación de golpeo
            }

        }
        else
        {
            Animator.SetBool("runningMush", false); // Detiene la animación si el jugador está fuera del rango
        }

        timeSinceLastAttack += Time.deltaTime; // Actualiza el tiempo del último ataque
    }

    private void Flip(bool isPlayerRight)
    {
        if ((isFacingRight && !isPlayerRight) || (!isFacingRight && isPlayerRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

   

    public void Hit()
    {
        health--;
        //ANIMACIÓN DE GOLPEO 

        if (health <= 0)
        {
            Destroy(gameObject);
            //INTRODUCIR MAÑANA LA ANIMACIÓN DE MUERTE --> Será Animator.SetBool("dieMush", health <= 0); o algo así, a falta de crear la animación de morir de mush
        }
    }
}




//INTRODUCIR OTRO TIPO DE MUSHROOM (cambiar color, tamaño, o lo que sea) QUE VIGILA DE UN PUNTO A OTRO, Y CUANDO GOLPEA SE CAE ATURDIDO (ver animación) PERO QUITA MÁS VIDA.
