using UnityEngine;

public class GhostController : MonoBehaviour
{
    public LayerMask Walls;
    public Transform PacMan;

    [Header("The Ghost Canvas Number.")]
    public int GhostID;

    Rigidbody GhostRB;
    Transform RespawnPoint;

    Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), };

    float hCurrent;
    float vCurrent;
    string dCurrent = null;
    
    void Start()
    {
        GhostRB = GetComponent<Rigidbody>();
        RespawnPoint = GetComponent<GhostMechanics>().RespawnPoint;

        U();
    }

    void FixedUpdate()
    {
        MoveDirection(hCurrent, vCurrent);
        //if (GhostID < 3 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(GetCurrentDirection()), out RaycastHit hit, Mathf.Infinity, Walls))
        //{
        //    Debug.DrawLine(transform.position, hit.point, Color.white);
        //}
    }

    void OnTriggerEnter(Collider o)
    {
        if (o.CompareTag("Switcher"))
            ArtificialIntelligence(o.gameObject.GetComponent<Switcher>());
    }

    #region Artificial Intelligence for the Ghosts

    void ArtificialIntelligence(Switcher switcher)
    {
        if (GhostID < 3 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(GetCurrentDirection()), out RaycastHit hit, Mathf.Infinity, Walls))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.white);
            //Debug.Log(name + "'s distance from " + hit.collider.gameObject.name + " is: " + hit.distance);
            ArtificialIntelligence(hit.collider.transform, switcher);
        }
    }

    /// <summary>
    /// Begins the Artifical Intelligence for each of the Ghosts, corresponding to their set ID.
    /// </summary>
    /// <param name="distance">The distance between this Ghost and the Collider in this Ghost's direction of travel.</param>
    /// <param name="PacManDistance">The disance between this Ghost and Pac Man.</param>
    /// <param name="switcher">The switcher that triggered this Ghost's Artifical Intelligence. It is default is null.</param>

    void ArtificialIntelligence(Transform Collider, Switcher switcher)
    {
        switch (GhostID)
        {
            case 1:
                RedGhostAI(Collider, switcher);
                break;
            case 2:
                PinkGhostAI(Collider, switcher);
                break;
            case 3:
                OrangeGhostAI(Collider, switcher);
                break;
            case 4:
                LightBlueGhostAI(Collider, switcher);
                break;
            default:
                Debug.LogWarning("Incorrect Ghost ID on " + name);
                break;
        }
    }

    /// <summary>
    /// Ghost's distance from the Collider.
    /// </summary>
    /// <param name="position">The Vector3 position of the Collider.</param>
    /// <returns>The float distance between the Ghost and the Collider.</returns>

    float GetColliderGhostDistance(Vector3 position)
    {
        return Vector3.Distance(transform.position, position);
    }

    /// <summary>
    /// Pac Man's distance from the collider.
    /// </summary>
    /// <param name="position">The Vecto3 position of the Collider.</param>
    /// <returns>The float distance between Pac Man and the Collider.</returns>

    float GetColliderPacManDistance(Vector3 position)
    {
        return Vector3.Distance(PacMan.position, position);
    }

    void RedGhostAI(Transform Collider, Switcher switcher)
    {
        float GhostColliderDistance = GetColliderGhostDistance(Collider.position);
        float PacManColliderDistance = GetColliderPacManDistance(Collider.position);
        float distance = 0f;
        Vector3 LongestDirection = directions[0];

        if (GhostColliderDistance >= PacManColliderDistance)
            for (int i = 0; i < directions.Length; i++)
            {
                Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
                if (hit.distance > distance)
                {
                    distance = hit.distance;
                    LongestDirection = directions[i];
                }
            }
        else
        {
            Invoke(switcher.MoveRandom(), 0f);
            Debug.Log(name + " moved randomly");
            return;
        }
        MoveDirection(LongestDirection);
        Debug.Log(name + " did not move randomly");
    }

    void PinkGhostAI(Transform Collider, Switcher switcher)
    {
        float GhostColliderDistance = GetColliderGhostDistance(Collider.position);
        float PacManColliderDistance = GetColliderPacManDistance(Collider.position);
        float distance = 0f;
        Vector3 ShortestDirection = directions[0];

        if (GhostColliderDistance >= PacManColliderDistance)
        {
            for (int i = 0; i < directions.Length; i++)
            {
                Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
                if (hit.distance > distance)
                {
                    distance = hit.distance;
                    ShortestDirection = directions[i];
                }
            }
        }
        else
        {
            Invoke(switcher.MoveRandom(), 0f);
            Debug.Log(name + " moved randomly");
            return;
        }
        MoveDirection(ShortestDirection);
        Debug.Log(name + " did not move randomly");
    }

    void OrangeGhostAI(Transform Collider, Switcher switcher)
    {

    }

    void LightBlueGhostAI(Transform Collider, Switcher switcher)
    {

    }

    #endregion

    /// <summary>
    /// Moves this Ghost.
    /// </summary>
    /// <param name="horizontal">Dictates a left or right movement.</param>
    /// <param name="vertical">Dictates an up or down movement.</param>

    void MoveDirection(float horizontal, float vertical)
    {
        if (enabled)
        {
            GhostRB.MovePosition(transform.position + new Vector3(horizontal, vertical, 0f) * GetComponent<GhostMechanics>().MoveSpeed * Time.deltaTime);
            hCurrent = horizontal;
            vCurrent = vertical;
        }
    }

    void MoveDirection(Vector3 direction)
    {
        if (enabled)
        {
            GhostRB.MovePosition(transform.position + direction * GetComponent<GhostMechanics>().MoveSpeed * Time.deltaTime);
            hCurrent = direction.x;
            vCurrent = direction.y;
        }
    }

    /// <summary>
    /// Gets the current facing direction of this Ghost as a Vector3.
    /// </summary>
    /// <returns>A Vector3 represeting the moving direction of this Ghost.</returns>
    
    Vector3 GetCurrentDirection()
    {
        switch (dCurrent)
        {
            case "U":
                return new Vector3(0f, 1f, 0f);
            case "D":
                return new Vector3(0f, -1f, 0f);
            case "L":
                return new Vector3(-1f, 0f, 0f);
            case "R":
                return new Vector3(1f, 0f, 0f);
            default:
                return new Vector3(hCurrent, vCurrent, 0f);
        }
    }

    void U()
    {
        hCurrent = 0;
        vCurrent = 1;
        dCurrent = "U";
    }
    void D()
    {
        hCurrent = 0;
        vCurrent = -1;
        dCurrent = "D";
    }
    void L()
    {
        hCurrent = -1;
        vCurrent = 0;
        dCurrent = "L";
    }
    void R()
    {
        hCurrent = 1;
        vCurrent = 0;
        dCurrent = "R";
    }

    public void ResetPositions()
    {
        transform.position = RespawnPoint.position;
    }
}
