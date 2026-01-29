using UnityEngine;

public class WallWalker : MonoBehaviour
{
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.6f;
    public float wallMoveSpeed = 5f;

    Rigidbody2D rb;
    bool isOnWall;
    float originalGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        CheckWall();

        if (isOnWall)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(0, vertical * wallMoveSpeed);
        }
    }

    void CheckWall()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);

        isOnWall = hitLeft || hitRight;

        if (isOnWall)
            rb.gravityScale = 0f;
        else
            rb.gravityScale = originalGravity;
    }
}
