using UnityEngine;

public class WallSlide : MonoBehaviour
{
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.6f;
    public float wallSlideSpeed = 3f;

    [Header("Wall Jump")]
    public float wallJumpForce = 14f;
    public Vector2 wallJumpDirection = new Vector2(1.3f, 1.4f);

    [Header("Rotation")]
    public float rotationApproachSpeed = 15f;
    public float rotationReturnSpeed = 20f;
    public float maxRotationAngle = 30f;

    public bool IsOnWall { get; private set; }
    public bool IsSliding { get; private set; }
    public bool wallLeft { get; private set; }

    Rigidbody2D rb;
    Collider2D col;
    float currentRotation;
    float wallApproachProgress;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentRotation = 0f;
    }

    void Update()
    {
        CheckWall();
        HandleWallSlide();
        HandleRotation();
    }

    void CheckWall()
    {
        bool wasOnWall = IsOnWall;
        IsOnWall = false;

        Vector2 origin = col.bounds.center;
        float distance = col.bounds.extents.x + wallCheckDistance;

        RaycastHit2D hitLeft = Physics2D.Raycast(origin, Vector2.left, distance, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(origin, Vector2.right, distance, wallLayer);

        if (hitLeft)
        {
            IsOnWall = true;
            wallLeft = true;
            wallApproachProgress = Mathf.Clamp01(wallApproachProgress + Time.deltaTime * 2f);
        }
        else if (hitRight)
        {
            IsOnWall = true;
            wallLeft = false;
            wallApproachProgress = Mathf.Clamp01(wallApproachProgress + Time.deltaTime * 2f);
        }
        else
        {
            wallApproachProgress = Mathf.Clamp01(wallApproachProgress - Time.deltaTime * 3f);
        }

        IsSliding = IsOnWall && rb.linearVelocity.y < -0.1f;
    }

    void HandleWallSlide()
    {
        if (IsSliding)
        {
            float targetYVelocity = -wallSlideSpeed;
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Lerp(rb.linearVelocity.y, targetYVelocity, Time.deltaTime * 10f)
            );
        }
    }

    void HandleRotation()
    {
        float targetRotation = 0f;
        float rotationSpeed = rotationReturnSpeed;

        if (IsOnWall)
        {
            float angle = wallLeft ? -maxRotationAngle : maxRotationAngle;
            targetRotation = angle * wallApproachProgress;
            rotationSpeed = rotationApproachSpeed;

            if (IsSliding)
            {
                float slideRotation = Mathf.Clamp(-rb.linearVelocity.y * 0.5f, 0f, 10f);
                targetRotation += wallLeft ? -slideRotation : slideRotation;
            }
        }

        currentRotation = Mathf.Lerp(
            currentRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    void OnDrawGizmosSelected()
    {
        if (col == null) return;

        Vector2 origin = col.bounds.center;
        float distance = col.bounds.extents.x + wallCheckDistance;

        Gizmos.color = IsOnWall ? Color.green : Color.yellow;
        Gizmos.DrawRay(origin, Vector2.left * distance);
        Gizmos.DrawRay(origin, Vector2.right * distance);
    }
}