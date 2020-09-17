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

    public void PlaySound(string n)
    {
        Sound s = FindSound(n);
        if (s != null&&!s.s.isPlaying)
            s.s.Play();
    }

    public void StopSound(string n)
    {
        Sound s = FindSound(n);
        if (s != null)
            s.s.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound s in Sounds)
            s.s.Stop();
    }

    Sound FindSound(string n)
    {
        return Array.Find(Sounds, sound => sound.Name == n);
    }
}
