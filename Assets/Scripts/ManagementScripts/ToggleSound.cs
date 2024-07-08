using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public Toggle toggle; // Reference to the UI Toggle
    public Toggle toggleSFX;
    public static bool sfxbool = true;// Reference to the UI Toggle

    void Start()
    {
        // Ensure the AudioSource is not playing at the start
        //audioSource.Stop();
        toggle.isOn = true;

        // Add a listener to the toggle to call the ToggleAudio method when its value changes
        toggle.onValueChanged.AddListener(ToggleAudio);
        toggleSFX.onValueChanged.AddListener(ToggleSFX);
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
    public void ToggleSFX(bool sfxbool)
    {
        sfxbool = !sfxbool;
    }
}
