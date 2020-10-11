using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public Sound[] Sounds;

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

        PlaySound("AMBIENT");
    }

    /// <summary>
    /// Plays sound of name n.
    /// </summary>
    /// <param name="n">The name of the requested sound to play in capital casing.</param>

    public void PlaySound(string n)
    {
        Sound s = FindSound(n);
        if (s != null&&!s.s.isPlaying)
            s.s.Play();
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
