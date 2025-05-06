using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    public float moveDistance = 3f;  // How far the platform moves from its start point
    public float moveSpeed = 2f;     // How fast the platform moves

    private Vector3 _startPosition;
    private bool _movingRight = true;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        // Move the platform
        if (_movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= _startPosition.x + moveDistance)
            {
                _movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= _startPosition.x - moveDistance)
            {
                _movingRight = true;
            }
        }
    }

    // Optional: Make sure the player stays parented to the platform when standing on it
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
