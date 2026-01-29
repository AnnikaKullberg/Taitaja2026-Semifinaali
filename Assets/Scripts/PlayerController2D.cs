using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float acceleration = 60f;
    [SerializeField] float deceleration = 80f;
    [SerializeField] float velocityPower = 1f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float coyoteTime = 0.08f;
    [SerializeField] float jumpBufferTime = 0.1f;
    [SerializeField] float jumpCutMultiplier = 0.35f;

    [Header("Wall")]
    [SerializeField] float wallSlideSpeed = 1.2f;
    [SerializeField] float wallJumpForce = 17f;
    [SerializeField] Vector2 wallJumpDirection = new Vector2(1.2f, 1.6f);
    [SerializeField] float wallJumpControlTime = 0.12f;
    [SerializeField] float wallCheckDistance = 0.6f;
    [SerializeField] LayerMask wallLayer;

    [Header("Wall Assist")]
    [SerializeField] float wallCoyoteTime = 0.1f;
    [SerializeField] float wallJumpBufferTime = 0.1f;

    [Header("Ground Check")]
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] Vector2 groundCheckOffset = new Vector2(0f, -0.4f);

    Rigidbody2D rb;
    Collider2D col;

    float horizontal;
    bool jumpReleased;

    bool isGrounded;
    bool isOnWall;
    bool isWallSliding;
    bool isWallJumping;

    float coyoteCounter;
    float jumpBufferCounter;
    float wallCoyoteCounter;
    float wallJumpBufferCounter;
    float wallJumpCounter;

    int wallDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        GetInput();
        UpdateTimers();
        CheckGround();
        CheckWall();
        HandleWallSlide();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void GetInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
            wallJumpBufferCounter = wallJumpBufferTime;
        }

        jumpReleased = Input.GetButtonUp("Jump");
    }

    void UpdateTimers()
    {
        coyoteCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;
        wallJumpBufferCounter -= Time.deltaTime;

        if (isOnWall)
            wallCoyoteCounter = wallCoyoteTime;
        else
            wallCoyoteCounter -= Time.deltaTime;

        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0)
                isWallJumping = false;
        }
    }

    void CheckGround()
    {
        Vector2 pos = (Vector2)transform.position + groundCheckOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, groundCheckSize, 0);

        isGrounded = false;
        foreach (Collider2D hit in hits)
        {
            if (hit != col && !hit.isTrigger)
            {
                isGrounded = true;
                coyoteCounter = coyoteTime;
                break;
            }
        }
    }

    void CheckWall()
    {
        RaycastHit2D right = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D left = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

        if (right)
        {
            isOnWall = true;
            wallDirection = 1;
        }
        else if (left)
        {
            isOnWall = true;
            wallDirection = -1;
        }
        else
        {
            isOnWall = false;
        }
    }

    void HandleWallSlide()
    {
        isWallSliding = isOnWall && !isGrounded && rb.linearVelocity.y <= 0;

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)
            );
            coyoteCounter = coyoteTime;
        }
    }

    void HandleJump()
    {
        if (jumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        if (jumpBufferCounter > 0 && coyoteCounter > 0 && !isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }

        if (wallJumpBufferCounter > 0 && wallCoyoteCounter > 0 && !isGrounded)
        {
            Vector2 dir = new Vector2(
                -wallDirection * wallJumpDirection.x,
                wallJumpDirection.y
            ).normalized;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dir * wallJumpForce, ForceMode2D.Impulse);

            isWallJumping = true;
            wallJumpCounter = wallJumpControlTime;

            wallJumpBufferCounter = 0;
            jumpBufferCounter = 0;
            wallCoyoteCounter = 0;
        }
    }

    void HandleMovement()
    {
        float control = isWallJumping ? 0.7f : 1f;

        float targetSpeed = horizontal * moveSpeed * control;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        float force = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velocityPower) * Mathf.Sign(speedDiff);
        rb.AddForce(Vector2.right * force);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
    }
}
