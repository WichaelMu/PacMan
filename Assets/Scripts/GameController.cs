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
        InvokeRepeating("PlaceAFruitAtRandom", 30f, 30f);   //  Spawns a Fruit after 30 seconds of loading Level 1 every 30 seconds.

        PlayerStats.LoadGame(); //  Loads the high score.
        PlayerStats.score = 0;  //  Resets this score to zero.

        Fruits = new GameObject[5]; //  Declares 5 Fruits.
        Fruits[0] = Apple;          //  The first fruit will be an Apple.
        Fruits[1] = Cherry;         //  The second fruit will be a Cherry.
        Fruits[2] = Melon;          //  The third fruit will be a Melon.
        Fruits[3] = Orange;         //  The fourth fruit will be an Orange.
        Fruits[4] = Strawberry;     //  The fifth fruit will be a Strawberry.

        DisplayStartingLives(); //  Show the number of lives.
        
        LifeCount = NumberOfLives-1;    //  The number of lives - 1 used to check if Pac Man has any more lives after dying.

        StartProcedure = Countdown();

        HighScore.text = PlayerStats._highScore.ToString(); //  Sets the high score text UI to the previous high score.

        PacMan = GameObject.FindWithTag("Player");  //  Assigns PacMan to Pac Man.

        GameTimer = UpdateTime();

        GameOver.gameObject.SetActive(false);   //  By default, set the GameOver! UI to be invisible.
    }

    void OnEnable()
    {
        BeginStartingProcedure();
    }

    void FixedUpdate()
    {
        UpdateScore();
    }

    /// <summary>
    /// Updates the score UI every time Pac Man eats a pellet. If the current score is greater than the previous high score, update it.
    /// </summary>

    void UpdateScore()
    {
        CurrentScore.text = PlayerStats.score.ToString();   //  Sets the current score to equal this game's score.
        if (PlayerStats.score > PlayerStats._highScore) //  If this score is greater than the previous high score.
        {
            PlayerStats.highScore = PlayerStats.score;  //  This high score = this score.
            HighScore.text = PlayerStats.highScore.ToString();  //  Update this UI's high score to equal the current score.
        }

        if (PelletHolder.childCount == 0)   //  If there are no more pellets remaining.
        {
            //  TODO: Display a message saying "LEVEL COMPLETE" or something. DONE.
            //  TODO: Exit back to the main menu. DONE.
            EndGame();
            //Debug.Log("This level is complete.");
        }

        if (PelletHolder.childCount % 5 == 0)   //  Every time Pac Man eats 5 pellets.
            PlayerStats.SaveGame(); //  Save the game.
    }

    /// <summary>
    /// Displays the number of lives Pac Man starts with, according to NumberOfLives.
    /// </summary>

    void DisplayStartingLives()
    {
        for (int i = 0; i < NumberOfLives; i++)
        {
            GameObject StartLife = Instantiate(Life, Vector3.zero, Quaternion.identity);    //  Instantiates a life to show in the UI.
            StartLife.transform.SetParent(LifeHolder);  //  Sets the parent of this life to LifeHolder.
            StartLife.transform.localPosition = new Vector3(0f + (30 * i), 0f, 0f); //  Set the liFe UI to be 30 canvas units apart.
            StartLife.transform.localScale = new Vector3(.5f, .5f, 0f); //  Half the scale of the lives UI.
        }
    }

    /// <summary>
    /// Update the lives UI when Pac Man dies.
    /// </summary>

    public void DeductLife()   //  TODO: Deduct a life when PacMan dies.
    {
        if ((LifeCount+1) <= 0) //  If there are no more lives remaining.
            EndGame();  //  End the game.
        else
            Destroy(LifeHolder.GetChild(LifeCount--).gameObject);   //  Destroy the UI element at the right-most life UI.
        PlayerStats.SaveGame(); //  Save the game.
    }

    /// <summary>
    /// Ends the level.
    /// </summary>

    void EndGame()
    {
        Enable(false);  //  Stops all Pac Man and Ghost Movement.

        DoNotRestart = true;    //  Do not restart the level.

        GameOver.gameObject.SetActive(true);    //  Display the GameOver! UI.
        StopCoroutine(GameTimer);   //  Stop the timer.

        Invoke("LoadMainMenu", 3f); //  Show the StartScene after 3 seconds.
        
        //Debug.Log("Pac Man is dead");
    }

    /// <summary>
    /// Loads the Start Scene.
    /// </summary>

    void LoadMainMenu()
    {
        PlayerStats.SaveGame(); //  Saves the game.
        SceneManager.LoadScene(0);  //  Loads the StartScene.
    }

    /// <summary>
    /// Begins the countdown at the beginning of the level.
    /// </summary>

    public void BeginStartingProcedure() { 
        StartCoroutine(StartProcedure);
    }

    /// <summary>
    /// Counts down 3 - 2 - 1 then displays GO!.
    /// </summary>

    IEnumerator Countdown()
    {
        Enable(false);  //  Stops all Pac Man and Ghost movement.
        READY.gameObject.SetActive(true);   //  Display the READY UI.
        START.gameObject.SetActive(false);  //  Hide the START UI.
        CountDown.gameObject.SetActive(false);  //  Hide the CountDown UI.
        yield return new WaitForFixedUpdate();  //  Wait for a fixed udpdate.
        FindObjectOfType<AudioController>().StopAllSounds();    //  Stop all sounds.
        FindObjectOfType<AudioController>().PlaySound("STARTING");  //  Play the starting sound.
        yield return new WaitForSeconds(4.75f); //  After 4.75 seconds.
        READY.gameObject.SetActive(false);  //  Hide the READY UI.
        CountDown.gameObject.SetActive(true);   //  Begin the countdown.
        for (int i = 3; i > 0; i--) //  Begin the countdown at 3.
        {
            CountDown.text = i.ToString();  //  Set the CountDown text to equal the index.
            yield return new WaitForSeconds(1f);    //  Wait one second before iterating.
        }
        CountDown.gameObject.SetActive(false);  //  Hide the CountDown UI.
        START.gameObject.SetActive(true);   //  Display the START UI.
        yield return new WaitForSeconds(1f);    //  Wait one second.
        START.gameObject.SetActive(false);  //  Hide the START UI.
        BeginGame();    //  Begin the game.
        StopCoroutine(Countdown()); //  Stop this IEnumerator.
    }

    /// <summary>
    /// Begins the game by enabling Pac Man and Ghost movement.
    /// </summary>

    void BeginGame()
    {
        PlayerStats.SaveGame(); //  Saves the game.
        Enable(true);   //  Enable Pac Man and Ghost movement.
        StartCoroutine(GameTimer);  //  Begin the timer.
    }

    /// <summary>
    /// Enables Pac Man and Ghost movement according to Boolean b.
    /// </summary>
    /// <param name="b">Boolean to enable or disable Pac Man and Ghost movement.</param>

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

    /// <summary>
    /// Update the time in minutes:seconds:milliseconds.
    /// </summary>

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

    /// <summary>
    /// Spawns a random Fruit.
    /// </summary>

    void PlaceAFruitAtRandom()  //  Spawning a Fruit. This is called in an InvokeRepeating() method in Awake().
    {
        int r = UnityEngine.Random.Range(0, 5); //  Randomly select a Fruit from Fruits[].
        if (Fruits[r] != null)  //  If that random Fruit is not null.
        {
            Instantiate(Fruits[r], FruitsSpawnPoint.position, Quaternion.identity); //  Spawn that random Fruit at FruitsSpawnPoint.
            Fruits[r] = null;   //  Set this Fruit's index to null so that it cannot be spawned again.
        }
    }
}
