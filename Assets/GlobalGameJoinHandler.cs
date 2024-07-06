using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalGameJoinHandler : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    [SerializeField] bool isTimerLudo = false;
    [SerializeField] OnlineGameType onlineGameType;
    GameMode currentGameMode = GameMode.CLASSIC;
    [SerializeField] MainMenuScript mainMenuScript;
    [SerializeField] Button classicBt, timerBt;
    [SerializeField] GameObject classicCheck, timerCheck;
    [SerializeField] TMP_Dropdown playerCount;
    [SerializeField] Button nextBt;
    GlobalGameJoinData_JStruct globalGameJoinData;
    public OnlineGameJoinDataRoot_JStruct gameLobbyData_JStruct1;

    [SerializeField] GameObject lobbylistPnl;
    [SerializeField] Transform LobbyItemContent;
    [SerializeField] LobbyListItem prefab_lobbyListItem;

    [SerializeField] DialogBox dialogBox;
    [SerializeField] GameObject loadPnl;
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

        gameLobbyData_JStruct1 = new OnlineGameJoinDataRoot_JStruct();
        gameLobbyData_JStruct1.meta = new Meta();
        //gameLobbyData_JStruct1.data = new GlobalGameData_JStruct();
        //gameLobbyData_JStruct1.data.PlayersInGame = new List<PlayerItem_JStruct> { new PlayerItem_JStruct() };
        //gameLobbyData_JStruct1.data.PlayersInGame[0].PlayerID = APIHandler.instance.key_playerId;
        //gameLobbyData_JStruct1.data.PlayersInGame[0].PlayerTeam = "RED";

    }
    void JoinGlobalGame()
    {
        globalGameJoinData.LobbyId = APIHandler.instance.key_lastLobbyId != null ? APIHandler.instance.key_lastLobbyId : "";
        globalGameJoinData.TimerMode = (currentGameMode == GameMode.TIMER);
        globalGameJoinData.PlayerCount = int.Parse(playerCount.options[playerCount.value].text);
        if (!dummyMode)
        {
            loadPnl.SetActive(true);
            APIHandler.instance.PostJoinGlobalGame(globalGameJoinData, JoinGlobalGameCallback);
        }
        else
        {
            gameLobbyData_JStruct1.meta.status = true;
            JoinGlobalGameCallback(true, gameLobbyData_JStruct1);
        }
    }
    void JoinGlobalGame(string lobbyId,bool timer,int lobbyPlayersCount)
    {
        APIHandler.instance.SetLobbyID(lobbyId);
        globalGameJoinData.LobbyId = lobbyId;
        globalGameJoinData.TimerMode = timer;
        globalGameJoinData.PlayerCount = lobbyPlayersCount;
        globalGameJoinData.PlayerCount = int.Parse(playerCount.options[playerCount.value].text);
        if (!dummyMode)
        {
            loadPnl.SetActive(true);
            APIHandler.instance.PostJoinGlobalGame(globalGameJoinData, JoinGlobalGameCallback);
        }
        else
        {
            gameLobbyData_JStruct1.meta.status = true;
            JoinGlobalGameCallback(true, gameLobbyData_JStruct1);
        }
    }
    void JoinGlobalGameCallback(bool success, OnlineGameJoinDataRoot_JStruct gameLobbyData_JStruct)
    {
        loadPnl.SetActive(false);
        if (success)
        {
            if (gameLobbyData_JStruct1.meta.status)
            {
                //onlineGameType.globalGameRootData = gameLobbyData_JStruct1;
                mainMenuScript.four_player_online();
            }
            else
            {
                Debug.Log("JoinGlobalGameCallback : " + gameLobbyData_JStruct1.meta.msg);
                dialogBox.Show(gameLobbyData_JStruct1.meta.msg);
                APIHandler.instance.GetLobbyList(GetLobbyListCallback);
            }
        }
    }
    void GetLobbyListCallback(bool success, LobbiesData_JStruct lobbiesData_JStruct)
    {
        loadPnl.SetActive(false);
        foreach (Transform obj in LobbyItemContent)
        {
            Destroy(obj.gameObject);
        }

        lobbylistPnl.SetActive(true);

        if (success && lobbiesData_JStruct.meta.status)
        {
            foreach(var lobby in lobbiesData_JStruct.data)
            {
                LobbyListItem item = Instantiate(prefab_lobbyListItem, LobbyItemContent);
                item.Initialize(lobby, JoinGlobalGame);
            }
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
