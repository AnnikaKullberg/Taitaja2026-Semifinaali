using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private Text bestTimeText;

    private float currentTime;
    public float bestTime;

    private void Start()
    {
        currentTime = 0f;
        bestTime = PlayerPrefs.GetFloat("BestTime", 0f);

        timerText.text = currentTime.ToString("00:00:00");
        bestTimeText.text = "Best Time: " + TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\.ff");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartTimer();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StopTimer();

        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ResetBestTime();

        }
    }

    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        CheckBestTime();
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
        if (currentTime > bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();

            Debug.Log("New Best Time!");

            bestTimeText.text = "Best Time: " + TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\.ff");
        }

        else
        {
            Debug.Log("Best Time: " + bestTime.ToString("00:00:00"));
        }
    }


}
