using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private const float GroundedAnimationGracePeriod = 0.12f;
    private const float GroundedJumpGracePeriod = 0.12f;

    private static int savedCoinCount;
    private static readonly HashSet<string> collectedCoinIds =
        new HashSet<string>();

    public int coin;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Fall Damage")]
    [SerializeField] private float fallDamageThreshold = 6f;
    [SerializeField] private float fallDamagePerUnit = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public AudioClip jumpclip;
    public AudioClip hurtClip;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private Health health;
    private MobileControls mobileControls;

    private bool isGrounded;
    private bool isJumping;
    private bool isKnockedBack;
    private bool isEmoting;
    private bool hasBeenGrounded;
    private float lastGroundedTime;
    private Vector2 groundVelocity;
    private MovingPlatform groundPlatform;
    private bool trackingFall;
    private float highestAirborneY;

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
        health = GetComponent<Health>();
        if (Application.isMobilePlatform)
        {
            mobileControls = FindAnyObjectByType<MobileControls>();
        }

        extraJumps = extraJumpsValue;
        coin = savedCoinCount;

        if (Checkpoint.TryGetPositionForCurrentScene(out Vector3 checkpointPosition))
        {
            transform.position = checkpointPosition;
        }
    }

    public bool CollectCoin(string coinId)
    {
        if (!collectedCoinIds.Add(coinId))
        {
            return false;
        }

        coin++;
        savedCoinCount = coin;
        return true;
    }

    public static bool HasCollectedCoin(string coinId)
    {
        return collectedCoinIds.Contains(coinId);
    }

    public static void ResetCoinProgress()
    {
        savedCoinCount = 0;
        collectedCoinIds.Clear();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (mobileControls != null && mobileControls.MoveInput != 0f)
        {
            moveInput = mobileControls.MoveInput;
        }

        bool mobileEmotePressed =
            mobileControls != null && mobileControls.ConsumeEmotePressed();
        if (Input.GetKeyDown(KeyCode.Alpha6) || mobileEmotePressed)
        {
            isEmoting = true;
        }

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
        bool mobileJumpPressed =
            mobileControls != null && mobileControls.ConsumeJumpPressed();
        if (!isKnockedBack &&
            (Input.GetKeyDown(KeyCode.Space) || mobileJumpPressed))
        {
            bool canJumpFromGround =
                isGrounded ||
                (!isJumping &&
                 hasBeenGrounded &&
                 Time.time - lastGroundedTime <= GroundedJumpGracePeriod);

            if (canJumpFromGround)
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

        UpdateFallDamage();

        if (isKnockedBack &&
            isGrounded &&
            rb.linearVelocity.y - groundVelocity.y <= 0f)
        {
            isKnockedBack = false;
        }
    }

    private void UpdateFallDamage()
    {
        if (!isGrounded)
        {
            if (!trackingFall)
            {
                trackingFall = true;
                highestAirborneY = transform.position.y;
            }
            else
            {
                highestAirborneY = Mathf.Max(
                    highestAirborneY,
                    transform.position.y
                );
            }

            return;
        }

        if (!trackingFall)
        {
            return;
        }

        float fallDistance = highestAirborneY - transform.position.y;
        trackingFall = false;

        if (health != null && fallDistance > fallDamageThreshold)
        {
            float damage =
                (fallDistance - fallDamageThreshold) * fallDamagePerUnit;
            health.TakeDamage(damage);
        }
    }

    private void SetAnimation(float moveInput)
    {
        string nextAnimation;
        if (isEmoting && Mathf.Abs(moveInput) > 0.01f)
        {
            isEmoting = false;
        }

        if (isEmoting)
        {
            nextAnimation = "Player_67";
        }
        else
        {
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

    public void PlayHurtSFX()
    {
        PlaySFX(hurtClip);
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