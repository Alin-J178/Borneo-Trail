
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError(
                "Assign both patrol points in the Inspector.",
                this
            );

            enabled = false;
            return;
        }

        target = pointB.position;
    }

    void FixedUpdate()
    {
        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        // Face the direction the enemy is moving
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX =
                target.x < rb.position.x;
        }

        // Turn around at each endpoint
        if (Vector2.Distance(nextPosition, target) < 0.02f)
        {
            target = target == (Vector2)pointB.position
                ? pointA.position
                : pointB.position;
        }
    }
}

