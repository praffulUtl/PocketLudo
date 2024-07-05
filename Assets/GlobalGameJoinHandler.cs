using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalGameJoinHandler : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    [SerializeField] OnlineGameType onlineGameType;
    GameMode currentGameMode = GameMode.CLASSIC;
    [SerializeField] MainMenuScript mainMenuScript;
    [SerializeField] Button classicBt,timerBt;
    [SerializeField] GameObject classicCheck,timerCheck;
    [SerializeField] TMP_Dropdown playerCount;
    [SerializeField] Button nextBt;
    GlobalGameJoinData_JStruct globalGameJoinData;
    public GlobalGameRootData_JStruct gameLobbyData_JStruct1;
    private void Start()
    {
        globalGameJoinData = new GlobalGameJoinData_JStruct();
        globalGameJoinData.PlayerID = APIHandler.instance.key_playerId;
        globalGameJoinData.TimerMode = (currentGameMode == GameMode.TIMER);
        classicCheck.SetActive(true);
        nextBt.onClick.AddListener(JoinGlobalGame);
        classicBt.onClick.AddListener(SetClassicMode);
        timerBt.onClick.AddListener(SetQuickMode);
        playerCount.onValueChanged.AddListener(checkDropdownValue);

        gameLobbyData_JStruct1 = new GlobalGameRootData_JStruct();
        gameLobbyData_JStruct1.meta = new Meta();
        gameLobbyData_JStruct1.data = new GlobalGameData_JStruct();
        gameLobbyData_JStruct1.data.PlayersInGame = new List<PlayerItem_JStruct> { new PlayerItem_JStruct() };
        gameLobbyData_JStruct1.data.PlayersInGame[0].PlayerID = APIHandler.instance.key_playerId;
        gameLobbyData_JStruct1.data.PlayersInGame[0].PlayerTeam = "RED";

    }
    void JoinGlobalGame()
    {
        globalGameJoinData.TimerMode = (currentGameMode == GameMode.TIMER);
        globalGameJoinData.BetAmount = 100;
        globalGameJoinData.PlayerCount = int.Parse(playerCount.options[playerCount.value].text);
        if (!dummyMode)
            APIHandler.instance.PostJoinGlobalGame(globalGameJoinData, JoinGlobalGameCallback);
        else
        {
            gameLobbyData_JStruct1.meta.status = true;
            JoinGlobalGameCallback(true, gameLobbyData_JStruct1);
        }
    }
    void JoinGlobalGameCallback(bool success,GlobalGameRootData_JStruct gameLobbyData_JStruct)
    {
        if(success && gameLobbyData_JStruct1.meta.status) 
        {
            onlineGameType.globalGameRootData = gameLobbyData_JStruct1;
            mainMenuScript.four_player_online();
        }
    }
    void SetQuickMode()
    {
        timerCheck.SetActive(true);
        classicCheck.SetActive(false);
        currentGameMode = GameMode.TIMER;
    }
    void SetClassicMode()
    {
        timerCheck.SetActive(false);
        classicCheck.SetActive(true);
        currentGameMode = GameMode.CLASSIC;
    }
    void checkDropdownValue(int i)
    {
        Debug.Log(int.Parse(playerCount.options[playerCount.value].text));
    }
}
public enum GameMode
{
    TIMER,
    CLASSIC
}
