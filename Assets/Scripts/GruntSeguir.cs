using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntSeguir : MonoBehaviour          //ENEMIGO SIGUE AL JUGADOR CUANDO ENTRA EN SU ZONA Y DEJA DE HACERLO CUANDO SE ALEJA.
{
    private Animator Animator;
    private Rigidbody2D Rigidbody2D;

    [SerializeField] private float speed;
    [SerializeField] private float followRange = 10.0f; // Rango de detección del jugador
    [SerializeField] private Transform player;
    private bool isFacingRight = true;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange) // Solo sigue si el jugador está dentro del rango
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            bool isPlayerRight = transform.position.x < player.transform.position.x;
            Flip(isPlayerRight);

            Animator.SetBool("running", speed != 0.0f);
        }
        else
        {
            Animator.SetBool("running", false); // Detiene la animación si el jugador está fuera del rango
        }
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


}
