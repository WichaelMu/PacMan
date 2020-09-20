using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClick : MonoBehaviour
{
    public void PlayIntermission()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(2);
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
