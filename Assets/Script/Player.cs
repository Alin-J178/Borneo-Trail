using UnityEngine;

public class Player : MonoBehaviour
{
    public int coin;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float facingDirection = 1f;
    private bool isKnockedBack;
    
    private Animator animator;

    public int extraJumpsValue = 1;
    private int extraJumps;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput != 0)
        {
            facingDirection = Mathf.Sign(moveInput);
        }

        if (!isKnockedBack)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // extra jumps logic
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (!isKnockedBack && Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
            }
        }

        SetAnimation(moveInput);
    }

    public void ApplyKnockback(float horizontalForce, float upwardSpeed)
    {
        isKnockedBack = true;
        rb.linearVelocity = new Vector2(
            -facingDirection * horizontalForce,
            Mathf.Max(rb.linearVelocity.y, upwardSpeed));
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isKnockedBack && isGrounded && rb.linearVelocity.y <= 0f)
        {
            isKnockedBack = false;
        }
    }
    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if(moveInput == 0)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }

        }
    }
}
