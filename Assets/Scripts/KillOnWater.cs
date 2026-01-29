using UnityEngine;
using UnityEngine.SceneManagement;

public class KillOnWater : MonoBehaviour
{
    [SerializeField] float respawnDelay = 0.3f;

    bool dead;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead) return;

        if (collision.collider.CompareTag("Water"))
        {
            dead = true;
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (dead) return;

        if (other.CompareTag("Water"))
        {
            dead = true;
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
