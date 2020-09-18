using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI CurrentScore;
    public TextMeshProUGUI HighScore;

    [Header("Fruits")]
    public GameObject Apple;
    public GameObject Cherry;
    public GameObject Melon;
    public GameObject Orange;
    public GameObject Strawberry;

    GameObject[] Fruits;

    void Awake()
    {
        InvokeRepeating("PlaceAFruitAtRandom", Random.Range(15f, 72f), 72f);

        Fruits = new GameObject[5];
        Fruits[0] = Apple;
        Fruits[1] = Cherry;
        Fruits[2] = Melon;
        Fruits[3] = Orange;
        Fruits[4] = Strawberry;

        DisplayStartingLives();

        LifeCount = NumberOfLives-1;
    }

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

    void PlaceAFruitAtRandom()
    {
        int r = Random.Range(0, 5);
        Vector3 FPos = new Vector3(4.2f, 3.425f, 0f);
        Instantiate(Fruits[r], FPos, Quaternion.identity);
        Debug.Log("Happened");
    }

    public void DeductLife()   //  TODO: Deduct a life when PacMan dies.
    {
        Destroy(LifeHolder.GetChild(LifeCount--).gameObject);
    }
}
