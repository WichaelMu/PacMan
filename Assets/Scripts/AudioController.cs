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
            s.s.playOnAwake = s.PlayOnAwake;
        }
    }

    public void PlaySound(string n)
    {
        Sound s = Array.Find(Sounds, sound => sound.Name==n);
        if (s != null)
        {
            s.s.Play();
            return;
        }
        Debug.Log("Sound of name: " + n + " could not be found");
    }
}
