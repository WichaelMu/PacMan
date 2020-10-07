using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OnClick : MonoBehaviour
{

    public TextMeshProUGUI CurrentHighScore;
    public TextMeshProUGUI TimePlayed;

    void Awake()
    {
        PlayerStats.LoadGame();
        try { CurrentHighScore.text = PlayerStats.highScore.ToString(); } catch (System.NullReferenceException){  }

        int milli = PlayerStats.timePlayed;
        int c = 0;
        int seconds = 0, minute = 0;

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
        TimePlayed.text = minute + ":" + seconds + ":" + milli;
    }

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
        SceneManager.LoadScene(4);
    }

    public void ViewAssets()
    {
        SceneManager.LoadScene(3);
    }

    public void ViewMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
