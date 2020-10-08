using UnityEngine;
using TMPro;

public class OnAwake : MonoBehaviour
{

    public TextMeshProUGUI CurrentHighScore;
    public TextMeshProUGUI TimePlayed;
    public Transform Particle;

    void Awake()
    {
        PlayerStats.LoadGame();
        try { CurrentHighScore.text = PlayerStats.highScore.ToString(); } catch (System.NullReferenceException) { }

        int milli = PlayerStats.timePlayed;
        int c = 0;
        int seconds = 0, minute = 0;

        for (int i = 0; i < milli; i++)
        {
            c++;
            if (c == 100)
            {
                seconds++;
                c = 0;
            }

            if (seconds == 60)
            {
                minute++;
                seconds = 0;
            }
        }
        TimePlayed.text = minute + ":" + seconds + ":" + milli;
    }
}
