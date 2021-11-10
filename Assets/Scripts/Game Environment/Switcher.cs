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

public class Switcher : MonoBehaviour
{
    /// <summary>
    /// If an entity is allowed to move in this direction.
    /// </summary>
    public bool up, down, left, right, allowHorizontal, allowVertical;

    void Awake()
    {
        if (up && down)
            allowVertical = true;

        if (left && right)
            allowHorizontal = true;
    }

    /// <summary>
    /// Asks this switcher if Pac Man can move in this [direction].
    /// </summary>
    /// <param name="direction"></param>
    /// <returns> A Boolean stating if a movement in [direction] is allowed by this swticher.</returns>

    public bool allowDirection(string direction)
    {
        switch (direction)
        {
            case "U":
                if (up)
                    return true;
                return false;
            case "D":
                if (down)
                    return true;
                return false;
            case "L":
                if (left)
                    return true;
                return false;
            case "R":
                if (right)
                    return true;
                return false;
        }

        return false;
    }

    /// <summary>
    /// Returns true if this switcher allows a direction in a Vector3 direction.
    /// </summary>
    /// <param name="direction">The Vector3 direction.</param>
    /// <returns>True if this switcher allows movement in this Vector3 direction.</returns>

    public bool allowDirection (Vector3 direction)
    {
        if (direction.x < 0 && left|| direction.x > 0 && right)
            return true;
        if (direction.y < 0 && down || direction.y > 0 && up)
            return true;
        return false;
    }

    /// <summary>
    /// The number of directions this switcher facilitates.
    /// </summary>
    /// <returns>Int number of allowed directions.</returns>

    public int NumberOfAllowedDirections()
    {
        int c = 0;

        if (up)     c++;
        if (down)   c++;
        if (left)   c++;
        if (right)  c++;
        return c;
    }

    /// <summary>
    /// Moves in a random legal direction.
    /// </summary>
    /// <returns>A random legal string direction.</returns>

    public string MoveRandom()
    {
        int r = Random.Range(0, 3);
        while (true)
        {
            switch (r)
            {
                case 0:
                    if (!up)
                    {
                        r++;
                        r %= 4;
                        continue;
                    }
                    else
                        return "U";
                case 1:
                    if (!down)
                    {
                        r++;
                        r %= 4;
                        continue;
                    }
                    else
                        return "D";
                case 2:
                    if (!left)
                        {
                        r++;
                        r %= 4;
                        continue;
                    }
                    else
                        return "L";
                case 3:
                    if (!right)
                    {
                        r++;
                        r %= 4;
                        continue;
                    }
                    else
                        return "R";
            }
            Debug.LogWarning("Unable to find a valid path");
            return "R"; //  Moves right by default.
        }
    }
}