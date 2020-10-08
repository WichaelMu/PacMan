using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public LayerMask Walls;
    public GameObject ExplodeParticle;

    Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), }; //  The up, down, left and right directions in their Vector3 equivalents.

    void Start()
    {
        Invoke("Explode", 4f);
    }

    void Explode()
    {
        Vector3[] ExplodeDirections = new[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, };
        float[] ExplodeDistances = new[] { -1f, -1f, -1f, -1f };
        int VIndex = 0;

        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 0f), transform.TransformDirection(directions[i]), out RaycastHit hit, 7f, Walls);
            if (hit.distance > .1f)
            {
                ExplodeDirections[VIndex] = directions[i];
                ExplodeDistances[i] = hit.distance;
                VIndex++;
            }
        }

        for (int i = 0; i < ExplodeDirections.Length; i++)
            if (ExplodeDirections[i] != Vector3.zero)
                for (int k = 0; k < ExplodeDistances[i]; k++)
                    Instantiate(ExplodeParticle, transform.position + ExplodeDirections[i], Quaternion.identity);
            else break;
        Destroy(gameObject);
    }
}
