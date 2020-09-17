using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip Sounds;
    public string Name;

    [Range(0f, 1f)]
    public float Volume, Pitch=1;

    public bool Loop;

    [HideInInspector]
    public AudioSource s;
}
