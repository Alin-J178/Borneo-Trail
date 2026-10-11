using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 2f;
    public Vector2 Velocity { get; private set; }

    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private Vector2 centerOffset;
    private Vector2 target;
    private bool movingToPointB;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError(
                "MovingPlatform needs Point A and Point B assigned.",
                this
            );
            enabled = false;
            return;
        }

        centerOffset = (Vector2)platformCollider.bounds.center - rb.position;
        rb.position = (Vector2)pointA.position - centerOffset;
        movingToPointB = true;
        target = (Vector2)pointB.position - centerOffset;
    }

    void FixedUpdate()
    {
        target = (Vector2)(movingToPointB ? pointB.position : pointA.position)
            - centerOffset;

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );

        Velocity = (nextPosition - rb.position) / Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, target) < 0.01f)
        {
            movingToPointB = !movingToPointB;
        }
    }
}