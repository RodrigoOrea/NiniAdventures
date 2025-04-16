using UnityEngine;

public class TileElevator : MonoBehaviour
{
    public float moveDistance = 3f;
    public float speed = 2f;

    private Vector3 startPos;
    private bool movingUp = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = transform.position.y + (movingUp ? speed : -speed) * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (movingUp && newY >= startPos.y + moveDistance)
        {
            movingUp = false;
        }
        else if (!movingUp && newY <= startPos.y)
        {
            movingUp = true;
        }
    }
}
