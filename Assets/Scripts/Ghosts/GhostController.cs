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

public class GhostController : GhostIntelligence
{

    /// <summary>
    /// The GhostID for 60%: P. This Ghost's World Space labelling.
    /// </summary>
    [Header("The Ghost Canvas Number.")]
    public int GhostID; //  The ID of the Ghosts that correspond to their World Space Canvas.

    /// <summary>
    /// The default position and respawn point for this Ghost.
    /// </summary>
    Transform RespawnPoint; //  The respawn point for this Ghost.

    GhostMechanics Mechanics;
    
    void Start()
    {
        Mechanics = GetComponent<GhostMechanics>();

        if (!Mechanics)
            Debug.LogWarning("Ghost with ID: " + GhostID + " does not have a <GhostMecahnics> component!");
        else
            RespawnPoint = Mechanics.RespawnPoint; //  Assigns this GhostController's respawn point to the same one in this Ghost's GhostMechanics.


        StartSequence();    //  Begins the starting sequence to begin/restart the game.
    }

    void OnTriggerEnter(Collider o)
    {
        if (o.CompareTag("Switcher"))   //  If this Ghost hits a switcher.
            ComputeArtificialIntelligence(o.gameObject.GetComponent<Switcher>());  //  It will begin the process of Artificial Intelligence.
    }

    #region Reset

    public void ResetPositions()
    {
        transform.position = RespawnPoint.position; //  Respawn this Ghost at GhostMechanic's respawn point position.
        StartSequence();    //  Re/Start the starting sequence.
    }

    void StartSequence()
    {
        U();    //  Default movement to up.
        if (GhostID == 4)   //  If this Ghost is the Light Blue.
            D();    //  Default movement to downm.
        LBM = 0;    //  Reset the Light Blue Movement index.
    }

    public void ResetLBM()
    {
        LBM = 0;    //  Reset the Light Blue Movement index.
    }

    #endregion
}
