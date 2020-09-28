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