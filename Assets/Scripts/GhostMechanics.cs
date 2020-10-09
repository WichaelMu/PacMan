﻿using UnityEngine;
using TMPro;

public class GhostMechanics : MonoBehaviour
{
    public Transform RespawnPoint;

    public TextMeshProUGUI ScaredTimer; //  This is the scared timer that is required for 60%.
    public TextMeshProUGUI NumberTags;  //  This is the Ghost's label (1-4) that is required for 60%.a

    public Transform GhostHolder;

    Rigidbody GhostRB;  //  The ghosts' Rigidbody.
    Animator Anim;  //  The animator for this ghost.
    SphereCollider Sphere;
    //Transform GhostHolder;

    AudioController AudioControl;   //  The AudioController for the game.

    public float MoveSpeed, ScaredResetTime;    //  The movement speed for the ghost . The time it takes for the ghost to no lnoger be scared.
    public bool ScaredState, IsAlive;   //  If the ghost is currently scared. If the ghost is currently alive (not dead/not just eyes).

    float DefaultMoveSpeed; //  The default movement speed for the ghosts.
    bool IsOutOfPortal;

    void Awake()
    {
        ScaredState = false;
        IsAlive = true;
        GhostRB = GetComponent<Rigidbody>();

        Anim = GetComponent<Animator>();

        Sphere = GetComponent<SphereCollider>();

        //AliveSprite = GetComponent<Animator>();

        DefaultMoveSpeed = MoveSpeed;

        //GhostHolder = GetComponentInParent<Transform>();

        AudioControl = FindObjectOfType<AudioController>();

        ScaredTimer.gameObject.SetActive(false);
        NumberTags.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!IsAlive)
            Lerp(RespawnPoint.position);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.green);
    }

    void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Switcher"))   //  This is not used as the ghosts will never move from their starting position.
        //Invoke(DetermineMovement(other.gameObject), 0f);
        //ArtificialIntelligence(); //  This is in GhostController.cs.
        if (other.CompareTag("PortalL") || other.CompareTag("PortalR"))
            PortalHandler(other.name);
    }

    void PortalHandler(string Portal)   //  There is a better way in doing this. In free-time, find a solution.
    {
        if (Portal.Equals("PortalL(Clone)") && IsOutOfPortal)   //  If this Ghost goes into the Left Portal and Pac Man is, in fact, out of any Portal, it will move this Ghost's position to the Right Portal.
            transform.position = new Vector3(8.4375f, 4.375f, 0f);
        if (Portal.Equals("PortalR(Clone)") && IsOutOfPortal)   //  If this Ghost goes into the Right Portal and Pac Man is, in fact, out of any Portal, it will move this Ghost's  position to the Left Portal.
            transform.position = new Vector3(0f, 4.375f, 0f);
        IsOutOfPortal = !IsOutOfPortal; //  Flip if Pac Man is out of a Portal. Pac Man will hit the opposite Portal when leaving the first. It will flip twice during one Portal movement. This is needed as, without it, Pac Man will endlessly go back and forth an infinite number of times.
    }

    public void SetScared(bool instruction)
    {
        if (instruction)
        {
            CancelInvoke();
            if (ScaredState)
                ResetState();
            GetScared();
        }
    }

    void GetScared()
    {
        CancelInvoke();
        MoveSpeed *= .7f;
        ScaredState = true;
        Anim.SetTrigger("GhostScared"); //  Plays the blue and white ghost scared state.
        //Invoke("ResetState", ScaredResetTime+4f);   //  The Ghosts will no longer be scared after <ScaredResetTime> + 4 seconds of being scared.

        //StopSound("AMBIENT");
        //PlaySound("GHOSTSCAREDSTATE");  //  Play the sound of the ghosts being scared. A very annoying sound.

        DetermineSound();
    }

    void ResetState()   //  Resets the ghost.
    {
        CancelInvoke();
        IsAlive = true;
        ScaredState = false;
        MoveSpeed = DefaultMoveSpeed;   //  The movement speed of the ghosts are reset.

        Anim.SetTrigger("GhostRecover");
        //Anim.Play("Default");

        ScaredTimer.gameObject.SetActive(false);
        NumberTags.gameObject.SetActive(true);

        Sphere.enabled = true;

        IsOutOfPortal = true;

        //StopSound("DEAD");  //  If the ghost dead sound is still playing, stop it.
        //StopSound("GHOSTSCAREDSTATE");  //  If the ghost scared sound is still playing, stop it.
        //PlaySound("AMBIENT");   //  Play the normal sound.

        DetermineSound();
    }

    public void OnHitPacMan()   //  This can only be called if this Ghost is scared.
    {
        CancelInvoke();
        ScaredState = false;
        IsAlive = false;

        //TODO: Create a dead state, just eyes, that track back to the Ghost's spawnpoint. DONE.
        //transform.position = new Vector3(500f, 500f, 0f);
        PlayerStats.score += 300;

        Anim.SetTrigger("GhostIsDead"); //  Set the ghost to play the animation with only eyes.

        GetComponent<GhostController>().ResetLBM();

        //Possibly decrease this time to fit the original game, 5 seconds seems too long.
        //Invoke("GhostRespawn", 5f); //  Set the ghost to respawn in 5 seconds.

        Sphere.enabled = false;

        PlaySound("EATGHOST");  //  Play the sound of the ghost being eaten by Pac Man.
        //PlaySound("DEAD");  //  Play the sound when Pac Man eats a scared ghost.

        DetermineSound();
        
        //TODO: Once the dead state eyes have returned to the Ghost's spawnpoint, reset the ghost; ScaredState = false, IsAlive = true;. DONE?
    }

    float time;

    void Lerp(Vector3 position)
    {
        time += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, position, time*.5f);

        if (Vector3.Distance(transform.position, position) < .1f)
        {
            time = 0;
            transform.position = position;
            return;
        }
    }

    void GhostRespawn() //  This is called in the 'Ghost Dead State' animation as an event at the end of the animation.
    {
        CancelInvoke();
        //transform.localPosition = new Vector3(1f, 1f, -.05f); //  Do not use for Assignment 3.
        GhostController GC = GetComponent<GhostController>();
        GC.ResetLBM();
        GC.ResetPositions();

        ResetState();
    }

    void ShowScaredTimerSeconds(int n)  //  This is called in the GhostRecovery animation as an event.
    {
        NumberTags.gameObject.SetActive(false);
        ScaredTimer.gameObject.SetActive(true);
        ScaredTimer.text = n.ToString();
    }

    void StopShowingScaredTimer()
    {
        NumberTags.gameObject.SetActive(true);
        ScaredTimer.gameObject.SetActive(false);
    }

    public void ResetPositions()
    {
        IsOutOfPortal = true;
        transform.position = RespawnPoint.position;
    }

    void DetermineSound()
    {
        GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        bool OneScared = false, OneDead = false;
        for (int i = 0; i < Ghosts.Length; i++)
        {
            GhostMechanics GM = Ghosts[i].GetComponent<GhostMechanics>();

            if (GM.ScaredState)
                OneScared = true;
            if (!GM.IsAlive)
                OneDead = true;
        }

        if (OneScared && !OneDead)  //  When at least one Ghost is scared, but no Ghosts are dead.
        {
            PlaySound("GHOSTSCAREDSTATE");
            StopSound("AMBIENT");
            return;
        }

        if (OneScared && OneDead)   //  When at least one Ghost is scared and one Ghost is dead.
        {
            StopSound("GHOSTSCAREDSTATE");
            StopSound("AMBIENT");
            PlaySound("DEAD");
            return;
        }

        if (!OneScared && !OneDead) //  When there are no Ghosts who are scared or dead.
        {
            StopSound("GHOSTSCAREDSTATE");
            StopSound("DEAD");
            PlaySound("AMBIENT");
        }
    }

    #region Audio Control

    //  Plays or Stops the sound <name>.

    void PlaySound(string name)
    {
        AudioControl.PlaySound(name);
    }

    void StopSound(string name)
    {
        AudioControl.StopSound(name);
    }

    #endregion

    #region The Rotation of the Ghosts

    void U()    // UP
    {
        RotateGhost(new Vector3(000f, 000f, 090f));
    }

    void D()    //  DOWN
    {
        RotateGhost(new Vector3(000f, 000f, 270f));
    }

    void L()    //  LEFT
    {
        RotateGhost(new Vector3(000f, 000f, 180f));
    }

    void R()    //  RIGHT
    {
        RotateGhost(new Vector3(000f, 000f, 000f));
    }

    void RotateGhost(Vector3 faceAngle)
    {
        transform.localEulerAngles = faceAngle; //  Rotates the ghost to face a direction.  //  This may not be used for Assignment 4.
    }

    #endregion
}