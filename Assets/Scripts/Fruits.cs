using UnityEngine;

public class Fruits : MonoBehaviour
{
    GameObject[] FruitsHolder;  //  The GameObject array that stores the fruits that have been collected by Pac Man in the order of collection.
    Sprite SPR; //  The sprite for this fruit.

    static int i = 0;   //  The static index used to indicate a Fruit's collection in the UI.

    void Awake()
    {
        SPR = GetComponent<SpriteRenderer>().sprite;    //  Assigns the sprite for this fruit to SPR.
        FruitsHolder = GameObject.FindGameObjectsWithTag("FRUITHOLDER");    //  Finds the FruitsHolder game object in the scene.
        Destroy(gameObject, 15f);   //  Upon spawning this fruit, destroy it after 15 seconds if Pac Man does not collect it.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //  If this fruit collides with Pac Man.
            EatenByPacman();
    }

    /// <summary>
    /// THe procedure if this Fruit is collected by Pac Man.
    /// </summary>

    void EatenByPacman()
    {
        SpriteRenderer spr = FruitsHolder[i].GetComponent<SpriteRenderer>();    //  Gets the sprite renderer for the game object at FruitsHolder[i].
        spr.sprite = SPR;   //  Change the sprite renderer for the game object at FruitsHolder[i] to be the collected Fruit's sprite.
        i++;    //  Increment the index.s

        Destroy(gameObject);    //  Destroy this Fruit.
    }
}
