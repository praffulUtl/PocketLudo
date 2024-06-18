using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class mainMenuManager : MonoBehaviour
{
    public static int playerNumber;

    public void twoPlayer()
    {
        soundManager.buttonAudioSource.Play();
        playerNumber = 2;
        SceneManager.LoadScene("gamePlay");
    }
    public void threePlayer()
    {
        soundManager.buttonAudioSource.Play();
        playerNumber = 3;
        SceneManager.LoadScene("gamePlay");
    }
    public void fourPlayer()
    {
        soundManager.buttonAudioSource.Play();
        playerNumber = 4;
        SceneManager.LoadScene("gamePlay");
    }
    public void exitf()
    {
        //soundManager.buttonAudioSource.Play();
        //if (EditorApplication.isPlaying)
        //    EditorApplication.isPlaying = false;
    }
}
