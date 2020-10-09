using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public LayerMask Walls;
    public GameObject ExplodeParticle;

    Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), }; //  The up, down, left and right directions in their Vector3 equivalents.

    List<GameObject> ExplodeParticles;
    int NumberofExplodedParticles = 0;

    IEnumerator RemoveParticles;

    void Start()
    {
        Invoke("Explode", 4f);
        RemoveParticles = Aftermath();
        StartCoroutine(RemoveParticles);

        ExplodeParticles = new List<GameObject>();
    }

    void Explode()
    {
        Vector3[] ExplodeDirections = new[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, };
        float[] ExplodeDistances = new[] { -1f, -1f, -1f, -1f };
        int VIndex = 0; //  Index for the Vector directions 'Explode Directions'.
        int FIndex = 0; //  Index for the Float distances 'Explode Distances'.

        for (int i = 0; i < directions.Length; i++)
        {
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, -.05f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, -.05f), transform.TransformDirection(directions[i]), Color.white, 5f);
            if (hit.distance > .05f)
            {
                ExplodeDirections[VIndex] = directions[i];
                ExplodeDistances[FIndex] = hit.distance;
                VIndex++;
                FIndex++;
            }
        }

        for (int i = 0; i < ExplodeDirections.Length && ExplodeDirections[i] != Vector3.zero; i++)
                for (float k = 0; k < ExplodeDistances[i] && k < 7f; k += .2f)
                    ExplodeParticles.Add(Instantiate(ExplodeParticle, transform.position + ExplodeDirections[i] * k, Quaternion.identity));
        NumberofExplodedParticles = ExplodeParticles.Count;
    }

    IEnumerator Aftermath()
    {
        yield return new WaitForSeconds(7f);
        for (int i = NumberofExplodedParticles - 1; i >= 0; i--)
        {
            Destroy(ExplodeParticles[i]);
            yield return new WaitForFixedUpdate();
        }
        
        Destroy(gameObject);
    }
}
