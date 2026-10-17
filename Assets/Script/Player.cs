using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private const float GroundedAnimationGracePeriod = 0.12f;

    public int coin;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public AudioClip jumpclip;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;

    private bool isGrounded;
    private bool isJumping;
    private bool isKnockedBack;
    private bool hasBeenGrounded;
    private float lastGroundedTime;
    private Vector2 groundVelocity;
    private MovingPlatform groundPlatform;

    private float facingDirection = 1f;

    public int extraJumpsValue = 1;
    private int extraJumps;

    private string currentAnimation = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            facingDirection = Mathf.Sign(moveInput);
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection < 0f;
            }
        }

        if (!isKnockedBack)
        {
            Vector2 movementVelocity = rb.linearVelocity;
            movementVelocity.x = moveInput * moveSpeed;

            if (isGrounded && !isJumping && groundPlatform != null)
            {
                movementVelocity.x += groundVelocity.x;
                movementVelocity.y = groundVelocity.y;
            }

            rb.linearVelocity = movementVelocity;
        }

        // Reset extra jumps when grounded
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        // Jump using Space
        if (!isKnockedBack && Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

                isJumping = true;
                isGrounded = false;
                PlaySFX(jumpclip);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

                isJumping = true;
                extraJumps--;
                PlaySFX(jumpclip);
            }
        }

        SetAnimation(moveInput);
    }

    void FixedUpdate()
    {
        Vector2 groundCheckPosition = groundCheck != null
            ? groundCheck.position
            : transform.position;

        if (playerCollider != null)
        {
            groundCheckPosition.y =
                playerCollider.bounds.min.y + groundCheckRadius * 0.5f;
        }

        Collider2D groundHit = Physics2D.OverlapCircle(
            groundCheckPosition,
            groundCheckRadius,
            groundLayer
        );

        isGrounded = false;
        groundVelocity = Vector2.zero;
        groundPlatform = null;
        if (groundHit != null && groundHit != playerCollider)
        {
            MovingPlatform movingPlatform =
                groundHit.GetComponentInParent<MovingPlatform>();
            if (movingPlatform != null)
            {
                groundPlatform = movingPlatform;
                groundVelocity = movingPlatform.Velocity;
            }
            else
            {
                Rigidbody2D groundRigidbody = groundHit.attachedRigidbody;
                groundVelocity = groundRigidbody != null
                    ? groundRigidbody.GetPointVelocity(
                        groundHit.ClosestPoint(groundCheckPosition)
                    )
                    : Vector2.zero;
            }

            float relativeVerticalVelocity =
                rb.linearVelocity.y - groundVelocity.y;
            isGrounded =
                relativeVerticalVelocity <= 0.01f ||
                (movingPlatform != null && !isJumping);
        }

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            hasBeenGrounded = true;
            isJumping = false;
        }

        if (isKnockedBack &&
            isGrounded &&
            rb.linearVelocity.y - groundVelocity.y <= 0f)
        {
            isKnockedBack = false;
        }
    }

    private void SetAnimation(float moveInput)
    {
        string nextAnimation;
        float relativeVerticalVelocity =
            rb.linearVelocity.y - groundVelocity.y;
        bool groundedForAnimation =
            isGrounded ||
            (hasBeenGrounded &&
             Time.time - lastGroundedTime <= GroundedAnimationGracePeriod &&
             relativeVerticalVelocity >= -0.1f &&
             relativeVerticalVelocity <= 0.05f);

        if (!groundedForAnimation)
        {
            if (relativeVerticalVelocity > 0.05f)
            {
                nextAnimation = "Player_Jump";
            }
            else
            {
                nextAnimation = "Player_Fall";
            }
        }
        else if (Mathf.Abs(moveInput) > 0.01f)
        {
            nextAnimation = "Player_Run";
        }
        else
        {
            nextAnimation = "Player_Idle";
        }

        // Play only when changing animation
        if (currentAnimation != nextAnimation)
        {
            animator.Play(nextAnimation);
            currentAnimation = nextAnimation;
        }
    }

    public void ApplyKnockback(
        float horizontalForce,
        float upwardSpeed)
    {
        isKnockedBack = true;
        isJumping = true;

        rb.linearVelocity = new Vector2(
            -facingDirection * horizontalForce,
            Mathf.Max(rb.linearVelocity.y, upwardSpeed)
        );
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;

            Vector3 groundCheckPosition = groundCheck.position;
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                groundCheckPosition.y =
                    playerCollider.bounds.min.y + groundCheckRadius * 0.5f;
            }

            Gizmos.DrawWireSphere(
                groundCheckPosition,
                groundCheckRadius
            );
        }
    }
}