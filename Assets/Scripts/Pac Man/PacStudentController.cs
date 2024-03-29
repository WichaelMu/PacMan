﻿/*
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

public class PacStudentController : MonoBehaviour
{
	/// <summary>
	/// The Game Controller that controls the logic for this game.
	/// </summary>
	[Header("Life Controller")]
	public GameController GameController;

	/// <summary>
	/// The parent where all the Ghosts can be accessed.
	/// </summary>
	[Header("Ghosts")]
	public Transform GhostHolder;

	/// <summary>
	/// The left portal's position.
	/// </summary>
	[Header("Portals")]
	public Transform PortalL;
	/// <summary>
	/// The right portal's position.
	/// </summary>
	public Transform PortalR;

	/// <summary>
	/// Pac Man's default position and spawn point.
	/// </summary>
	[Header("Spawn Point")]
	public GameObject SpawnPoint;

	/// <summary>
	/// The particle effect that plays when Pac Man hits a wall.
	/// </summary>
	[Header("")]
	public GameObject WallBump;
	/// <summary>
	/// The particle effect that plays when Pac Man dies.
	/// </summary>
	public GameObject PacManDeath;

	//Rigidbody PacMan;
	/// <summary>
	/// The switcher that Pac Man collides with.
	/// </summary>
	Switcher switcher;
	/// <summary>
	/// Pac Man's animator.
	/// </summary>
	Animator DeadStateAnim;
	/// <summary>
	/// The audio controller that controls the sounds that play.
	/// </summary>
	AudioController AudioControl;
	ParticleSystem dust;

	/// <summary>
	/// The movement speed for Pac Man.
	/// </summary>
	public float moveSpeed;

	/// <summary>
	/// The default movement speed for Pac Man.
	/// </summary>
	float DefaultMoveSpeed;
	bool LR = false, IsStill, IsOutOfPortal = true, IsAlive;
	string nextAvailableTurn, currentInput;

	void Awake()
	{
		AudioControl = FindObjectOfType<AudioController>();
		if (GhostHolder == null)
			GhostHolder = GameObject.FindWithTag("GHOSTHOLDER").GetComponent<Transform>();
		dust = GetComponentInChildren<ParticleSystem>();
	}

	void Start()
	{
		//PacMan = GetComponent<Rigidbody>();
		DefaultMoveSpeed = moveSpeed;

		DeadStateAnim = GetComponent<Animator>();
		ResetGame();

		PlayerStats.score = 0;
	}

	void Update()
	{
		KeyboardControl();
	}

	void FixedUpdate()
	{
		Movement();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Switcher"))   //  A switcher is a corner in the game. I don't know why I didn't name it corner.
			OnSwitcher(other.gameObject);
		else if (other.CompareTag("Pellet"))     //  When Pac Man hits a Pellet.
			EatPellet(other.gameObject);
		else if (other.CompareTag("BigPellet"))  //  WHen Pac Man hits a Big Pellet.
			EatBigPellet(other.gameObject);
		else if (other.CompareTag("PortalL") || other.CompareTag("PortalR")) //  When Pac Man enters a Portal.
			PortalHandler(other.name);
		if (other.CompareTag("Fruit"))      //  When Pac Man hits a Fruit.
			EatFruit();
		if (other.CompareTag("Ghost"))      //  When Pac Man hits a Ghost.
			OnHitGhost(other.gameObject);
	}

	/// <summary>
	/// When Pac Man is on a corner.
	/// </summary>
	/// <param name="Switcher">The specific switcher Pac Man is on.</param>

	void OnSwitcher(GameObject Switcher)
	{
		this.switcher = Switcher.GetComponent<Switcher>();  //  Sets the corner Pac Man is on to <this.switcher>. This corner is accessed if Pac Man is stuck on a corner.

		if (switcher.allowDirection(nextAvailableTurn))     //  If this switcher allows Pac Man to turn in the direction of <nextAvailableTurn>, it will turn in that direction.
			Invoke(nextAvailableTurn, 0f);

		if (!switcher.allowDirection(currentInput))         //  If this switcher does not allow Pac Man to turn in the direction of <nextAvailableTurn>, it will stop Pac Man's movement in that corner until the play tells Pac Man to move in a direction that *is* allowed by this switcher.
			HoldPosition(true);
	}

	/// <summary>
	/// When Pac Man hits a pellet.
	/// </summary>
	/// <param name="Pellet">The pellet that Pac Man collided with.</param>

	void EatPellet(GameObject Pellet)
	{
		Destroy(Pellet);            //  Remove the Pellet from the game.
		PlayerStats.score += 10;
		PlaySound("EATPELLET");     //  Play the sound of Pac Man eating a Pellet.
	}

	/// <summary>
	/// When Pac Man hits a big pellet.
	/// </summary>
	/// <param name="BigPellet">The big pellet that Pac Man collided with.</param>

	void EatBigPellet(GameObject BigPellet)
	{
		Destroy(BigPellet);         //  Remove the Big Pellet from the game.
		PlayerStats.score += 50;
		PlaySound("EATFRUIT");      //  Play the sound of Pac Man eating a Fruit/Big Pellet.
		StopSound("AMBIENT");       //  Stops the ambient from playing.

		for (int i = 0; i < 4; i++) //  For every ghost in the game.
		{
			GhostMechanics gc = GhostHolder.GetChild(i).gameObject.GetComponent<GhostMechanics>();
			if (gc.IsAlive)         //  If that ghost is alive.
				gc.SetScared(true); //  Set that ghost to a scared state.
		}
	}

	/// <summary>
	/// When Pac Man hits a Fruit.
	/// </summary>

	void EatFruit()                 //  The main functinality of Pac Man eating a Fruit is done on it's own script called 'Fruit.cs'.Fruit
	{
		//Destroying the Fruit is done on Fruit.cs;
		PlaySound("EATFRUIT");      //  Play the sound of Pac Man eating a Fruit.
		PlayerStats.score += 100;
		PlayerStats.SaveGame();
	}

	/// <summary>
	/// When Pac Man hits a Ghost.
	/// </summary>
	/// <param name="Ghost">The Ghost that Pac Man collided with.</param>

	void OnHitGhost(GameObject Ghost)
	{
		GhostMechanics ghost = Ghost.GetComponent<GhostMechanics>();

		if (ghost.ScaredState)          //  If the ghost is scared.
			OnHitScaredGhost(ghost);    //  Kill that ghost.
		else
			PacManIsDead();             //  Otherwise, kill Pac Man.

		PlayerStats.SaveGame();
	}

	/// <summary>
	/// When Pac Man hits a non-scared Ghost.
	/// </summary>

	void PacManIsDead()
	{
		//Ghost.GetComponent<GhostMechanics>().OnHitPacMan();
		//TODO: Play the death animation for Pac Man. DONE

		//Debug.Log("PacMan is Dead");

		DeadStateAnim.SetBool("PacManIsDead", true);    //  Play Pac Man's dead animation.
		AudioControl.StopAllSounds();    //  Stop every sound that is being played.
		PlaySound("PACMANDEATH");   //  Play the sound of Pac Man dieding.
		moveSpeed = 0;  //  Stop Pac Man.
		IsAlive = false;    //  Set Pac Man's alive state to false.

		Destroy(Instantiate(PacManDeath, transform.position, Quaternion.identity), 5f);

		PlayerStats.timesDied++;
		FindObjectOfType<GameController>().DeductLife();    //  Deducts a life in the Life UI on-screen.

		for (int i = 0; i < 4; i++)
		{
			GameObject Ghost = GhostHolder.GetChild(i).gameObject;
			Ghost.GetComponent<GhostController>().enabled = false;
			Ghost.GetComponent<GhostMechanics>().enabled = false;
		}

		if (!GameController.DoNotRestart)
			Invoke("ResetGame", 4f);    //  Reset the game in 4 seconds.

		PlayerStats.SaveGame();
	}

	/// <summary>
	/// When Pac Man hits a Ghost who is scared.
	/// </summary>
	/// <param name="Ghost">The scared Ghost that Pac Man collided with.</param>

	void OnHitScaredGhost(GhostMechanics Ghost)
	{
		//TODO: Play the death animation for The Ghosts. DONE.
		Ghost.OnHitPacMan();    //  This can only be called if the ghost that was hit is scared.
	}

	/// <summary>
	/// When Pac Man hits a portal.
	/// </summary>
	/// <param name="Portal">The portal that Pac Man collided with.</param>

	void PortalHandler(string Portal)   //  There is a better way in doing this. In free-time, find a solution.
	{
		if (Portal.Equals("PortalL(Clone)") && IsOutOfPortal)   //  If PacMan goes into the Left Portal and Pac Man is, in fact, out of any Portal, it will move Pac Man's position to the Right Portal.
			transform.position = new Vector3(8.4375f, 4.375f, 0f);
		else if (Portal.Equals("PortalR(Clone)") && IsOutOfPortal)   //  If PacMan goes into the Right Portal and Pac Man is, in fact, out of any Portal, it will move Pac Man's position to the Left Portal.
			transform.position = new Vector3(0f, 4.375f, 0f);
		IsOutOfPortal = !IsOutOfPortal; //  Flip if Pac Man is out of a Portal. Pac Man will hit the opposite Portal when leaving the first. It will flip twice during one Portal movement. This is needed as, without it, Pac Man will endlessly go back and forth an infinite number of times.
	}

	/// <summary>
	/// Resets the game to the necessary default values.
	/// </summary>

	void ResetGame()
	{
		IsAlive = true; //  Sets Pac Man to be alive.
		DeadStateAnim.SetBool("PacManIsDead", false);   //  Stop Pac Man's death animation.
		moveSpeed = DefaultMoveSpeed;   //  Pac Man's movement speed is back to it's default value.
		IsOutOfPortal = true;   //  Set Pac Man to be outside of any Portal.
		transform.position = SpawnPoint.transform.position; //  Pac Man will be moved to his spawn point.
		R();    //  Pac Man will turn right by default.
		PlaySound("AMBIENT");   //  Play the ambient sound.

		for (int i = 0; i < 4; i++)
		{
			GameObject Ghost = GhostHolder.GetChild(i).gameObject;
			GhostController GC = Ghost.GetComponent<GhostController>();
			GC.enabled = true;
			GC.ResetPositions();
			GhostMechanics GM = Ghost.GetComponent<GhostMechanics>();
			GM.enabled = true;
			GM.ResetPositions();
		}
	}

	#region Sound Controller

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

	#endregion

	#region The Movement of Pac Man

	/// <summary>
	/// The time used for lerping.
	/// </summary>
	float t;    //  The time for lerping.

	/// <summary>
	/// The movement of Pac Man by lerping.
	/// </summary>
	void Movement() //  This is called in the FixedUpdate().
	{
		//PacMan.MovePosition(transform.position + (transform.right * moveSpeed * Time.deltaTime));   //  Pac Man's constant movement.
		t += Time.fixedDeltaTime;
		float UD = 0f, LR = 0f;
		switch (currentInput)
		{
			case "U": UD = .5f; LR = 0f; break;
			case "D": UD = -.5f; LR = 0f; break;
			case "L": LR = -.5f; UD = 0f; break;
			case "R": LR = .5f; UD = 0f; break;
		}
		transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + LR, transform.position.y + UD, transform.position.z), t / (LevelGenerator.PIXEL_32 / (moveSpeed * .75f)));
		t = 0;
	}

	/// <summary>
	/// The keyboard inputs to control Pac Man's movement.
	/// </summary>

	void KeyboardControl()  //  These are the keyboard inputs from the user.
	{
		string lastInput = Input.inputString;

		#region WASD Controls

		switch (lastInput.ToUpper())
		{
			case "W": determineRotation("U"); break;
			case "S": determineRotation("D"); break;
			case "A": determineRotation("L"); break;
			case "D": determineRotation("R"); break;
		}

		#endregion

		#region ARROW KEY Controls

		if (Input.GetKey(KeyCode.UpArrow)) determineRotation("U");
		if (Input.GetKey(KeyCode.DownArrow)) determineRotation("D");
		if (Input.GetKey(KeyCode.LeftArrow)) determineRotation("L");
		if (Input.GetKey(KeyCode.RightArrow)) determineRotation("R");

		#endregion

		if (Input.GetKey(KeyCode.K))
			EatBigPellet(null);
	}

	/// <summary>
	/// Determine if Pac Man is allowed to move horizontally or vertically at any given moment.
	/// </summary>
	/// <param name="whereTo"></param>

	void determineRotation(string whereTo)
	{
		if (IsStill)    //  If Pac Man is at a standstill.
		{
			if (switcher.allowDirection(whereTo))  //  If the user enters a valid movement.
				Invoke(whereTo, 0f);    //  Turn in the valid movement.
			return; //  If Pac Man is at a standstill, do not execute anymore code.
		}

		if (LR) //  If Pac Man is moving left or right, ignore all corners and allow Pac Man to turn the 180 degree opposite direction, without the need of a switcher.
		{
			if (whereTo == "L") { L(); return; }
			if (whereTo == "R") { R(); return; }
		}
		else nextTurn(whereTo); //  Otherwise, make the next turn for Pac Man where the user wants to turn next. This will be done at the next switcher.

		if (!LR) //  If Pac Man is moving up or down, ignore all corners and allow Pac Man to turn the 180 degree opposite direction, without the need of a switcher.
		{
			if (whereTo == "U") { U(); return; }
			if (whereTo == "D") { D(); return; }
		}
		else nextTurn(whereTo); //  Otherwise, make the next turn for Pac Man where the user wants to turn next. This will be done at the next switcher.
	}

	/// <summary>
	/// On the next switcher move in whereTo direction.
	/// </summary>
	/// <param name="whereTo">The string direction on where to move on the next Switcher.</param>

	void nextTurn(string whereTo)
	{
		nextAvailableTurn = whereTo;    //  Sets the next turn at the next switcher to <whereTo> direction.
	}

	#region The Rotation of Pac Man

	void U()    //  Up turn.
	{
		rotateP(new Vector3(000f, 000f, 090f)); //  Rotates Pac Man to face up.

		//Debug.Log("Moving Up");
		PlayerStats.timesTurnedU++;

		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "U"; //  Set the current direction of Pac Man to up.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = false; //  Set Pac Man's movement so that he is NOT facing left or right.
	}

	void D()    //  Down turn.
	{
		rotateP(new Vector3(000f, 000f, 270f)); //  Rotates Pac Man to face down.

		//Debug.Log("Moving Down");
		PlayerStats.timesTurnedD++;

		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "D"; //  Set the current direction of Pac Man to down.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = false; //  Set Pac Man's movement so that he is NOT facing left or right.
	}

	void L()    //  Left turn.
	{
		rotateP(new Vector3(000f, 000f, 180f)); //  Rotates Pac Man to face left.

		//Debug.Log("Moving Left");
		PlayerStats.timesTurnedL++;

		//nextTurn("L");
		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "L"; //  Set the current direction of Pac Man to left.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = true; //  Set Pac Man's movement so that he IS facing left or right.
	}

	void R()    //  Right turn.
	{
		rotateP(new Vector3(000f, 000f, 000f)); //  Rotates Pac Man to face right.

		//Debug.Log("Moving Right");
		PlayerStats.timesTurnedR++;

		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "R"; //  Set the current direction of Pac Man to right.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = true; //  Set Pac Man's movement so that he IS facing left or right.
	}

	void rotateP(Vector3 faceAngle)
	{
		transform.localEulerAngles = faceAngle; //  Rotates Pac Man to face a direction.
	}

	#endregion

	/// <summary>
	/// Removes the next requested input input direction.
	/// </summary>

	void NullifyNextAvTurn()
	{
		nextAvailableTurn = null;   //  Sets the next turn to be null.
	}

	/// <summary>
	/// Stops Pac Man movement according to instruction.
	/// </summary>
	/// <param name="instruction">Boolean instruction on whether or not to stop Pac Man's movement.</param>

	void HoldPosition(bool instruction) //  Stop Pac Man from moving if an illegal turn is requested from the user or if Pac Man cannot continue to move in currentInput direction.
	{
		if (instruction)    //  True to hold position.
		{
			moveSpeed = 0f; //  Stop Pac Man.
			IsStill = true; //  Set Pac Man so that he is at a standstill.
			PlaySound("WALL");  //  Play the sound of Pac Man colliding with a wall.
			Destroy(Instantiate(WallBump, transform.position, Quaternion.identity), 2f);
			DeadStateAnim.speed = 0f;   //  Pauses the Pac Man walking animation.
			dust.Stop();    //  Stops the dust particles from playing.
			return; //  If Pac Man is set to be at a standstill, do not execute any more code.
		}

		if (IsAlive)    //  If Pac Man is alive.
			moveSpeed = DefaultMoveSpeed;   //  Reset Pac Man's movement speed.
		IsStill = false;    //  Set Pac Man so that he is no longer at a standstill.
		dust.Play();    //  Resumes the dust particles to plays.
		DeadStateAnim.speed = 1.5f; //  Restores the speed of Pac Man's walking animation.
	}

	#endregion
}
