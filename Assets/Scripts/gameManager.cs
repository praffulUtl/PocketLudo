using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    private int totalRedInHouse, totalGreenInHouse, totalblueInHouse, totalyellowInHouse;
    public GameObject frameRed, frameBlue, frameGreen, frameYellow;
    public GameObject redPlayerBorder1, redPlayerBorder2, redPlayerBorder3, redPlayerBorder4;
    public GameObject greenPlayerBorder1, greenPlayerBorder2, greenPlayerBorder3, greenPlayerBorder4;
    public GameObject bluePlayerBorder1, bluePlayerBorder2, bluePlayerBorder3, bluePlayerBorder4;
    public GameObject yellowPlayerBorder1, yellowPlayerBorder2, yellowPlayerBorder3, yellowPlayerBorder4;

    public Vector3 redPlayerPos1, redPlayerPos2, redPlayerPos3, redPlayerPos4;
    public Vector3 greenPlayerPos1, greenPlayerPos2, greenPlayerPos3, greenPlayerPos4;
    public Vector3 bluePlayerPos1, bluePlayerPos2, bluePlayerPos3, bluePlayerPos4;
    public Vector3 yellowPlayerPos1, yellowPlayerPos2, yellowPlayerPos3, yellowPlayerPos4;

    public Button diceRollButton;

    public Transform diceRoll;
    public Transform redDiceRoll, greenDiceRoll, blueDiceRoll, yellowDiceRoll;

    public Button redPlayerButton1, redPlayerButton2, redPlayerButton3, redPlayerButton4;
    public Button greenPlayerButton1, greenPlayerButton2, greenPlayerButton3, greenPlayerButton4;
    public Button bluePlayerButton1, bluePlayerButton2, bluePlayerButton3, bluePlayerButton4;
    public Button yellowPlayerButton1, yellowPlayerButton2, yellowPlayerButton3, yellowPlayerButton4;

    public GameObject blueScreen, greenScreen, redScreen, yellowScreen;

    private string playerTurn = "red";
    private string currentPlayer = "none";
    private string currentPlayerName = "none";

    public GameObject redPlayer1, redPlayer2, redPlayer3, redPlayer4;
    public GameObject bluePlayer1, bluePlayer2, bluePlayer3, bluePlayer4;
    public GameObject greenPlayer1, greenPlayer2, greenPlayer3, greenPlayer4;
    public GameObject yellowPlayer1, yellowPlayer2, yellowPlayer3, yellowPlayer4;

    private int redPlayerSteps1, redPlayerSteps2, redPlayerSteps3, redPlayerSteps4;
    private int bluePlayerSteps1, bluePlayerSteps2, bluePlayerSteps3, bluePlayerSteps4;
    private int greenPlayerSteps1, greenPlayerSteps2, greenPlayerSteps3, greenPlayerSteps4;
    private int yellowPlayerSteps1, yellowPlayerSteps2, yellowPlayerSteps3, yellowPlayerSteps4;

    private int selectDiceAnimation;

    public GameObject diceRollAnimation1, diceRollAnimation2, diceRollAnimation3, diceRollAnimation4, diceRollAnimation5, diceRollAnimation6;

    public List<GameObject> redMovementBlock = new List<GameObject>();
    public List<GameObject> greenMovementBlock = new List<GameObject>();
    public List<GameObject> blueMovementBlock = new List<GameObject>();
    public List<GameObject> yellowMovementBlock = new List<GameObject>();

    public GameObject confirmScreen;
    public GameObject gameCompletedScreen;

    private System.Random randNo;

    public void exitcall()
    {
        soundManager.buttonAudioSource.Play();
        SceneManager.LoadScene("mainMenu");
    }

    IEnumerator GameCompletedCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        gameCompletedScreen.SetActive(true);

    }
    public void playAgain()
    {
        soundManager.buttonAudioSource.Play();
        SceneManager.LoadScene("gamePlay");
    }

    private void inializeDice()
    {
       
        diceRollButton.interactable = true;
        diceRollAnimation1.SetActive(false);
        diceRollAnimation2.SetActive(false);
        diceRollAnimation3.SetActive(false);
        diceRollAnimation4.SetActive(false);
        diceRollAnimation5.SetActive(false);
        diceRollAnimation6.SetActive(false);

        //===================================game complete
        if (mainMenuManager.playerNumber == 2)
        {
            if (totalRedInHouse > 3)
            {
                soundManager.winAudioSource.Play();
                redScreen.SetActive(true);
                StartCoroutine("GameCompletedCoroutine");
                playerTurn = "none";
            }
            else if (totalGreenInHouse > 3)
            {
                soundManager.winAudioSource.Play();
                greenScreen.SetActive(true);
                StartCoroutine("GameCompletedCoroutine");
                playerTurn = "none";
            }
        }
        else if (mainMenuManager.playerNumber == 4)
        {
            //==4player left at 4 players mode
            if (totalRedInHouse > 3 && totalblueInHouse < 4 && totalGreenInHouse < 4 && totalyellowInHouse < 4 && playerTurn == "red")
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                playerTurn = "blue";
            }
            if (totalblueInHouse > 3 && totalRedInHouse < 4 && totalGreenInHouse < 4 && totalyellowInHouse < 4 && playerTurn == "blue")
            {
                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                blueScreen.SetActive(true);
                playerTurn = "green";
            }
            if (totalGreenInHouse > 3 && totalRedInHouse < 4 && totalblueInHouse < 4 && totalyellowInHouse < 4 && playerTurn == "green")
            {
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                greenScreen.SetActive(true);
                playerTurn = "yellow";
            }
            if (totalyellowInHouse > 3 && totalRedInHouse < 4 && totalblueInHouse < 4 && totalGreenInHouse < 4 && playerTurn == "yellow")
            {
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                yellowScreen.SetActive(true);
                playerTurn = "red";
            }
            //===3players left at 4 players mode
            // red+blue completed
            if (totalRedInHouse > 3 && totalblueInHouse > 3 && totalGreenInHouse < 4 && totalyellowInHouse < 4 && playerTurn == "red" || playerTurn == "blue")
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                blueScreen.SetActive(true);
                playerTurn = "green";
            }
            //red+green completed
            if (totalRedInHouse > 3 && totalblueInHouse < 4 && totalGreenInHouse > 3 && totalyellowInHouse < 4 && playerTurn == "red" || playerTurn == "green")
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                greenScreen.SetActive(true);
                if (playerTurn == "red")
                    playerTurn = "blue";
                else if (playerTurn == "green")
                    playerTurn = "yellow";
            }
            //red+yellow completed
            if (totalRedInHouse > 3 && totalblueInHouse < 4 && totalGreenInHouse < 4 && totalyellowInHouse > 3 && playerTurn == "red" || playerTurn == "yellow")
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                yellowScreen.SetActive(true);
                playerTurn = "blue";
            }
            //blue+green completed
            if (totalRedInHouse < 4 && totalblueInHouse > 3 && totalGreenInHouse > 3 && totalyellowInHouse < 4 && playerTurn == "blue" || playerTurn == "green")
            {
                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                blueScreen.SetActive(true);
                greenScreen.SetActive(true);
                playerTurn = "yellow";
            }
            //blue+yellow completed
            if (totalRedInHouse < 4 && totalblueInHouse > 3 && totalGreenInHouse < 4 && totalyellowInHouse > 3 && playerTurn == "blue" || playerTurn == "yellow")
            {
                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                blueScreen.SetActive(true);
                yellowScreen.SetActive(true);
                if (playerTurn == "blue")
                    playerTurn = "green";
                else if (playerTurn == "yellow")
                    playerTurn = "red";
            }
            //green+yellow completed
            if (totalRedInHouse < 4 && totalblueInHouse < 4 && totalGreenInHouse > 3 && totalyellowInHouse > 3 && playerTurn == "green" || playerTurn == "yellow")
            {
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                greenScreen.SetActive(true);
                yellowScreen.SetActive(true);
                playerTurn = "red";
            }
            //==========game completed by three
            //yellow loose
            if (totalRedInHouse > 3 && totalblueInHouse > 3 && totalGreenInHouse > 3 && totalyellowInHouse < 4)
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                blueScreen.SetActive(true);
                greenScreen.SetActive(true);
                StartCoroutine(GameCompletedCoroutine());
                playerTurn = "none";
            }
            //green loose
            if (totalRedInHouse > 3 && totalblueInHouse > 3 && totalGreenInHouse < 4 && totalyellowInHouse > 3)
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                blueScreen.SetActive(true);
                yellowScreen.SetActive(true);
                StartCoroutine(GameCompletedCoroutine());
                playerTurn = "none";
            }
            //blue loose
            if (totalRedInHouse > 3 && totalblueInHouse < 4 && totalGreenInHouse > 3 && totalyellowInHouse > 3)
            {
                if (!redScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                redScreen.SetActive(true);
                greenScreen.SetActive(true);
                yellowScreen.SetActive(true);
                StartCoroutine(GameCompletedCoroutine());
                playerTurn = "none";
            }
            //red loose
            if (totalRedInHouse < 4 && totalblueInHouse > 3 && totalGreenInHouse > 3 && totalyellowInHouse > 3)
            {

                if (!blueScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!greenScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                if (!yellowScreen.activeInHierarchy)
                {
                    soundManager.winAudioSource.Play();
                }
                blueScreen.SetActive(true);
                greenScreen.SetActive(true);
                yellowScreen.SetActive(true);
                StartCoroutine(GameCompletedCoroutine());
                playerTurn = "none";
            }
        }

        //current player pos value (need to cut)
        if (currentPlayerName.Contains("red player"))
        {
            if (currentPlayerName == "red player 1")
            {
                currentPlayer = playerRed1.redPlayerColName1;
            }
            else if (currentPlayerName == "red player 2")
            {
                currentPlayer = playerRed2.redPlayerColName2;
            }
            else if (currentPlayerName == "red player 3")
            {
                currentPlayer = playerRed3.redPlayerColName3;
            }
            else if (currentPlayerName == "red player 4")
            {
                currentPlayer = playerRed4.redPlayerColName4;
            }
        }
        if (currentPlayerName.Contains("green player"))
        {
            if (currentPlayerName == "green player 1")
            {
                currentPlayer = playerGreen1.greenPlayerColName1;
            }
            else if (currentPlayerName == "green player 2")
            {
                currentPlayer = playerGreen2.greenPlayerColName2;
            }
            else if (currentPlayerName == "green player 3")
            {
                currentPlayer = playerGreen3.greenPlayerColName3;
            }
            else if (currentPlayerName == "green player 4")
            {
                currentPlayer = playerGreen4.greenPlayerColName4;
            }
        }
        if (currentPlayerName.Contains("blue player"))
        {
            if (currentPlayerName == "blue player 1")
            {
                currentPlayer = playerBlue1.bluePlayerColName1;
            }
            else if (currentPlayerName == "blue player 2")
            {
                currentPlayer = playerBlue2.bluePlayerColName2;
            }
            else if (currentPlayerName == "blue player 3")
            {
                currentPlayer = playerBlue3.bluePlayerColName3;
            }
            else if (currentPlayerName == "blue player 4")
            {
                currentPlayer = playerBlue4.bluePlayerColName4;
            }
        }
        if (currentPlayerName.Contains("yellow player"))
        {
            if (currentPlayerName == "yellow player 1")
            {
                currentPlayer = playerYellow1.yellowPlayerColName1;
            }
            else if (currentPlayerName == "yellow player 2")
            {
                currentPlayer = playerYellow2.yellowPlayerColName2;
            }
            else if (currentPlayerName == "yellow player 3")
            {
                currentPlayer = playerYellow3.yellowPlayerColName3;
            }
            else if (currentPlayerName == "yellow player 4")
            {
                currentPlayer = playerYellow4.yellowPlayerColName4;
            }
        }
        //======================cut/kill player
        if (currentPlayerName != "none")
        {
            if (mainMenuManager.playerNumber == 2)
            {
                //====red is killer
                if (currentPlayerName.Contains("red player"))
                {
                    if (currentPlayer == playerGreen1.greenPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer1.transform.position = greenPlayerPos1;
                        playerGreen1.greenPlayerColName1 = "none";
                        greenPlayerSteps1 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerGreen2.greenPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer2.transform.position = greenPlayerPos2;
                        playerGreen2.greenPlayerColName2 = "none";
                        greenPlayerSteps2 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerGreen3.greenPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer3.transform.position = greenPlayerPos3;
                        playerGreen3.greenPlayerColName3 = "none";
                        greenPlayerSteps3 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerGreen4.greenPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer4.transform.position = greenPlayerPos4;
                        playerGreen4.greenPlayerColName4 = "none";
                        greenPlayerSteps4 = 0;
                        playerTurn = "red";
                    }
                }
                //====green is killer
                if (currentPlayerName.Contains("green player"))
                {
                    if (currentPlayer == playerRed1.redPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer1.transform.position = redPlayerPos1;
                        playerRed1.redPlayerColName1 = "none";
                        redPlayerSteps1 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerRed2.redPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer2.transform.position = redPlayerPos2;
                        playerRed2.redPlayerColName2 = "none";
                        redPlayerSteps2 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerRed3.redPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer3.transform.position = redPlayerPos3;
                        playerRed3.redPlayerColName3 = "none";
                        redPlayerSteps3 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerRed4.redPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer4.transform.position = redPlayerPos4;
                        playerRed4.redPlayerColName4 = "none";
                        redPlayerSteps4 = 0;
                        playerTurn = "green";
                    }
                }
            }
            //======4 player mode kill
            else if (mainMenuManager.playerNumber == 4)
            {
                //============== red is killer at 4 players mode
                if (currentPlayerName.Contains("red player"))
                {
                    //green
                    if (currentPlayer == playerGreen1.greenPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer1.transform.position = greenPlayerPos1;
                        playerGreen1.greenPlayerColName1 = "none";
                        greenPlayerSteps1 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerGreen2.greenPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer2.transform.position = greenPlayerPos2;
                        playerGreen2.greenPlayerColName2 = "none";
                        greenPlayerSteps2 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerGreen3.greenPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer3.transform.position = greenPlayerPos3;
                        playerGreen3.greenPlayerColName3 = "none";
                        greenPlayerSteps3 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerGreen4.greenPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer4.transform.position = greenPlayerPos4;
                        playerGreen4.greenPlayerColName4 = "none";
                        greenPlayerSteps4 = 0;
                        playerTurn = "red";
                    }
                    //blue
                    if (currentPlayer == playerBlue1.bluePlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer1.transform.position = bluePlayerPos1;
                        playerBlue1.bluePlayerColName1 = "none";
                        bluePlayerSteps1 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerBlue2.bluePlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer2.transform.position = bluePlayerPos2;
                        playerBlue2.bluePlayerColName2 = "none";
                        bluePlayerSteps2 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerBlue3.bluePlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer3.transform.position = bluePlayerPos3;
                        playerBlue3.bluePlayerColName3 = "none";
                        bluePlayerSteps3 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerBlue4.bluePlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer4.transform.position = bluePlayerPos4;
                        playerBlue4.bluePlayerColName4 = "none";
                        bluePlayerSteps4 = 0;
                        playerTurn = "red";
                    }
                    //yellow
                    if (currentPlayer == playerYellow1.yellowPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer1.transform.position = yellowPlayerPos1;
                        playerYellow1.yellowPlayerColName1 = "none";
                        yellowPlayerSteps1 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerYellow2.yellowPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer2.transform.position = yellowPlayerPos2;
                        playerYellow2.yellowPlayerColName2 = "none";
                        yellowPlayerSteps2 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerYellow3.yellowPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer3.transform.position = yellowPlayerPos3;
                        playerYellow3.yellowPlayerColName3 = "none";
                        yellowPlayerSteps3 = 0;
                        playerTurn = "red";
                    }
                    if (currentPlayer == playerYellow4.yellowPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer4.transform.position = yellowPlayerPos4;
                        playerYellow4.yellowPlayerColName4 = "none";
                        yellowPlayerSteps4 = 0;
                        playerTurn = "red";
                    }
                }
                //========blue is killer at 4players mode
                if (currentPlayerName.Contains("blue player"))
                {
                    //green
                    if (currentPlayer == playerGreen1.greenPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer1.transform.position = greenPlayerPos1;
                        playerGreen1.greenPlayerColName1 = "none";
                        greenPlayerSteps1 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerGreen2.greenPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer2.transform.position = greenPlayerPos2;
                        playerGreen2.greenPlayerColName2 = "none";
                        greenPlayerSteps2 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerGreen3.greenPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer3.transform.position = greenPlayerPos3;
                        playerGreen3.greenPlayerColName3 = "none";
                        greenPlayerSteps3 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerGreen4.greenPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer4.transform.position = greenPlayerPos4;
                        playerGreen4.greenPlayerColName4 = "none";
                        greenPlayerSteps4 = 0;
                        playerTurn = "blue";
                    }
                    //red
                    if (currentPlayer == playerRed1.redPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer1.transform.position = redPlayerPos1;
                        playerRed1.redPlayerColName1 = "none";
                        redPlayerSteps1 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerRed2.redPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer2.transform.position = redPlayerPos2;
                        playerRed2.redPlayerColName2 = "none";
                        redPlayerSteps2 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerRed3.redPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer3.transform.position = redPlayerPos3;
                        playerRed3.redPlayerColName3 = "none";
                        redPlayerSteps3 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerRed4.redPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer4.transform.position = redPlayerPos4;
                        playerRed4.redPlayerColName4 = "none";
                        redPlayerSteps4 = 0;
                        playerTurn = "blue";
                    }
                    //yellow
                    if (currentPlayer == playerYellow1.yellowPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer1.transform.position = yellowPlayerPos1;
                        playerYellow1.yellowPlayerColName1 = "none";
                        yellowPlayerSteps1 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerYellow2.yellowPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer2.transform.position = yellowPlayerPos2;
                        playerYellow2.yellowPlayerColName2 = "none";
                        yellowPlayerSteps2 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerYellow3.yellowPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer3.transform.position = yellowPlayerPos3;
                        playerYellow3.yellowPlayerColName3 = "none";
                        yellowPlayerSteps3 = 0;
                        playerTurn = "blue";
                    }
                    if (currentPlayer == playerYellow4.yellowPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer4.transform.position = yellowPlayerPos4;
                        playerYellow4.yellowPlayerColName4 = "none";
                        yellowPlayerSteps4 = 0;
                        playerTurn = "blue";
                    }
                }
                //===========green is killer at 4 player mode
                if (currentPlayerName.Contains("green player"))
                {
                    //red
                    if (currentPlayer == playerRed1.redPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer1.transform.position = redPlayerPos1;
                        playerRed1.redPlayerColName1 = "none";
                        redPlayerSteps1 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerRed2.redPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer2.transform.position = redPlayerPos2;
                        playerRed2.redPlayerColName2 = "none";
                        redPlayerSteps2 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerRed3.redPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer3.transform.position = redPlayerPos3;
                        playerRed3.redPlayerColName3 = "none";
                        redPlayerSteps3 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerRed4.redPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer4.transform.position = redPlayerPos4;
                        playerRed4.redPlayerColName4 = "none";
                        redPlayerSteps4 = 0;
                        playerTurn = "green";
                    }
                    //blue
                    if (currentPlayer == playerBlue1.bluePlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer1.transform.position = bluePlayerPos1;
                        playerBlue1.bluePlayerColName1 = "none";
                        bluePlayerSteps1 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerBlue2.bluePlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer2.transform.position = bluePlayerPos2;
                        playerBlue2.bluePlayerColName2 = "none";
                        bluePlayerSteps2 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerBlue3.bluePlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer3.transform.position = bluePlayerPos3;
                        playerBlue3.bluePlayerColName3 = "none";
                        bluePlayerSteps3 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerBlue4.bluePlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer4.transform.position = bluePlayerPos4;
                        playerBlue4.bluePlayerColName4 = "none";
                        bluePlayerSteps4 = 0;
                        playerTurn = "green";
                    }
                    //yellow
                    if (currentPlayer == playerYellow1.yellowPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer1.transform.position = yellowPlayerPos1;
                        playerYellow1.yellowPlayerColName1 = "none";
                        yellowPlayerSteps1 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerYellow2.yellowPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer2.transform.position = yellowPlayerPos2;
                        playerYellow2.yellowPlayerColName2 = "none";
                        yellowPlayerSteps2 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerYellow3.yellowPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer3.transform.position = yellowPlayerPos3;
                        playerYellow3.yellowPlayerColName3 = "none";
                        yellowPlayerSteps3 = 0;
                        playerTurn = "green";
                    }
                    if (currentPlayer == playerYellow4.yellowPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        yellowPlayer4.transform.position = yellowPlayerPos4;
                        playerYellow4.yellowPlayerColName4 = "none";
                        yellowPlayerSteps4 = 0;
                        playerTurn = "green";
                    }
                }
                //=========yellow is killer at 4 players mode
                if (currentPlayerName.Contains("yellow player"))
                {
                    //red
                    if (currentPlayer == playerRed1.redPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer1.transform.position = redPlayerPos1;
                        playerRed1.redPlayerColName1 = "none";
                        redPlayerSteps1 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerRed2.redPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer2.transform.position = redPlayerPos2;
                        playerRed2.redPlayerColName2 = "none";
                        redPlayerSteps2 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerRed3.redPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer3.transform.position = redPlayerPos3;
                        playerRed3.redPlayerColName3 = "none";
                        redPlayerSteps3 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerRed4.redPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        redPlayer4.transform.position = redPlayerPos4;
                        playerRed4.redPlayerColName4 = "none";
                        redPlayerSteps4 = 0;
                        playerTurn = "yellow";
                    }
                    //blue
                    if (currentPlayer == playerBlue1.bluePlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer1.transform.position = bluePlayerPos1;
                        playerBlue1.bluePlayerColName1 = "none";
                        bluePlayerSteps1 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerBlue2.bluePlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer2.transform.position = bluePlayerPos2;
                        playerBlue2.bluePlayerColName2 = "none";
                        bluePlayerSteps2 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerBlue3.bluePlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer3.transform.position = bluePlayerPos3;
                        playerBlue3.bluePlayerColName3 = "none";
                        bluePlayerSteps3 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerBlue4.bluePlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        bluePlayer4.transform.position = bluePlayerPos4;
                        playerBlue4.bluePlayerColName4 = "none";
                        bluePlayerSteps4 = 0;
                        playerTurn = "yellow";
                    }
                    //green
                    if (currentPlayer == playerGreen1.greenPlayerColName1 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer1.transform.position = greenPlayerPos1;
                        playerGreen1.greenPlayerColName1 = "none";
                        greenPlayerSteps1 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerGreen2.greenPlayerColName2 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer2.transform.position = greenPlayerPos2;
                        playerGreen2.greenPlayerColName2 = "none";
                        greenPlayerSteps2 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerGreen3.greenPlayerColName3 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer3.transform.position = greenPlayerPos3;
                        playerGreen3.greenPlayerColName3 = "none";
                        greenPlayerSteps3 = 0;
                        playerTurn = "yellow";
                    }
                    if (currentPlayer == playerGreen4.greenPlayerColName4 && currentPlayer != "star")
                    {
                        soundManager.dismissalAudioSource.Play();
                        greenPlayer4.transform.position = greenPlayerPos4;
                        playerGreen4.greenPlayerColName4 = "none";
                        greenPlayerSteps4 = 0;
                        playerTurn = "yellow";
                    }
                }
            }
        }
        //player border
        if (mainMenuManager.playerNumber == 2)
        {
            if (playerTurn == "red")
            {
                diceRoll.position = redDiceRoll.position;
                frameRed.SetActive(true);
                frameGreen.SetActive(false);
            }
            else if (playerTurn == "green")
            {
                diceRoll.position = greenDiceRoll.position;
                frameRed.SetActive(false);
                frameGreen.SetActive(true);
            }
            redPlayerButton1.interactable = false;
            redPlayerButton2.interactable = false;
            redPlayerButton3.interactable = false;
            redPlayerButton4.interactable = false;

            greenPlayerButton1.interactable = false;
            greenPlayerButton2.interactable = false;
            greenPlayerButton3.interactable = false;
            greenPlayerButton4.interactable = false;

            redPlayerBorder1.SetActive(false);
            redPlayerBorder2.SetActive(false);
            redPlayerBorder3.SetActive(false);
            redPlayerBorder4.SetActive(false);

            greenPlayerBorder1.SetActive(false);
            greenPlayerBorder2.SetActive(false);
            greenPlayerBorder3.SetActive(false);
            greenPlayerBorder4.SetActive(false);

        }
        else if (mainMenuManager.playerNumber == 3)
        {
            if (playerTurn == "red")
            {
                diceRoll.position = redDiceRoll.position;
                frameRed.SetActive(true);
                frameBlue.SetActive(false);
                frameGreen.SetActive(false);
                frameYellow.SetActive(false);
            }
            else if (playerTurn == "blue")
            {
                diceRoll.position = blueDiceRoll.position;
                frameRed.SetActive(false);
                frameBlue.SetActive(true);
                frameGreen.SetActive(false);
                frameYellow.SetActive(false);
            }
            else if (playerTurn == "green")
            {
                diceRoll.position = greenDiceRoll.position;
                frameRed.SetActive(false);
                frameBlue.SetActive(false);
                frameGreen.SetActive(true);
                frameYellow.SetActive(false);
            }

            redPlayerButton1.interactable = false;
            redPlayerButton2.interactable = false;
            redPlayerButton3.interactable = false;
            redPlayerButton4.interactable = false;

            bluePlayerButton1.interactable = false;
            bluePlayerButton2.interactable = false;
            bluePlayerButton3.interactable = false;
            bluePlayerButton4.interactable = false;

            greenPlayerButton1.interactable = false;
            greenPlayerButton2.interactable = false;
            greenPlayerButton3.interactable = false;
            greenPlayerButton4.interactable = false;

            redPlayerBorder1.SetActive(false);
            redPlayerBorder2.SetActive(false);
            redPlayerBorder3.SetActive(false);
            redPlayerBorder4.SetActive(false);

            bluePlayerBorder1.SetActive(false);
            bluePlayerBorder2.SetActive(false);
            bluePlayerBorder3.SetActive(false);
            bluePlayerBorder4.SetActive(false);

            greenPlayerBorder1.SetActive(false);
            greenPlayerBorder2.SetActive(false);
            greenPlayerBorder3.SetActive(false);
            greenPlayerBorder4.SetActive(false);

        }
        else if (mainMenuManager.playerNumber == 4)
        {
            if (playerTurn == "red")
            {
                diceRoll.position = redDiceRoll.position;
                frameRed.SetActive(true);
                frameBlue.SetActive(false);
                frameGreen.SetActive(false);
                frameYellow.SetActive(false);
            }
            else if (playerTurn == "blue")
            {
                diceRoll.position = blueDiceRoll.position;
                frameRed.SetActive(false);
                frameBlue.SetActive(true);
                frameGreen.SetActive(false);
                frameYellow.SetActive(false);
            }
            else if (playerTurn == "green")
            {
                diceRoll.position = greenDiceRoll.position;
                frameRed.SetActive(false);
                frameBlue.SetActive(false);
                frameGreen.SetActive(true);
                frameYellow.SetActive(false);
            }
            else if (playerTurn == "yellow")
            {
                diceRoll.position = yellowDiceRoll.position;
                frameRed.SetActive(false);
                frameBlue.SetActive(false);
                frameGreen.SetActive(false);
                frameYellow.SetActive(true);
            }

            redPlayerButton1.interactable = false;
            redPlayerButton2.interactable = false;
            redPlayerButton3.interactable = false;
            redPlayerButton4.interactable = false;

            bluePlayerButton1.interactable = false;
            bluePlayerButton2.interactable = false;
            bluePlayerButton3.interactable = false;
            bluePlayerButton4.interactable = false;

            greenPlayerButton1.interactable = false;
            greenPlayerButton2.interactable = false;
            greenPlayerButton3.interactable = false;
            greenPlayerButton4.interactable = false;

            yellowPlayerButton1.interactable = false;
            yellowPlayerButton2.interactable = false;
            yellowPlayerButton3.interactable = false;
            yellowPlayerButton4.interactable = false;

            redPlayerBorder1.SetActive(false);
            redPlayerBorder2.SetActive(false);
            redPlayerBorder3.SetActive(false);
            redPlayerBorder4.SetActive(false);

            bluePlayerBorder1.SetActive(false);
            bluePlayerBorder2.SetActive(false);
            bluePlayerBorder3.SetActive(false);
            bluePlayerBorder4.SetActive(false);

            greenPlayerBorder1.SetActive(false);
            greenPlayerBorder2.SetActive(false);
            greenPlayerBorder3.SetActive(false);
            greenPlayerBorder4.SetActive(false);

            yellowPlayerBorder1.SetActive(false);
            yellowPlayerBorder2.SetActive(false);
            yellowPlayerBorder3.SetActive(false);
            yellowPlayerBorder4.SetActive(false);
        }

    }

    public void diceRollFunc()
    {
        soundManager.diceAudioSource.Play();
        diceRollButton.interactable = false;
        selectDiceAnimation = randNo.Next(1, 7);
        diceRollAnimation1.SetActive(false);
        diceRollAnimation2.SetActive(false);
        diceRollAnimation3.SetActive(false);
        diceRollAnimation4.SetActive(false);
        diceRollAnimation5.SetActive(false);
        diceRollAnimation6.SetActive(false);
        if (selectDiceAnimation == 1)
            diceRollAnimation1.SetActive(true);
        else if (selectDiceAnimation == 2)
            diceRollAnimation2.SetActive(true);
        else if (selectDiceAnimation == 3)
            diceRollAnimation3.SetActive(true);
        else if (selectDiceAnimation == 4)
            diceRollAnimation4.SetActive(true);
        else if (selectDiceAnimation == 5)
            diceRollAnimation5.SetActive(true);
        else if (selectDiceAnimation == 6)
            diceRollAnimation6.SetActive(true);

        StartCoroutine("playerNotInitialized");

    }
    IEnumerator playerNotInitialized()
    {
        yield return new WaitForSeconds(1f);
        if (playerTurn == "red")
        {
            //border movement 
            //red1
            if ((redMovementBlock.Count - redPlayerSteps1) >= selectDiceAnimation && redPlayerSteps1 > 0 && redMovementBlock.Count > redPlayerSteps1)
            {
                redPlayerBorder1.SetActive(true);
                redPlayerButton1.interactable = true;
            }
            else
            {
                redPlayerBorder1.SetActive(false);
                redPlayerButton1.interactable = false;
            }
            //red2
            if ((redMovementBlock.Count - redPlayerSteps2) >= selectDiceAnimation && redPlayerSteps2 > 0 && redMovementBlock.Count > redPlayerSteps2)
            {
                redPlayerBorder2.SetActive(true);
                redPlayerButton2.interactable = true;
            }
            else
            {
                redPlayerBorder2.SetActive(false);
                redPlayerButton2.interactable = false;
            }
            //red3
            if ((redMovementBlock.Count - redPlayerSteps3) >= selectDiceAnimation && redPlayerSteps3 > 0 && redMovementBlock.Count > redPlayerSteps3)
            {
                redPlayerBorder3.SetActive(true);
                redPlayerButton3.interactable = true;
            }
            else
            {
                redPlayerBorder3.SetActive(false);
                redPlayerButton3.interactable = false;
            }
            //red4
            if ((redMovementBlock.Count - redPlayerSteps4) >= selectDiceAnimation && redPlayerSteps4 > 0 && redMovementBlock.Count > redPlayerSteps4)
            {
                redPlayerBorder4.SetActive(true);
                redPlayerButton4.interactable = true;
            }
            else
            {
                redPlayerBorder4.SetActive(false);
                redPlayerButton4.interactable = false;
            }
            //if 6 to enter, show possiblity
            if (selectDiceAnimation == 6)
            {
                if (redPlayerSteps1 == 0)
                {
                    redPlayerBorder1.SetActive(true);
                    redPlayerButton1.interactable = true;
                }
                if (redPlayerSteps2 == 0)
                {
                    redPlayerBorder2.SetActive(true);
                    redPlayerButton2.interactable = true;
                }
                if (redPlayerSteps3 == 0)
                {
                    redPlayerBorder3.SetActive(true);
                    redPlayerButton3.interactable = true;
                }
                if (redPlayerSteps4 == 0)
                {
                    redPlayerBorder4.SetActive(true);
                    redPlayerButton4.interactable = true;
                }

            }
            ////////if no move switch turn
            if (!redPlayerBorder1.activeInHierarchy && !redPlayerBorder2.activeInHierarchy && !redPlayerBorder3.activeInHierarchy && !redPlayerBorder4.activeInHierarchy)
            {
                redPlayerButton1.interactable = false;
                redPlayerButton2.interactable = false;
                redPlayerButton3.interactable = false;
                redPlayerButton4.interactable = false;
                if (mainMenuManager.playerNumber == 2)
                {
                    playerTurn = "green";
                    inializeDice();
                }
                else if (mainMenuManager.playerNumber == 4)
                {
                    playerTurn = "blue";
                    inializeDice();
                }
            }
        }
        else if (playerTurn == "blue")
        {
            //if 6 to enter, show possiblity
            if (selectDiceAnimation == 6)
            {
                if (bluePlayerSteps1 == 0)
                {
                    bluePlayerBorder1.SetActive(true);
                    bluePlayerButton1.interactable = true;
                }
                if (bluePlayerSteps2 == 0)
                {
                    bluePlayerBorder2.SetActive(true);
                    bluePlayerButton2.interactable = true;
                }
                if (bluePlayerSteps3 == 0)
                {
                    bluePlayerBorder3.SetActive(true);
                    bluePlayerButton3.interactable = true;
                }
                if (bluePlayerSteps4 == 0)
                {
                    bluePlayerBorder4.SetActive(true);
                    bluePlayerButton4.interactable = true;
                }

            }
            ////////if no move switch turn
            if (!bluePlayerBorder1.activeInHierarchy && !bluePlayerBorder2.activeInHierarchy && !bluePlayerBorder3.activeInHierarchy && !bluePlayerBorder4.activeInHierarchy)
            {
                bluePlayerButton1.interactable = false;
                bluePlayerButton2.interactable = false;
                bluePlayerButton3.interactable = false;
                bluePlayerButton4.interactable = false;
                playerTurn = "green";
                inializeDice();
            }
        }
        else if (playerTurn == "green")
        {
            //green1
            if ((greenMovementBlock.Count - greenPlayerSteps1) >= selectDiceAnimation && greenPlayerSteps1 > 0 && greenMovementBlock.Count > greenPlayerSteps1)
            {
                greenPlayerBorder1.SetActive(true);
                greenPlayerButton1.interactable = true;
            }
            else
            {
                greenPlayerBorder1.SetActive(false);
                greenPlayerButton1.interactable = false;
            }
            //green2
            if ((greenMovementBlock.Count - greenPlayerSteps2) >= selectDiceAnimation && greenPlayerSteps2 > 0 && greenMovementBlock.Count > greenPlayerSteps2)
            {
                greenPlayerBorder2.SetActive(true);
                greenPlayerButton2.interactable = true;
            }
            else
            {
                greenPlayerBorder2.SetActive(false);
                greenPlayerButton2.interactable = false;
            }
            //green3
            if ((greenMovementBlock.Count - greenPlayerSteps3) >= selectDiceAnimation && greenPlayerSteps3 > 0 && greenMovementBlock.Count > greenPlayerSteps3)
            {
                greenPlayerBorder3.SetActive(true);
                greenPlayerButton3.interactable = true;
            }
            else
            {
                greenPlayerBorder3.SetActive(false);
                greenPlayerButton3.interactable = false;
            }
            //green4
            if ((greenMovementBlock.Count - greenPlayerSteps4) >= selectDiceAnimation && greenPlayerSteps4 > 0 && greenMovementBlock.Count > greenPlayerSteps4)
            {
                greenPlayerBorder4.SetActive(true);
                greenPlayerButton4.interactable = true;
            }
            else
            {
                greenPlayerBorder4.SetActive(false);
                greenPlayerButton4.interactable = false;
            }
            //if 6 to enter, show possiblity
            if (selectDiceAnimation == 6)
            {
                if (greenPlayerSteps1 == 0)
                {
                    greenPlayerBorder1.SetActive(true);
                    greenPlayerButton1.interactable = true;
                }
                if (greenPlayerSteps2 == 0)
                {
                    greenPlayerBorder2.SetActive(true);
                    greenPlayerButton2.interactable = true;
                }
                if (greenPlayerSteps3 == 0)
                {
                    greenPlayerBorder3.SetActive(true);
                    greenPlayerButton3.interactable = true;
                }
                if (greenPlayerSteps4 == 0)
                {
                    greenPlayerBorder4.SetActive(true);
                    greenPlayerButton4.interactable = true;
                }

            }
            ////////if no move switch turn
            if (!greenPlayerBorder1.activeInHierarchy && !greenPlayerBorder2.activeInHierarchy && !greenPlayerBorder3.activeInHierarchy && !greenPlayerBorder4.activeInHierarchy)
            {
                greenPlayerButton1.interactable = false;
                greenPlayerButton2.interactable = false;
                greenPlayerButton3.interactable = false;
                greenPlayerButton4.interactable = false;
                if (mainMenuManager.playerNumber == 2)
                {
                    playerTurn = "red";
                    inializeDice();
                }
                else if (mainMenuManager.playerNumber == 4)
                {
                    playerTurn = "yellow";
                    inializeDice();
                }
            }
        }
        else if (playerTurn == "yellow")
        {
            //if 6 to enter, show possiblity
            if (selectDiceAnimation == 6)
            {
                if (yellowPlayerSteps1 == 0)
                {
                    yellowPlayerBorder1.SetActive(true);
                    yellowPlayerButton1.interactable = true;
                }
                if (yellowPlayerSteps2 == 0)
                {
                    yellowPlayerBorder2.SetActive(true);
                    yellowPlayerButton2.interactable = true;
                }
                if (yellowPlayerSteps3 == 0)
                {
                    yellowPlayerBorder3.SetActive(true);
                    yellowPlayerButton3.interactable = true;
                }
                if (yellowPlayerSteps4 == 0)
                {
                    yellowPlayerBorder4.SetActive(true);
                    yellowPlayerButton4.interactable = true;
                }

            }
            ////////if no move switch turn
            if (!yellowPlayerBorder1.activeInHierarchy && !yellowPlayerBorder2.activeInHierarchy && !yellowPlayerBorder3.activeInHierarchy && !yellowPlayerBorder4.activeInHierarchy)
            {
                yellowPlayerButton1.interactable = false;
                yellowPlayerButton2.interactable = false;
                yellowPlayerButton3.interactable = false;
                yellowPlayerButton4.interactable = false;
                playerTurn = "red";
                inializeDice();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        mainMenuManager.playerNumber = 2;
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 25;
        randNo = new System.Random();

        diceRollAnimation1.SetActive(false);
        diceRollAnimation2.SetActive(false);
        diceRollAnimation3.SetActive(false);
        diceRollAnimation4.SetActive(false);
        diceRollAnimation5.SetActive(false);
        diceRollAnimation6.SetActive(false);

        //initial positions
        redPlayerPos1 = redPlayer1.transform.position;
        redPlayerPos2 = redPlayer2.transform.position;
        redPlayerPos3 = redPlayer3.transform.position;
        redPlayerPos4 = redPlayer4.transform.position;

        greenPlayerPos1 = greenPlayer1.transform.position;
        greenPlayerPos2 = greenPlayer2.transform.position;
        greenPlayerPos3 = greenPlayer3.transform.position;
        greenPlayerPos4 = greenPlayer4.transform.position;

        bluePlayerPos1 = bluePlayer1.transform.position;
        bluePlayerPos2 = bluePlayer2.transform.position;
        bluePlayerPos3 = bluePlayer3.transform.position;
        bluePlayerPos4 = bluePlayer4.transform.position;

        yellowPlayerPos1 = yellowPlayer1.transform.position;
        yellowPlayerPos2 = yellowPlayer2.transform.position;
        yellowPlayerPos3 = yellowPlayer3.transform.position;
        yellowPlayerPos4 = yellowPlayer4.transform.position;
        //border deactivate
        redPlayerBorder1.SetActive(false);
        redPlayerBorder2.SetActive(false);
        redPlayerBorder3.SetActive(false);
        redPlayerBorder4.SetActive(false);

        greenPlayerBorder1.SetActive(false);
        greenPlayerBorder2.SetActive(false);
        greenPlayerBorder3.SetActive(false);
        greenPlayerBorder4.SetActive(false);

        bluePlayerBorder1.SetActive(false);
        bluePlayerBorder2.SetActive(false);
        bluePlayerBorder3.SetActive(false);
        bluePlayerBorder4.SetActive(false);

        yellowPlayerBorder1.SetActive(false);
        yellowPlayerBorder2.SetActive(false);
        yellowPlayerBorder3.SetActive(false);
        yellowPlayerBorder4.SetActive(false);
        //screen deactivate
        redScreen.SetActive(false);
        blueScreen.SetActive(false);
        greenScreen.SetActive(false);
        yellowScreen.SetActive(false);

        if (mainMenuManager.playerNumber == 2)
        {
            playerTurn = "red";
            frameRed.SetActive(true);
            frameGreen.SetActive(false);
            diceRoll.position = redDiceRoll.position;

            bluePlayer1.SetActive(false);
            bluePlayer2.SetActive(false);
            bluePlayer3.SetActive(false);
            bluePlayer4.SetActive(false);
            yellowPlayer1.SetActive(false);
            yellowPlayer2.SetActive(false);
            yellowPlayer3.SetActive(false);
            yellowPlayer4.SetActive(false);
        }
        if (mainMenuManager.playerNumber == 3)
        {
            playerTurn = "red";
            frameRed.SetActive(true);
            frameGreen.SetActive(false);
            diceRoll.position = redDiceRoll.position;

            bluePlayer1.SetActive(false);
            bluePlayer2.SetActive(false);
            bluePlayer3.SetActive(false);
            bluePlayer4.SetActive(false);
            yellowPlayer1.SetActive(false);
            yellowPlayer2.SetActive(false);
            yellowPlayer3.SetActive(false);
            yellowPlayer4.SetActive(false);
            greenPlayer1.SetActive(false);
            greenPlayer2.SetActive(false);
            greenPlayer3.SetActive(false);
            greenPlayer4.SetActive(false);
        }
        else if (mainMenuManager.playerNumber == 4)
        {
            playerTurn = "red";
            frameRed.SetActive(true);
            frameGreen.SetActive(false);
            frameBlue.SetActive(false);
            frameYellow.SetActive(false);
            diceRoll.position = redDiceRoll.position;
        }

    }
    public void redPlayerMovement1()
    {
        soundManager.playerAudioSource.Play();

        redPlayerBorder1.SetActive(false);
        redPlayerBorder2.SetActive(false);
        redPlayerBorder3.SetActive(false);
        redPlayerBorder4.SetActive(false);

        redPlayerButton1.interactable = false;
        redPlayerButton2.interactable = false;
        redPlayerButton3.interactable = false;
        redPlayerButton4.interactable = false;

        if (playerTurn == "red")
        {
            if ((redMovementBlock.Count - redPlayerSteps1) > selectDiceAnimation)
            {
                if (redPlayerSteps1 > 0)
                {
                    Vector3[] redPlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath1[i] = redMovementBlock[redPlayerSteps1 + i].transform.position;

                    }
                    redPlayerSteps1 = redPlayerSteps1 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "red";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    if (redPlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(redPlayer1, iTween.Hash("path", redPlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer1, iTween.Hash("position", redPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "red player 1";
                }
                else if (selectDiceAnimation == 6 && redPlayerSteps1 == 0)
                {
                    Vector3[] redPlayerPath1 = new Vector3[1];
                    redPlayerPath1[0] = redMovementBlock[redPlayerSteps1].transform.position;
                    redPlayerSteps1 += 1;
                    playerTurn = "red";
                    currentPlayerName = "red player 1";
                    iTween.MoveTo(redPlayer1, iTween.Hash("position", redPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "red" && (redMovementBlock.Count - redPlayerSteps1) == selectDiceAnimation)
                {
                    Vector3[] redPlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath1[i] = redMovementBlock[redPlayerSteps1 + i].transform.position;

                    }
                    redPlayerSteps1 += selectDiceAnimation;
                    if (redPlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(redPlayer1, iTween.Hash("path", redPlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer1, iTween.Hash("position", redPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "red";
                    totalRedInHouse += 1;
                    redPlayerButton1.enabled = false;
                }
                else
                {
                    if (redPlayerSteps2 == 0 && redPlayerSteps3 == 0 && redPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    inializeDice();
                }

            }
        }
    }

    public void redPlayerMovement2()
    {
        soundManager.playerAudioSource.Play();

        redPlayerBorder1.SetActive(false);
        redPlayerBorder2.SetActive(false);
        redPlayerBorder3.SetActive(false);
        redPlayerBorder4.SetActive(false);

        redPlayerButton1.interactable = false;
        redPlayerButton2.interactable = false;
        redPlayerButton3.interactable = false;
        redPlayerButton4.interactable = false;

        if (playerTurn == "red")
        {
            if ((redMovementBlock.Count - redPlayerSteps2) > selectDiceAnimation)
            {
                if (redPlayerSteps2 > 0)
                {
                    Vector3[] redPlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath2[i] = redMovementBlock[redPlayerSteps2 + i].transform.position;

                    }
                    redPlayerSteps2 = redPlayerSteps2 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "red";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    if (redPlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(redPlayer2, iTween.Hash("path", redPlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer2, iTween.Hash("position", redPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "red player 2";
                }
                else if (selectDiceAnimation == 6 && redPlayerSteps2 == 0)
                {
                    Vector3[] redPlayerPath2 = new Vector3[1];
                    redPlayerPath2[0] = redMovementBlock[redPlayerSteps2].transform.position;
                    redPlayerSteps2 += 1;
                    playerTurn = "red";
                    currentPlayerName = "red player 2";
                    iTween.MoveTo(redPlayer2, iTween.Hash("position", redPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "red" && (redMovementBlock.Count - redPlayerSteps2) == selectDiceAnimation)
                {
                    Vector3[] redPlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath2[i] = redMovementBlock[redPlayerSteps2 + i].transform.position;

                    }
                    redPlayerSteps2 += selectDiceAnimation;
                    if (redPlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(redPlayer2, iTween.Hash("path", redPlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer2, iTween.Hash("position", redPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "red";
                    totalRedInHouse += 1;
                    redPlayerButton2.enabled = false;
                }
                else
                {
                    if (redPlayerSteps1 == 0 && redPlayerSteps3 == 0 && redPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void redPlayerMovement3()
    {
        soundManager.playerAudioSource.Play();

        redPlayerBorder1.SetActive(false);
        redPlayerBorder2.SetActive(false);
        redPlayerBorder3.SetActive(false);
        redPlayerBorder4.SetActive(false);

        redPlayerButton1.interactable = false;
        redPlayerButton2.interactable = false;
        redPlayerButton3.interactable = false;
        redPlayerButton4.interactable = false;

        if (playerTurn == "red")
        {
            if ((redMovementBlock.Count - redPlayerSteps3) > selectDiceAnimation)
            {
                if (redPlayerSteps3 > 0)
                {
                    Vector3[] redPlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath3[i] = redMovementBlock[redPlayerSteps3 + i].transform.position;

                    }
                    redPlayerSteps3 = redPlayerSteps3 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "red";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    if (redPlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(redPlayer3, iTween.Hash("path", redPlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer3, iTween.Hash("position", redPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "red player 3";
                }
                else if (selectDiceAnimation == 6 && redPlayerSteps3 == 0)
                {
                    Vector3[] redPlayerPath3 = new Vector3[1];
                    redPlayerPath3[0] = redMovementBlock[redPlayerSteps3].transform.position;
                    redPlayerSteps3 += 1;
                    playerTurn = "red";
                    currentPlayerName = "red player 3";
                    iTween.MoveTo(redPlayer3, iTween.Hash("position", redPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "red" && (redMovementBlock.Count - redPlayerSteps3) == selectDiceAnimation)
                {
                    Vector3[] redPlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath3[i] = redMovementBlock[redPlayerSteps3 + i].transform.position;

                    }
                    redPlayerSteps3 += selectDiceAnimation;
                    if (redPlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(redPlayer3, iTween.Hash("path", redPlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer3, iTween.Hash("position", redPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "red";
                    totalRedInHouse += 1;
                    redPlayerButton3.enabled = false;
                }
                else
                {
                    if (redPlayerSteps1 == 0 && redPlayerSteps2 == 0 && redPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void redPlayerMovement4()
    {
        soundManager.playerAudioSource.Play();

        redPlayerBorder1.SetActive(false);
        redPlayerBorder2.SetActive(false);
        redPlayerBorder3.SetActive(false);
        redPlayerBorder4.SetActive(false);

        redPlayerButton1.interactable = false;
        redPlayerButton2.interactable = false;
        redPlayerButton3.interactable = false;
        redPlayerButton4.interactable = false;

        if (playerTurn == "red")
        {
            if ((redMovementBlock.Count - redPlayerSteps4) > selectDiceAnimation)
            {
                if (redPlayerSteps4 > 0)
                {
                    Vector3[] redPlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath4[i] = redMovementBlock[redPlayerSteps4 + i].transform.position;

                    }
                    redPlayerSteps4 = redPlayerSteps4 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "red";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    if (redPlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(redPlayer4, iTween.Hash("path", redPlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer4, iTween.Hash("position", redPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "red player 4";
                }
                else if (selectDiceAnimation == 6 && redPlayerSteps4 == 0)
                {
                    Vector3[] redPlayerPath4 = new Vector3[1];
                    redPlayerPath4[0] = redMovementBlock[redPlayerSteps4].transform.position;
                    redPlayerSteps4 += 1;
                    playerTurn = "red";
                    currentPlayerName = "red player 4";
                    iTween.MoveTo(redPlayer4, iTween.Hash("position", redPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "red" && (redMovementBlock.Count - redPlayerSteps4) == selectDiceAnimation)
                {
                    Vector3[] redPlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        redPlayerPath4[i] = redMovementBlock[redPlayerSteps4 + i].transform.position;

                    }
                    redPlayerSteps4 += selectDiceAnimation;
                    if (redPlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(redPlayer4, iTween.Hash("path", redPlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(redPlayer4, iTween.Hash("position", redPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "red";
                    totalRedInHouse += 1;
                    redPlayerButton4.enabled = false;
                }
                else
                {
                    if (redPlayerSteps1 == 0 && redPlayerSteps2 == 0 && redPlayerSteps3 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "green";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "blue";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void greenPlayerMovement1()
    {
        soundManager.playerAudioSource.Play();

        greenPlayerBorder1.SetActive(false);
        greenPlayerBorder2.SetActive(false);
        greenPlayerBorder3.SetActive(false);
        greenPlayerBorder4.SetActive(false);

        greenPlayerButton1.interactable = false;
        greenPlayerButton2.interactable = false;
        greenPlayerButton3.interactable = false;
        greenPlayerButton4.interactable = false;

        if (playerTurn == "green")
        {
            if ((greenMovementBlock.Count - greenPlayerSteps1) > selectDiceAnimation)
            {
                if (greenPlayerSteps1 > 0)
                {
                    Vector3[] greenPlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath1[i] = greenMovementBlock[greenPlayerSteps1 + i].transform.position;

                    }
                    greenPlayerSteps1 = greenPlayerSteps1 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "green";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    if (greenPlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer1, iTween.Hash("path", greenPlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer1, iTween.Hash("position", greenPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "green player 1";
                }
                else if (selectDiceAnimation == 6 && greenPlayerSteps1 == 0)
                {
                    Vector3[] greenPlayerPath1 = new Vector3[1];
                    greenPlayerPath1[0] = greenMovementBlock[greenPlayerSteps1].transform.position;
                    greenPlayerSteps1 += 1;
                    playerTurn = "green";
                    currentPlayerName = "green player 1";
                    iTween.MoveTo(greenPlayer1, iTween.Hash("position", greenPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "green" && (greenMovementBlock.Count - greenPlayerSteps1) == selectDiceAnimation)
                {
                    Vector3[] greenPlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath1[i] = greenMovementBlock[greenPlayerSteps1 + i].transform.position;

                    }
                    greenPlayerSteps1 += selectDiceAnimation;
                    if (greenPlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer1, iTween.Hash("path", greenPlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer1, iTween.Hash("position", greenPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "green";
                    totalGreenInHouse += 1;
                    greenPlayerButton1.enabled = false;
                }
                else
                {
                    if (greenPlayerSteps2 == 0 && greenPlayerSteps3 == 0 && greenPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    inializeDice();
                }

            }
        }
    }

    public void greenPlayerMovement2()
    {
        soundManager.playerAudioSource.Play();

        greenPlayerBorder1.SetActive(false);
        greenPlayerBorder2.SetActive(false);
        greenPlayerBorder3.SetActive(false);
        greenPlayerBorder4.SetActive(false);

        greenPlayerButton1.interactable = false;
        greenPlayerButton2.interactable = false;
        greenPlayerButton3.interactable = false;
        greenPlayerButton4.interactable = false;

        if (playerTurn == "green")
        {
            if ((greenMovementBlock.Count - greenPlayerSteps2) > selectDiceAnimation)
            {
                if (greenPlayerSteps2 > 0)
                {
                    Vector3[] greenPlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath2[i] = greenMovementBlock[greenPlayerSteps2 + i].transform.position;

                    }
                    greenPlayerSteps2 = greenPlayerSteps2 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "green";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    if (greenPlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer2, iTween.Hash("path", greenPlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer2, iTween.Hash("position", greenPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "green player 2";
                }
                else if (selectDiceAnimation == 6 && greenPlayerSteps2 == 0)
                {
                    Vector3[] greenPlayerPath2 = new Vector3[1];
                    greenPlayerPath2[0] = greenMovementBlock[greenPlayerSteps2].transform.position;
                    greenPlayerSteps2 += 1;
                    playerTurn = "green";
                    currentPlayerName = "green player 2";
                    iTween.MoveTo(greenPlayer2, iTween.Hash("position", greenPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "green" && (greenMovementBlock.Count - greenPlayerSteps2) == selectDiceAnimation)
                {
                    Vector3[] greenPlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath2[i] = greenMovementBlock[greenPlayerSteps2 + i].transform.position;

                    }
                    greenPlayerSteps2 += selectDiceAnimation;
                    if (greenPlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer2, iTween.Hash("path", greenPlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer2, iTween.Hash("position", greenPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "green";
                    totalGreenInHouse += 1;
                    greenPlayerButton2.enabled = false;
                }
                else
                {
                    if (greenPlayerSteps1 == 0 && greenPlayerSteps3 == 0 && greenPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void greenPlayerMovement3()
    {
        soundManager.playerAudioSource.Play();

        greenPlayerBorder1.SetActive(false);
        greenPlayerBorder2.SetActive(false);
        greenPlayerBorder3.SetActive(false);
        greenPlayerBorder4.SetActive(false);

        greenPlayerButton1.interactable = false;
        greenPlayerButton2.interactable = false;
        greenPlayerButton3.interactable = false;
        greenPlayerButton4.interactable = false;

        if (playerTurn == "green")
        {
            if ((greenMovementBlock.Count - greenPlayerSteps3) > selectDiceAnimation)
            {
                if (greenPlayerSteps3 > 0)
                {
                    Vector3[] greenPlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath3[i] = greenMovementBlock[greenPlayerSteps3 + i].transform.position;

                    }
                    greenPlayerSteps3 = greenPlayerSteps3 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "green";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    if (greenPlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer3, iTween.Hash("path", greenPlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer3, iTween.Hash("position", greenPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "green player 3";
                }
                else if (selectDiceAnimation == 6 && greenPlayerSteps3 == 0)
                {
                    Vector3[] greenPlayerPath3 = new Vector3[1];
                    greenPlayerPath3[0] = greenMovementBlock[greenPlayerSteps3].transform.position;
                    greenPlayerSteps3 += 1;
                    playerTurn = "green";
                    currentPlayerName = "green player 3";
                    iTween.MoveTo(greenPlayer3, iTween.Hash("position", greenPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "green" && (greenMovementBlock.Count - greenPlayerSteps3) == selectDiceAnimation)
                {
                    Vector3[] greenPlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath3[i] = greenMovementBlock[greenPlayerSteps3 + i].transform.position;

                    }
                    greenPlayerSteps3 += selectDiceAnimation;
                    if (greenPlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer3, iTween.Hash("path", greenPlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer3, iTween.Hash("position", greenPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "green";
                    totalGreenInHouse += 1;
                    greenPlayerButton3.enabled = false;
                }
                else
                {
                    if (greenPlayerSteps1 == 0 && greenPlayerSteps2 == 0 && greenPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void greenPlayerMovement4()
    {
        soundManager.playerAudioSource.Play();

        greenPlayerBorder1.SetActive(false);
        greenPlayerBorder2.SetActive(false);
        greenPlayerBorder3.SetActive(false);
        greenPlayerBorder4.SetActive(false);

        greenPlayerButton1.interactable = false;
        greenPlayerButton2.interactable = false;
        greenPlayerButton3.interactable = false;
        greenPlayerButton4.interactable = false;

        if (playerTurn == "green")
        {
            if ((greenMovementBlock.Count - greenPlayerSteps4) > selectDiceAnimation)
            {
                if (greenPlayerSteps4 > 0)
                {
                    Vector3[] greenPlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath4[i] = greenMovementBlock[greenPlayerSteps4 + i].transform.position;

                    }
                    greenPlayerSteps4 = greenPlayerSteps4 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "green";
                    }
                    else
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    if (greenPlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer4, iTween.Hash("path", greenPlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer4, iTween.Hash("position", greenPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "green player 4";
                }
                else if (selectDiceAnimation == 6 && greenPlayerSteps4 == 0)
                {
                    Vector3[] greenPlayerPath4 = new Vector3[1];
                    greenPlayerPath4[0] = greenMovementBlock[greenPlayerSteps4].transform.position;
                    greenPlayerSteps4 += 1;
                    playerTurn = "green";
                    currentPlayerName = "green player 4";
                    iTween.MoveTo(greenPlayer4, iTween.Hash("position", greenPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "green" && (greenMovementBlock.Count - greenPlayerSteps4) == selectDiceAnimation)
                {
                    Vector3[] greenPlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        greenPlayerPath4[i] = greenMovementBlock[greenPlayerSteps4 + i].transform.position;

                    }
                    greenPlayerSteps4 += selectDiceAnimation;
                    if (greenPlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(greenPlayer4, iTween.Hash("path", greenPlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(greenPlayer4, iTween.Hash("position", greenPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "green";
                    totalGreenInHouse += 1;
                    greenPlayerButton4.enabled = false;
                }
                else
                {
                    if (greenPlayerSteps1 == 0 && greenPlayerSteps2 == 0 && greenPlayerSteps3 == 0 && selectDiceAnimation != 6)
                    {
                        if (mainMenuManager.playerNumber == 2)
                            playerTurn = "red";
                        else if (mainMenuManager.playerNumber == 4)
                            playerTurn = "yellow";
                    }
                    inializeDice();
                }

            }
        }
    }
    //==============blue
    public void bluePlayerMovement1()
    {
        soundManager.playerAudioSource.Play();

        bluePlayerBorder1.SetActive(false);
        bluePlayerBorder2.SetActive(false);
        bluePlayerBorder3.SetActive(false);
        bluePlayerBorder4.SetActive(false);

        bluePlayerButton1.interactable = false;
        bluePlayerButton2.interactable = false;
        bluePlayerButton3.interactable = false;
        bluePlayerButton4.interactable = false;

        if (playerTurn == "blue")
        {
            if ((blueMovementBlock.Count - bluePlayerSteps1) > selectDiceAnimation)
            {
                if (bluePlayerSteps1 > 0)
                {
                    Vector3[] bluePlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath1[i] = blueMovementBlock[bluePlayerSteps1 + i].transform.position;

                    }
                    bluePlayerSteps1 = bluePlayerSteps1 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "blue";
                    }
                    else
                    {
                        playerTurn = "green";

                    }
                    if (bluePlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer1, iTween.Hash("path", bluePlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer1, iTween.Hash("position", bluePlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "blue player 1";
                }
                else if (selectDiceAnimation == 6 && bluePlayerSteps1 == 0)
                {
                    Vector3[] bluePlayerPath1 = new Vector3[1];
                    bluePlayerPath1[0] = blueMovementBlock[bluePlayerSteps1].transform.position;
                    bluePlayerSteps1 += 1;
                    playerTurn = "blue";
                    currentPlayerName = "blue player 1";
                    iTween.MoveTo(bluePlayer1, iTween.Hash("position", bluePlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "blue" && (blueMovementBlock.Count - bluePlayerSteps1) == selectDiceAnimation)
                {
                    Vector3[] bluePlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath1[i] = blueMovementBlock[bluePlayerSteps1 + i].transform.position;

                    }
                    bluePlayerSteps1 += selectDiceAnimation;
                    if (bluePlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer1, iTween.Hash("path", bluePlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer1, iTween.Hash("position", bluePlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "yellow";
                    totalblueInHouse += 1;
                    bluePlayerButton1.enabled = false;
                }
                else
                {
                    if (bluePlayerSteps2 == 0 && bluePlayerSteps3 == 0 && bluePlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "green";
                    }
                    inializeDice();
                }

            }
        }
    }

    public void bluePlayerMovement2()
    {
        soundManager.playerAudioSource.Play();

        bluePlayerBorder1.SetActive(false);
        bluePlayerBorder2.SetActive(false);
        bluePlayerBorder3.SetActive(false);
        bluePlayerBorder4.SetActive(false);

        bluePlayerButton1.interactable = false;
        bluePlayerButton2.interactable = false;
        bluePlayerButton3.interactable = false;
        bluePlayerButton4.interactable = false;

        if (playerTurn == "blue")
        {
            if ((blueMovementBlock.Count - bluePlayerSteps2) > selectDiceAnimation)
            {
                if (bluePlayerSteps2 > 0)
                {
                    Vector3[] bluePlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath2[i] = blueMovementBlock[bluePlayerSteps2 + i].transform.position;

                    }
                    bluePlayerSteps2 = bluePlayerSteps2 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "blue";
                    }
                    else
                    {
                        playerTurn = "green";
                    }
                    if (bluePlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer2, iTween.Hash("path", bluePlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer2, iTween.Hash("position", bluePlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "blue player 2";
                }
                else if (selectDiceAnimation == 6 && bluePlayerSteps2 == 0)
                {
                    Vector3[] bluePlayerPath2 = new Vector3[1];
                    bluePlayerPath2[0] = blueMovementBlock[bluePlayerSteps2].transform.position;
                    bluePlayerSteps2 += 1;
                    playerTurn = "blue";
                    currentPlayerName = "blue player 2";
                    iTween.MoveTo(bluePlayer2, iTween.Hash("position", bluePlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "blue" && (blueMovementBlock.Count - bluePlayerSteps2) == selectDiceAnimation)
                {
                    Vector3[] bluePlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath2[i] = blueMovementBlock[bluePlayerSteps2 + i].transform.position;

                    }
                    bluePlayerSteps2 += selectDiceAnimation;
                    if (bluePlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer2, iTween.Hash("path", bluePlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer2, iTween.Hash("position", bluePlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "blue";
                    totalblueInHouse += 1;
                    bluePlayerButton2.enabled = false;
                }
                else
                {
                    if (bluePlayerSteps1 == 0 && bluePlayerSteps3 == 0 && bluePlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "green";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void bluePlayerMovement3()
    {
        soundManager.playerAudioSource.Play();

        bluePlayerBorder1.SetActive(false);
        bluePlayerBorder2.SetActive(false);
        bluePlayerBorder3.SetActive(false);
        bluePlayerBorder4.SetActive(false);

        bluePlayerButton1.interactable = false;
        bluePlayerButton2.interactable = false;
        bluePlayerButton3.interactable = false;
        bluePlayerButton4.interactable = false;

        if (playerTurn == "blue")
        {
            if ((blueMovementBlock.Count - bluePlayerSteps3) > selectDiceAnimation)
            {
                if (bluePlayerSteps3 > 0)
                {
                    Vector3[] bluePlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath3[i] = blueMovementBlock[bluePlayerSteps3 + i].transform.position;

                    }
                    bluePlayerSteps3 = bluePlayerSteps3 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "blue";
                    }
                    else
                    {
                        playerTurn = "green";
                    }
                    if (bluePlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer3, iTween.Hash("path", bluePlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer3, iTween.Hash("position", bluePlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "blue player 3";
                }
                else if (selectDiceAnimation == 6 && bluePlayerSteps3 == 0)
                {
                    Vector3[] bluePlayerPath3 = new Vector3[1];
                    bluePlayerPath3[0] = blueMovementBlock[bluePlayerSteps3].transform.position;
                    bluePlayerSteps3 += 1;
                    playerTurn = "blue";
                    currentPlayerName = "blue player 3";
                    iTween.MoveTo(bluePlayer3, iTween.Hash("position", bluePlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "blue" && (blueMovementBlock.Count - bluePlayerSteps3) == selectDiceAnimation)
                {
                    Vector3[] bluePlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath3[i] = blueMovementBlock[bluePlayerSteps3 + i].transform.position;

                    }
                    bluePlayerSteps3 += selectDiceAnimation;
                    if (bluePlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer3, iTween.Hash("path", bluePlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer3, iTween.Hash("position", bluePlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "blue";
                    totalblueInHouse += 1;
                    bluePlayerButton3.enabled = false;
                }
                else
                {
                    if (bluePlayerSteps1 == 0 && bluePlayerSteps2 == 0 && bluePlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "green";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void bluePlayerMovement4()
    {
        soundManager.playerAudioSource.Play();

        bluePlayerBorder1.SetActive(false);
        bluePlayerBorder2.SetActive(false);
        bluePlayerBorder3.SetActive(false);
        bluePlayerBorder4.SetActive(false);

        bluePlayerButton1.interactable = false;
        bluePlayerButton2.interactable = false;
        bluePlayerButton3.interactable = false;
        bluePlayerButton4.interactable = false;

        if (playerTurn == "blue")
        {
            if ((blueMovementBlock.Count - bluePlayerSteps4) > selectDiceAnimation)
            {
                if (bluePlayerSteps4 > 0)
                {
                    Vector3[] bluePlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath4[i] = blueMovementBlock[bluePlayerSteps4 + i].transform.position;

                    }
                    bluePlayerSteps4 = bluePlayerSteps4 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "blue";
                    }
                    else
                    {
                        playerTurn = "green";
                    }
                    if (bluePlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer4, iTween.Hash("path", bluePlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer4, iTween.Hash("position", bluePlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "blue player 4";
                }
                else if (selectDiceAnimation == 6 && bluePlayerSteps4 == 0)
                {
                    Vector3[] bluePlayerPath4 = new Vector3[1];
                    bluePlayerPath4[0] = blueMovementBlock[bluePlayerSteps4].transform.position;
                    bluePlayerSteps4 += 1;
                    playerTurn = "blue";
                    currentPlayerName = "blue player 4";
                    iTween.MoveTo(bluePlayer4, iTween.Hash("position", bluePlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "blue" && (blueMovementBlock.Count - bluePlayerSteps4) == selectDiceAnimation)
                {
                    Vector3[] bluePlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        bluePlayerPath4[i] = blueMovementBlock[bluePlayerSteps4 + i].transform.position;

                    }
                    bluePlayerSteps4 += selectDiceAnimation;
                    if (bluePlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(bluePlayer4, iTween.Hash("path", bluePlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(bluePlayer4, iTween.Hash("position", bluePlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "blue";
                    totalblueInHouse += 1;
                    bluePlayerButton4.enabled = false;
                }
                else
                {
                    if (bluePlayerSteps1 == 0 && bluePlayerSteps2 == 0 && bluePlayerSteps3 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "green";
                    }
                    inializeDice();
                }

            }
        }
    }
    //==============yellow
    public void yellowPlayerMovement1()
    {
        soundManager.playerAudioSource.Play();

        yellowPlayerBorder1.SetActive(false);
        yellowPlayerBorder2.SetActive(false);
        yellowPlayerBorder3.SetActive(false);
        yellowPlayerBorder4.SetActive(false);

        yellowPlayerButton1.interactable = false;
        yellowPlayerButton2.interactable = false;
        yellowPlayerButton3.interactable = false;
        yellowPlayerButton4.interactable = false;

        if (playerTurn == "yellow")
        {
            if ((yellowMovementBlock.Count - yellowPlayerSteps1) > selectDiceAnimation)
            {
                if (yellowPlayerSteps1 > 0)
                {
                    Vector3[] yellowPlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath1[i] = yellowMovementBlock[yellowPlayerSteps1 + i].transform.position;

                    }
                    yellowPlayerSteps1 = yellowPlayerSteps1 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "yellow";
                    }
                    else
                    {
                        playerTurn = "red";

                    }
                    if (yellowPlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer1, iTween.Hash("path", yellowPlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer1, iTween.Hash("position", yellowPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "yellow player 1";
                }
                else if (selectDiceAnimation == 6 && yellowPlayerSteps1 == 0)
                {
                    Vector3[] yellowPlayerPath1 = new Vector3[1];
                    yellowPlayerPath1[0] = yellowMovementBlock[yellowPlayerSteps1].transform.position;
                    yellowPlayerSteps1 += 1;
                    playerTurn = "yellow";
                    currentPlayerName = "yellow player 1";
                    iTween.MoveTo(yellowPlayer1, iTween.Hash("position", yellowPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "yellow" && (yellowMovementBlock.Count - yellowPlayerSteps1) == selectDiceAnimation)
                {
                    Vector3[] yellowPlayerPath1 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath1[i] = yellowMovementBlock[yellowPlayerSteps1 + i].transform.position;

                    }
                    yellowPlayerSteps1 += selectDiceAnimation;
                    if (yellowPlayerPath1.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer1, iTween.Hash("path", yellowPlayerPath1, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer1, iTween.Hash("position", yellowPlayerPath1[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "yellow";
                    totalyellowInHouse += 1;
                    yellowPlayerButton1.enabled = false;
                }
                else
                {
                    if (yellowPlayerSteps2 == 0 && yellowPlayerSteps3 == 0 && yellowPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "red";
                    }
                    inializeDice();
                }

            }
        }
    }

    public void yellowPlayerMovement2()
    {
        soundManager.playerAudioSource.Play();

        yellowPlayerBorder1.SetActive(false);
        yellowPlayerBorder2.SetActive(false);
        yellowPlayerBorder3.SetActive(false);
        yellowPlayerBorder4.SetActive(false);

        yellowPlayerButton1.interactable = false;
        yellowPlayerButton2.interactable = false;
        yellowPlayerButton3.interactable = false;
        yellowPlayerButton4.interactable = false;

        if (playerTurn == "yellow")
        {
            if ((yellowMovementBlock.Count - yellowPlayerSteps2) > selectDiceAnimation)
            {
                if (yellowPlayerSteps2 > 0)
                {
                    Vector3[] yellowPlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath2[i] = yellowMovementBlock[yellowPlayerSteps2 + i].transform.position;

                    }
                    yellowPlayerSteps2 = yellowPlayerSteps2 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "yellow";
                    }
                    else
                    {
                        playerTurn = "red";
                    }
                    if (yellowPlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer2, iTween.Hash("path", yellowPlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer2, iTween.Hash("position", yellowPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "yellow player 2";
                }
                else if (selectDiceAnimation == 6 && yellowPlayerSteps2 == 0)
                {
                    Vector3[] yellowPlayerPath2 = new Vector3[1];
                    yellowPlayerPath2[0] = yellowMovementBlock[yellowPlayerSteps2].transform.position;
                    yellowPlayerSteps2 += 1;
                    playerTurn = "yellow";
                    currentPlayerName = "yellow player 2";
                    iTween.MoveTo(yellowPlayer2, iTween.Hash("position", yellowPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "yellow" && (yellowMovementBlock.Count - yellowPlayerSteps2) == selectDiceAnimation)
                {
                    Vector3[] yellowPlayerPath2 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath2[i] = yellowMovementBlock[yellowPlayerSteps2 + i].transform.position;

                    }
                    yellowPlayerSteps2 += selectDiceAnimation;
                    if (yellowPlayerPath2.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer2, iTween.Hash("path", yellowPlayerPath2, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer2, iTween.Hash("position", yellowPlayerPath2[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "yellow";
                    totalyellowInHouse += 1;
                    yellowPlayerButton2.enabled = false;
                }
                else
                {
                    if (yellowPlayerSteps1 == 0 && yellowPlayerSteps3 == 0 && yellowPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "red";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void yellowPlayerMovement3()
    {
        soundManager.playerAudioSource.Play();

        yellowPlayerBorder1.SetActive(false);
        yellowPlayerBorder2.SetActive(false);
        yellowPlayerBorder3.SetActive(false);
        yellowPlayerBorder4.SetActive(false);

        yellowPlayerButton1.interactable = false;
        yellowPlayerButton2.interactable = false;
        yellowPlayerButton3.interactable = false;
        yellowPlayerButton4.interactable = false;

        if (playerTurn == "yellow")
        {
            if ((yellowMovementBlock.Count - yellowPlayerSteps3) > selectDiceAnimation)
            {
                if (yellowPlayerSteps3 > 0)
                {
                    Vector3[] yellowPlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath3[i] = yellowMovementBlock[yellowPlayerSteps3 + i].transform.position;

                    }
                    yellowPlayerSteps3 = yellowPlayerSteps3 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "yellow";
                    }
                    else
                    {
                        playerTurn = "red";
                    }
                    if (yellowPlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer3, iTween.Hash("path", yellowPlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer3, iTween.Hash("position", yellowPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "yellow player 3";
                }
                else if (selectDiceAnimation == 6 && yellowPlayerSteps3 == 0)
                {
                    Vector3[] yellowPlayerPath3 = new Vector3[1];
                    yellowPlayerPath3[0] = yellowMovementBlock[yellowPlayerSteps3].transform.position;
                    yellowPlayerSteps3 += 1;
                    playerTurn = "yellow";
                    currentPlayerName = "yellow player 3";
                    iTween.MoveTo(yellowPlayer3, iTween.Hash("position", yellowPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "yellow" && (yellowMovementBlock.Count - yellowPlayerSteps3) == selectDiceAnimation)
                {
                    Vector3[] yellowPlayerPath3 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath3[i] = yellowMovementBlock[yellowPlayerSteps3 + i].transform.position;

                    }
                    yellowPlayerSteps3 += selectDiceAnimation;
                    if (yellowPlayerPath3.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer3, iTween.Hash("path", yellowPlayerPath3, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer3, iTween.Hash("position", yellowPlayerPath3[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "yellow";
                    totalyellowInHouse += 1;
                    yellowPlayerButton3.enabled = false;
                }
                else
                {
                    if (yellowPlayerSteps1 == 0 && yellowPlayerSteps2 == 0 && yellowPlayerSteps4 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "red";
                    }
                    inializeDice();
                }

            }
        }
    }
    public void yellowPlayerMovement4()
    {
        soundManager.playerAudioSource.Play();

        yellowPlayerBorder1.SetActive(false);
        yellowPlayerBorder2.SetActive(false);
        yellowPlayerBorder3.SetActive(false);
        yellowPlayerBorder4.SetActive(false);

        yellowPlayerButton1.interactable = false;
        yellowPlayerButton2.interactable = false;
        yellowPlayerButton3.interactable = false;
        yellowPlayerButton4.interactable = false;

        if (playerTurn == "yellow")
        {
            if ((yellowMovementBlock.Count - yellowPlayerSteps4) > selectDiceAnimation)
            {
                if (yellowPlayerSteps4 > 0)
                {
                    Vector3[] yellowPlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath4[i] = yellowMovementBlock[yellowPlayerSteps4 + i].transform.position;

                    }
                    yellowPlayerSteps4 = yellowPlayerSteps4 + selectDiceAnimation;
                    if (selectDiceAnimation == 6)
                    {
                        playerTurn = "yellow";
                    }
                    else
                    {
                        playerTurn = "red";
                    }
                    if (yellowPlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer4, iTween.Hash("path", yellowPlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer4, iTween.Hash("position", yellowPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    currentPlayerName = "yellow player 4";
                }
                else if (selectDiceAnimation == 6 && yellowPlayerSteps4 == 0)
                {
                    Vector3[] yellowPlayerPath4 = new Vector3[1];
                    yellowPlayerPath4[0] = yellowMovementBlock[yellowPlayerSteps4].transform.position;
                    yellowPlayerSteps4 += 1;
                    playerTurn = "yellow";
                    currentPlayerName = "yellow player 4";
                    iTween.MoveTo(yellowPlayer4, iTween.Hash("position", yellowPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                }
            }
            ///completed
            else
            {
                if (playerTurn == "yellow" && (yellowMovementBlock.Count - yellowPlayerSteps4) == selectDiceAnimation)
                {
                    Vector3[] yellowPlayerPath4 = new Vector3[selectDiceAnimation];
                    for (int i = 0; i < selectDiceAnimation; i++)
                    {
                        yellowPlayerPath4[i] = yellowMovementBlock[yellowPlayerSteps4 + i].transform.position;

                    }
                    yellowPlayerSteps4 += selectDiceAnimation;
                    if (yellowPlayerPath4.Length > 1)
                    {
                        iTween.MoveTo(yellowPlayer4, iTween.Hash("path", yellowPlayerPath4, "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    else
                    {
                        iTween.MoveTo(yellowPlayer4, iTween.Hash("position", yellowPlayerPath4[0], "speed", 125, "time", 2.0f, "easetype", "easeOutExpo", "looptype", "none", "oncomplete", "inializeDice", "oncompletetarget", this.gameObject));
                    }
                    playerTurn = "yellow";
                    totalyellowInHouse += 1;
                    yellowPlayerButton4.enabled = false;
                }
                else
                {
                    if (yellowPlayerSteps1 == 0 && yellowPlayerSteps2 == 0 && yellowPlayerSteps3 == 0 && selectDiceAnimation != 6)
                    {
                        playerTurn = "red";
                    }
                    inializeDice();
                }

            }
        }
    }

}
