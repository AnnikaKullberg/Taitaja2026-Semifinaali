using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager Instance;

    public int maxScores = 10;
    public List<int> highscores = new List<int>();

    private const string SAVE_KEY = "HIGHSCORES";

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int newScore)
    {
        highscores.Add(newScore);
        highscores = highscores
            .OrderByDescending(score => score)
            .Take(maxScores)
            .ToList();

        SaveScores();
    }

    void SaveScores()
    {
        string data = string.Join(",", highscores);
        PlayerPrefs.SetString(SAVE_KEY, data);
        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        highscores.Clear();

        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return;

        string data = PlayerPrefs.GetString(SAVE_KEY);
        string[] scores = data.Split(',');

        foreach (string s in scores)
        {
            if (int.TryParse(s, out int score))
                highscores.Add(score);
        }
    }

    public List<int> GetHighscores()
    {
        return highscores;
    }
}
