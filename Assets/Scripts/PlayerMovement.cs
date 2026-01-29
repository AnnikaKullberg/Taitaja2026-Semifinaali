using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 originalScale;
    WallSlide wallSlide;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Wall Movement")]
    public float wallMoveDamping = 0.7f;
    public float wallExitBoost = 1.2f;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float coyoteTime = 0.1f;
    public float wallCoyoteTime = 0.15f;

    [Header("Gravity")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float wallGravityMultiplier = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Facing Direction")]
    public float turnSpeed = 20f;
    public float wallTurnDamping = 0.3f;

    Rigidbody2D rb;
    float horizontalInput;
    bool isGrounded;
    float coyoteTimer;
    float wallCoyoteTimer;
    float lastWallDirection;
    bool wasOnWall;

    void HandleFacingDirection(float input)
    {
        if (wallSlide != null && wallSlide.IsOnWall)
        {
            // Face away from the wall when wall sliding
            float targetScale = wallSlide.wallLeft ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);
            float currentScale = transform.localScale.x;
            float newScale = Mathf.Lerp(currentScale, targetScale, turnSpeed * wallTurnDamping * Time.deltaTime);

            transform.localScale = new Vector3(
                newScale,
                originalScale.y,
                originalScale.z
            );
            return;
        }

        // Normal movement facing
        if (input > 0.01f)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
        else if (input < -0.01f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(originalScale.x),
                originalScale.y,
                originalScale.z
            );
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        wallSlide = GetComponent<WallSlide>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Get raw input (no smoothing)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // DEBUG: Check input values
        if (Input.GetKeyDown(KeyCode.D)) Debug.Log("D pressed, input should be +1");
        if (Input.GetKeyDown(KeyCode.A)) Debug.Log("A pressed, input should be -1");

        HandleFacingDirection(horizontalInput);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Coyote timer handling
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // Wall coyote timer
        if (wallSlide != null && wallSlide.IsOnWall)
        {
            wallCoyoteTimer = wallCoyoteTime;
            lastWallDirection = wallSlide.wallLeft ? -1 : 1;
            wasOnWall = true;
        }
        else if (wasOnWall)
        {
            wallCoyoteTimer -= Time.deltaTime;
            if (wallCoyoteTimer <= 0) wasOnWall = false;
        }

        coyoteTimer = Mathf.Max(coyoteTimer, 0f);
        wallCoyoteTimer = Mathf.Max(wallCoyoteTimer, 0f);

        // Jump handling
        if (Input.GetButtonDown("Jump"))
        {
            if (coyoteTimer > 0f) // Ground jump
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                coyoteTimer = 0f;
            }
            else if (wallCoyoteTimer > 0f) // Wall jump
            {
                // Simple wall jump - always jump away from wall
                float jumpDirection = wallSlide.wallLeft ? 1f : -1f; // Away from wall

                Vector2 wallJumpForceVector = new Vector2(
                    jumpDirection * wallSlide.wallJumpDirection.x,
                    wallSlide.wallJumpDirection.y
                ) * wallSlide.wallJumpForce;

                rb.linearVelocity = Vector2.zero;
                rb.AddForce(wallJumpForceVector, ForceMode2D.Impulse);
                wallCoyoteTimer = 0f;
                wasOnWall = false;
            }
        }

        BetterJumpPhysics();
    }

    void FixedUpdate()
    {
        // Apply movement based on input direction
        float targetSpeed = horizontalInput * moveSpeed;

        // DEBUG: Check what's happening
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            Debug.Log($"FixedUpdate: Input={horizontalInput}, TargetSpeed={targetSpeed}, VelocityX={rb.linearVelocity.x}");
        }

        // Reduce movement control when sliding on wall
        if (wallSlide != null && wallSlide.IsSliding)
        {
            targetSpeed *= wallMoveDamping;

            // Allow slight push away from wall
            if ((wallSlide.wallLeft && horizontalInput > 0.1f) ||
                (!wallSlide.wallLeft && horizontalInput < -0.1f))
            {
                targetSpeed *= wallExitBoost;
            }
        }

        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

        // Apply modified gravity when near walls but not sliding
        if (wallSlide != null && wallSlide.IsOnWall && !wallSlide.IsSliding && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (wallGravityMultiplier - 1) * Time.fixedDeltaTime;
        }

        float movement = speedDiff * accelRate;
        rb.AddForce(movement * Vector2.right);

        // DEBUG: Force correct direction if needed
        if (Mathf.Abs(horizontalInput) > 0.1f && Mathf.Abs(rb.linearVelocity.x) < 0.1f)
        {
            Debug.LogWarning("Player not moving despite input!");
        }
    }

    void BetterJumpPhysics()
    {
        // Different gravity multipliers based on state
        float currentFallMultiplier = fallMultiplier;
        float currentLowJumpMultiplier = lowJumpMultiplier;

        // Reduced gravity when near walls but not sliding
        if (wallSlide != null && wallSlide.IsOnWall && !wallSlide.IsSliding)
        {
            currentFallMultiplier *= 0.7f;
            currentLowJumpMultiplier *= 0.8f;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (currentFallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (currentLowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}