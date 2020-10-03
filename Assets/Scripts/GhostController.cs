using UnityEngine;

public class GhostController : MonoBehaviour
{

    public LayerMask Walls;
    public Transform PacMan;

    [Header("The Ghost Canvas Number.")]
    public int GhostID;

    Rigidbody GhostRB;
    
    void Start()
    {
        GhostRB = GetComponent<Rigidbody>();

        ArtificialIntelligence();
    }

    void OnTriggerEnter(Collider o)
    {
        if (o.CompareTag("Switcher"))
            ArtificialIntelligence(o.gameObject.GetComponent<Switcher>());
    }

    #region Artificial Intelligence for the Ghosts

    void ArtificialIntelligence(Switcher switcher = null)
    {
        //  Perform a Raycast if this Ghost is Red or Pink.
        if (GhostID < 3 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(Vector3.up), out RaycastHit hit, Mathf.Infinity, Walls))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.white);
            Debug.Log(name + "'s distance from " + hit.collider.gameObject.name + " is: " + hit.distance);
            ArtificialIntelligence(hit.distance, GetPacManGhostDistance(transform.position), switcher);
        }
    }

    /// <summary>
    /// Begins the Artifical Intelligence for the Ghosts.
    /// </summary>
    /// <param name="distance">The distance between this Ghost and the Collider in this Ghost's direction of travel.</param>
    /// <param name="PacManDistance">The disance between this Ghost and Pac Man.</param>
    /// <param name="switcher">The switcher that triggered this Ghost's Artifical Intelligence. It is default is null.</param>

    void ArtificialIntelligence(float distance, float PacManDistance, Switcher switcher = null)
    {
        switch (GhostID)
        {
            case 1:
                RedGhostAI(distance, PacManDistance, switcher);
                break;
            case 2:
                PinkGhostAI(distance, PacManDistance, switcher);
                break;
            case 3:
                OrangeGhostAI(distance, PacManDistance, switcher);
                break;
            case 4:
                LightBlueGhostAI(distance, PacManDistance, switcher);
                break;
            default:
                Debug.LogWarning("Incorrect Ghost ID on " + name);
                break;
        }
    }

    /// <summary>
    /// Pac Man's distance from the Ghost.
    /// </summary>
    /// <param name="position">The Ghost's Vector3 position.</param>
    /// <returns>The float distance between the Ghost and Pac Man.</returns>

    float GetPacManGhostDistance(Vector3 position)
    {
        return Vector3.Distance(PacMan.position, position);
    }

    void RedGhostAI(float distance, float PacManDistance, Switcher switcher = null)
    {
        float _nextDistance = 0f;
        if (switcher != null && distance >= PacManDistance)   //  If the switcher is not null and the distance is greater than or equal to Pac Man's distance.
            for (int h = -1; h < 2; h++)    //  The horitzontal direction.
                for (int v = -1; v < 2; v++)    //  The vertical direction.
                    if (Physics.Raycast(transform.position, new Vector3(h, v, 0f).normalized, out RaycastHit hit, Mathf.Infinity, Walls))
                        if (hit.distance > _nextDistance)
                            if (switcher.allowDirection(h, v))
                                MoveDirection(h, v);

    }

    void PinkGhostAI(float distance, float PacManDistance, Switcher switcher = null)
    {
        
    }

    void OrangeGhostAI(float distance, float PacManDistance, Switcher switcher = null)
    {
        
    }

    void LightBlueGhostAI(float distance, float PacManDistance, Switcher switcher = null)
    {
        
    }

    void MoveDirection(int h, int v)
    {

    }

    #endregion
}
