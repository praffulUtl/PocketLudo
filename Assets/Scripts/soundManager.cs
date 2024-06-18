using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    public AudioClip buttonAudioClip;
    public AudioClip dismissalAudioClip;
    public AudioClip diceAudioClip;
    public AudioClip winAudioClip;
    public AudioClip safeAudioClip;
    public AudioClip playerAudioClip;

    public static AudioSource buttonAudioSource;
    public static AudioSource dismissalAudioSource;
    public static AudioSource diceAudioSource;
    public static AudioSource winAudioSource;
    public static AudioSource safeAudioSource;
    public static AudioSource playerAudioSource;

    AudioSource addAudio(AudioClip clip, bool playOnAwake, bool loop, float volume)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.playOnAwake = playOnAwake;
        audioSource.loop = loop;
        audioSource.volume = volume;
        return audioSource;
    }
    // Start is called before the first frame update
    void Start()
    {
        buttonAudioSource = addAudio(buttonAudioClip, false, false, 1.0f);
        dismissalAudioSource = addAudio(dismissalAudioClip, false, false, 1.0f);
        diceAudioSource = addAudio(diceAudioClip, false, false, 1.0f);
        winAudioSource = addAudio(winAudioClip, false, false, 1.0f);
        safeAudioSource = addAudio(safeAudioClip, false, false, 1.0f);
        playerAudioSource = addAudio(playerAudioClip, false, false, 1.0f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
