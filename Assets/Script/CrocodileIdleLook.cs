using UnityEngine;

/// <summary>
/// While idle, the crocodile turns to face the player.
/// When the player walks past to the other side, the sprite flips
/// horizontally so the crocodile keeps looking at them.
/// </summary>
public class CrocodileIdleLook : MonoBehaviour
{
    [SerializeField, Tooltip("Reference to the player Transform. Left empty = auto-find by tag.")]
    private Transform player;

    [SerializeField, Tooltip("Check this if the crocodile sprite faces right by default (unchecked = faces left).")]
    private bool facingRightByDefault = true;

    [SerializeField, Tooltip("Minimum horizontal distance before flipping. Prevents jitter when the player is directly above/below.")]
    private float flipThreshold = 0.1f;

    private SpriteRenderer spriteRenderer;
    private bool facingRight;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Reset any manual flipX toggle done in the inspector so the script
        // starts from the sprite's default (right-facing) orientation.
        spriteRenderer.flipX = false;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        facingRight = facingRightByDefault;
    }

    private void Update()
    {
        if (player == null || spriteRenderer == null)
        {
            return;
        }

        float offset = player.position.x - transform.position.x;

        if (Mathf.Abs(offset) > flipThreshold)
        {
            bool shouldFaceRight = offset > 0;

            if (shouldFaceRight != facingRight)
            {
                facingRight = shouldFaceRight;
                // flipX = true mirrors the sprite.
                // If sprite faces right by default: flipX=true → faces left, flipX=false → faces right.
                // If sprite faces left by default:  flipX=true → faces right, flipX=false → faces left.
                spriteRenderer.flipX = facingRight != facingRightByDefault;
            }
        }
    }
}
