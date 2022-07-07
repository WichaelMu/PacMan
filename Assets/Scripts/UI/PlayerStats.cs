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

using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

	public TextMeshProUGUI CurrentScore;

	public static int timesTurnedU = 0, timesTurnedD = 0, timesTurnedL = 0, timesTurnedR = 0;

	/// <summary>
	/// The current level 1 score.
	/// </summary>
	public static int score = 0;
	/// <summary>
	/// The previous high score.
	/// </summary>
	public static int _highScore = 0;
	/// <summary>
	/// The current high score.
	/// </summary>
	public static int highScore = 0;
	/// <summary>
	/// The number of times died.
	/// </summary>
	public static int timesDied = 0;
	/// <summary>
	/// The total amount of time played in milliseconds.
	/// </summary>
	public static int timePlayed = 0;
	/// <summary>
	/// The amount of time played in minutes.
	/// </summary>
	public static int minutes = 00;
	/// <summary>
	/// The amount of time played in seconds.
	/// </summary>
	public static int seconds = 00;
	/// <summary>
	/// The amount of time played in milliseconds.
	/// </summary>
	public static int ms = 00;
	/// <summary>
	/// The amount of minutes played for the previous high score.
	/// </summary>
	static int _minutes = 00;
	/// <summary>
	/// The amount of seconds played for the previous high score.
	/// </summary>
	static int _seconds = 00;
	/// <summary>
	/// The amount of milliseconds played for the previous high score.
	/// </summary>
	static int _ms = 00;

	/// <summary>
	/// Saves the game.
	/// </summary>

	public static void SaveGame()
	{
		PlayerPrefs.SetInt("timeTurnedU", timesTurnedU);
		PlayerPrefs.SetInt("timeTurnedD", timesTurnedD);
		PlayerPrefs.SetInt("timeTurnedL", timesTurnedL);
		PlayerPrefs.SetInt("timeTurnedR", timesTurnedR);
		PlayerPrefs.SetInt("timesDied", timesDied);
		PlayerPrefs.SetInt("timePlayed", timePlayed);

		minutes += _minutes;
		seconds %= 60;
		seconds += _seconds;
		ms %= 100;
		ms += _ms;

		if (highScore > _highScore)
		{
			PlayerPrefs.SetInt("_highScore", highScore);
			PlayerPrefs.SetString("StringTimePlayed", string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, ms));
			PlayerPrefs.SetInt("minutes", _minutes);
			PlayerPrefs.SetInt("seconds", _seconds);
			PlayerPrefs.SetInt("ms", _ms);
		}
	}

	/// <summary>
	/// Loads the game.
	/// </summary>

	public static void LoadGame()
	{
		timesTurnedU = PlayerPrefs.GetInt("timeTurnedU");
		timesTurnedD = PlayerPrefs.GetInt("timeTurnedD");
		timesTurnedL = PlayerPrefs.GetInt("timeTurnedL");
		timesTurnedR = PlayerPrefs.GetInt("timeTurnedR");
		_highScore = PlayerPrefs.GetInt("_highScore");
		timesDied = PlayerPrefs.GetInt("timesDied");
		timePlayed = PlayerPrefs.GetInt("timePlayed");
		_minutes = PlayerPrefs.GetInt("minutes");
		_seconds = PlayerPrefs.GetInt("seconds");
		_ms = PlayerPrefs.GetInt("ms");

		minutes = 0;
		seconds = 0;
		ms = 0;
	}
}
