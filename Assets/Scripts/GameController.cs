using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI CurrentScore;
    public TextMeshProUGUI HighScore;

    void Update()
    {
        UpdateScore();  
    }

    void UpdateScore()
    {
        CurrentScore.text = PlayerStats.score.ToString();
        if (PlayerStats.score>PlayerStats.highScore)
            HighScore.text = PlayerStats.score.ToString();
    }
}
