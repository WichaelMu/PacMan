using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public LayerMask Walls;
    public GameObject ExplodeParticle;

    float MaximumRange = 1;
    readonly Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), }; //  The up, down, left and right directions in their Vector3 equivalents.

    AudioController AudioControl;
    List<GameObject>[] ExplodeQuarters = new[] { new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>()};
    int[] NumberOfExplosions = new[] { 0, 0, 0, 0 };
    List<GameObject> ExplodeParticles;

    IEnumerator RemoveParticles;

    int DetonationTime = 4;

    static int ExplosionSound = 0;

    void Start()
    {
        AudioControl = FindObjectOfType<AudioController>();
        Invoke("Detonate", DetonationTime);
        RemoveParticles = Aftermath();
        StartCoroutine(RemoveParticles);

        ExplodeParticles = new List<GameObject>();
    }

    void Detonate()
    {
        AudioControl.PlaySound("EX" + ExplosionSound);
        ExplosionSound++;
        ExplosionSound %= 5;

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
            for (float k = 0; k < ExplodeDistances[i] && k < MaximumRange; k += .2f)
            {
                //ExplodeParticles.Add(Instantiate(ExplodeParticle, transform.position + ExplodeDirections[i] * k, Quaternion.identity));
                ExplodeQuarters[i].Add(Instantiate(ExplodeParticle, transform.position + ExplodeDirections[i] * k, Quaternion.identity));
                NumberOfExplosions[i]++;
            }
        transform.position = new Vector3 (100f, 100f, 100f);
    }

    IEnumerator Aftermath()
    {
        int[] last = new[] { 0, 0, 0, 0 };
        yield return new WaitForSeconds(DetonationTime + 1.5f);

        //for (int i = NumberofExplodedParticles - 1; i >= 0; i--)
        //{
        //    Destroy(ExplodeParticles[i]);
        //    yield return new WaitForFixedUpdate();
        //}

        for (int i = 0; i < ExplodeQuarters.Length; i++)
            last[i] = ExplodeQuarters[i].Count - 1;

        while (true)
        {
            if (last[0] == 0 && last[1] == 0 && last[2] == 0 && last[3] == 0)
                break;
            for (int i = 0; i < ExplodeQuarters.Length; i++)    //  change this to int i = 1?
            {
                try
                {
                    Destroy(ExplodeQuarters[i][last[i]]);
                }
                catch
                {
                    
                }
                if (last[i]!=0)
                    last[i]--;
            }

            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < ExplodeQuarters.Length; i++)
            Destroy(ExplodeQuarters[i][0]);
        Destroy(gameObject);
    }

    public void SetMaximumRange(int amount)
    {
        MaximumRange += amount;
    }

    public void SetMaximumRange(float amount)
    {
        MaximumRange += amount;
    }
}
