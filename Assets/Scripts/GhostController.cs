using UnityEngine;

public class GhostController : MonoBehaviour
{
    public LayerMask Walls; //  The layer the walls are situated.
    public Transform PacMan;    //  The transform of Pac Man.

    [Header("The Ghost Canvas Number.")]
    public int GhostID; //  The ID of the Ghosts that correspond to their World Space Canvas.

    Rigidbody GhostRB;
    Transform RespawnPoint; //  The respawn point for this Ghost.

    readonly Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), }; //  The up, down, left and right directions in their Vector3 equivalents.
    Vector3 opposite;

    float hCurrent; //  The current horizontal direction.
    float vCurrent; //  The current vertical direction.
    string dCurrent = null; //  The current direction as a string, "U", "D", "L" and "R".
    
    void Start()
    {
        GhostRB = GetComponent<Rigidbody>();
        RespawnPoint = GetComponent<GhostMechanics>().RespawnPoint; //  Assigns this GhostController's respawn point to the same one in this Ghost's GhostMechanics.

        StartSequence();    //  Begins the starting sequence to begin/restart the game.
    }

    void FixedUpdate()
    {
        MoveDirection(hCurrent, vCurrent);  //  By default, this will move the Ghosts upwards at the beginning/restart of the game. It will move down if this is the Light Blue Ghost.
        //if (GhostID < 3 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(GetCurrentDirection()), out RaycastHit hit, Mathf.Infinity, Walls))
        //{
        //    Debug.DrawLine(transform.position, hit.point, Color.white);
        //}
    }

    void OnTriggerEnter(Collider o)
    {
        if (o.CompareTag("Switcher"))   //  If this Ghost hits a switcher.
            ArtificialIntelligence(o.gameObject.GetComponent<Switcher>());  //  It will begin the process of Artificial Intelligence.
    }

    #region Artificial Intelligence for the Ghosts

    void ArtificialIntelligence(Switcher switcher)
    {
        //  If the ID of the Ghost is 1 or 2, i.e., the Red Ghost or the Pink Ghost, they will perform a raycast in the direction they are facing.
        if (GhostID < 3 && Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(GetCurrentDirection()), out RaycastHit hit, Mathf.Infinity, Walls))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.white);
            //Debug.Log(name + "'s distance from " + hit.collider.gameObject.name + " is: " + hit.distance);
            ArtificialIntelligence(hit.collider.transform, switcher);   //  If the raycast hits something, begin the Artificial Intelilgence for the Red Ghost and the Pink Ghost.
        }
        if (GhostID == 3)   //  If this Ghost is Orange.
            OrangeGhostAI(switcher);    //  Do the Artificial Intelligence for the Orange Ghost.
        if (GhostID == 4)   //  If this Ghost is Light Blue.
            LightBlueGhostAI(); //  Do the Artificial Intelligence for the Light Blue Ghost.
    }

    /// <summary>
    /// Begins the Artifical Intelligence for each of the Ghosts, corresponding to their set ID.
    /// </summary>
    /// <param name="distance">The distance between this Ghost and the Collider in this Ghost's direction of travel.</param>
    /// <param name="PacManDistance">The disance between this Ghost and Pac Man.</param>
    /// <param name="switcher">The switcher that triggered this Ghost's Artifical Intelligence. It is default is null.</param>

    void ArtificialIntelligence(Transform Collider, Switcher switcher)
    {
        if (GhostID == 1)   //  If this Ghost is Red.
            RedGhostAI(Collider, switcher); //  Do the Artificial Intelligence for the Red Ghost.
        if (GhostID == 2)   //  If this Ghost is Pink.
            PinkGhostAI(Collider, switcher);    //  Do the Artificial Intelligence for the Pink Ghost.
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
    /// Moves the Red Ghost in a direction where the distance to the next Collider is greater than or equal to Pac Man's distance to the Red Ghost.
    /// </summary>
    /// <param name="Collider">The Collider this Red Ghost is going towards.</param>
    /// <param name="switcher">The Switcher this Red Ghost is currently on.</param>

    void RedGhostAI(Transform Collider, Switcher switcher)
    {
        float GhostColliderDistance = GetColliderGhostDistance(Collider.position);
        float distance = 0f;    //  The maximum distance to compare to when finding the longest distance to the next Collider.
        bool found = false; //  If a valid path was found.
        Vector3 LongestDirection = directions[0];   //  By default, set the longest found direction to up.
        Vector3 opposite = GetOppositeDirection();  //  Gets the opposite direction.

        for (int i = 0; i < directions.Length; i++)
        {
            //  Get information about a raycast hit on any wall in the directions up, down, left and right.
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            //  If the raycast distance is greater than the current maximum distance, the distance of the raycast is greater than .5, i.e., do not count this direction if this Ghost is directly facing a wall in that direction, if the current switcher allows a movement in this direction, this Ghost's distance to the Collider is greater than Pac Man's distance to this Ghost, and if this direction is not the current opposite direction.
            if (hit.distance > distance && hit.distance > .05f && switcher.allowDirection(directions[i]) && (GhostColliderDistance >= GetGhostPacManDistance() && directions[i] != opposite) && hit.distance != Mathf.Infinity)
            {
                distance = hit.distance;    //  Set the maximum distance to this raycast.
                LongestDirection = directions[i];   //  Set the longest direction to this raycast's direction.
                found = true;   //  A path was found.
            }
        }

        if (found)  //  If a path was found.
            MoveDirection(LongestDirection);    //  Move in this direction.
        else
        {
            for (int i = 0; i < directions.Length; i++)
                if (directions[i] != opposite)
                {
                    Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
                    if (hit.distance != Mathf.Infinity && hit.distance > .05f)
                        if (switcher.allowDirection(directions[i]))
                        {
                            MoveDirection(directions[i]);
                            return;
                        }
                }
        }
        #region ignore

        //if (GhostColliderDistance >= GetGhostPacManDistance(PacMan.position))
        //    MoveDirection(LongestDirection);
        //else
        //    Invoke(switcher.MoveRandom(), 0f);

        #endregion
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
        float distance = Mathf.Infinity;    //  The minimum distance to be compared to when finding the shortest distance to Pac Man.
        bool found = false; //  If a valid path was found.
        //Vector3[] AllowedDirections = new[] { Vector3.right, -Vector3.right, -Vector3.up, Vector3.up, };
        Vector3 ShortestDirection = directions[0];  //  By default, set the shortest found direction to go up.

        for (int i = 0; i < directions.Length; i++)
        {
            //  Get information about a raycast hit on any wall in the directions up, down, left and right.
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            //  If the raycast distance is greater than the current minimum distance, the distance of the raycast is greater than .5, i.e., do not count this direction if this Ghost is directly facing a wall in that direction, if the current switcher allows a movement in this direction, this Ghost's distance to Pac Man is greater is greater than Pac Man's distance to the Collider, and if this direction is not the current opposite direction.
            if ((hit.distance < distance) && (hit.distance > .05f) && switcher.allowDirection(directions[i]) && (GhostPacManDistance >= PacManColliderDistance) && (directions[i] != opposite) && (hit.distance != Mathf.Infinity))
            {
                distance = hit.distance;    //  Set the minimum distance to this raycast.
                ShortestDirection = directions[i];  //  Set the shortest direction to this raycast's direction.
                found = true;   //  A path was found.
            }
        }

        if (found)  //  If a path was found.
            MoveDirection(ShortestDirection);   //  Move in this direction.
        else
            for (int i = 0; i < directions.Length; i++)
                if (directions[i] != opposite)
                {
                    Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
                    if (hit.distance != Mathf.Infinity && hit.distance > .05f)
                        if (switcher.allowDirection(directions[i]))
                        {
                            MoveDirection(directions[i]);
                            return;
                        }
                }

        #region ignore
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
        #endregion
    }

    void OrangeGhostAI(Switcher switcher)
    {
        int i = 0;
        while (true)
        {
            string s = switcher.MoveRandom();   //  Get a valid random movement from the switcher.
            if (ConvertStringDirectionToVectorDirection(s) != GetOppositeDirection() && Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls)) //  If this random movement is allowed and is not the opposite direction.
            {
                Invoke(s, 0f);  //  Move in this direction.
                return;
            }
            i++;
            i %= 4;
        }
    }

    //  A constant string that defines the movememt for the Light Blue Ghost to loop around the perimeter of the level in a clockwise rotation from the spawning position.
    readonly string[] strict = new[] { "D", "R", "D", "L", "D", "U", "L", "D", "L", "L", "U", "U", "R", "U", "U", "L", "U", "U", "R", "R", "D", "R", "U", "R", "R", "D", "D", "L", "D", "D", "R", "D", "D", "L", "L", "L", };
    int LBM = 0;    //  Light Blue Movement index.

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
        Invoke(strict[LBM], 0f);    //  Move in the direction of pre-defined movements at index LBM.
        LBM++;  //  Increment this LBM index.
        if (LBM == strict.Length - 1)   //  If the LBM is at the end of the string loop.
            LBM = 5;    //  Restart at the position where the loop should begin.
    }

    #endregion

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
    /// Moves this Ghost using float values horizontal and vertical.
    /// </summary>
    /// <param name="horizontal">Dictates a left or right movement.</param>
    /// <param name="vertical">Dictates an up or down movement.</param>

    void MoveDirection(float horizontal, float vertical)
    {
        if (enabled)    //  If GhostController.cs is enabled.
        {
            GhostRB.MovePosition(transform.position + new Vector3(horizontal, vertical, 0f) * GetComponent<GhostMechanics>().MoveSpeed * Time.deltaTime);   //  Move at a constant rate towards horizontal or vertical at GhostMechanics MoveSpeed.
            hCurrent = horizontal;  //  Set the current horizontal movement to horizontal.
            vCurrent = vertical;    //  Set the current vertical movement to vertical.
        }
    }

    /// <summary>
    /// Moves this Ghost using a Vector3 direction.
    /// </summary>
    /// <param name="direction">The normalized Vector3 direction.</param>

    void MoveDirection(Vector3 direction)
    {
        if (enabled)    //  If GhostController.cs is enabled.
        {
            GhostRB.MovePosition(transform.position + direction * GetComponent<GhostMechanics>().MoveSpeed * Time.deltaTime);   //  Move at a constant rate towards Vector3 direction at GhostMechanics MoveSpeed.
            hCurrent = direction.x; //  Set the current horizontal movement to the x value of direction.
            vCurrent = direction.y; //  Set the current vertical movement to the y value of direction.

            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == direction) //  If the directions[i] is equal to the requested direction.
                    dCurrent = ConvertVectorDirectionToStringDirection(directions[i]);  //  Set the current direction to this Vector3 direction.

            opposite = GetOppositeDirection();
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
                return new Vector3(0f, 1f, 0f); //  Vector3 up.
            case "D":
                return new Vector3(0f, -1f, 0f);    //  Vector3 down.
            case "L":
                return new Vector3(-1f, 0f, 0f);    //  Vector3 left.
            case "R":
                return new Vector3(1f, 0f, 0f); //  Vector3 right.
            default:
                Debug.LogWarning("Problem with getting the current direction of: " + name);
                return new Vector3(hCurrent, vCurrent, 0f); //  Default Vector3 to continue its current direction.
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
                return directions[0];   //  Vector3 up.
            case "D":
                return directions[1];   //  Vector3 down.
            case "L":
                return directions[2];   //  Vector3 left.
            case "R":
                return directions[3];   //  Vector3 right.
        }
        Debug.LogWarning("Cannot convert string " + s + " to Vector3 for: " + name);
        return Vector3.zero;    //  Conversion failed.
    }

    /// <summary>
    /// Converts the Vector3 direction 'v' to a string direction.
    /// </summary>
    /// <param name="v">The Vector3 direction needed to oppose.</param>
    /// <returns>The equivalent string direction of 'v'.</returns>

    string ConvertVectorDirectionToStringDirection(Vector3 v)
    {
        if (v == new Vector3(0f, 1f, 0f))
            return "U"; //  Up.
        if (v == new Vector3(0f, -1f, 0f))
            return "D"; //  Down.
        if (v == new Vector3(-1f, 0f, 0f))
            return "L"; //  Left.
        if (v == new Vector3(1f, 0f, 0f))
            return "R"; //  Right.
        Debug.LogWarning("Unable to convert Vector3 " + v + " to string for: " + name);
        return null;    //  Conversion failed.
    }

    public void ResetPositions()
    {
        transform.position = RespawnPoint.position; //  Respawn this Ghost at GhostMechanic's respawn point position.
        StartSequence();    //  Re/Start the starting sequence.
    }

    void StartSequence()
    {
        U();    //  Default movement to up.
        if (GhostID == 4)   //  If this Ghost is the Light Blue.
            D();    //  Default movement to downm.
        LBM = 0;    //  Reset the Light Blue Movement index.
    }

    public void ResetLBM()
    {
        LBM = 0;    //  Reset the Light Blue Movement index.
    }

    #region The Orientations of the Ghosts.

    /*
     * void <direction>();
     * horizontal value.
     * vertical value.
     * current direction.
     */

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
