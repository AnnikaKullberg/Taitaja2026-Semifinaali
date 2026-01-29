using UnityEngine;

public class WallSlide : MonoBehaviour
{
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.6f;
    public float wallSlideSpeed = 3f;
    public float wallJumpForce = 14f;
    public Vector2 wallJumpDirection = new Vector2(1.2f, 1.4f);
    [Header("Rotation")]
    public float rotationSpeed = 12f;


    Rigidbody2D rb;
    bool isOnWall;
    bool wallLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckWall();

        if (isOnWall && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }

        if (isOnWall && Input.GetButtonDown("Jump"))
        {
            WallJump();
        }
        HandleRotation();
    }

    void CheckWall()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);

        if (hitLeft)
        {
            isOnWall = true;
            wallLeft = true;
        }
        else if (hitRight)
        {
            isOnWall = true;
            wallLeft = false;
        }
        else
        {
            isOnWall = false;
        }
    }

    void WallJump()
    {
        Vector2 dir = new Vector2(
            wallLeft ? wallJumpDirection.x : -wallJumpDirection.x,
            wallJumpDirection.y
        );

        rb.linearVelocity = new Vector2(0, 0);
        rb.AddForce(dir * wallJumpForce, ForceMode2D.Impulse);
    }



void HandleRotation()
    {
        float targetRotation = 0f;

        if (isOnWall)
        {
            // Rotate 90° away from the wall
            targetRotation = wallLeft ? -90f : 90f;
        }

        float currentZ = transform.eulerAngles.z;
        float smoothZ = Mathf.LerpAngle(
            currentZ,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0, 0, smoothZ);
    }

}
