using UnityEngine;
using UnityEngine.SceneManagement;

public class WinFlah : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float winDelay = 0.5f;
    [SerializeField] string nextSceneName;

    bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Invoke(nameof(Win), winDelay);
        }
    }

    void Win()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
