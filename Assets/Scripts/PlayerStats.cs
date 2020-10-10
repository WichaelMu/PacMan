using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{

    public TextMeshProUGUI CurrentScore;

    public static int timesTurnedU = 0, timesTurnedD = 0, timesTurnedL = 0, timesTurnedR = 0;
    public static int score = 0;
    public static int _highScore = 0;
    public static int highScore = 0;
    public static int timesDied = 0;
    public static int timePlayed = 0;
    public static int minutes = 00;
    public static int seconds = 00;
    public static int ms = 00;
    static int _minutes = 00;
    static int _seconds = 00;
    static int _ms = 00;

    public static void SaveGame()
    {
        PlayerPrefs.SetInt("timeTurnedU", timesTurnedU);
        PlayerPrefs.SetInt("timeTurnedD", timesTurnedD);
        PlayerPrefs.SetInt("timeTurnedL", timesTurnedL);
        PlayerPrefs.SetInt("timeTurnedR", timesTurnedR);
        PlayerPrefs.SetInt("timesDied", timesDied);
        PlayerPrefs.SetInt("timePlayed", timePlayed);

        minutes += _minutes;
        seconds %= 60;
        seconds += _seconds;
        ms %= 100;
        ms += _ms;

        if (highScore > _highScore)
        {
            PlayerPrefs.SetInt("_highScore", highScore);
            PlayerPrefs.SetString("StringTimePlayed", string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, ms));
            PlayerPrefs.SetInt("minutes", _minutes);
            PlayerPrefs.SetInt("seconds", _seconds);
            PlayerPrefs.SetInt("ms", _ms);
        }
    }

    public static void LoadGame()
    {
        timesTurnedU    =   PlayerPrefs.GetInt("timeTurnedU");
        timesTurnedD    =   PlayerPrefs.GetInt("timeTurnedD");
        timesTurnedL    =   PlayerPrefs.GetInt("timeTurnedL");
        timesTurnedR    =   PlayerPrefs.GetInt("timeTurnedR");
        _highScore      =   PlayerPrefs.GetInt("_highScore");
        timesDied       =   PlayerPrefs.GetInt("timesDied");
        timePlayed      =   PlayerPrefs.GetInt("timePlayed");
        _minutes        =   PlayerPrefs.GetInt("minutes");
        _seconds        =   PlayerPrefs.GetInt("seconds");
        _ms             =   PlayerPrefs.GetInt("ms");

        minutes = 0;
        seconds = 0;
        ms = 0;
    }
}
