using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Dynamites that are used for the innovation scene.
/// </summary>

public class Dynamite : MonoBehaviour
{
    /// <summary>
    /// The Dynamite will only check for raycast hits to walls.
    /// </summary>
    public LayerMask Walls;
    /// <summary>
    /// The particle that indicates a fatal position.
    /// </summary>
    public GameObject ExplodeParticle;

    /// <summary>
    /// The maximum range of a Dynamite's explosion is 1 in-game unit by default.
    /// </summary>
    float MaximumRange = 1;
    /// <summary>
    /// The up, down, left and right directions in their Vector3 equivalents.
    /// </summary>
    readonly Vector3[] directions = new[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), };

    /// <summary>
    /// The Audio Controller for innovation.
    /// </summary>
    AudioController AudioControl;
    /// <summary>
    /// The list of a list ExplodeParticles to be destroyed after DetonationTime.
    /// </summary>
    readonly List<GameObject>[] ExplodeQuarters = new[] { new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>()};
    /// <summary>
    /// The number of fatal positions by this Dynamite after detonation in up, down, left right directions.
    /// </summary>
    readonly int[] NumberOfExplosions = new[] { 0, 0, 0, 0 };

    /// <summary>
    /// Remove the ExplodeParticles after DetonationTime + 1.5 seconds.
    /// </summary>
    IEnumerator RemoveParticles;

    /// <summary>
    /// The time in seconds for this Dynamite to detonate.
    /// </summary>
    readonly int DetonationTime = 3;

    /// <summary>
    /// The sound for this Dynamite's detonation. This is continuous for every Dynamite and will loop after every 5 explosions.
    /// </summary>
    static int ExplosionSound = 0;

    void Start()
    {
        AudioControl = FindObjectOfType<AudioController>();
        Invoke("Detonate", DetonationTime); //  Invokes the detonation of this Dynamite after DetonationTIme seconds.
        RemoveParticles = Aftermath();
        StartCoroutine(RemoveParticles);
    }

    /// <summary>
    /// Causes this Dynamite to detonate with a range of float MaximumRange.
    /// </summary>

    void Detonate()
    {
        AudioControl.PlaySound("EX" + ExplosionSound);  //  Plays the sound of an explosion upon detonation.
        ExplosionSound++;   //  Increments the sound index.
        ExplosionSound %= 5;    //  The sound index loops at 5.

        //  The directions that this Dynamite detonated in.
        Vector3[] ExplodeDirections = new[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, };
        //  The distances of this Dynamite's explosions. This corresponds to the direction in ExplodeDirections[].
        float[] ExplodeDistances = new[] { -1f, -1f, -1f, -1f };
        int VIndex = 0; //  Index for the Vector directions ExplodeDirections[].
        int FIndex = 0; //  Index for the Float distances ExplodeDistances[].

        for (int i = 0; i < directions.Length; i++)
        {
            //  Performs a raycast in directions[] of index i that collides with Layermask Walls for an infinite range.
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, -.05f), transform.TransformDirection(directions[i]), out RaycastHit hit, Mathf.Infinity, Walls);
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, -.05f), transform.TransformDirection(directions[i]), Color.white, 5f);
            if (hit.distance > .05f)    //  If the distance of the raycast is greater than .05 units.
            {
                ExplodeDirections[VIndex] = directions[i];  //  Store this Vector3 direction to ExplodeDirections[] using VIndex index.
                ExplodeDistances[FIndex] = hit.distance;    //  Store the distance of the explosion in directions[] of index i.
                VIndex++;   //  Increment the index for ExplodeDirections[].
                FIndex++;   //  Increment the index for ExplodeDistances[].
            }
        }

        for (int i = 0; i < ExplodeDirections.Length && ExplodeDirections[i] != Vector3.zero; i++)  //  For every ExplodeDirections[] that are not Vector3.zero.
            for (float k = 0; k < ExplodeDistances[i] && k < MaximumRange; k += .2f)    //  For the entire distance of the ExplodeDistance[i] in ExplodeDirection[i] or until MaximumRange.
            {
                //ExplodeParticles.Add(Instantiate(ExplodeParticle, transform.position + ExplodeDirections[i] * k, Quaternion.identity));
                ExplodeQuarters[i].Add(Instantiate(ExplodeParticle, transform.position + ExplodeDirections[i] * k, Quaternion.identity));   //  Add an ExplodeParticle to ExplodeQuarters[i], corresponding to directions[i].
                NumberOfExplosions[i]++;    //  Increment the NumberOfExplosions[] for directions[i].
            }
        transform.position = new Vector3 (100f, 100f, 100f);    //  Move this Dynamite out of view.
    }

    /// <summary>
    /// After DetonationTime + 1.5 seconds, begin removing ExplodeParticles sequentially.
    /// </summary>
    /// <returns>Begin removing fatal positions after 1.5 seconds after this Dynamite's detonation. Remove the last fatal position in ExplodeQuarters[].</returns>

    IEnumerator Aftermath()
    {
        int[] last = new[] { 0, 0, 0, 0 };  //  The array of ints that stores the last position in ExplodeQuarters for directions[i].
        yield return new WaitForSeconds(DetonationTime + 1.5f); //  Wait 1.5 seconds after detonation.

        //for (int i = NumberofExplodedParticles - 1; i >= 0; i--)
        //{
        //    Destroy(ExplodeParticles[i]);
        //    yield return new WaitForFixedUpdate();
        //}

        for (int i = 0; i < ExplodeQuarters.Length; i++)
            last[i] = ExplodeQuarters[i].Count - 1; //  Populate the last[] to equal the number of ExplodeParticles in directions[i].

        while (true)
        {
            if (last[0] == 0 && last[1] == 0 && last[2] == 0 && last[3] == 0)
                break;  //  If there are no ExplodePartcles remaining in ExplodeQuarters[], break the loop.
            for (int i = 0; i < ExplodeQuarters.Length; i++)    //  This can be simplified into i < 4.
            {
                if (last[i] >= 0)   //  If the last[] for ExplodeQuarters[i] is >= 0.
                    Destroy(ExplodeQuarters[i][last[i]]);   //  Destroy the last ExplodeParticle at ExplodeQuarters[i].
                if (last[i]!=0) //  If the last[] at ExplodeQuarters[i] is not 0.
                    last[i]--;  //  Decrement the last[].
            }

            yield return new WaitForFixedUpdate();  //  Iterate through this loop every fixed update.
        }

        for (int i = 0; i < ExplodeQuarters.Length; i++)    //  Iterate through the ExplodeQuarters again.
            Destroy(ExplodeQuarters[i][0]); //  Ensure that there are no remaining ExplodeParticles.
        Destroy(gameObject);    //  Finally, destroy this Dynamite.
    }

    /// <summary>
    /// Sets the MaximumRange to a specified amount.
    /// </summary>
    /// <param name="amount">Sets the MaximumRange to int amount.</param>

    public void SetMaximumRange(int amount)
    {
        MaximumRange += amount;
    }

    /// <summary>
    /// Sets the MaximumRange to a specified amount.
    /// </summary>
    /// <param name="amount">Sets the MaximumRange to float amount.</param>

    public void SetMaximumRange(float amount)
    {
        MaximumRange += amount;
    }
}
