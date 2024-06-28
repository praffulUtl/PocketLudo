using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalGameJoinHandler : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    GameMode currentGameMode = GameMode.CLASSIC;
    [SerializeField] MainMenuScript mainMenuScript;
    [SerializeField] Button classicBt,quickBt;
    [SerializeField] GameObject classicCheck,quickCheck;
    [SerializeField] Button nextBt;
    GlobalGameJoinData_JStruct globalGameJoinData;
    public GameLobby_JStruct gameLobbyData_JStruct1;
    private void Start()
    {
        globalGameJoinData = new GlobalGameJoinData_JStruct();
        globalGameJoinData.PlayerID = APIHandler.instance.key_playerId;
        globalGameJoinData.GameMode = currentGameMode.ToString();
        classicCheck.SetActive(true);
        nextBt.onClick.AddListener(JoinGlobalGame);
        classicBt.onClick.AddListener(SetClassicMode);
        quickBt.onClick.AddListener(SetQuickMode);

        gameLobbyData_JStruct1 = new GameLobby_JStruct();
        gameLobbyData_JStruct1.meta = new Meta();
        gameLobbyData_JStruct1.data = new GameLobbyData_JStruct();
        gameLobbyData_JStruct1.data.PlayersInGame = new List<PlayerItem_JStruct> { new PlayerItem_JStruct() };
        gameLobbyData_JStruct1.data.PlayersInGame[0].PlayerID = APIHandler.instance.key_playerId;
        gameLobbyData_JStruct1.data.PlayersInGame[0].PlayerTeam = "RED";

    }
    void JoinGlobalGame()
    {
        globalGameJoinData.GameMode = currentGameMode.ToString();
        if (!dummyMode)
            APIHandler.instance.PostJoinGlobalGame(globalGameJoinData, JoinGlobalGameCallback);
        else
        {
            gameLobbyData_JStruct1.meta.status = true;
            JoinGlobalGameCallback(true, gameLobbyData_JStruct1);
        }
    }
    void JoinGlobalGameCallback(bool success,GameLobby_JStruct gameLobbyData_JStruct)
    {
        if(success && gameLobbyData_JStruct1.meta.status) 
        {
            mainMenuScript.four_player_online();
        }
    }
    void SetQuickMode()
    {
        quickCheck.SetActive(true);
        classicCheck.SetActive(false);
        currentGameMode = GameMode.QUICK;
    }
    void SetClassicMode()
    {
        quickCheck.SetActive(false);
        classicCheck.SetActive(true);
        currentGameMode = GameMode.CLASSIC;
    }

}
public enum GameMode
{
    QUICK,
    CLASSIC
}
