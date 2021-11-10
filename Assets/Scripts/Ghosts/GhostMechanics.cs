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
using TMPro;

public class GhostMechanics : MonoBehaviour
{
    /// <summary>
    /// The respawn point for this Ghost.
    /// </summary>
    public Transform RespawnPoint;

    /// <summary>
    /// The timer that shows how long this Ghost will be scared for.
    /// </summary>
    public TextMeshProUGUI ScaredTimer; //  This is the scared timer that is required for 60%.
    /// <summary>
    /// The text that labels this Ghost 1, 2, 3 or 4.
    /// </summary>
    public TextMeshProUGUI NumberTags;  //  This is the Ghost's label (1-4) that is required for 60%.a

    /// <summary>
    /// Where all the Ghosts can be accessed.
    /// </summary>
    public Transform GhostHolder;

    /// <summary>
    /// The animator for this Ghost.
    /// </summary>
    Animator Anim;  //  The animator for this ghost.
    /// <summary>
    /// THe sphere collider for this Ghost.
    /// </summary>
    SphereCollider Sphere;
    //Transform GhostHolder;

    /// <summary>
    /// The audio controller that controls the sounds that play.
    /// </summary>
    AudioController AudioControl;   //  The AudioController for the game.

    public float MoveSpeed, ScaredResetTime;    //  The movement speed for the ghost . The time it takes for the ghost to no lnoger be scared.
    /// <summary>
    /// The state of this Ghost.
    /// </summary>
    public bool ScaredState = false, IsAlive = true;   //  If the ghost is currently scared. If the ghost is currently alive (not dead/not just eyes).

    /// <summary>
    /// The default movement speed for this Ghost.
    /// </summary>
    float DefaultMoveSpeed; //  The default movement speed for the ghosts.
    bool IsOutOfPortal;

    void Awake()
    {
        ScaredState = false;
        IsAlive = true;

        Anim = GetComponent<Animator>();

        Sphere = GetComponent<SphereCollider>();

        //AliveSprite = GetComponent<Animator>();

        DefaultMoveSpeed = MoveSpeed;

        //GhostHolder = GetComponentInParent<Transform>();

        AudioControl = FindObjectOfType<AudioController>();

        ScaredTimer.gameObject.SetActive(false);
        //NumberTags.gameObject.SetActive(true);
        NumberTags.transform.localPosition = new Vector3(NumberTags.transform.localPosition.x, 1f, NumberTags.transform.localPosition.z);
    }

