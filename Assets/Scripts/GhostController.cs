using System;
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

        StartSequence();
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
        if (GhostID == 3)
            OrangeGhostAI(switcher);
        if (GhostID == 4)
            LightBlueGhostAI();
    }

    /// <summary>
    /// Begins the Artifical Intelligence for each of the Ghosts, corresponding to their set ID.
    /// </summary>
    /// <param name="distance">The distance between this Ghost and the Collider in this Ghost's direction of travel.</param>
    /// <param name="PacManDistance">The disance between this Ghost and Pac Man.</param>
    /// <param name="switcher">The switcher that triggered this Ghost's Artifical Intelligence. It is default is null.</param>

    void ArtificialIntelligence(Transform Collider, Switcher switcher)
    {
        if (GhostID == 1)
            RedGhostAI(Collider, switcher);
        if (GhostID == 2)
            PinkGhostAI(Collider, switcher);
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
    /// <param name="position">The Vector3 position of the Collider.</param>
    /// <returns>The float distance between Pac Man and the Collider.</returns>

    float GetColliderPacManDistance(Vector3 position)
    {
        return Vector3.Distance(PacMan.position, position);
    }

    /// <summary>
    /// The distance between this Ghost and Pac Man.
    /// </summary>
    /// <returns>The fliat distance between this Ghost and Pac Man.</returns>

    float GetGhostPacManDistance()
    {
        return Vector3.Distance(transform.position, PacMan.position);
    }

    /// <summary>
    /// Moves the Red Ghost in a direction where the distance to the next Collider is greater than or equal to Pac Man's distance to the Red Ghost.
    /// </summary>
    /// <param name="Collider">The Collider this Red Ghost is going towards.</param>
    /// <param name="switcher">The Switcher this Red Ghost is currently on.</param>

    void RedGhostAI(Transform Collider, Switcher switcher)
    {
        float GhostColliderDistance = GetColliderGhostDistance(Collider.position);
        float PacManColliderDistance = GetColliderPacManDistance(Collider.position);
        float distance = 0f;
        bool found = false;
        Vector3 LongestDirection = directions[0];
        Vector3 opposite = GetOppositeDirection();

        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            if (hit.distance > distance && hit.distance > .05f && switcher.allowDirection(directions[i]) && (GhostColliderDistance >= GetGhostPacManDistance()))
            {
                distance = hit.distance;
                if (directions[i] != opposite)
                    LongestDirection = directions[i];
                found = true;
            }
        }

        if (found)
            MoveDirection(LongestDirection);
        else
        {
            while (true)
            {
                string s = switcher.MoveRandom();
                if (switcher.allowDirection(s) && ConvertStringDirectionToVectorDirection(s) != GetOppositeDirection())
                {
                    Invoke(s, 0f);
                    return;
                }
            }
        }

        //if (GhostColliderDistance >= GetGhostPacManDistance(PacMan.position))
        //    MoveDirection(LongestDirection);
        //else
        //    Invoke(switcher.MoveRandom(), 0f);
    }

    /// <summary>
    /// Moves the Pink Ghost in a direction where the distance from the next Collider's position is closer than or equal to Pac Man's position than the Pink Ghost is to Pac Man's position.
    /// </summary>
    /// <param name="Collider">The Collider this Pink Ghost is going towards.</param>
    /// <param name="switcher">The Switcher this Pink Ghost is currently on.</param>

    void PinkGhostAI(Transform Collider, Switcher switcher)
    {
        float GhostPacManDistance = GetGhostPacManDistance();
        float PacManColliderDistance = GetColliderPacManDistance(Collider.position);
        float distance = Mathf.Infinity;
        bool found = false;
        //Vector3[] AllowedDirections = new[] { Vector3.right, -Vector3.right, -Vector3.up, Vector3.up, };
        Vector3 ShortestDirection = directions[0];
        Vector3 opposite = GetOppositeDirection();

        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            if (hit.distance < distance && hit.distance > .05f && switcher.allowDirection(directions[i]) && (GhostPacManDistance >= PacManColliderDistance))
            {
                distance = hit.distance;
                if (directions[i] != opposite)
                    ShortestDirection = directions[i];
                found = true;
            }
        }

        if (found)
            MoveDirection(ShortestDirection);
        else
        {
            int i = 0;
            while (i < 4)
            {
                string s = switcher.MoveRandom();
                if (switcher.allowDirection(s) && ConvertStringDirectionToVectorDirection(s) != GetOppositeDirection())
                {
                    Invoke(s, 0f);
                    return;
                }
                i++;
            }
            Invoke(switcher.MoveRandom(), 0f);
        }

        //Array.Reverse(AllowedDirections);
        //for (int i = 0; i < AllowedDirections.Length; i++)
        //    if (Vector3.Distance(transform.position, PacMan.position) >= PacManColliderDistance && switcher.allowDirection(AllowedDirections[i]))
        //    {
        //        MoveDirection(AllowedDirections[i]);
        //        Debug.Log(name + " did not move randomly");
        //    }
        //    else
        //    {
        //        Invoke(switcher.MoveRandom(), 0f);
        //        Debug.Log(name + " moved randomly");
        //    }
    }

    void OrangeGhostAI(Switcher switcher)
    {
        while (true)
        {
            string s = switcher.MoveRandom();
            if (ConvertStringDirectionToVectorDirection(s) != GetOppositeDirection())
            {
                Invoke(s, 0f);
                return;
            }
        }
    }

    string[] strict = new[] { "D", "R", "D", "L", "D", "U", "L", "D", "L", "L", "U", "U", "R", "U", "U", "L", "U", "U", "R", "R", "D", "R", "U", "R", "R", "D", "D", "L", "D", "D", "R", "D", "D", "L", "L", "L", };
    int LBM = 0;

    void LightBlueGhostAI()
    {
        #region Light Blue's Strict Movement
        //  The Light Blue Ghost needs to move:
        /*
         * Left
         * Down
         * Right
         * Down
         * Left
         * Down

         * LOOP HERE

         * Up
         * Left
         * Down
         * Left
         * Left
         * Up
         * Up
         * Right
         * Up
         * Up
         * Left
         * Up
         * Up
         * Right
         * Right
         * Down
         * Right
         * Up
         * 
         * Right
         * Right
         * Down
         * Down
         * Left
         * Down
         * Down
         * Right
         * Down
         * Down
         * Left
         * Left
         */
        #endregion
        Invoke(strict[LBM], 0f);
        LBM++;
        if (LBM == strict.Length - 1)
            LBM = 5;
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

            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == direction)
                    dCurrent = ConvertVectorDirectionToStringDirection(directions[i]);
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

    /// <summary>
    /// Gets the opposite direction of this Ghost as a Vector3.
    /// </summary>
    /// <returns>Vector3 representing the opposite moving direction of this Ghost.</returns>

    Vector3 GetOppositeDirection()
    {
        return GetCurrentDirection() * -1f;
    }

    /// <summary>
    /// Converts the string direction 's' to a Vector3 direction.
    /// </summary>
    /// <param name="s">The string direction needed to oppose.</param>
    /// <returns>The equivalent Vector3 direction of 's'.</returns>

    Vector3 ConvertStringDirectionToVectorDirection(string s)
    {
        switch (s)
        {
            case "U":
                return directions[0];
            case "D":
                return directions[1];
            case "L":
                return directions[2];
            case "R":
                return directions[3];
        }
        Debug.LogWarning("Cannot convert string " + s + " to Vector3.");
        return Vector3.zero;
    }

    /// <summary>
    /// Converts the Vector3 direction 'v' to a string direction.
    /// </summary>
    /// <param name="v">The Vector3 direction needed to oppose.</param>
    /// <returns>The equivalent string direction of 'v'.</returns>

    string ConvertVectorDirectionToStringDirection(Vector3 v)
    {
        if (v == new Vector3(0f, 1f, 0f))
            return "U";
        if (v == new Vector3(0f, -1f, 0f))
            return "D";
        if (v == new Vector3(-1f, 0f, 0f))
            return "L";
        if (v == new Vector3(1f, 0f, 0f))
            return "R";
        Debug.LogWarning("Unable to convert Vector3 " + v + " to string.");
        return null;
    }

    public void ResetPositions()
    {
        transform.position = RespawnPoint.position;
        StartSequence();
    }

    void StartSequence()
    {
        U();
        if (GhostID == 4)
            D();
        LBM = 0;
    }

    public void ResetLBM()
    {
        LBM = 0;
    }

    #region The Orientations of the Ghosts.

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

    #endregion
}
