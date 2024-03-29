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

public class PlayerControllers : MonoBehaviour
{
	[Header("Player ID")]
	public int ID;

	[Header("Weapons")]
	public Dynamite Dynamite;

	[Header("Life Controller")]
	public GameControllerII GameController;

	[Header("Portals")]
	[SerializeField]
	Transform PortalL;
	[SerializeField]
	Transform PortalR;

	Rigidbody pRB;
	Switcher switcher;
	AudioController AudioControl;

	public float moveSpeed;

	Vector3 direction = Vector3.right;
	float DefaultMoveSpeed;
	bool LR = false, IsStill, IsOutOfPortal = true, IsAlive;
	string nextAvailableTurn, currentInput;
	int collected = 1;

	void Awake()
	{
		pRB = GetComponent<Rigidbody>();
		AudioControl = FindObjectOfType<AudioController>();
		GameController = FindObjectOfType<GameControllerII>();
	}

	void Start()
	{
		DefaultMoveSpeed = moveSpeed;

		ResetGame();

		PortalL = GameObject.FindWithTag("PortalL").transform;
		PortalR = GameObject.FindWithTag("PortalR").transform;
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
		else if (other.CompareTag("Pellet")) //  When Pac Man hits a Pellet.
			EatPellet(other.gameObject);
		else if (other.CompareTag("BigPellet"))  //  WHen Pac Man hits a Big Pellet.
			EatBigPellet(other.gameObject);
		else if (other.CompareTag("PortalL") || other.CompareTag("PortalR")) //  When Pac Man enters a Portal.
			PortalHandler(other.name);
		if (other.CompareTag("Explosion") || other.CompareTag("ExplosionGHOST"))
			KillPlayer();
		if (other.CompareTag("Ghost"))
			KillPlayer(other.gameObject);
	}

	void OnSwitcher(GameObject Switcher)
	{
		this.switcher = Switcher.GetComponent<Switcher>(); //  Sets the corner Pac Man is on to <this.switcher>. This corner is accessed if Pac Man is stuck on a corner.

		if (switcher.allowDirection(nextAvailableTurn)) //  If this switcher allows Pac Man to turn in the direction of <nextAvailableTurn>, it will turn in that direction.
			Invoke(nextAvailableTurn, 0f);

		if (!switcher.allowDirection(currentInput)) //  If this switcher does not allow Pac Man to turn in the direction of <nextAvailableTurn>, it will stop Pac Man's movement in that corner until the play tells Pac Man to move in a direction that *is* allowed by this switcher.
			HoldPosition(true);
	}

	void EatPellet(GameObject Pellet)
	{
		Destroy(Pellet);    //  Remove the Pellet from the game.

		GameController.UpdateScore(ID, 10);

		if (collected % 50 == 0)
			collected++;

		PlaySound("EATPELLET"); //  Play the sound of Pac Man eating a Pellet.
	}
	void EatBigPellet(GameObject BigPellet)
	{
		Destroy(BigPellet); //  Remove the Big Pellet from the game.

		GameController.UpdateScore(ID, 50);
		collected++;

		GameObject[] Ghosts = GameObject.FindGameObjectsWithTag("Ghost");
		foreach (GameObject g in Ghosts)
			if (g.GetComponent<InnovationAI>().SpecifyTarget == ID)
				Destroy(g);

		PlaySound("EATFRUIT");  //  Play the sound of Pac Man eating a Fruit/Big Pellet.
	}

	void PortalHandler(string Portal)   //  There is a better way in doing this. In free-time, find a solution.
	{
		if (Portal.Equals("PortalL(Clone)") && IsOutOfPortal)   //  If PacMan goes into the Left Portal and Pac Man is, in fact, out of any Portal, it will move Pac Man's position to the Right Portal.
			transform.position = PortalR.position;
		else if (Portal.Equals("PortalR(Clone)") && IsOutOfPortal)   //  If PacMan goes into the Right Portal and Pac Man is, in fact, out of any Portal, it will move Pac Man's position to the Left Portal.
			transform.position = PortalL.position;
		IsOutOfPortal = !IsOutOfPortal; //  Flip if Pac Man is out of a Portal. Pac Man will hit the opposite Portal when leaving the first. It will flip twice during one Portal movement. This is needed as, without it, Pac Man will endlessly go back and forth an infinite number of times.
	}

