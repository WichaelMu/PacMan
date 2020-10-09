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

    GameObject P1;
    int _P1Score = 0;
    GameObject P2;
    int _P2Score = 0;

    IEnumerator GameTimer;
    int minute = 00, seconds = 00, milli = 000;

    void Awake()
    {
        P1 = GameObject.FindWithTag("RED");
        P2 = GameObject.FindWithTag("BLUE");

        StartProcedure = Countdown();

        GameTimer = UpdateTime();

        GameOver.gameObject.SetActive(false);

        P1Score.text = _P1Score.ToString();
        P2Score.text = _P2Score.ToString();
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

    public void EndGame()
    {
        Enable(false);  //  Stops all Pac Man and Ghost Movement.

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);
        Message.text = "PLAYER " + (_P1Score < _P2Score ? "1" : "2") + " WINS BY SCORE!";

        Invoke("LoadMainMenu", 3f);

        //Debug.Log("Pac Man is dead");
    }

    public void EndGame(int ID)
    {
        Enable(false);  //  Stops all movement.

        GameOver.gameObject.SetActive(true);
        StopCoroutine(GameTimer);
        Message.text = "PLAYER " + (ID == 1 ? "2" : "1") + " WINS BY KILL!";

        Invoke("LoadMainMenu", 3f);
    }

    void LoadMainMenu()
    {
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
        Enable(true);
        StartCoroutine(GameTimer);
    }

    void Enable(bool b)
    {
        //  PLAYER CONTROLLERS EN/DISABLE.
        P1.GetComponent<PlayerControllers>().enabled = b;
        P2.GetComponent<PlayerControllers>().enabled = b;
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
}
