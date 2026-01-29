using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Text latestTimeText;
    [SerializeField] private Text bestTimeText;

    private float latestTime;
    private float bestTime;

    public AudioClip btnClickSFX;
    private AudioSource audioSource;

    private void Start()
    {
        optionsPanel.SetActive(false);

        latestTime = PlayerPrefs.GetFloat("LatestTime", 0f);
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        latestTimeText.text = latestTime > 0 ? "Latest Time: " + TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\.ff") : "Latest Time: --:--.--";
        bestTimeText.text = bestTime > 0 ? "Best Time: " + TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\.ff") : "Best Time: --:--.--";

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        StartCoroutine(PlayAndStart());
    }

    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void HideOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        StartCoroutine(EndGame());
    }

    public void PlayButtonClickSound()
    {
        if (audioSource != null && btnClickSFX != null)
        {
            audioSource.PlayOneShot(btnClickSFX);
        }
    }

    IEnumerator PlayAndStart()
    {
        audioSource.PlayOneShot(btnClickSFX);

        yield return new WaitForSeconds(btnClickSFX.length);

        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator EndGame()
    {
        audioSource.PlayOneShot(btnClickSFX);

        yield return new WaitForSeconds(btnClickSFX.length);

        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
