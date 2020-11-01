using UnityEngine;

public class CherryController : MonoBehaviour
{
    /// <summary>
    /// The Cherry to be instantiated and lerped across the screen.
    /// </summary>
    public GameObject Cherry;
    /// <summary>
    ///  //  The time for lerping.
    /// </summary>
    float time;

    GameObject CurrentCherry;

    void Start()
    {
        InvokeRepeating("Refresh", 0f, 30f);    //  Repeat the Cherry lerping every 30 seconds, starting upon loading Level 1.
    }

    /// <summary>
    /// Lerps Cherry across the screen.
    /// </summary>

    void Update()
    {
        float delta = Mathf.Pow(((Time.time - time) / 5f), 3);
        if (CurrentCherry!=null)
            CurrentCherry.transform.position = Vector3.Lerp(transform.position, new Vector3(-20f, transform.position.y, transform.position.z), delta);
    }

    /// <summary>
    /// Instantiates Cherry and destroys Cherry after 10 seconds.
    /// </summary>

    void Refresh()
    {
        CurrentCherry = Instantiate(Cherry, Vector3.zero, Quaternion.identity);
        time = Time.time;
        Destroy(CurrentCherry, 10f);
    }
}
