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

using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	public Sound[] Sounds;

	/// <summary>
	/// Populates the Sounds array to match the settings.
	/// </summary>

	void Awake()
	{
		foreach (Sound s in Sounds)
		{
			s.s = gameObject.AddComponent<AudioSource>();
			s.s.clip = s.Sounds;
			s.s.volume = s.Volume;
			s.s.pitch = s.Pitch;
			s.s.loop = s.Loop;
		}

		PlaySound("AMBIENT");   //  Plays the AMBIENT Pac Man sound by default.
	}

	/// <summary>
	/// Plays sound of name n.
	/// </summary>
	/// <param name="n">The name of the requested sound to play in capital casing.</param>

	public void PlaySound(string n)
	{
		Sound s = FindSound(n);
		if (s != null && !s.s.isPlaying)
			s.s.Play();
		if (s == null)
			Debug.LogWarning("Sound of name: " + n + " could not be played!");
	}

	/// <summary>
	/// Stops sound of name n.
	/// </summary>
	/// <param name="n">The name of the requested sound to stop playing in capital casing.</param>

	public void StopSound(string n)
	{
		Sound s = FindSound(n);
		if (s != null)
			s.s.Stop();
		if (s == null)
			Debug.LogWarning("Sound of name: " + n + " could not be stopped!");
	}

	/// <summary>
	/// Stop every sound in the game.
	/// </summary>

	public void StopAllSounds()
	{
		foreach (Sound s in Sounds)
			s.s.Stop();
	}

	/// <summary>
	/// Returns a sound in the Sounds array.
	/// </summary>
	/// <param name="n">The name of the requested sound.</param>
	/// <returns>The sound clip of the requested sound.</returns>

	Sound FindSound(string n)
	{
		return Array.Find(Sounds, sound => sound.Name == n);
	}
}
