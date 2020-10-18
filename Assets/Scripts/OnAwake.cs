﻿using UnityEngine;
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

        switch (Random.Range(1, 11))
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
        }
    }

    void Start()
    {
        AudioControl.StopSound("AMBIENT");
        AudioControl.PlaySound("PIANO");
        AudioControl.PlaySound("STARTING");
    }
}
