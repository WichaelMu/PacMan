using UnityEngine;
using TMPro;

public class OnAwake : MonoBehaviour
{

    public TextMeshProUGUI CurrentHighScore;
    public TextMeshProUGUI TimePlayed;
    public TextMeshProUGUI subtitle;

    void Awake()
    {
        PlayerStats.LoadGame();
        try { CurrentHighScore.text = PlayerStats._highScore.ToString(); } catch (System.NullReferenceException) { }

        int milli = PlayerStats.timePlayed;
        int c = 00;
        int seconds = 00, minute = 00;

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
        //TimePlayed.text = minute + ":" + seconds + ":" + milli;
        TimePlayed.text = (PlayerPrefs.GetString("StringTimePlayed").Length == 0 ? "00:00:00" : PlayerPrefs.GetString("StringTimePlayed"));

        switch (Random.Range(1, 6))
        {
            case 1:
                subtitle.text = "TYPING LETTERS HERE SO THIS FIELD IS NOT EMPTY. IF YOU WANT TO WRITE SOMETHING USEFUL HERE, EMAIL ME OR SOMETHING";
                break;
            case 2:
                subtitle.text = "ABSOLUTE GAMING MASTERMIND RECREATES PAC MAN AND CLAPS ALL OPPOSITION";
                break;
            case 3:
                subtitle.text = "SPECIAL THANKS TO TORU IWATANI!";
                break;
            case 4:
                subtitle.text = "APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA ";
                break;
            case 5:
                subtitle.text = "GITHUB.COM/WICHAELMU";
                break;
        }
    }
}
