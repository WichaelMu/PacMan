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
using UnityEngine.SceneManagement;

public class OnClick : MonoBehaviour
{

	public TextMeshProUGUI HighScore;
	public TextMeshProUGUI TimePlayed;

	public void PlayIntermission()
	{
		SceneManager.LoadScene(1);
	}

	public void PlayGame()
	{
		SceneManager.LoadScene(2);
	}

	public void PlayInnovation()
	{
		SceneManager.LoadScene(3);
	}

	public void ViewAssets()
	{
		SceneManager.LoadScene(3);
	}

	public void ViewMainMenu()
	{
		PlayerStats.SaveGame();
		SceneManager.LoadScene(0);
	}

	public void ResetSaves()
	{
		PlayerPrefs.DeleteAll();
		HighScore.text = "0";
		TimePlayed.text = "00:00:00";
	}

	public void QuitGame()
	{
		Application.Quit();
		Debug.Log("Game Quit");
	}
}
