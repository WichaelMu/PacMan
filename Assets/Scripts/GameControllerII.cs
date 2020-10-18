using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameControllerII : MonoBehaviour
{

    [Header("Score Control")]   //  This keeps tract of the scores and the time played.
    public TextMeshProUGUI P1Score;
    public TextMeshProUGUI P2Score;
    public TextMeshProUGUI Time;

    [Header("Pellet Control")]
    public Transform PelletHolder;

    [Header("Game Control")]
    public TextMeshProUGUI GameOver;
    public TextMeshProUGUI Message;

    IEnumerator StartProcedure;
    [Header("Start Control")]
    public TextMeshProUGUI READY;
    public TextMeshProUGUI CountDown;
    public TextMeshProUGUI START;

    [Header("Natural Enemies")]
    public GameObject Ghost;
    Transform GhostSpawnPoint;

    AudioController AudioControl;

    GameObject P1;
    int _P1Score = 0;
    GameObject P2;
    int _P2Score = 0;

    IEnumerator GameTimer;
    int minute = 00, seconds = 00, milli = 000;

    void Awake()
    {
        AudioControl = FindObjectOfType<AudioController>();

        P1 = GameObject.FindWithTag("RED");
        P2 = GameObject.FindWithTag("GREEN");

        StartProcedure = Countdown();

        GameTimer = UpdateTime();

        GameOver.gameObject.SetActive(false);

        P1Score.text = _P1Score.ToString();
        P2Score.text = _P2Score.ToString();
    }

    void Start()
    {
        GhostSpawnPoint = GameObject.FindWithTag("GhostSpawn").transform;
    }

    void OnEnable()
    {
        BeginStartingProcedure();
    }

    /// <summary>
    /// Updates the score every time either player eats a pellet.
    /// </summary>
    /// <param name="ID">The ID that corresponds to the player who ate a pellet.</param>
    /// <param name="amount">The amount to increase ID player's score.</param>

    public void UpdateScore(int ID, int amount)
    {
        if (ID == 1)
            _P1Score += amount;
        if (ID == 2)
            _P2Score += amount;
        P1Score.text = "" + _P1Score;
        P2Score.text = "" + _P2Score;

        if (PelletHolder.childCount == 0)   //  If there are no more pellets.
            EndGame();  //  End the game.
    }

    /// <summary>
    /// Ends the game by score.
    /// </summary>

    void EndGame()
    {
        Enable(false);  //  Stops movement.

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);
        Message.text = "PLAYER " + (_P1Score < _P2Score ? "1" : "2") + " WINS BY SCORE!";

        Invoke("LoadMainMenu", 3f);

        //Debug.Log("Pac Man is dead");
    }

    /// <summary>
    /// Ends the game by kill.
    /// </summary>
    /// <param name="ID">The ID representing the winning player.</param>
    /// <param name="ghost">The Ghost who killed a player.</param>

    public void EndGame(int ID, GameObject ghost = null)
    {
        Enable(false);  //  Stops all movement.

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);
        Message.text = "PLAYER " + (ID == 1 ? "2" : "1");
        Message.text += (ghost == null) ? " WINS BY KILL!" : " WINS BY GHOST KILL";
        Invoke("LoadMainMenu", 3f);
    }

    /// <summary>
    /// Loads the Start Scene.
    /// </summary>

    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Begins the countdown 3 - 2 - 1 GO!.
    /// </summary>

    public void BeginStartingProcedure()
    {
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
        yield return new WaitForSeconds(1f);
        READY.gameObject.SetActive(false);
        CountDown.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            CountDown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        AudioControl.PlaySound("COMPLEX");
        CountDown.gameObject.SetActive(false);
        START.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        START.gameObject.SetActive(false);
        BeginGame();
        StopCoroutine(Countdown());
    }

    /// <summary>
    /// Begins the game by enabling player movement.
    /// </summary>

    void BeginGame()
    {
        Enable(true);
        InvokeRepeating("SpawnGhost", 0, 30f);  //  Spawns two Ghosts every 30 seconds.
        StartCoroutine(GameTimer);
    }

    /// <summary>
    /// Enables player movement according to Boolean b.
    /// </summary>
    /// <param name="b">Boolean to enable or disable player movement.</param>

    void Enable(bool b)
    {
        //  PLAYER CONTROLLERS EN/DISABLE.
        P1.GetComponent<PlayerControllers>().enabled = b;
        P2.GetComponent<PlayerControllers>().enabled = b;

        GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject g in Ghosts)
            g.GetComponent<InnovationAI>().enabled = b;
    }

    IEnumerator UpdateTime()
    {
        while (true)
        {
            Time.text = minute + ":" + seconds + ":" + milli;
            yield return new WaitForSeconds(.01f);
            milli++;
            PlayerStats.timePlayed++;
            if (milli == 100)
            {
                seconds++;
                milli = 0;
            }

            if (seconds == 60)
            {
                minute++;
                seconds = 0;
            }
        }
    }

    /// <summary>
    /// Spawns two Ghosts that target either player.
    /// </summary>

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(Ghost, GhostSpawnPoint.position, Quaternion.identity);
        ghost.GetComponent<InnovationAI>().SpecifyTarget = 1;
        GameObject _ghost = Instantiate(Ghost, GhostSpawnPoint.position, Quaternion.identity);
        _ghost.GetComponent<InnovationAI>().SpecifyTarget = 2;
    }
}
