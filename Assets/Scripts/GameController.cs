using System.Collections;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Score Control")]
    public TextMeshProUGUI CurrentScore;
    public TextMeshProUGUI HighScore;

    [Header("Pellet Control")]
    public Transform PelletHolder;

    [Header("Fruits")]
    public GameObject Apple;
    public GameObject Cherry;
    public GameObject Melon;
    public GameObject Orange;
    public GameObject Strawberry;

    [Header("Life Control")]
    public GameObject Life;
    public Transform LifeHolder;

    [Range(1, 10)]
    public int NumberOfLives;

    GameObject[] Fruits;
    int LifeCount;

    IEnumerator StartProcedure;
    [Header("Start Control")]
    public TextMeshProUGUI CountDown;
    public TextMeshProUGUI START;
    public GameObject PacMan;
    public Transform GhostHolder;

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

        StartProcedure = Countdown();
        StartCoroutine(StartProcedure);
    }

    void FixedUpdate()
    {
        UpdateScore();
    }

    void UpdateScore()
    {
        CurrentScore.text = PlayerStats.score.ToString();
        if (PlayerStats.score>PlayerStats.highScore)
            HighScore.text = PlayerStats.score.ToString();
        if (PelletHolder.childCount == 0)
        {
            Debug.Log("This level is complete.");
        }
    }

    void PlaceAFruitAtRandom()
    {
        int r = Random.Range(0, 5);
        Vector3 FPos = new Vector3(4.2f, 3.425f, 0f);
        if (Fruits[r] != null)
        {
            Instantiate(Fruits[r], FPos, Quaternion.identity);
            Fruits[r] = null;
        }
    }

    void DisplayStartingLives()
    {
        for (int i = 0; i < NumberOfLives; i++)
        {
            GameObject StartLife = Instantiate(Life, Vector3.zero, Quaternion.identity);
            StartLife.transform.SetParent(LifeHolder);
            StartLife.transform.localPosition = new Vector3(-238.5f + (30 * i), -304.5f, 0f);
            StartLife.transform.localScale = new Vector3(.5f, .5f, 0f);
        }
    }

    public void DeductLife()   //  TODO: Deduct a life when PacMan dies.
    {
        Destroy(LifeHolder.GetChild(LifeCount--).gameObject);
    }

    IEnumerator Countdown()
    {
        START.enabled = false;
        int c = 3;
        for (int i = c; i > 0; i--)
        {
            CountDown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        CountDown.enabled = false;
        START.enabled = true;
        yield return new WaitForSeconds(1f);
        START.enabled = false;
    }
}
