using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    public float dashSpeed = 22f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.25f;

    Rigidbody2D rb;
    bool canDash = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DoDash());
        }
    }

    IEnumerator DoDash()
    {
        canDash = false;

        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (input == Vector2.zero)
            input = new Vector2(transform.localScale.x, 0);

        rb.linearVelocity = input.normalized * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
