using TMPro;
using UnityEngine;

public class OnAwake : MonoBehaviour
{

	/// <summary>
	/// The current high score.
	/// </summary>
	public TextMeshProUGUI CurrentHighScore;
	/// <summary>
	/// The time played for this high score.
	/// </summary>
	public TextMeshProUGUI TimePlayed;
	/// <summary>
	/// The 'Creative Subtitle' for this game.
	/// </summary>
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
		int SubtitleIndex = PlayerPrefs.GetInt("SubtitleText", 0);
		PlayerPrefs.SetInt("SubtitleText", SubtitleIndex > 22 ? 0 : SubtitleIndex + 1);

		switch (SubtitleIndex)
		{
			case 0:
				subtitle.text = "TYPING LETTERS HERE SO THIS FIELD IS NOT EMPTY. IF YOU WANT TO WRITE SOMETHING USEFUL HERE, EMAIL ME OR SOMETHING";
				break;
			case 1:
				subtitle.text = "VIDEO GAME MASTERMIND STOMPS ALL GHOST OPPOSITION";
				break;
			case 2:
				subtitle.text = "SPECIAL THANKS TO TORU IWATANI!";
				break;
			case 3:
				subtitle.text = "APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA APPA";
				break;
			case 4:
				subtitle.text = "GITHUB.COM/WICHAELMU";
				break;
			case 5:
				subtitle.text = "PLAY THIS GAME AND BECOME A GAMING GENIUS BEATING EVERY SINGLE PAC MAN WORLD RECORD";
				break;
			case 6:
				subtitle.text = "THIS GAME IS UNBELIEVABLE. PLAY FAST AND BECOME A PAC MAN MASTERMIND";
				break;
			case 7:
				subtitle.text = "ITS TIME, DON'T ASK QUESTIONS. JUST PLAY THE GAME";
				break;
			case 8:
				subtitle.text = "CHECK YOUR POSTURE. HAVE YOU HAD A SIP OF WATER RECENTLY? KEEP HYDRATED. REMMEBER TO TAKE A BREAK!";
				break;
			case 9:
				subtitle.text = "TWO + TWO IS FOUR. MINUS ONE, THATS THREE QUICK MAFFS";
				break;
			case 10:
				subtitle.text = "A REIMAGINED PAC MAN GAME";
				break;
			case 11:
				subtitle.text = "A PAC MAN GAME CREATED BY ALFRED NOBEL";
				break;
			case 12:
				subtitle.text = "A GAME MADE FOR THE UNIVERSITY OF TECHNOLOGY SYDNEY";
				break;
			case 13:
				subtitle.text = "ALSO TRY FIREPLAY 2";
				break;
			case 14:
				subtitle.text = "THIS GAME WAS MADE WITH 3,552 LINES OF CODE";
				break;
			case 15:
				subtitle.text = "A BUGGY PAC MAN GAME";
				break;
			case 16:
				subtitle.text = "WELCOME TO PAC MAN BY MICHAEL WU";
				break;
			case 17:
				subtitle.text = "THIS GAME WILL WARM YOUR COMPUTER SO YOU CAN KEEP WARM DURING THE WINTER";
				break;
			case 18:
				subtitle.text = "50,000 PEOPLE USED TO LIVE HERE... NOW IT'S A GHOST TOWN";
				break;
			case 19:
				subtitle.text = "AUSTRALIANS ALL LET US REJOICE FOR WE ARE YOUNG AND FREE!";
				break;
			case 20:
				subtitle.text = "MY PASSWORD IS ***************";
				break;
			case 21:
				subtitle.text = "ARE YOU HUNGRY? I'M HUNGRY. USE MY CODE tou76p ON UBEREATS";
				break;
			case 22:
				subtitle.text = "1. E4 E5 2. Bc4 Nc6 3. Qh5 Nf6 QxF7#";
				break;
		}
	}

	void Start()
	{
		AudioControl.StopSound("AMBIENT");
		AudioControl.PlaySound("PIANO");
		//AudioControl.PlaySound("STARTING");
	}
}
