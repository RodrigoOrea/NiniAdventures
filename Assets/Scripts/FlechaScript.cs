using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaScript : MonoBehaviour
{

    public float Speed;
    private Rigidbody2D Rigibody2D;
    // Start is called before the first frame update
    void Start()
    {
        Rigibody2D = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Rigibody2D.velocity = Vector2.right * Speed;
    }
}
