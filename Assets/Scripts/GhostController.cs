﻿/*
    Copyright 2020 :: Michael Wu

    Redistribution and use in source and binary forms, with or without modification,
    are permitted provided that the following conditions are met:

	1.Redistributions or derivations of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.

	2. Redistributions or derivative works in binary form must reproduce the above
	copyright notice. This list of conditions and the following	disclaimer must be
	reproduced in the documentation and/or other materials provided with the distribution.

	3. Neither the name of the copyright holder nor the names of its contributors may
	be used to endorse or promote products derived from this software without specific
	prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS	"AS IS" AND ANY
	EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
	OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
	SHALL THE COPYRIGHT	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
    INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
    BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
	CONTRACT, STRICT LIABILITY, OR TORT	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
	ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	SUCH DAMAGE.

	Links
	~~~~~
	GitHub:     https://github.com/WichaelMu/
    Itch.io:    https://wichael-mu.itch.io/

*/

using UnityEngine;

public class GhostController : MonoBehaviour
{
    /// <summary>
    /// The layer where the walls are situated.
    /// </summary>
    public LayerMask Walls;
    /// <summary>
    /// The transform of Pac Man.
    /// </summary>
    public Transform PacMan;

    /// <summary>
    /// The GhostID for 60%: P. This Ghost's World Space labelling.
    /// </summary>
    [Header("The Ghost Canvas Number.")]
    public int GhostID; //  The ID of the Ghosts that correspond to their World Space Canvas.

    /// <summary>
    /// The default position and respawn point for this Ghost.
    /// </summary>
    Transform RespawnPoint; //  The respawn point for this Ghost.

    GhostMechanics Mechanics;

