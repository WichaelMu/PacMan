using UnityEngine;

public class Switcher : MonoBehaviour
{

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
    /// Returns true if this switcher allows a direction in a horizontal or vertical movement.
    /// </summary>
    /// <param name="h">The horizontal direction. -1 is Left, 1 is Right.</param>
    /// <param name="v">The vertical direction. -1 is Down, 1 is Up.</param>
    /// <returns>True if this switcher allows movement in this horizontal or vertical direction.</returns>

    public bool allowDirection (int h, int v)
    {
        if (h == 0 && v == 0)
            return false;
        if (h < 0 && left || h > 0 && right || v < 0 && down || v > 0 && up)
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
}