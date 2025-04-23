using UnityEngine;
using System.Collections;

public class BirdMovement : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;
    public float telegraphDuration = 0.5f; // Seconds bird pauses before attacking

    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private float previousX;
    private bool playerDetected = false;
    private bool isTelegraphing = false;

    private Coroutine telegraphCoroutine;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        previousX = transform.position.x;
    }

    void Update()
    {
        if (playerDetected || isTelegraphing) return; // Pause movement when preparing to attack

        float offsetX = Mathf.Sin(Time.time * speed) * moveDistance;
        Vector3 newPosition = new Vector3(startPosition.x + offsetX, startPosition.y, startPosition.z);
        transform.position = newPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (newPosition.x > previousX);
        }

        previousX = newPosition.x;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerDetected)
        {
            playerDetected = true;
            telegraphCoroutine = StartCoroutine(TelegraphAttack());
        }
    }

    private IEnumerator TelegraphAttack()
    {
        isTelegraphing = true;

        // Wait for a moment before attacking (bird keeps flapping in place)
        yield return new WaitForSeconds(telegraphDuration);

        // Proceed to Step 3: Attack logic goes here
        Debug.Log("Attack time! (We'll do this next)");

        isTelegraphing = false;
        // Optionally: set playerDetected = false here if you want the bird to attack once per detection
    }
}
