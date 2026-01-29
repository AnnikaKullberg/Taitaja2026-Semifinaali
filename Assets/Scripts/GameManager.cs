using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private Text bestTimeText;

    private float currentTime;
    private float latestTime;
    private float bestTime;

    public GameObject player;
    public GameObject water;

    private void Start()
    {
        currentTime = 0f;
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
        latestTime = PlayerPrefs.GetFloat("LatestTime", 0f);

        bestTimeText.text = bestTime > 0 ? "Best Time: " + TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\.ff") : "Best Time: --:--.--";
        timerText.text = currentTime > 0 ? TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss\.ff") : "00:00.00";
    }

    public void StartRun()
    {
        StartTimer();
        water.GetComponent<Water>().canMove = true;
        water.GetComponent<Water>().StartCoroutine("FadeInAudio");
    }

    public void EndRun()
    {
        StopTimer();
        water.GetComponent<Water>().canMove = false;
        water.GetComponent<Water>().audioSource.volume = 0f;
        water.GetComponent<Water>().animator.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartRun();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            EndRun();

        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ResetBestTime();

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");

        }
    }

    public void PlayerLose()
    {
        StopTimer();
    }

    public void PlayerWin()
    {
        StopTimer();
    }


    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        CheckBestTime();
        latestTime = currentTime;
        PlayerPrefs.SetFloat("LatestTime", latestTime);
        currentTime = 0f;
    }

    public void ResetBestTime()
    {
        bestTime = 0f;
        PlayerPrefs.DeleteKey("BestTime");
        PlayerPrefs.Save();
        bestTimeText.text = "Best Time: --:--.--";
        Debug.Log("Best Time Reset!");
    }

    IEnumerator Timer()
    {
        while (true)
        {
            yield return null;
            currentTime += Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            int hundredths = time.Milliseconds / 10;

            timerText.text = string.Format("{0:00}:{1:00}.{2:00}",
                time.Minutes,
                time.Seconds,
                hundredths);
        }
    }

    public void CheckBestTime()
    {
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();

            Debug.Log("New Best Time!");

            bestTimeText.text = "Best Time: " + TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\.ff");
        }

        else
        {
            Debug.Log("Best Time: " + bestTime.ToString("00:00.00"));
        }
    }


}
