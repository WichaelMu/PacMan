using UnityEngine;

public class CherryController : MonoBehaviour
{
    float time;
    
    void Start()
    {
        InvokeRepeating("Refresh", 0f, 30f);
    }

    void Update()
    {
        float delta = Mathf.Pow(((Time.time - time) / 20f), 3);
        transform.position = Vector3.Lerp(transform.position, new Vector3(-20f, transform.position.y, transform.position.z), delta);
    }

    void Refresh()
    {
        transform.position = new Vector3(20f, 4.387f, -.05f);
        time = Time.time;
    }
}
