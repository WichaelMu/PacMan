﻿using UnityEngine;

public class GhostController : MonoBehaviour
{
    Rigidbody GhostRB;
    Animator Anim;
    Transform GhostHolder;

    public float MoveSpeed, ScaredResetTime;
    public bool ScaredState, IsAlive;

    float DefaultMoveSpeed;

    void Awake()
    {
        ScaredState = false;
        IsAlive = true;

        GhostRB = GetComponent<Rigidbody>();

        Anim = GetComponent<Animator>();

        //AliveSprite = GetComponent<Animator>();

        DefaultMoveSpeed = MoveSpeed;

        GhostHolder = GetComponentInParent<Transform>();
    }

    void FixedUpdate()
    {
        //ConstantMovement();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Switcher"))
        {
            Invoke(DetermineMovement(other.gameObject), 0f);
        }

    }

    string DetermineMovement(GameObject Switch)
    {
        Switcher switcher = Switch.GetComponent<Switcher>();

        float Randomiser = Random.Range(0f, 1f);
        string Moves = RandomMovement(Randomiser);

        for (int i = 0; i < Moves.Length; i++)
            if (switcher.allowDirection(Moves[i].ToString()))
                return Moves[i].ToString();
            else
                i = 0;
        return null;
    }

    string RandomMovement(float Randomiser)
    {
        if (Randomiser < .1f)
            return "UDLR";
        if (Randomiser < .2f)
            return "DLRU";
        if (Randomiser < .3f)
            return "LRUD";
        if (Randomiser < .4f)
            return "RUDL";
        if (Randomiser < .5f)
            return "UDRL";
        if (Randomiser < .6f)
            return "DRLU";
        if (Randomiser < .7f)
            return "RLUD";
        if (Randomiser < .8f)
            return "LUDR";
        if (Randomiser < .9f)
            return "DULR";
        if (Randomiser <= 1f)
            return "ULRD";
        return null;
    }

    public void SetScared(bool instruction)
    {
        if (instruction && ScaredState)
        {
            CancelInvoke();
            Invoke("ResetState", ScaredResetTime);
            return;
        }

        if (instruction)
        {
            MoveSpeed = .7f;
            ScaredState = true;
            Anim.SetTrigger("GhostScared");
            Invoke("RecoveryState", ScaredResetTime);
            Invoke("ResetState", ScaredResetTime+4f);   //  The Ghosts will no longer be scared after <ScaredResetTime> seconds of being scared.
            PlaySound("GHOSTSCAREDSTATE");
        }
    }
    void RecoveryState()
    {
        Anim.SetTrigger("GhostRecovering");
        PlaySound("DEAD");
    }

    void ResetState()
    {
        ScaredState = false;
        MoveSpeed = DefaultMoveSpeed;
        Anim.SetTrigger("GhostScared");
        StopSound("GHOSTSCAREDSTATE");
        StopSound("DEAD");
        PlaySound("AMBIENT");

        foreach (Transform t in GhostHolder)
            if (t.GetComponent<GhostController>().ScaredState)
            {
                StopSound("DEAD");
                StopSound("AMBIENT");
                PlaySound("GHOSTSCAREDSTATE");
                break;
            }
    }

    public void OnHitPacMan()   //  This can only be called if this Ghost is scared.
    {
        IsAlive = false;

        //TODO: Create a dead state, just eyes, that track back to the Ghost's spawnpoint.
        transform.position = new Vector3(500f, 500f, 0f);
        PlayerStats.score += 100;

        Invoke("GhostRespawn", 5f);

        PlaySound("EATGHOST");
        PlaySound("DEAD");
        
        //TODO: Once the dead state eyes have returned to the Ghost's spawnpoint, reset the ghost; ScaredState = false, IsAlive = true;.
    }

    void GhostRespawn()
    {
        transform.localPosition = new Vector3(1f, 1f, .043f);
        ScaredState = false;
        IsAlive = true;
    }

    void ConstantMovement()
    {
        GhostRB.MovePosition(transform.position + (transform.up * MoveSpeed * Time.deltaTime));
    }

    void PlaySound(string name)
    {
        FindObjectOfType<AudioController>().PlaySound(name);
    }

    void StopSound(string name)
    {
        FindObjectOfType<AudioController>().StopSound(name);
    }


    #region The Rotation of the Ghosts

    void U()
    {
        RotateGhost(new Vector3(000f, 000f, 090f));
    }

    void D()
    {
        RotateGhost(new Vector3(000f, 000f, 270f));
    }

    void L()
    {
        RotateGhost(new Vector3(000f, 000f, 180f));
    }

    void R()
    {
        RotateGhost(new Vector3(000f, 000f, 000f));
    }

    void RotateGhost(Vector3 faceAngle)
    {
        transform.localEulerAngles = faceAngle;
    }

    #endregion
}