	void ResetGame()
	{
		IsAlive = true; //  Sets Pac Man to be alive.
		moveSpeed = DefaultMoveSpeed;   //  Pac Man's movement speed is back to it's default value.
		IsOutOfPortal = true;   //  Set Pac Man to be outside of any Portal.
		if (ID == 1)
			transform.position = GameObject.FindWithTag("RED;SP").transform.position;
		if (ID == 2)
			transform.position = GameObject.FindWithTag("BLUE;SP").transform.position;
		R();    //  Pac Man will turn right by default.
		PlaySound("AMBIENT");   //  Play the ambient sound.
	}

	void DropDynamite()
	{
		Dynamite dynamite = Instantiate(Dynamite, new Vector3(transform.position.x, transform.position.y, -.05f), transform.rotation);
		dynamite.SetMaximumRange(collected);
	}

	void KillPlayer(GameObject ghost = null)
	{
		Destroy(gameObject);
		if (ghost == null)
			GameController.EndGame(ID);
		else
			GameController.EndGame(ID, ghost);
	}

	#region Sound Controller

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

	#region The Movement of Pac Man

	//float t;

	void Movement() //  This is called in the FixedUpdate().
	{
		pRB.MovePosition(transform.position + (direction * moveSpeed * Time.deltaTime));   //  Pac Man's constant movement.
	}

	void KeyboardControl()  //  These are the keyboard inputs from the user.
	{
		string lastInput = Input.inputString;

		if (Input.GetKey(KeyCode.K))
			EatBigPellet(null);

		#region P1 Controls

		if (ID == 1)
		{
			switch (lastInput.ToUpper())
			{
				case "W":
					determineRotation("U");
					break;
				case "S":
					determineRotation("D");
					break;
				case "A":
					determineRotation("L");
					break;
				case "D":
					determineRotation("R");
					break;
				case " ":
					DropDynamite();
					break;
			}
			return;
		}

		#endregion

		#region P2 Controls

		if (ID == 2)
		{
			if (Input.GetKey(KeyCode.UpArrow)) determineRotation("U");
			if (Input.GetKey(KeyCode.DownArrow)) determineRotation("D");
			if (Input.GetKey(KeyCode.LeftArrow)) determineRotation("L");
			if (Input.GetKey(KeyCode.RightArrow)) determineRotation("R");
			if (Input.GetKeyDown(KeyCode.Slash)) DropDynamite();
			return;
		}

		#endregion

	}

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
	void nextTurn(string whereTo)
	{
		nextAvailableTurn = whereTo;    //  Sets the next turn at the next switcher to <whereTo> direction.
	}

	#region The Rotation of Pac Man

	void U()    //  Up turn.
	{
		direction = Vector3.up;

		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "U"; //  Set the current direction of Pac Man to up.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = false; //  Set Pac Man's movement so that he is NOT facing left or right.
	}

	void D()    //  Down turn.
	{
		direction = -Vector3.up;

		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "D"; //  Set the current direction of Pac Man to down.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = false; //  Set Pac Man's movement so that he is NOT facing left or right.
	}

	void L()    //  Left turn.
	{
		direction = -Vector3.right;

		//nextTurn("L");
		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "L"; //  Set the current direction of Pac Man to left.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = true; //  Set Pac Man's movement so that he IS facing left or right.
	}

	void R()    //  Right turn.
	{
		direction = Vector3.right;

		NullifyNextAvTurn();    //  Set the next turn to null.
		currentInput = "R"; //  Set the current direction of Pac Man to right.

		HoldPosition(false);    //  Set Pac Man so that he is no longer at a standstill.
		LR = true; //  Set Pac Man's movement so that he IS facing left or right.
	}

	#endregion

	void NullifyNextAvTurn()
	{
		nextAvailableTurn = null;   //  Sets the next turn to be null.
	}

	void HoldPosition(bool instruction) //  Stop Pac Man from moving if an illegal turn is requested from the user.
	{
		if (instruction)
		{
			moveSpeed = 0f; //  Stop Pac Man.
			IsStill = true; //  Set Pac Man so that he is at a standstill.
			return; //  If Pac Man is set to be at a standstill, do not execute any more code.
		}

		if (IsAlive)    //  If Pac Man is alive.
			moveSpeed = DefaultMoveSpeed;   //  Reset Pac Man's movement speed.
		IsStill = false;    //  Set Pac Man so that he is no longer at a standstill.
	}

	#endregion
}
