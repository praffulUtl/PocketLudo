using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour 
{
	[SerializeField] bool dummyMode = false;
	public static int howManyPlayers;
	public void two_player()
	{
		//SoundManagerScript.buttonAudioSource.Play ();
		howManyPlayers = 2;
		SceneManager.LoadScene ("Ludo1");
		//SceneManager.LoadScene("Game234Plr");
	}

	public void three_player()
	{
		//SoundManagerScript.buttonAudioSource.Play ();
		howManyPlayers = 3;
        SceneManager.LoadScene ("Ludo1");
        //SceneManager.LoadScene("Game234Plr");
    }

    public void four_player()
    {
        //SoundManagerScript.buttonAudioSource.Play ();
        howManyPlayers = 4;
        SceneManager.LoadScene("Ludo1");
        //SceneManager.LoadScene("Game234Plr");
    }

    public void four_player_online()
    {
        //SoundManagerScript.buttonAudioSource.Play ();
        howManyPlayers = 4;
        if (!dummyMode)
            SceneManager.LoadScene("Ludoonline");
        else
            SceneManager.LoadScene("Ludo1");
        //SceneManager.LoadScene("Game234Plr");
    }
    public void timerLudo()
    {
        //SoundManagerScript.buttonAudioSource.Play ();
        howManyPlayers = 4;
     
            SceneManager.LoadScene("TimerLudo");
        //else
        //    SceneManager.LoadScene("Ludo1");
        //SceneManager.LoadScene("Game234Plr");
    }
    public void vsComputer()
    {
        howManyPlayers = 2;
        SceneManager.LoadScene("Ludo2");
    }

    public void quit()
	{
		//SoundManagerScript.buttonAudioSource.Play ();
		Application.Quit ();
	}
	public void onClickRechargeWallet()
	{
        SceneManager.LoadScene("Recharge");
    }
	public void onClickMainGame()
	{
        SceneManager.LoadScene("mainMenu");
    }

}
