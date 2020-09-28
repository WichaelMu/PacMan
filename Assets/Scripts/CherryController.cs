using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject Cherry;
    float time;

    GameObject CurrentCherry;

    void Start()
    {
        InvokeRepeating("Refresh", 0f, 30f);
    }

    void Update()
    {
        float delta = Mathf.Pow(((Time.time - time) / 5f), 3);
        if (CurrentCherry!=null)
            CurrentCherry.transform.position = Vector3.Lerp(transform.position, new Vector3(-20f, transform.position.y, transform.position.z), delta);
    }

    void Refresh()
    {
        CurrentCherry = Instantiate(Cherry, Vector3.zero, Quaternion.identity);
        time = Time.time;
        Destroy(CurrentCherry, 10f);
    }
}
