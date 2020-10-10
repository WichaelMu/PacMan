using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        SceneManager.LoadScene(4);
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
