using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance => _instance;

    private const string HighScoreKey = "HighScore";
    private const string GemCountKey = "GemCount";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void SetHighScore(int score)
    {
        PlayerPrefs.SetInt(HighScoreKey, score);
    }

    public int GetGemCount()
    {
        return PlayerPrefs.GetInt(GemCountKey, 0);
    }

    public void SetGemCount(int count)
    {
        PlayerPrefs.SetInt(GemCountKey, count);
    }
}
