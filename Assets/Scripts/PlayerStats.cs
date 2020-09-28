using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{

    public TextMeshProUGUI CurrentScore;

    public static int timesTurnedU = 0, timesTurnedD = 0, timesTurnedL = 0, timesTurnedR = 0;
    public static int score = 0;
    public static int highScore = 0;
    public static int timesDied = 0;
    public static float timePlayed = 0f;

    //public static void UpdateScore()
    //{
    //    //CurrentScore.text = score.ToString();
    //}
}
