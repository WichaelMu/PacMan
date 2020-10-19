using UnityEngine;
using TMPro;

public class OnAwake : MonoBehaviour
{

    public TextMeshProUGUI CurrentHighScore;
    public TextMeshProUGUI TimePlayed;
    public TextMeshProUGUI subtitle;

    AudioController AudioControl;

    void Awake()
    {
        AudioControl = FindObjectOfType<AudioController>();

        PlayerStats.LoadGame();
        //try {  } catch (System.NullReferenceException) { }
        CurrentHighScore.text = PlayerStats._highScore.ToString();

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

        switch (Random.Range(1, 23))
        {
            case 1:
                subtitle.text = "TYPING LETTERS HERE SO THIS FIELD IS NOT EMPTY. IF YOU WANT TO WRITE SOMETHING USEFUL HERE, EMAIL ME OR SOMETHING";
                break;
            case 2:
                subtitle.text = "VIDEO GAME MASTERMIND STOMPS ALL GHOST OPPOSITION";
                break;
            case 3:
                subtitle.text = "SPECIAL THANKS TO TORU IWATANI!";
                break;
            case 4:
                subtitle.text = "APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA";
                break;
            case 5:
                subtitle.text = "GITHUB.COM/WICHAELMU";
                break;
            case 6:
                subtitle.text = "PLAY THIS GAME AND BECOME A GAMING GENIUS BEATING EVERY SINGLE PAC MAN WORLD RECORD";
                break;
            case 7:
                subtitle.text = "THIS GAME IS UNBELIEVABLE. PLAY FAST AND BECOME A PAC MAN MASTERMIND";
                break;
            case 8:
                subtitle.text = "ITS TIME, DON'T ASK QUESTIONS. JUST PLAY THE GAME";
                break;
            case 9:
                subtitle.text = "CHECK YOUR POSTURE. HAVE YOU HAD A SIP OF WATER RECENTLY? KEEP HYDRATED. REMMEBER TO TAKE A BREAK!";
                break;
            case 10:
                subtitle.text = "TWO + TWO IS FOUR. MINUS ONE, THATS THREE QUICK MAFFS";
                break;
            case 11:
                subtitle.text = "A REIMAGINED PAC MAN GAME";
                break;
            case 12:
                subtitle.text = "A PAC MAN GAME CREATED BY ALFRED NOBEL";
                break;
            case 13:
                subtitle.text = "A GAME MADE FOR THE UNIVERSITY OF TECHNOLOGY SYDNEY";
                break;
            case 14:
                subtitle.text = "ALSO TRY FIREPLAY 2";
                break;
            case 15:
                subtitle.text = "THIS GAME WAS MADE IN 3,552 LINES OF CODE";
                break;
            case 16:
                subtitle.text = "A BUGGY PAC MAN GAME";
                break;
            case 17:
                subtitle.text = "WELCOME TO PAC MAN BY MICHAEL WU";
                break;
            case 18:
                subtitle.text = "THIS GAME WILL WARM YOUR COMPUTER SO YOU CAN KEEP WARM DURING THE WINTER";
                break;
            case 19:
                subtitle.text = "50,000 PEOPLE USED TO LIVE HERE... NOW IT'S A GHOST TOWN";
                break;
            case 20:
                subtitle.text = "THE COST OF A SINGLE B-2 BOMBER IS USD$2,000,000,000";
                break;
            case 21:
                subtitle.text = "MY PASSWORD IS ***************";
                break;
            case 22:
                subtitle.text = "ARE YOU HUNGRY? I'M HUNGRY. USE MY CODE tou76p ON UBEREATS";
                break;
        }
    }

    void Start()
    {
        AudioControl.StopSound("AMBIENT");
        AudioControl.PlaySound("PIANO");
        AudioControl.PlaySound("STARTING");
    }
}
