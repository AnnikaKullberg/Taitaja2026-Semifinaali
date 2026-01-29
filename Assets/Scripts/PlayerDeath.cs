using UnityEngine;
using UnityEngine.Events;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private LayerMask hazardLayer;
    [SerializeField] private string hazardTag = "Hazard"; // Alternative to layer
    [SerializeField] private bool useLayer = true;

    [Header("Death Response")]
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private bool respawnAtCheckpoint = true;
    [SerializeField] private float deathDelay = 0.2f;
    [SerializeField] private GameObject deathEffectPrefab;

    [Header("Visual Feedback")]
    [SerializeField] private bool disableRendererOnDeath = true;
    [SerializeField] private bool disableColliderOnDeath = true;
    [SerializeField] private Color deathFlashColor = Color.red;
    [SerializeField] private float deathFlashDuration = 0.3f;

    // Component References
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private Rigidbody2D rb;
    private PlayerController2D playerController;

    // State
    private bool isDead = false;
    private Color originalColor;
    private Vector2 checkpointPosition;

    void Start()
    {
        // Get component references
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController2D>();

        // Store original color for flashing effect
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Set initial checkpoint to starting position
        checkpointPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForHazard(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CheckForHazard(other.gameObject);
    }

    void CheckForHazard(GameObject hazardObject)
    {
        if (isDead) return;

        bool isHazard = false;

        if (useLayer)
        {
            // Check if object is on hazard layer
            if ((hazardLayer.value & (1 << hazardObject.layer)) > 0)
            {
                isHazard = true;
            }
        }
        else
        {
            // Check if object has hazard tag
            if (hazardObject.CompareTag(hazardTag))
            {
                isHazard = true;
            }
        }

        if (isHazard)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Visual feedback
        StartCoroutine(DeathFlash());

        // Disable player control
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Disable physics
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Disable visual components
        if (disableRendererOnDeath && spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (disableColliderOnDeath && playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // Spawn death effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Invoke death event
        onDeath.Invoke();

        // Handle respawn or reload
        if (respawnAtCheckpoint)
        {
            Invoke("Respawn", deathDelay);
        }
        else
        {
            // If not respawning, reload the scene after delay
            Invoke("ReloadScene", deathDelay + 0.5f);
        }
    }

    private System.Collections.IEnumerator DeathFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = deathFlashColor;
        yield return new WaitForSeconds(deathFlashDuration);
        spriteRenderer.color = originalColor;
    }

    public void SetCheckpoint(Vector2 position)
    {
        checkpointPosition = position;
        Debug.Log($"Checkpoint set at: {position}");
    }

    void Respawn()
    {
        // Reset position to checkpoint
        transform.position = checkpointPosition;

        // Re-enable components
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (disableRendererOnDeath && spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (disableColliderOnDeath && playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        // Reset state
        isDead = false;

        // Reset sprite color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void ReloadScene()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    // Public method to trigger death from other scripts (e.g., falling off map)
    public void KillPlayer()
    {
        Die();
    }

    // Public method to check if player is dead
    public bool IsDead()
    {
        return isDead;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize checkpoint in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checkpointPosition, 0.5f);
        Gizmos.DrawLine(transform.position, checkpointPosition);
    }
}