    /// <summary>
    /// The Vector3 directions. [0] Up, [1] Down, [2] Left, [3] Right.
    /// </summary>
    readonly Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), }; //  The up, down, left and right directions in their Vector3 equivalents.
    /// <summary>
    /// The opposite direction of this Ghost.
    /// </summary>
    Vector3 opposite;

    /// <summary>
    /// The current horizontal direction of this Ghost.
    /// </summary>
    float hCurrent; //  The current horizontal direction.
    /// <summary>
    /// The current vertical direction of this Ghost.
    /// </summary>
    float vCurrent; //  The current vertical direction.
    /// <summary>
    /// The current direction of this Ghost.
    /// </summary>
    string dCurrent = null; //  The current direction as a string, "U", "D", "L" and "R".
    
    void Start()
    {
        Mechanics = GetComponent<GhostMechanics>();

        if (!Mechanics)
            Debug.LogWarning("Ghost with ID: " + GhostID + " does not have a <GhostMecahnics> component!");
        else
            RespawnPoint = Mechanics.RespawnPoint; //  Assigns this GhostController's respawn point to the same one in this Ghost's GhostMechanics.


        StartSequence();    //  Begins the starting sequence to begin/restart the game.
    }

    void FixedUpdate()
    {
        MoveDirection(hCurrent, vCurrent);  //  By default, this will move the Ghosts upwards at the beginning/restart of the game. It will move down if this is the Light Blue Ghost.
    }

    void OnTriggerEnter(Collider o)
    {
        if (o.CompareTag("Switcher"))   //  If this Ghost hits a switcher.
            ArtificialIntelligence(o.gameObject.GetComponent<Switcher>());  //  It will begin the process of Artificial Intelligence.
    }

    #region Artificial Intelligence for the Ghosts

    /// <summary>
    /// Begins the Artifical Intelligence for each of the Ghosts, corresponding to their set ID.
    /// </summary>
    /// <param name="switcher">The switcher that triggered this Ghost's Artifical Intelligence. It is default is null.</param>

    void ArtificialIntelligence(Switcher switcher)
    {
        if (OrderNo227(switcher))
            return;

        if (GhostID == 1)   //  If this Ghost is Red.
            RedGhostAI(switcher); //  Do the Artificial Intelligence for the Red Ghost.
        else if (GhostID == 2)   //  If this Ghost is Pink.
            PinkGhostAI(switcher);    //  Do the Artificial Intelligence for the Pink Ghost.
        else if (GhostID == 3)   //  If this Ghost is Orange.
            OrangeGhostAI(switcher);    //  Do the Artificial Intelligence for the Orange Ghost.
        else if (GhostID == 4)   //  If this Ghost is Light Blue.
            LightBlueGhostAI(); //  Do the Artificial Intelligence for the Light Blue Ghost.
    }

    /// <summary>
    /// Moves the Red Ghost in a direction where the distance to the next Collider is greater than or equal to Pac Man's distance to the Red Ghost.
    /// </summary>
    /// <param name="switcher">The Switcher this Red Ghost is currently on.</param>

    void RedGhostAI(Switcher switcher)
    {
        //float PacManColliderDistance;
        bool found = false; //  If a valid path was found.
        Vector3 LongestDirection = directions[0];   //  By default, set the longest found direction to up.

        for (int i = 0; i < directions.Length; i++)
        {
            //  Get information about a raycast hit on any wall in the directions up, down, left and right.
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            if (hit.transform == null)
                continue;
            //  If the raycast distance is greater than the current maximum distance, the distance of the raycast is greater than .5, i.e., do not count this direction if this Ghost is directly facing a wall in that direction, if the current switcher allows a movement in this direction, this Ghost's distance to the Collider is greater than Pac Man's distance to this Ghost, and if this direction is not the current opposite direction.
            if (switcher.allowDirection(directions[i]) && (hit.distance <= GetColliderPacManDistance(hit.transform.position)) && (directions[i] != opposite) && hit.distance != Mathf.Infinity)
            {
                LongestDirection = directions[i];   //  Set the longest direction to this raycast's direction.
                found = true;   //  A path was found.
            }
        }

        if (found)  //  If a path was found.
            MoveDirection(LongestDirection);    //  Move in this direction.
        else
            MoveRandomly(switcher); //  Move randomly.
    }

    /// <summary>
    /// Moves the Pink Ghost in a direction where the distance from the next Collider's position is closer than or equal to Pac Man's position than the Pink Ghost is to Pac Man's position.
    /// </summary>
    /// <param name="switcher">The Switcher this Pink Ghost is currently on.</param>

    void PinkGhostAI(Switcher switcher)
    {
        float PacManColliderDistance;   //   The distance between Pac Man and the collider.
        float distance = Mathf.Infinity;    //  The minimum distance to be compared to when finding the shortest distance to Pac Man.
        bool found = false; //  If a valid path was found.
        //Vector3[] AllowedDirections = new[] { Vector3.right, -Vector3.right, -Vector3.up, Vector3.up, };
        Vector3 ShortestDirection = directions[0];  //  By default, set the shortest found direction to go up.

        for (int i = 0; i < directions.Length; i++)
        {
            //  Get information about a raycast hit on any wall in the directions up, down, left and right.
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            if (hit.transform == null)
                continue;
            PacManColliderDistance = GetColliderPacManDistance(hit.collider.transform.position);
            //  If the raycast distance is greater than the current minimum distance, the distance of the raycast is greater than .5, i.e., do not count this direction if this Ghost is directly facing a wall in that direction, if the current switcher allows a movement in this direction, this Ghost's distance to Pac Man is greater is greater than Pac Man's distance to the Collider, and if this direction is not the current opposite direction.
            if (((hit.distance < distance) && (hit.distance > .05f) && switcher.allowDirection(directions[i]) && (PacManColliderDistance <= hit.distance) && (directions[i] != opposite) && (hit.distance != Mathf.Infinity)))
            {
                distance = hit.distance;    //  Set the minimum distance to this raycast.
                ShortestDirection = directions[i];  //  Set the shortest direction to this raycast's direction.
                found = true;   //  A path was found.
            }
        }

        if (found)  //  If a path was found.
            MoveDirection(ShortestDirection);   //  Move in this direction.
        else
            MoveRandomly(switcher); //  Move randomly.
    }

    /// <summary>
    /// Moves the Orange Ghost in a random valid direction.
    /// </summary>
    /// <param name="switcher"></param>

    void OrangeGhostAI(Switcher switcher)
    {
        MoveRandomly(switcher);
    }

    //  A constant string that defines the movememt for the Light Blue Ghost to loop around the perimeter of the level in a clockwise rotation from the spawning position.
    readonly string[] strict = new[] { "D", "R", "D", "L", "D", "U", "L", "D", "L", "L", "U", "U", "R", "U", "U", "L", "U", "U", "R", "R", "D", "R", "U", "R", "R", "D", "D", "L", "D", "D", "R", "D", "D", "L", "L", "L", };
    int LBM = 0;    //  Light Blue Movement index.

    /// <summary>
    /// Moves the Light Blue Ghost in a set of pre-defined directions that take it clockwise around the level.
    /// </summary>

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

    float t;

    /// <summary>
    /// Moves this Ghost using float values horizontal and vertical by lerping.
    /// </summary>
    /// <param name="horizontal">Dictates a left or right movement.</param>
    /// <param name="vertical">Dictates an up or down movement.</param>

    void MoveDirection(float horizontal, float vertical)
    {
        t += Time.fixedDeltaTime;

        if (enabled)    //  If GhostController.cs is enabled.
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + horizontal, transform.position.y + vertical, transform.position.z), t / (LevelGenerator.PIXEL_32 / (Mechanics.MoveSpeed * .35f)));

            hCurrent = horizontal;  //  Set the current horizontal movement to horizontal.
            vCurrent = vertical;    //  Set the current vertical movement to vertical.
        }

        t = 0;
    }

    /// <summary>
    /// Moves this Ghost using a Vector3 direction by lerping.
    /// </summary>
    /// <param name="direction">The normalized Vector3 direction.</param>

    void MoveDirection(Vector3 direction)
    {
        t += Time.fixedDeltaTime;

        if (enabled)    //  If GhostController.cs is enabled.
        {
            //GhostRB.MovePosition(transform.position + direction * GetComponent<GhostMechanics>().MoveSpeed * Time.deltaTime);   //  Move at a constant rate towards Vector3 direction at GhostMechanics MoveSpeed.

            string s = ConvertVectorDirectionToStringDirection(direction);

            float UD = 0f, LR = 0f;
            switch (s)
            {
                case "U":
                    UD = .5f;
                    LR = 0f;
                    break;
                case "D":
                    UD = -.5f;
                    LR = 0f;
                    break;
                case "L":
                    LR = -.5f;
                    UD = 0f;
                    break;
                case "R":
                    LR = .5f;
                    UD = 0f;
                    break;
            }

            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + LR, transform.position.y + UD, transform.position.z), t / (LevelGenerator.PIXEL_32 / (Mechanics.MoveSpeed * .35f)));

            hCurrent = direction.x; //  Set the current horizontal movement to the x value of direction.
            vCurrent = direction.y; //  Set the current vertical movement to the y value of direction.

            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == direction) //  If the directions[i] is equal to the requested direction.
                    dCurrent = ConvertVectorDirectionToStringDirection(directions[i]);  //  Set the current direction to this Vector3 direction.
            opposite = GetOppositeDirection();
        }

        t = 0;
    }

    /// <summary>
    /// Moves this Ghost in the only allowed direction if there are only two possible directions.
    /// Does not allow backwards movement; NOT ONE STEP BACK!
    /// </summary>
    /// <param name="switcher"></param>
    /// <returns>True if there are only two possible directions.</returns>

    bool OrderNo227(Switcher switcher)
    {
        if (switcher.NumberOfAllowedDirections() == 1)
        {
            string s = switcher.MoveRandom();
            while (ConvertStringDirectionToVectorDirection(s) == opposite)
                s = switcher.MoveRandom();

            MoveDirection(ConvertStringDirectionToVectorDirection(s));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Moves in a random valid direction that is not the opposite direction.
    /// </summary>
    /// <param name="switcher">The switcher this Ghost is currently on.</param>

    void MoveRandomly(Switcher switcher)
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

        //string s;
        //while ((s = switcher.MoveRandom()) != null) //  This will always return true.
        //{
        //    if (switcher.allowDirection(s) && ConvertStringDirectionToVectorDirection(s) != opposite)   //  If the random direction is allowed by switcher and that this direction is not the opposite direction.
        //    {
        //        Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(ConvertStringDirectionToVectorDirection(s)), out RaycastHit hit, Mathf.Infinity, Walls);
        //        if (hit.distance != Mathf.Infinity && hit.distance > .05f)
        //        {
        //            MoveDirection(ConvertStringDirectionToVectorDirection(s));
        //            return;
        //        }
        //    }
        //}
    }

    #region Get Methods

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

    #region Conversions

    /// <summary>
    /// Converts the string direction 's' to a Vector3 direction.
    /// </summary>
    /// <param name="s">The string direction needed to oppose.</param>
    /// <returns>The equivalent Vector3 direction of string s.</returns>

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
    /// <returns>The equivalent string direction of Vector3 v.</returns>

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

    #endregion

    #endregion

    #region Reset
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

    #endregion

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
        vCurrent = .5f;
        dCurrent = "U";
    }
    void D()
    {
        hCurrent = 0;
        vCurrent = -.5f;
        dCurrent = "D";
    }
    void L()
    {
        hCurrent = -.5f;
        vCurrent = 0;
        dCurrent = "L";
    }
    void R()
    {
        hCurrent = .5f;
        vCurrent = 0;
        dCurrent = "R";
    }

    #endregion
}
