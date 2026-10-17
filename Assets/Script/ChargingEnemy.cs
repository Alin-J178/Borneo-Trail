using UnityEngine;

public class ChargingEnemy : MonoBehaviour
{
    [Header("Detection")]
    public Transform player;
    public float detectionRange = 5f;

    [Header("Movement")]
    public float chargeSpeed = 7f;
    public float chargeDistance = 4f;
    public float returnSpeed = 3f;

    [Header("Obstacle Detection")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float groundCheckDistance = 1.2f;
    public float wallCheckDistance = 0.15f;

    private Vector3 startPosition;
    private Vector3 chargeTarget;
    private bool isCharging;
    private bool isReturning;
    private float chargeDirection;
    private Animator animator;

    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startPosition = transform.position;
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        PlayAnimation("ChargeCrocodile_Idle");

        if (enemyCollider == null)
        {
            Debug.LogError(
                "ChargingEnemy needs a Collider2D for obstacle detection.",
                this
            );
            enabled = false;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (isCharging)
        {
            // Stop if a wall is ahead or there is no ground ahead.
            if (WallAhead() || NoGroundAhead())
            {
                isCharging = false;
                isReturning = true;
                PlayAnimation("Chargecroco_walk");
                return;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                chargeTarget,
                chargeSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, chargeTarget) < 0.05f)
            {
                isCharging = false;
                isReturning = true;
                PlayAnimation("Chargecroco_walk");
            }
        }
        else if (isReturning)
        {
            SetFacingDirection(startPosition.x - transform.position.x);

            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                returnSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, startPosition) < 0.05f)
            {
                transform.position = startPosition;
                isReturning = false;
                PlayAnimation("ChargeCrocodile_Idle");
            }
        }
        else
        {
            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            if (distance <= detectionRange)
            {
                chargeDirection =
                    player.position.x >= transform.position.x ? 1f : -1f;

                chargeTarget = transform.position +
                    Vector3.right * chargeDirection * chargeDistance;

                isCharging = true;
                PlayAnimation("Chargecroco_walkAngry");
                SetFacingDirection(chargeDirection);
            }
        }
    }

    bool WallAhead()
    {
        Bounds bounds = enemyCollider.bounds;

        Vector2 origin = new Vector2(
            chargeDirection > 0 ? bounds.max.x : bounds.min.x,
            bounds.center.y
        );

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.right * chargeDirection,
            wallCheckDistance,
            wallLayer
        );

        return hit.collider != null;
    }

    bool NoGroundAhead()
    {
        Bounds bounds = enemyCollider.bounds;

        // Check just beyond the front foot of the enemy.
        float frontX = chargeDirection > 0
            ? bounds.max.x + 0.05f
            : bounds.min.x - 0.05f;

        Vector2 origin = new Vector2(frontX, bounds.min.y + 0.05f);

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (enemyCollider != null)
        {
            Bounds bounds = enemyCollider.bounds;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(
                new Vector2(
                    chargeDirection >= 0 ? bounds.max.x : bounds.min.x,
                    bounds.center.y
                ),
                Vector2.right * chargeDirection * wallCheckDistance
            );

            Gizmos.color = Color.green;
            Gizmos.DrawRay(
                new Vector2(
                    chargeDirection >= 0
                        ? bounds.max.x + 0.05f
                        : bounds.min.x - 0.05f,
                    bounds.min.y + 0.05f
                ),
                Vector2.down * groundCheckDistance
            );
        }
    }
    
    void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }

    void SetFacingDirection(float direction)
    {
        if (spriteRenderer != null && direction != 0f)
        {
            spriteRenderer.flipX = direction < 0f;
        }
    }
}
