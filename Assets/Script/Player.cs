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
    private bool isKnockedBack;
    private float lastGroundedTime;

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
            rb.linearVelocity = new Vector2(
                moveInput * moveSpeed,
                rb.linearVelocity.y
            );
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

                isGrounded = false;
                PlaySFX(jumpclip);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );

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

        isGrounded =
            rb.linearVelocity.y <= 0.01f &&
            Physics2D.OverlapCircle(
                groundCheckPosition,
                groundCheckRadius,
                groundLayer
            );

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (isKnockedBack &&
            isGrounded &&
            rb.linearVelocity.y <= 0f)
        {
            isKnockedBack = false;
        }
    }

    private void SetAnimation(float moveInput)
    {
        string nextAnimation;
        bool groundedForAnimation =
            isGrounded ||
            (Time.time - lastGroundedTime <= GroundedAnimationGracePeriod &&
             rb.linearVelocity.y >= -0.1f &&
             rb.linearVelocity.y <= 0.05f);

        if (!groundedForAnimation)
        {
            if (rb.linearVelocity.y > 0.05f)
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