    void Update()
    {
        if (!IsAlive)   //  If this Ghost is not alive.
            Lerp(RespawnPoint.position);    //  Lerp this Ghost back to spawn.
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up), Color.green);
    }

    void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Switcher"))   //  This is not used as the ghosts will never move from their starting position.
        //Invoke(DetermineMovement(other.gameObject), 0f);
        //ArtificialIntelligence(); //  This is in GhostController.cs.
        //  The Ghosts should no longer be in contact with a portal for Assignment 4.
        if (other.CompareTag("PortalL") || other.CompareTag("PortalR")) //  If this Ghost enters a portal.
            PortalHandler(other.name);
    }

    /// <summary>
    /// Teleports this Ghost to the opposite portal.
    /// </summary>
    /// <param name="Portal">The portal that this Ghost entered.</param>

    // The Ghosts should no longer be in contact with a portal for Assignment 4.
    void PortalHandler(string Portal)   //  There is a better way in doing this. In free-time, find a solution.
    {
        if (Portal.Equals("PortalL(Clone)") && IsOutOfPortal)   //  If this Ghost goes into the Left Portal and Pac Man is, in fact, out of any Portal, it will move this Ghost's position to the Right Portal.
            transform.position = new Vector3(8.4375f, 4.375f, 0f);
        else if (Portal.Equals("PortalR(Clone)") && IsOutOfPortal)   //  If this Ghost goes into the Right Portal and Pac Man is, in fact, out of any Portal, it will move this Ghost's  position to the Left Portal.
            transform.position = new Vector3(0f, 4.375f, 0f);
        IsOutOfPortal = !IsOutOfPortal; //  Flip if Pac Man is out of a Portal. Pac Man will hit the opposite Portal when leaving the first. It will flip twice during one Portal movement. This is needed as, without it, Pac Man will endlessly go back and forth an infinite number of times.
    }

    /// <summary>
    /// Sets this Ghost to be scared according to Boolean instruction.
    /// </summary>
    /// <param name="instruction">Boolean to determine if this Ghost is scared.</param>

    public void SetScared(bool instruction)
    {
        if (instruction)    //  If this Ghost needs to be scared.
        {
            CancelInvoke(); //  Stop all invoke methods.
            if (ScaredState)    //  If this Ghost is already scared.
                ResetState();   //  Reset this Ghost's state.
            GetScared();    //  Scare this Ghost.
        }
    }

    /// <summary>
    /// Scare this Ghost.
    /// </summary>

    void GetScared()
    {
        CancelInvoke(); //  Stop all invoke methods.
        MoveSpeed *= .7f;   //  Slow this Ghost's movement speed to 70% of its original.
        ScaredState = true; //  Set the scared state of this Ghost to true.
        Anim.SetTrigger("GhostScared"); //  Plays the blue and white ghost scared state.
        //Invoke("ResetState", ScaredResetTime+4f);   //  The Ghosts will no longer be scared after <ScaredResetTime> + 4 seconds of being scared.

        //StopSound("AMBIENT");
        //PlaySound("GHOSTSCAREDSTATE");  //  Play the sound of the ghosts being scared. A very annoying sound.

        DetermineSound();   //  Determine the sound that needs to be played.
    }

    /// <summary>
    /// Resets this Ghost's state to be default.
    /// </summary>

    void ResetState()   //  Resets the ghost.
    {
        CancelInvoke(); //  Stop all invoke methods.
        IsAlive = true; //  Reset this Ghost to be alive.
        ScaredState = false;    //  Reset this Ghost to no longer be scared.
        MoveSpeed = DefaultMoveSpeed;   //  The movement speed of the ghosts are reset.

        Anim.SetTrigger("GhostRecover");    //  Plays the normal Ghost animation.
        //Anim.Play("Default");

        ScaredTimer.gameObject.SetActive(false);    //  Hide the ScaredTimer UI.
        //NumberTags.gameObject.SetActive(true);  //  Show the NumberTags UI.
        NumberTags.transform.localPosition = new Vector3(NumberTags.transform.localPosition.x, 1f, NumberTags.transform.localPosition.z);

        Sphere.enabled = true;  //  Enable the sphere collider.

        IsOutOfPortal = true;   //  Reset this Ghost to be out of a portal.

        //StopSound("DEAD");  //  If the ghost dead sound is still playing, stop it.
        //StopSound("GHOSTSCAREDSTATE");  //  If the ghost scared sound is still playing, stop it.
        //PlaySound("AMBIENT");   //  Play the normal sound.

        DetermineSound();   //  Determine the sound that needs to be played.
    }

    /// <summary>
    /// Kill this Ghost if it is scared and hit by Pac Man.
    /// </summary>

    public void OnHitPacMan()   //  This can only be called if this Ghost is scared.
    {
        CancelInvoke(); //  Stop all invoke methods.
        ScaredState = false;    //  This Ghost is no longer scared.
        IsAlive = false;    //  This Ghost is dead.

        //TODO: Create a dead state, just eyes, that track back to the Ghost's spawnpoint. DONE.
        //transform.position = new Vector3(500f, 500f, 0f);
        PlayerStats.score += 300;   //  Increment the player's score by 300.

        Anim.SetTrigger("GhostIsDead"); //  Set the ghost to play the animation with only eyes.

        GetComponent<GhostController>().ResetLBM(); //  If this Ghost is the Light Blue Ghost, reset the strict clockwise movement.

        //Possibly decrease this time to fit the original game, 5 seconds seems too long.
        //Invoke("GhostRespawn", 5f); //  Set the ghost to respawn in 5 seconds.

        Sphere.enabled = false; //  Disable the sphere collider.

        PlaySound("EATGHOST");  //  Play the sound of the ghost being eaten by Pac Man.
        //PlaySound("DEAD");  //  Play the sound when Pac Man eats a scared ghost.

        DetermineSound();   //  Determine the sound that needs to be played.
        
        //TODO: Once the dead state eyes have returned to the Ghost's spawnpoint, reset the ghost; ScaredState = false, IsAlive = true;. DONE?
    }

    float time;

    /// <summary>
    /// Lerp this Ghost to Vector3 position.
    /// </summary>
    /// <param name="position">The Vector3 position for the destination of the lerp.</param>

    void Lerp(Vector3 position)
    {
        time += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, position, time*.1f);

        if (Vector3.Distance(transform.position, position) < .1f)
        {
            time = 0;
            transform.position = position;
            GhostRespawn();

            for (int i = 0; i < 4; i++)
                if (GhostHolder.GetChild(i).GetComponent<GhostMechanics>().ScaredState)
                    SetScared(true);
            return;
        }
    }

    /// <summary>
    /// Cause this Ghost to respawn.
    /// </summary>

    void GhostRespawn() //  This is called in the 'Ghost Dead State' animation as an event at the end of the animation.
    {
        CancelInvoke(); //  Stop all invoke methods.
        //transform.localPosition = new Vector3(1f, 1f, -.05f); //  Do not use for Assignment 3.
        GhostController GC = GetComponent<GhostController>();
        GC.ResetLBM();  //  If this Ghost is the Light Blue Ghost, reset the strict clockwise movement.
        GC.ResetPositions();    //  Reset the positions of the Ghosts to their spawn point.

        ResetState();   //  Reset this Ghost's state.
    }

    /// <summary>
    /// Shows the time remaining to be scared.
    /// </summary>
    /// <param name="n">Int number of seconds remaining to recovering from being scared.</param>

    void ShowScaredTimerSeconds(int n)  //  This is called in the GhostRecovery animation as an event.
    {
        //NumberTags.gameObject.SetActive(false);
        NumberTags.transform.localPosition = new Vector3(NumberTags.transform.localPosition.x, 30f, NumberTags.transform.localPosition.z);
        ScaredTimer.gameObject.SetActive(true);
        ScaredTimer.text = n.ToString();
    }

    /// <summary>
    /// Stop showing the scared timer.
    /// </summary>

    void StopShowingScaredTimer()
    {
        //NumberTags.gameObject.SetActive(true);
        NumberTags.transform.localPosition = new Vector3(NumberTags.transform.localPosition.x, 1f, NumberTags.transform.localPosition.z);
        ScaredTimer.gameObject.SetActive(false);
    }

    /// <summary>
    /// Reset this Ghost's position to it's spawn point.
    /// </summary>

    public void ResetPositions()
    {
        IsOutOfPortal = true;
        transform.position = RespawnPoint.position;
    }

    #region Audio Control

    //  Plays or Stops the sound <name>.

    /// <summary>
    /// Plays sound with name.
    /// </summary>
    /// <param name="name">The string name of the sound to play.</param>

    void PlaySound(string name)
    {
        AudioControl.PlaySound(name);
    }

    /// <summary>
    /// Stops sound with name.
    /// </summary>
    /// <param name="name">The string name of the sound to stop.</param>

    void StopSound(string name)
    {
        AudioControl.StopSound(name);
    }

    /// <summary>
    /// Determins the sound that needs to be played according to the other Ghost's states.
    /// </summary>

    void DetermineSound()
    {
        GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("Ghost");   //  Find all the Ghosts in the level.
        bool OneScared = false, OneDead = false;    //  If at least one is scared and if at least one is dead.
        for (int i = 0; i < Ghosts.Length; i++) //  For every Ghost in the level.
        {
            GhostMechanics GM = Ghosts[i].GetComponent<GhostMechanics>();   //  Get the Ghost's GhostMechanics.cs component.

            if (GM.ScaredState) //  If this Ghost is scared.
                OneScared = true;   //  At least one Ghost is scared.
            if (!GM.IsAlive)    //  If this Ghost is not alive.
                OneDead = true; //  At least one Ghost is dead.
        }

        if (OneScared && !OneDead)  //  When at least one Ghost is scared, but no Ghosts are dead.
        {
            PlaySound("GHOSTSCAREDSTATE");
            StopSound("DEAD");
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