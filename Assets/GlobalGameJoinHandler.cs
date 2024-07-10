using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalGameJoinHandler : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    [SerializeField] string dummyLobbyList = "";
    [SerializeField] bool isTimerLudo = false;
    [SerializeField] OnlineGameType onlineGameType;
    GameMode currentGameMode = GameMode.CLASSIC;
    [SerializeField] MainMenuScript mainMenuScript;
    [SerializeField] Button classicBt, classicBtList, timerBt, timerBtList;
    [SerializeField] GameObject classicCheck, classicCheckList, timerCheck, timerCheckList;
    [SerializeField] TMP_Dropdown playerCount;
    [SerializeField] Button nextBt, listBt;
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
        classicCheckList.SetActive(true);
        nextBt.onClick.AddListener(JoinGlobalGame);
        listBt.onClick.AddListener(LoadLobbyList);
        classicBt.onClick.AddListener(SetClassicMode);
        timerBt.onClick.AddListener(SetQuickMode);
        classicBtList.onClick.AddListener(SetClassicMode);
        timerBtList.onClick.AddListener(SetQuickMode);
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
        globalGameJoinData.TimerMode = (currentGameMode == GameMode.TIMER);
        globalGameJoinData.PlayerCount = int.Parse(playerCount.options[playerCount.value].text);
        globalGameJoinData.LobbyId = null;
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
            if (gameLobbyData_JStruct.meta.status)
            {
                //onlineGameType.globalGameRootData = gameLobbyData_JStruct1;
                onlineGameType.lobbyId = gameLobbyData_JStruct.data.lobbyId;
                onlineGameType.isTimer = isTimerLudo;
                onlineGameType.gameType = GameType.GLOBAL;
                mainMenuScript.four_player_online();
            }
            else
            {
                Debug.Log("JoinGlobalGameCallback : " + gameLobbyData_JStruct.meta.msg);
                dialogBox.Show(gameLobbyData_JStruct.meta.msg);
                LoadLobbyList();
            }
        }
    }
    void LoadLobbyList()
    {
        loadPnl.SetActive(true);
        if (!dummyMode)
        APIHandler.instance.GetLobbyList(GetLobbyListCallback);
        else
        {
            LobbiesData_JStruct dt = JsonConvert.DeserializeObject<LobbiesData_JStruct>(dummyLobbyList);
            GetLobbyListCallback(true, dt);
        }

    }
    void GetLobbyListCallback(bool success, LobbiesData_JStruct lobbiesData_JStruct)
    {
        loadPnl.SetActive(false);
        if (LobbyItemContent.childCount > 0)
        {
            foreach (Transform obj in LobbyItemContent)
            {
                Destroy(obj.gameObject);
            }
        }

        lobbylistPnl.SetActive(true);

        if (success && lobbiesData_JStruct.meta.status)
        {
            foreach (var lobby in lobbiesData_JStruct.data)
            {
                LobbyListItem item = Instantiate(prefab_lobbyListItem, LobbyItemContent);
                item.Initialize(lobby, JoinGlobalGame);
                item.gameObject.SetActive(lobby.timerMode == isTimerLudo);
            }
        }
    }

    void SetQuickMode()
    {
        timerCheck.SetActive(true);
        classicCheck.SetActive(false);
        timerCheckList.SetActive(true);
        classicCheckList.SetActive(false);
        currentGameMode = GameMode.TIMER;
        isTimerLudo = (currentGameMode == GameMode.TIMER);
        SwitchLobbies();
    }
    void SetClassicMode()
    {
        timerCheck.SetActive(false);
        classicCheck.SetActive(true);
        timerCheckList.SetActive(false);
        classicCheckList.SetActive(true);
        currentGameMode = GameMode.CLASSIC;
        isTimerLudo = (currentGameMode == GameMode.TIMER);
        SwitchLobbies();
    }
    void SwitchLobbies()
    {
        if (LobbyItemContent.childCount > 0)
        {
            foreach (Transform obj in LobbyItemContent)
            {
                obj.gameObject.SetActive(false);
                obj.gameObject.SetActive(obj.GetComponent<LobbyListItem>().IsTimer == isTimerLudo);
            }
        }
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
