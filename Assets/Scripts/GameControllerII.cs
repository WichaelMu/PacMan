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

    public void UpdateScore(int ID, int amount)
    {
        if (ID == 1)
            _P1Score += amount;
        if (ID == 2)
            _P2Score += amount;
        P1Score.text = "" + _P1Score;
        P2Score.text = "" + _P2Score;

        if (PelletHolder.childCount == 0)
            EndGame();
    }

    void EndGame()
    {
        Enable(false);  //  Stops movement.

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);
        Message.text = "PLAYER " + (_P1Score < _P2Score ? "1" : "2") + " WINS BY SCORE!";

        Invoke("LoadMainMenu", 3f);

        //Debug.Log("Pac Man is dead");
    }

    public void EndGame(int ID, GameObject ghost = null)
    {
        Enable(false);  //  Stops all movement.

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);
        Message.text = "PLAYER " + (ID == 1 ? "2" : "1");
        if (ghost == null)
            Message.text += " WINS BY KILL!";
        else
            Message.text += " WINS BY GHOST KILL";
        Invoke("LoadMainMenu", 3f);
    }

    void LoadMainMenu()
    {
        PlayerStats.SaveGame();
        SceneManager.LoadScene(0);
    }

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
        int c = 3;
        for (int i = c; i > 0; i--)
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

    void BeginGame()
    {
        Enable(true);
        InvokeRepeating("SpawnGhost", 0, 30f);
        StartCoroutine(GameTimer);
    }

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

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(Ghost, GhostSpawnPoint.position, Quaternion.identity);
        ghost.GetComponent<InnovationAI>().SpecifyTarget = 1;
        GameObject _ghost = Instantiate(Ghost, GhostSpawnPoint.position, Quaternion.identity);
        _ghost.GetComponent<InnovationAI>().SpecifyTarget = 2;
    }
}
