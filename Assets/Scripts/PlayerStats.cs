using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{

    public TextMeshProUGUI CurrentScore;

    public static int timesTurnedU = 0, timesTurnedD = 0, timesTurnedL = 0, timesTurnedR = 0;
    public static int score = 0;
    public static int highScore = 0;
    public static int timesDied = 0;
    public static int timePlayed = 0;

    //public static void UpdateScore()
    //{
    //    //CurrentScore.text = score.ToString();
    //}

    public static void SaveGame()
    {
        PlayerPrefs.SetInt("timeTurnedU", timesTurnedU);
        PlayerPrefs.SetInt("timeTurnedD", timesTurnedD);
        PlayerPrefs.SetInt("timeTurnedL", timesTurnedL);
        PlayerPrefs.SetInt("timeTurnedR", timesTurnedR);
        PlayerPrefs.SetInt("highScore", highScore);
        PlayerPrefs.SetInt("timesDied", timesDied);
        PlayerPrefs.SetInt("timePlayed", timePlayed);
    }

    public static void LoadGame()
    {
        timesTurnedU = PlayerPrefs.GetInt("timeTurnedU");
        timesTurnedD = PlayerPrefs.GetInt("timeTurnedD");
        timesTurnedL = PlayerPrefs.GetInt("timeTurnedL");
        timesTurnedR = PlayerPrefs.GetInt("timeTurnedR");
        highScore = PlayerPrefs.GetInt("highScore");
        timesDied = PlayerPrefs.GetInt("timesDied");
        timePlayed = PlayerPrefs.GetInt("timePlayed");
    }
}
