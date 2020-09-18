using UnityEngine;

public class Fruits : MonoBehaviour
{
    GameObject[] FruitsHolder;
    Sprite SPR;

    static int i = 0;

    void Awake()
    {
        SPR = GetComponent<SpriteRenderer>().sprite;
        FruitsHolder = GameObject.FindGameObjectsWithTag("FRUITHOLDER");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            EatenByPacman();
    }

    void EatenByPacman()
    {
        SpriteRenderer spr = FruitsHolder[i].GetComponent<SpriteRenderer>();
        spr.sprite = SPR;
        i++;

        Debug.Log("Eaten");
        Destroy(gameObject);
    }
}
