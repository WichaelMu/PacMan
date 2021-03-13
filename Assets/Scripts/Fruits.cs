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

public class Fruits : MonoBehaviour
{
    GameObject[] FruitsHolder;  //  The GameObject array that stores the fruits that have been collected by Pac Man in the order of collection.
    Sprite SPR; //  The sprite for this fruit.

    static int i = 0;   //  The static index used to indicate a Fruit's collection in the UI.

    void Awake()
    {
        SPR = GetComponent<SpriteRenderer>().sprite;    //  Assigns the sprite for this fruit to SPR.
        FruitsHolder = GameObject.FindGameObjectsWithTag("FRUITHOLDER");    //  Finds the FruitsHolder game object in the scene.
        Destroy(gameObject, 15f);   //  Upon spawning this fruit, destroy it after 15 seconds if Pac Man does not collect it.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //  If this fruit collides with Pac Man.
            EatenByPacman();
    }

    /// <summary>
    /// THe procedure if this Fruit is collected by Pac Man.
    /// </summary>

    void EatenByPacman()
    {
        SpriteRenderer spr = FruitsHolder[i].GetComponent<SpriteRenderer>();    //  Gets the sprite renderer for the game object at FruitsHolder[i].
        spr.sprite = SPR;   //  Change the sprite renderer for the game object at FruitsHolder[i] to be the collected Fruit's sprite.
        i++;    //  Increment the index.s

        Destroy(gameObject);    //  Destroy this Fruit.
    }
}
