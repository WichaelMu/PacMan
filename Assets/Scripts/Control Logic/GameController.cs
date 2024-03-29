﻿/*
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

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
	/// <summary>
	/// The current score for this level.
	/// </summary>
	[Header("Score Control")]   //  This keeps tract of the scores and the time played.
	public TextMeshProUGUI CurrentScore;
	/// <summary>
	/// The current high score text.
	/// </summary>
	public TextMeshProUGUI HighScore;
	/// <summary>
	/// The time spent on this level.
	/// </summary>
	public TextMeshProUGUI Time;

	/// <summary>
	/// Where all the pellets can be accessed.
	/// </summary>
	[Header("Pellet Control")]
	public Transform PelletHolder;

	/// <summary>
	/// The spawn point for the bonus Fruits.
	/// </summary>
	[Header("Fruits")]
	public Transform FruitsSpawnPoint;
	public GameObject Apple;
	public GameObject Cherry;
	public GameObject Melon;
	public GameObject Orange;
	public GameObject Strawberry;
	GameObject[] Fruits;

	/// <summary>
	/// The text saying GameOver!.
	/// </summary>
	[Header("Life Control")]
	public TextMeshProUGUI GameOver;
	/// <summary>
	/// The life UI.
	/// </summary>
	public GameObject Life;
	/// <summary>
	/// Where the lives are held and shown on-screen.
	/// </summary>
	public Transform LifeHolder;
	/// <summary>
	/// The number of lives that the Player begins with.
	/// </summary>
	[Range(1, 10)]
	public int NumberOfLives;
	[HideInInspector]
	public bool DoNotRestart = false;
	int LifeCount;

	/// <summary>
	/// The beginning message 3, 2, 1, GO!.
	/// </summary>
	IEnumerator StartProcedure;
	[Header("Start Control")]
	public TextMeshProUGUI READY;
	public TextMeshProUGUI CountDown;
	public TextMeshProUGUI START;
	public GameObject PacMan;
	public Transform GhostHolder;
	AudioController audioController;

	IEnumerator GameTimer;
	int minute = 00, seconds = 00, milli = 000;

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

		LifeCount = NumberOfLives - 1;    //  The number of lives - 1 used to check if Pac Man has any more lives after dying.

		StartProcedure = Countdown();

		HighScore.text = PlayerStats._highScore.ToString(); //  Sets the high score text UI to the previous high score.

		PacMan = GameObject.FindWithTag("Player");  //  Assigns PacMan to Pac Man.

		GameTimer = UpdateTime();

		GameOver.gameObject.SetActive(false);   //  By default, set the GameOver! UI to be invisible.

		audioController = FindObjectOfType<AudioController>();
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
		if ((LifeCount + 1) <= 0) //  If there are no more lives remaining.
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

	public void BeginStartingProcedure()
	{
		StartCoroutine(StartProcedure);
	}

	/// <summary>
	/// Counts down 3 - 2 - 1 then displays GO!.
	/// </summary>

	IEnumerator Countdown()
	{
#if !UNITY_EDITOR // Disable the Countdown.
		Enable(false);  //  Stops all Pac Man and Ghost movement.
		READY.gameObject.SetActive(true);   //  Display the READY UI.
		START.gameObject.SetActive(false);  //  Hide the START UI.
		CountDown.gameObject.SetActive(false);  //  Hide the CountDown UI.
		yield return new WaitForFixedUpdate();  //  Wait for a fixed udpdate.
		audioController.StopAllSounds();    //  Stop all sounds.
		audioController.PlaySound("STARTING");  //  Play the starting sound.
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
#else
		yield return null;
#endif
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
		PacMan.GetComponent<PacStudentController>().enabled = b;
	}

	/// <summary>
	/// Update the time in minutes:seconds:milliseconds.
	/// </summary>

	IEnumerator UpdateTime()
	{
		while (true)
		{
			Time.text = minute.ToString("00") + ":" + seconds.ToString("00") + ":" + milli.ToString("00");
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
