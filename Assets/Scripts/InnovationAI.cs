/*
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

/// <summary>
/// The AI used by the Ghosts in the Innovation Scene.
/// </summary>
public class InnovationAI : MonoBehaviour   //  All Ghosts in the Innovation Scene will use the Ghost 2 AI that chases either Player 1, or Player 2.
{
    public LayerMask Walls; //  The layer the walls are situated.
    public Dynamite Dynamite;

    /// <summary>
    /// The Player this Ghost will chase.
    /// </summary>
    public int SpecifyTarget;

    [SerializeField]
    Transform Target;
    Rigidbody GhostRB;

    readonly Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f), }; //  The up, down, left and right directions in their Vector3 equivalents.
    Vector3 opposite;
    Transform PortalL;
    Transform PortalR;
    bool IsOutOfPortal = true;

    readonly float MoveSpeed = 1.5f;
    float hCurrent; //  The current horizontal direction.
    float vCurrent; //  The current vertical direction.
    string dCurrent = null; //  The current direction as a string, "U", "D", "L" and "R".

    void Start()
    {
        PortalL = GameObject.FindWithTag("PortalL").transform;
        PortalR = GameObject.FindWithTag("PortalR").transform;

        InvokeRepeating("DropDynamite", 5, 15);
        GhostRB = GetComponent<Rigidbody>();
        if (SpecifyTarget == 1)
            Target = GameObject.FindWithTag("RED").transform;
        else if (SpecifyTarget == 2)
            Target = GameObject.FindWithTag("GREEN").transform;

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
        else if (o.CompareTag("PortalL") || o.CompareTag("PortalR"))
            PortalHandler(o.name);
        else if (o.CompareTag("Explosion") || o.CompareTag("ExplosionGHOST"))
            Destroy(gameObject);
    }

    #region Artificial Intelligence for the Ghosts

    /// <summary>
    /// Begins the Artifical Intelligence for each of the Ghosts, corresponding to their set ID.
    /// </summary>
    /// <param name="distance">The distance between this Ghost and the Collider in this Ghost's direction of travel.</param>
    /// <param name="PacManDistance">The disance between this Ghost and Pac Man.</param>
    /// <param name="switcher">The switcher that triggered this Ghost's Artifical Intelligence. It is default is null.</param>

    void ArtificialIntelligence(Switcher switcher)
    {
        float PacManColliderDistance;
        float distance = Mathf.Infinity;    //  The minimum distance to be compared to when finding the shortest distance to Pac Man.
        bool found = false; //  If a valid path was found.
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
            MoveRandomly(switcher);
    }

    #endregion

    /// <summary>
    /// Moves this Ghost using float values horizontal and vertical.
    /// </summary>
    /// <param name="horizontal">Dictates a left or right movement.</param>
    /// <param name="vertical">Dictates an up or down movement.</param>

    void MoveDirection(float horizontal, float vertical)
    {
        if (enabled)    //  If InnovationAI.cs is enabled.
        {
            GhostRB.MovePosition(transform.position + new Vector3(horizontal, vertical, 0f) * 1.5f * Time.deltaTime);   //  Move at a constant rate towards horizontal or vertical at GhostMechanics MoveSpeed.
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
        if (enabled)    //  If InnovationAI.cs is enabled.
        {
            GhostRB.MovePosition(transform.position + direction * MoveSpeed * Time.deltaTime);   //  Move at a constant rate towards Vector3 direction at GhostMechanics MoveSpeed.
            hCurrent = direction.x; //  Set the current horizontal movement to the x value of direction.
            vCurrent = direction.y; //  Set the current vertical movement to the y value of direction.

            for (int i = 0; i < directions.Length; i++)
                if (directions[i] == direction) //  If the directions[i] is equal to the requested direction.
                    dCurrent = ConvertVectorDirectionToStringDirection(directions[i]);  //  Set the current direction to this Vector3 direction.

            opposite = GetOppositeDirection();
        }
    }

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
    }

    void DropDynamite()
    {
        Dynamite dynamite = Instantiate(Dynamite, new Vector3(transform.position.x, transform.position.y, -.05f), transform.rotation);
        dynamite.SetMaximumRange(1);
    }

    void PortalHandler(string Portal)   //  There is a better way in doing this. In free-time, find a solution.
    {
        if (Portal.Equals("PortalL(Clone)") && IsOutOfPortal)   //  If PacMan goes into the Left Portal and Pac Man is, in fact, out of any Portal, it will move Pac Man's position to the Right Portal.
            transform.position = PortalR.position;
        else if (Portal.Equals("PortalR(Clone)") && IsOutOfPortal)   //  If PacMan goes into the Right Portal and Pac Man is, in fact, out of any Portal, it will move Pac Man's position to the Left Portal.
            transform.position = PortalL.position;
        IsOutOfPortal = !IsOutOfPortal; //  Flip if Pac Man is out of a Portal. Pac Man will hit the opposite Portal when leaving the first. It will flip twice during one Portal movement. This is needed as, without it, Pac Man will endlessly go back and forth an infinite number of times.
    }

    #region Get Methods

    /// <summary>
    /// Pac Man's distance from the collider.
    /// </summary>
    /// <param name="position">The Vector3 position of the Collider.</param>
    /// <returns>The float distance between Pac Man and the Collider.</returns>

    float GetColliderPacManDistance(Vector3 position)
    {
        return Vector3.Distance(Target.position, position);
    }

    /// <summary>
    /// The distance between this Ghost and Pac Man.
    /// </summary>
    /// <returns>The fliat distance between this Ghost and Pac Man.</returns>

    float GetGhostPacManDistance()
    {
        return Vector3.Distance(transform.position, Target.position);
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

    #endregion

    #endregion

    #region Reset

    void StartSequence()
    {
        U();    //  Default movement to up.
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
