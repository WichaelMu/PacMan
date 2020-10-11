using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Score Control")]   //  This keeps tract of the scores and the time played.
    public TextMeshProUGUI CurrentScore;
    public TextMeshProUGUI HighScore;
    public TextMeshProUGUI Time;

    [Header("Pellet Control")]
    public Transform PelletHolder;

    [Header("Fruits")]
    public Transform FruitsSpawnPoint;
    public GameObject Apple;
    public GameObject Cherry;
    public GameObject Melon;
    public GameObject Orange;
    public GameObject Strawberry;
    GameObject[] Fruits;

    [Header("Life Control")]
    public TextMeshProUGUI GameOver;
    public GameObject Life;
    public Transform LifeHolder;
    [Range(1, 10)]
    public int NumberOfLives;
    [HideInInspector]
    public bool DoNotRestart = false;
    int LifeCount;

    IEnumerator StartProcedure;
    [Header("Start Control")]
    public TextMeshProUGUI READY;
    public TextMeshProUGUI CountDown;
    public TextMeshProUGUI START;
    public GameObject PacMan;
    public Transform GhostHolder;

    IEnumerator GameTimer;
    int minute=00, seconds=00, milli=000;

    void Awake()
    {
        //InvokeRepeating("PlaceAFruitAtRandom", UnityEngine.Random.Range(15f, 72f), 72f);
        InvokeRepeating("PlaceAFruitAtRandom", 2f, 5f);

        PlayerStats.LoadGame();
        PlayerStats.score = 0;

        Fruits = new GameObject[5];
        Fruits[0] = Apple;
        Fruits[1] = Cherry;
        Fruits[2] = Melon;
        Fruits[3] = Orange;
        Fruits[4] = Strawberry;

        DisplayStartingLives();
        
        LifeCount = NumberOfLives-1;

        StartProcedure = Countdown();

        HighScore.text = PlayerStats.highScore.ToString();

        PacMan = GameObject.FindWithTag("Player");

        GameTimer = UpdateTime();

        GameOver.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        BeginStartingProcedure();
    }

    void FixedUpdate()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        CurrentScore.text = PlayerStats.score.ToString();
        if (PlayerStats.score > PlayerStats.highScore)
        {
            PlayerStats.highScore = PlayerStats.score;
            HighScore.text = PlayerStats.highScore.ToString();
        }
        if (PelletHolder.childCount == 0)
        {
            //  TODO: Display a message saying "LEVEL COMPLETE" or something.
            //  TODO: Exit back to the main menu.
            EndGame();
            Debug.Log("This level is complete.");
        }

        if (PelletHolder.childCount % 5 == 0)
            SaveGame();
    }

    void DisplayStartingLives()
    {
        for (int i = 0; i < NumberOfLives; i++)
        {
            GameObject StartLife = Instantiate(Life, Vector3.zero, Quaternion.identity);
            StartLife.transform.SetParent(LifeHolder);
            StartLife.transform.localPosition = new Vector3(0f + (30 * i), 0f, 0f);
            StartLife.transform.localScale = new Vector3(.5f, .5f, 0f);
        }
    }

    public void DeductLife()   //  TODO: Deduct a life when PacMan dies.
    {
        if ((LifeCount+1) <= 0)
            EndGame();
        else
            Destroy(LifeHolder.GetChild(LifeCount--).gameObject);
        SaveGame();
    }

    void EndGame()
    {
        Enable(false);  //  Stops all Pac Man and Ghost Movement.

        DoNotRestart = true;

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);

        Invoke("LoadMainMenu", 3f);
        
        //Debug.Log("Pac Man is dead");
    }

    void LoadMainMenu()
    {
        SaveGame();
        SceneManager.LoadScene(0);
    }

    void SaveGame()
    {
        PlayerStats.SaveGame();
    }

    public void BeginStartingProcedure() { 
        StartCoroutine(StartProcedure);
    }

    IEnumerator Countdown()
    {
        Enable(false);
        READY.gameObject.SetActive(true);
        START.gameObject.SetActive(false);
        CountDown.gameObject.SetActive(false);
        yield return new WaitForFixedUpdate();
        FindObjectOfType<AudioController>().StopAllSounds();
        FindObjectOfType<AudioController>().PlaySound("STARTING");
        yield return new WaitForSeconds(4.75f);
        READY.gameObject.SetActive(false);
        CountDown.gameObject.SetActive(true);
        int c = 3;
        for (int i = c; i > 0; i--)
        {
            CountDown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        CountDown.gameObject.SetActive(false);
        START.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        START.gameObject.SetActive(false);
        BeginGame();
        StopCoroutine(Countdown());
    }

    void BeginGame()
    {
        SaveGame();
        Enable(true);
        StartCoroutine(GameTimer);
    }

    void Enable(bool b)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject Ghost = GhostHolder.GetChild(i).gameObject;
            Ghost.GetComponent<GhostController>().enabled = b;
            Ghost.GetComponent<GhostMechanics>().enabled = b;
        }
        PacMan.GetComponent<PacManController>().enabled = b;
    }

    IEnumerator UpdateTime()
    {
        while (true)
        {
            Time.text = minute + ":" + seconds + ":" + milli;
            yield return new WaitForSeconds(.01f);
            milli++;
            PlayerStats.ms = milli;

            if (milli == 100)
            {
                seconds++;
                PlayerStats.seconds = seconds;
                milli = 0;
                PlayerStats.ms = milli;
            }

            if (seconds == 60)
            {
                minute++;
                PlayerStats.minutes++;
                seconds = 0;
            }
        }
    }

    void PlaceAFruitAtRandom()  //  Spawning a Fruit. This is called in an InvokeRepeating() method in Awake().
    {
        int r = UnityEngine.Random.Range(0, 5);
        Vector3 FPos = FruitsSpawnPoint.position;
        if (Fruits[r] != null)
        {
            Instantiate(Fruits[r], FPos, Quaternion.identity);
            Fruits[r] = null;
        }
    }
}
