using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalGameSelectHandler : MonoBehaviour
{
    [SerializeField] MainMenuScript mainMenuScript;
    PlayersCount playersCount = PlayersCount.TWO_Player;
    [SerializeField]Button TwoPlayerBt,ThreePlayerBt,FourPlayerBt;
    [SerializeField] Button nextBt;
    private void Start()
    {
        TwoPlayerBt.onClick.AddListener(SetTwoPlayer);
        ThreePlayerBt.onClick.AddListener(SetThreePlayer);
        FourPlayerBt.onClick.AddListener(SetFourPlayer);
        nextBt.onClick.AddListener(joinGame);
    }
    void SetTwoPlayer()
    {
        playersCount = PlayersCount.TWO_Player;
    }
    void SetThreePlayer()
    {
        playersCount = PlayersCount.THREE_PLAYER;
    }
    void SetFourPlayer()
    {
        playersCount = PlayersCount.FOUR_Player;
    }
    void joinGame()
    {
        switch(playersCount) 
        { 
            case PlayersCount.TWO_Player:
                mainMenuScript.two_player();
                break;
            case PlayersCount.THREE_PLAYER:
                mainMenuScript.three_player();
                break;
            case PlayersCount.FOUR_Player:
                mainMenuScript.four_player();
                break;
        }
    }
}
public enum PlayersCount
{
    TWO_Player = 2,
    THREE_PLAYER = 3,
    FOUR_Player = 4
}

