using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour
{
    private Collider2D coll;
    public GameManager gameManager;
    Rigidbody2D rb;

    public float speed = 5f;
    public bool canMove;

    public AudioSource audioSource;
    public Animator animator;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        canMove = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canMove)
        {
            Vector2 newPosition = new Vector2(rb.position.x + speed * Time.fixedDeltaTime, rb.position.y);
            rb.MovePosition(newPosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            canMove = false;
            gameManager.PlayerLose();
            
        }
    }

    IEnumerator FadeInAudio()
    {
        float targetVolume = 0.8f;
        float fadeDuration = 2f;
        float currentTime = 0f;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}
