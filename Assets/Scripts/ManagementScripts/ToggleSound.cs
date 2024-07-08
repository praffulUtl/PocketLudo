using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public Toggle toggle; // Reference to the UI Toggle for music
    public TMP_Text musicToggleText; // Reference to the TMP_Text for the music toggle
    public TMP_Text sfxToggleText; // Reference to the TMP_Text for the SFX toggle
    public Toggle toggleSFX; // Reference to the UI Toggle for SFX
    public static bool sfxbool = true; // Reference to the SFX toggle state

    void Start()
    {
        // Ensure the AudioSource is not playing at the start
        //audioSource.Stop();
        toggle.isOn = true;
        toggleSFX.isOn = true;

        // Add a listener to the toggle to call the ToggleAudio method when its value changes
        toggle.onValueChanged.AddListener(ToggleAudio);
        toggleSFX.onValueChanged.AddListener(ToggleSFX);

        // Initialize the text based on the initial toggle state
        UpdateMusicToggleText(toggle.isOn);
        UpdateSFXToggleText(toggleSFX.isOn);
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

        // Update the text for the music toggle
        UpdateMusicToggleText(isOn);
    }

    public void ToggleSFX(bool isOn)
    {
        sfxbool = isOn;

        // Update the text for the SFX toggle
        UpdateSFXToggleText(isOn);
    }

    void UpdateMusicToggleText(bool isOn)
    {
        musicToggleText.text = isOn ? "On" : "Off";
    }

    void UpdateSFXToggleText(bool isOn)
    {
        sfxToggleText.text = isOn ? "On" : "Off";
    }
}
