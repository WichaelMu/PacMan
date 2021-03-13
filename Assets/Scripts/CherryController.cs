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

public class CherryController : MonoBehaviour
{
    /// <summary>
    /// The Cherry to be instantiated and lerped across the screen.
    /// </summary>
    public GameObject Cherry;
    /// <summary>
    ///  //  The time for lerping.
    /// </summary>
    float time;

    GameObject CurrentCherry;

    void Start()
    {
        InvokeRepeating("Refresh", 0f, 30f);    //  Repeat the Cherry lerping every 30 seconds, starting upon loading Level 1.
    }

    /// <summary>
    /// Lerps Cherry across the screen.
    /// </summary>

    void Update()
    {
        float delta = Mathf.Pow(((Time.time - time) / 5f), 3);
        if (CurrentCherry!=null)
            CurrentCherry.transform.position = Vector3.Lerp(transform.position, new Vector3(-20f, transform.position.y, transform.position.z), delta);
    }

    /// <summary>
    /// Instantiates Cherry and destroys Cherry after 10 seconds.
    /// </summary>

    void Refresh()
    {
        CurrentCherry = Instantiate(Cherry, Vector3.zero, Quaternion.identity);
        time = Time.time;
        Destroy(CurrentCherry, 10f);
    }
}
