using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public Toggle toggle; // Reference to the UI Toggle

    void Start()
    {
        // Ensure the AudioSource is not playing at the start
        audioSource.Stop();

        // Add a listener to the toggle to call the ToggleAudio method when its value changes
        toggle.onValueChanged.AddListener(ToggleAudio);
    }

    void ToggleAudio(bool isOn)
    {
        if (isOn)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
