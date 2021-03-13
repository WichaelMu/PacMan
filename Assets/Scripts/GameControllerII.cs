/*
    Copyright 2020 :: Michael Wu

    Redistribution and use in source and binary forms, with or without modification,
    are permitted provided that the following conditions are met:

	1.Redistributions or derivations of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.

	2. Redistributions or derivative works in binary form must reproduce the above
	copyright notice. This list of conditions and the following	disclaimer must be
	reproduced in the documentation and/or other materials provided with the distribution.

	3. Neither the name of the copyright holder nor the names of its contributors may
	be used to endorse or promote products derived from this software without specific
	prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS	"AS IS" AND ANY
	EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
	OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
	SHALL THE COPYRIGHT	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
    INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
    BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
	CONTRACT, STRICT LIABILITY, OR TORT	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
	ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	SUCH DAMAGE.

	Links
	~~~~~
	GitHub:     https://github.com/WichaelMu/
    Itch.io:    https://wichael-mu.itch.io/

*/

using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameControllerII : MonoBehaviour
{
    /// <summary>
    /// The current score for Player 1 (Orange).
    /// </summary>
    [Header("Score Control")]   //  This keeps tract of the scores and the time played.
    public TextMeshProUGUI P1Score;
    /// <summary>
    /// The current score for Player 2 (Green).
    /// </summary>
    public TextMeshProUGUI P2Score;
    /// <summary>
    /// The amount of time spent playing level 2.
    /// </summary>
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

    /// <summary>
    /// The Ghosts that chase the players.
    /// </summary>
    [Header("Natural Enemies")]
    public GameObject Ghost;
    /// <summary>
    /// The spawn point for the Ghost that chases Player 1.
    /// </summary>
    Transform GhostSpawnPoint;
    /// <summary>
    /// The spawn point for the Ghost that chases Player 2.
    /// </summary>
    Transform GhostSpawnPoint1;

    AudioController AudioControl;

    /// <summary>
    /// Player 1.
    /// </summary>
    GameObject P1;
    /// <summary>
    /// The score for Player 1.
    /// </summary>
    int _P1Score = 0;
    /// <summary>
    /// Player 2.
    /// </summary>
    GameObject P2;
    /// <summary>
    /// The score for Player 2.
    /// </summary>
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
        GhostSpawnPoint1 = GameObject.FindWithTag("GhostSpawn1").transform;
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

        if (PelletHolder.childCount < 2)   //  If there are no more pellets.
            InvokeRepeating(nameof(SpawnGhost), 0f, 12f);
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

        Invoke(nameof(LoadMainMenu), 3f);

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
        Invoke(nameof(LoadMainMenu), 3f);
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
        AudioControl.StopAllSounds();
        AudioControl.PlaySound("STARTING");
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
        InvokeRepeating(nameof(SpawnGhost), 0, 30f);  //  Spawns two Ghosts every 30 seconds.
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
        GameObject _ghost = Instantiate(Ghost, GhostSpawnPoint1.position, Quaternion.identity);
        _ghost.GetComponent<InnovationAI>().SpecifyTarget = 2;
    }
}
