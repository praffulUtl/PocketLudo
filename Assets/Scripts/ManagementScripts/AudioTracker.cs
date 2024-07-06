using System.Collections.Generic;
using UnityEngine;

public class AudioTracker : MonoBehaviour
{
    // List to store currently playing audio sources
    private List<AudioSource> playingAudioSources = new List<AudioSource>();

    void Update()
    {
        // Clear the list at the start of each frame
        playingAudioSources.Clear();

        // Find all active audio sources in the scene   
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        // Check each audio source to see if it is playing
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.isPlaying)
            {
                playingAudioSources.Add(audioSource);
            }
        }

        // Optionally, log the names of currently playing audio sources
        LogPlayingAudioSources();
    }

    // Method to log the currently playing audio sources
    void LogPlayingAudioSources()
    {
        string logMessage = "Currently playing audio sources: ";
        foreach (AudioSource audioSource in playingAudioSources)
        {
            logMessage += audioSource.gameObject.name + " ";
        }
        Debug.Log(logMessage);
    }
}
