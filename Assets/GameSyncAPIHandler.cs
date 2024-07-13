using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine.UI;
using System.Security.Cryptography;
using Newtonsoft.Json.Converters;
using WebSocketSharp;

public class GameSyncAPIHandler : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    [SerializeField] string dummyData = "";
    [SerializeField] Image redTimerImg, blueTimerImg, greenTimerImg, yellowTimerImg;
    [SerializeField] OnlineGameType DataKeeper;
    [SerializeField] string dummydata = "";
    [SerializeField] GameScriptOnline gameScript;
    [SerializeField] WaitingScreen waitingScreen;
    [SerializeField] Button startGameBt;
    float playerTurnDuration = 30f;
    List<string> playerTeams = new List<string> {"R","B","G","Y"};
    bool checkPlayerNotInGame = false;
    public DataRoot<OurPlayerDataSet> dataToBeSent;
    DataRoot<GetPlayers> getPlayersLobbyData;
    public string ourPlayerTeam = "RED"; // R B G Y
    bool diceRolled = false;
    string lastPlayerTurn = "";
    int lobbyId;
    PlayerTeam ourplayerTeam;
    string sendJson = "";

    //private ClientWebSocket webSocket;
    private Uri serverUri = new Uri("ws://localhost:8080"); // Replace with your server address

    public bool socketConnected = false;
                    WebSocketSharp.WebSocket wes;

    private void Awake()
    {
        dataToBeSent = new DataRoot<OurPlayerDataSet>();
        dataToBeSent.type = "movePiece";
        dataToBeSent.data = new OurPlayerDataSet();
        dataToBeSent.data.PlayerPiece = new List<PlayerPiece> { new PlayerPiece(), new PlayerPiece(), new PlayerPiece(), new PlayerPiece() };

        getPlayersLobbyData = new DataRoot<GetPlayers>();
        getPlayersLobbyData.data = new GetPlayers();
    }

    void Start()
    {
        Debug.Log("Connection attempt");
                    wes = new WebSocket("ws://localhost:8080");
                    wes.OnOpen += OnConnection;
                    wes.OnClose += OnwsClose;
                    wes.OnMessage += OnMessage;
                    wes.Connect();
        if(dummyMode)
        {
            LobbyPlayers lobbyPlayers = JsonConvert.DeserializeObject<LobbyPlayers>(dummyData);
            StartProcess(true,lobbyPlayers);
            return;
        }
        bool playerTeamFound = false;
        DataKeeper = FindAnyObjectByType<OnlineGameType>();

        if (DataKeeper.gameType == GameType.GLOBAL)
        {
            dataToBeSent.data.gameLobbyID = DataKeeper.lobbyId;
            dataToBeSent.data.playerID = APIHandler.instance.key_playerId;
        }
        else if (DataKeeper.gameType == GameType.TOURNAMENT && DataKeeper.joinTurnamentJoinData.meta != null)
        {
            foreach (var player in DataKeeper.joinTurnamentJoinData.data.PlayersInGame)
            {
                if (!playerTeamFound && player.PlayerID == APIHandler.instance.key_playerId)
                {
                    ourPlayerTeam = player.PlayerTeam;
                    playerTeamFound = true;
                    break;
                }
            }
        }
        if (APIHandler.instance == null)
            Debug.Log("APIHandler.instance is null");

        APIHandler.instance.GetLobbyPlayers(DataKeeper.lobbyId, StartProcess);
        Destroy(DataKeeper.gameObject);

        startGameBt.onClick.AddListener(StartGame);
    }
    private async void StartProcess(bool success, LobbyPlayers lobbyPlayers)
    {
        if (success)
        {
            if (lobbyPlayers.meta.status)
            {
                waitingScreen.actionOnTimerEnd = OnTimerEnd;
                playerTurnDuration = lobbyPlayers.data.PlayerTurnSeconds;
                gameScript.OnInitializeDiceActiion += SetupPlayerAutoTurn;
                waitingScreen.ShowPlayerCount(lobbyPlayers.data.players.Count);
                waitingScreen.ShowLobbyid(lobbyPlayers.data.lobbyId);
                foreach (var player in lobbyPlayers.data.players)
                {
                    if (player.PlayerId == APIHandler.instance.key_playerId)
                        dataToBeSent.data.playerTeam = player.playerTeam;
                    PlayerTeam playerTeam = Enum.Parse<PlayerTeam>(player.playerTeam);
                    ourplayerTeam = playerTeam;
                    switch (playerTeam)
                    {
                        case PlayerTeam.R:
                            ourPlayerTeam = "RED";
                            break;
                        case PlayerTeam.B:
                            ourPlayerTeam = "BLUE";
                            break;
                        case PlayerTeam.G:
                            ourPlayerTeam = "GREEN";
                            break;
                        case PlayerTeam.Y:
                            ourPlayerTeam = "YELLOW";
                            break;
                    }
                }

                gameScript.SetOurPlayerPieceButton(ourPlayerTeam);


                double tmp = lobbyPlayers.data.createdAt;
                DateTime now = DateTime.Now;
                DateTime end = UnixTimestampConverter.UnixTimeStampToDateTime(tmp + lobbyPlayers.data.RemainSeconds * 1000);
                double sec = (end - now).TotalSeconds;
                Debug.Log("seconds : " + sec);
                Debug.Log("created at time : "+ now);
                Debug.Log("end at time : " + end);

                waitingScreen.SetTime(dummyMode ? 60 : sec, ourplayerTeam);

                lobbyId = lobbyPlayers.data.lobbyId;

                if (!dummyMode)
                {
                    //webSocket = new ClientWebSocket();
                    //await Connect();
                    //await SendRequest();
                    //await ReceiveResponse();

                }
            }
        } 

    }
    void OnConnection(System.Object obj, EventArgs e)
    {
        Debug.Log("Connection open");
        StartCoroutine(startSendingmsg()) ;
    }
    void OnwsClose(System.Object send, CloseEventArgs e)
    {
        Debug.Log("Closed");
    }
    void OnMessage(System.Object send, MessageEventArgs e)
    {
        Debug.Log($"Response received: {e.Data}");

    }
    void SendData(string json)
    {
        wes.Send(json);
    }

    IEnumerator startSendingmsg()
    {
        var waitSec = new WaitForSeconds(5f);
        while(true)
        {
            SendData("Hello from unity");
            yield return waitSec;
        }
    }

    void StartGame()
    {
        Debug.Log("StartGame");

        DataRoot<StartGameData> startData = new DataRoot<StartGameData>();
        startData.type = "startGame";
        startData.data = new StartGameData();
        startData.data.lobbyId = lobbyId;
        startData.data.playerId = APIHandler.instance.key_playerId;
        startData.data.startGame = true;

        Debug.Log("StartGame--1");
        Debug.Log("StartGame : "+startData.data.lobbyId);
        sendJson = JsonConvert.SerializeObject(startData);
        Debug.Log("ws StartGame sent : " + sendJson);
        Task task = SendRequest();
        StartCoroutine(GetPlayersInLobby());
    }
    IEnumerator GetPlayersInLobby()
    {
        var waitSec = new WaitForSeconds(5f);
        int times = 5;
        while (times > 0)
        {
            Debug.Log("GetPlayersInLobby");
            getPlayersLobbyData.type = "getPlayers";
            getPlayersLobbyData.data.lobbyID = lobbyId;
            sendJson = JsonConvert.SerializeObject(getPlayersLobbyData);
            Debug.Log("ws GetPlayersInLobby : " + sendJson);
            Task task = SendRequest();
            times--;
            yield return waitSec;
        }
    }

    void OnTimerEnd()
    {
        Debug.Log("setup turn");
        StartCoroutine(SetupTurns());
        StartCoroutine(checkForPlayerTurnChange());
        waitingScreen.close();
    }
    void StartGlobalGameCallback(bool success, StartGame_JStruct startGameData)
    {
        if(success)
        {
            if(startGameData.meta.status)
            {

            }

        }
    }

    private async Task Connect()
    {
        try
        {
            //await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("WebSocket connected!");
            socketConnected = true;
            StartGame();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection error: {e.Message}");
        }
    }

    private async Task SendRequest()
    {
        //var requestBody = new
        //{
        //    meta = new
        //    {
        //        msg = "Profile data found successfully",
        //        status = true
        //    },
        //    data = new
        //    {
        //        GameLobbyID = "",
        //        PlayerID = "",
        //        PlayerTeam = "",
        //        DiceNumber = -1,
        //        Playerpiece = new List<PlayerPiece>
        //        {
        //            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 },
        //            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 },
        //            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 },
        //            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 }
        //        }
        //    }
        //};

        //string jsonRequest = JsonConvert.SerializeObject(requestBody);
        string jsonRequest = sendJson;
        Debug.Log("Request sent data : "+jsonRequest);
        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonRequest);

        try
        {
            //await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
            Debug.Log("Request sent!");
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket send error: {e.Message}");
        }
    }

    private async Task ReceiveResponse()
    {
        //DiceRollButton.enabled = dataToBeSent.data.PlayerTurn;
        var buffer = new byte[1024];
        //WebSocketReceiveResult result;

        try
        {
            //result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            //string responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            //Debug.Log($"Response received: {responseJson}");

            //var responseObject = JsonConvert.DeserializeObject<DataRoot<System.Object>>(responseJson);
            //if (responseObject.type == "movePiece")
            //    ProcessResponseData(responseJson);
            //if (responseObject.type == "getPlayers")
            //    ProcessLobbyData(responseJson);


            // Handle the response object as needed
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket receive error: {e.Message}");
        }
    }

    public void SendPlayerData()
    {
        sendJson = JsonConvert.SerializeObject(dataToBeSent);
        Debug.Log("ws SendPlayerData : " + sendJson);
        Task task = SendRequest();

        //task.Start();

        //RenamedResponse renamedResponse = JsonConvert.DeserializeObject<RenamedResponse>(dummydata);

    }
    [ContextMenu("TriggerDummyResponse")]
    public void TriggerDummyResponse()
    {
        ProcessResponseData(dummydata);
    }
    void ProcessLobbyData(string json)
    {
        var responseObject = JsonConvert.DeserializeObject<DataRoot<LobbyPlayerData>>(json);
        waitingScreen.ShowPlayerCount(responseObject.data.players.Count);
    }
    void ProcessResponseData(string jsonString)
    {
        dataToBeSent.data.playerTurn = true;
        var responseObject = JsonConvert.DeserializeObject<DataRoot<OtherPlayersData>>(jsonString);
        Debug.Log("sec :" + responseObject.data.remainSeconds);

        //if (!checkPlayerNotInGame)
        //{
        //    foreach (var player in responseObject.data.OtherPlayer)
        //    {
        //        playerTeams.Remove(player.PlayerTeam);
        //    }
        //    checkPlayerNotInGame = false;
        //}
        //foreach (var player in responseObject.data.OtherPlayer)
        //{
        //    if (player.PlayerTeam != GetColorInitial(ourPlayerTeam) && !playerTeams.Contains(player.PlayerTeam))
        //        gameScript.DiceRoll(0);
        //}

        foreach (var player in responseObject.data.OtherPlayer)
        {
            if (waitingScreen.isOpen && responseObject.data.remainSeconds > 0)
            {
                Debug.Log("wait screen :" + player.PlayerTeam);
                waitingScreen.SetTime(responseObject.data.remainSeconds,ourplayerTeam);
                //if (waitingScreen.InitializeCount < 4)
                //    waitingScreen.AddPlayer(player.PlayerTeam);
            }
            else
            {
                waitingScreen.close();

                if (player.PlayerTurn && dataToBeSent.data.playerTurn)
                {
                    dataToBeSent.data.playerTurn = false;
                    // DiceRollButton.enabled = dataToBeSent.data.PlayerTurn;
                }

                if (!diceRolled)
                {
                    if (player.PlayerTeam == GetColorInitial(gameScript.PlayerTurn))
                    {
                        lastPlayerTurn = gameScript.PlayerTurn;
                        gameScript.DiceRoll(player.DiceNumber);
                        if (player.DiceNumber > 0)
                            diceRolled = true;
                        return;
                    }
                }
                else
                {
                    Debug.Log("dice rolled");
                    if (player.PlayerTeam == PlayerTeam.R.ToString())
                    {
                        if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                        {
                            if (player.Playerpiece[0].movementBlockIndex > 0)
                                gameScript.redPlayerI_UI(1);
                            else if (player.Playerpiece[1].movementBlockIndex > 0)
                                gameScript.redPlayerII_UI(1);
                            else if (player.Playerpiece[2].movementBlockIndex > 0)
                                gameScript.redPlayerIII_UI(1);
                            else if (player.Playerpiece[3].movementBlockIndex > 0)
                                gameScript.redPlayerIV_UI(1);
                        }
                    }
                    else if (player.PlayerTeam == PlayerTeam.B.ToString())
                    {
                        if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                        {
                            if (player.Playerpiece[0].movementBlockIndex > 0)
                                gameScript.bluePlayerI_UI(1);
                            else if (player.Playerpiece[1].movementBlockIndex > 0)
                                gameScript.bluePlayerII_UI(1);
                            else if (player.Playerpiece[2].movementBlockIndex > 0)
                                gameScript.bluePlayerIII_UI(1);
                            else if (player.Playerpiece[3].movementBlockIndex > 0)
                                gameScript.bluePlayerIV_UI(1);
                        }
                    }
                    else if (player.PlayerTeam == PlayerTeam.G.ToString())
                    {
                        if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                        {
                            if (player.Playerpiece[0].movementBlockIndex > 0)
                                gameScript.greenPlayerI_UI(1);
                            else if (player.Playerpiece[1].movementBlockIndex > 0)
                                gameScript.greenPlayerII_UI(1);
                            else if (player.Playerpiece[2].movementBlockIndex > 0)
                                gameScript.greenPlayerIII_UI(1);
                            else if (player.Playerpiece[3].movementBlockIndex > 0)
                                gameScript.greenPlayerIV_UI(1);
                        }
                    }
                    else if (player.PlayerTeam == PlayerTeam.Y.ToString())
                    {
                        if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                        {
                            if (player.Playerpiece[0].movementBlockIndex > 0)
                                gameScript.yellowPlayerI_UI(1);
                            else if (player.Playerpiece[1].movementBlockIndex > 0)
                                gameScript.yellowPlayerII_UI(1);
                            else if (player.Playerpiece[2].movementBlockIndex > 0)
                                gameScript.yellowPlayerIII_UI(1);
                            else if (player.Playerpiece[3].movementBlockIndex > 0)
                                gameScript.yellowPlayerIV_UI(1);
                        }
                    }
                    if (player.PlayerTeam == GetColorInitial(gameScript.PlayerTurn))
                    {
                        diceRolled = false;
                        Debug.Log("dice rolled--2" + diceRolled);
                    }
                }
            }
        }
    }

    IEnumerator SetupTurns()
    {
        Debug.Log("SetupTurns");
        yield return new WaitForSeconds(1f);
        int i = 0;
        while(i<4)
        {
            gameScript.DiceRoll(0);
            i++;
        }
    }

    IEnumerator checkForPlayerTurnChange()
    {
        var waitSec = new WaitForSeconds(0.25f);
        while(true) 
        {
            if(diceRolled && lastPlayerTurn != gameScript.PlayerTurn)
            {
                lastPlayerTurn = gameScript.PlayerTurn;
                diceRolled = false;
            }
            //if (gameScript.PlayerTurn == ourPlayerTeam)
            //{
            //    if (StartOurPlayerTimerCoroutine == null)
            //    {
            //        StartOurPlayerTimerCoroutine = StartCoroutine(StartOurPlayerTimer());
            //    }
            //}
            //else if (StartOurPlayerTimerCoroutine != null)
            //{
            //    StopCoroutine(StartOurPlayerTimerCoroutine);
            //    StartOurPlayerTimerCoroutine = null;
            //}

            if (gameScript.PlayerTurn != lastPlayerTurn)
            {
                if (StartOtherPlayerTimerCoroutine == null)
                {
                    StartOtherPlayerTimerCoroutine = StartCoroutine(StartOtherPlayerTimer());
                }
            }
            else if (StartOtherPlayerTimerCoroutine != null)
            {
                StopCoroutine(StartOurPlayerTimerCoroutine);
                StartOtherPlayerTimerCoroutine = null;
            }

            yield return waitSec;
        }
    }

    public void SetupPlayerAutoTurn()
    {
        if (StartOtherPlayerTimerCoroutine == null)
        StartOtherPlayerTimerCoroutine = StartCoroutine(StartOtherPlayerTimer());
    }

    public void ResetDataToBeSent()
    {        
        foreach(var piece in dataToBeSent.data.PlayerPiece)
        {
            piece.movementBlockIndex = -1;
        }
    }

    Coroutine StartOurPlayerTimerCoroutine;
    IEnumerator StartOurPlayerTimer()
    {
        var waitsec = new WaitForSeconds(1f);
        int countSec = 15;
        while(countSec>0)
        {
            countSec--;
            Debug.Log("Our turn :"+countSec);
            yield return waitsec;
        }
        Debug.Log("Player turn over");
        StopOurPlayerTimer();
    }
    void StopOurPlayerTimer()
    {
        StopCoroutine(StartOurPlayerTimerCoroutine);
        StartOurPlayerTimerCoroutine = null;

        if (gameScript.selectDiceNumAnimation > 0)
        {
            if (gameScript.PlayerTurn == ourPlayerTeam)
            {
                switch (gameScript.PlayerTurn)
                {
                    case "RED":
                        if (gameScript.redPlayerI_Border.activeInHierarchy)
                            gameScript.redPlayerI_UI(1);
                        else if (gameScript.redPlayerII_Border.activeInHierarchy)
                            gameScript.redPlayerII_UI(1);
                        else if (gameScript.redPlayerIII_Border.activeInHierarchy)
                            gameScript.redPlayerIII_UI(1);
                        else if (gameScript.redPlayerIV_Border.activeInHierarchy)
                            gameScript.redPlayerIV_UI(1);
                        break;
                    case "BLUE":
                        if (gameScript.bluePlayerI_Border.activeInHierarchy)
                        {
                            Debug.Log("Blue player piece 1");
                            gameScript.bluePlayerI_UI(1);
                        }
                        else if (gameScript.bluePlayerII_Border.activeInHierarchy)
                            gameScript.bluePlayerII_UI(1);
                        else if (gameScript.bluePlayerIII_Border.activeInHierarchy)
                            gameScript.bluePlayerIII_UI(1);
                        else if (gameScript.bluePlayerIV_Border.activeInHierarchy)
                            gameScript.bluePlayerIV_UI(1);
                        break;
                    case "GREEN":
                        if (gameScript.greenPlayerI_Border.activeInHierarchy)
                            gameScript.greenPlayerI_UI(1);
                        else if (gameScript.greenPlayerII_Border.activeInHierarchy)
                            gameScript.greenPlayerII_UI(1);
                        else if (gameScript.greenPlayerIII_Border.activeInHierarchy)
                            gameScript.greenPlayerIII_UI(1);
                        else if (gameScript.greenPlayerIV_Border.activeInHierarchy)
                            gameScript.greenPlayerIV_UI(1);
                        break;
                    case "YELLOW":
                        if (gameScript.yellowPlayerI_Border.activeInHierarchy)
                            gameScript.yellowPlayerI_UI(1);
                        else if (gameScript.yellowPlayerII_Border.activeInHierarchy)
                            gameScript.yellowPlayerII_UI(1);
                        else if (gameScript.yellowPlayerIII_Border.activeInHierarchy)
                            gameScript.yellowPlayerIII_UI(1);
                        else if (gameScript.yellowPlayerIV_Border.activeInHierarchy)
                            gameScript.yellowPlayerIV_UI(1);
                        break;
                }

            }
        }
        else
        {
            gameScript.DiceRoll(1);
        }
    }


    Coroutine StartOtherPlayerTimerCoroutine;
    IEnumerator StartOtherPlayerTimer()
    {
        var waitsec = new WaitForSeconds(1f);
        float countSec = playerTurnDuration;
        while (countSec > 0)
        {
            redTimerImg.fillAmount = 0;
            blueTimerImg.fillAmount = 0;
            greenTimerImg.fillAmount = 0;
            yellowTimerImg.fillAmount = 0;

            float fill = countSec / 30f;
            //Debug.Log( countSec+" : "+fill);
            switch (gameScript.PlayerTurn)
            {
                case "RED":
                    redTimerImg.fillAmount = fill;
                    break;
                case "BLUE":
                    blueTimerImg.fillAmount = fill;
                    break;
                case "GREEN":
                    greenTimerImg.fillAmount = fill;
                    break;
                case "YELLOW":
                    yellowTimerImg.fillAmount = fill;
                    break;
            }
            countSec--;
            //Debug.Log("others turn :" + countSec);
            yield return waitsec;
        }
        //Debug.Log("other Player turn over");
        StopOtherPlayerTimer();
    }
    void StopOtherPlayerTimer()
    {
        Debug.Log("sTOP : " + gameScript.PlayerTurn);
        StopCoroutine(StartOtherPlayerTimerCoroutine);
        StartOtherPlayerTimerCoroutine = null;

        if (gameScript.selectDiceNumAnimation > 0)
        {
            Debug.Log("Blue player turn :"+ gameScript.selectDiceNumAnimation);
            switch (gameScript.PlayerTurn)
            {
                case "RED":
                    if (gameScript.redPlayerI_Border.activeInHierarchy)
                        gameScript.redPlayerI_UI(1);
                    else if (gameScript.redPlayerII_Border.activeInHierarchy)
                        gameScript.redPlayerII_UI(1);
                    else if (gameScript.redPlayerIII_Border.activeInHierarchy)
                        gameScript.redPlayerIII_UI(1);
                    else if (gameScript.redPlayerIV_Border.activeInHierarchy)
                        gameScript.redPlayerIV_UI(1);
                    break;
                case "BLUE":
                    Debug.Log("Blue player turn");
                    if (gameScript.bluePlayerI_Border.activeInHierarchy)
                    {
                        Debug.Log("Blue player piece 1");
                        gameScript.bluePlayerI_UI(1);
                    }
                    else if (gameScript.bluePlayerII_Border.activeInHierarchy)
                        gameScript.bluePlayerII_UI(1);
                    else if (gameScript.bluePlayerIII_Border.activeInHierarchy)
                        gameScript.bluePlayerIII_UI(1);
                    else if (gameScript.bluePlayerIV_Border.activeInHierarchy)
                        gameScript.bluePlayerIV_UI(1);
                    break;
                case "GREEN":
                    if (gameScript.greenPlayerI_Border.activeInHierarchy)
                        gameScript.greenPlayerI_UI(1);
                    else if (gameScript.greenPlayerII_Border.activeInHierarchy)
                        gameScript.greenPlayerII_UI(1);
                    else if (gameScript.greenPlayerIII_Border.activeInHierarchy)
                        gameScript.greenPlayerIII_UI(1);
                    else if (gameScript.greenPlayerIV_Border.activeInHierarchy)
                        gameScript.greenPlayerIV_UI(1);
                    break;
                case "YELLOW":
                    if (gameScript.yellowPlayerI_Border.activeInHierarchy)
                        gameScript.yellowPlayerI_UI(1);
                    else if (gameScript.yellowPlayerII_Border.activeInHierarchy)
                        gameScript.yellowPlayerII_UI(1);
                    else if (gameScript.yellowPlayerIII_Border.activeInHierarchy)
                        gameScript.yellowPlayerIII_UI(1);
                    else if (gameScript.yellowPlayerIV_Border.activeInHierarchy)
                        gameScript.yellowPlayerIV_UI(1);
                    break;
            }
        }
        else
        {
            gameScript.DiceRoll(1);
        }
    }
    private void OnApplicationQuit()
    {
        //webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        //webSocket?.Dispose();
    }

    #region gameplay
    //public void DiceRoll(int i = -1)
    //{
    //    SoundManagerScript.diceAudioSource.Play();
    //    DiceRollButton.interactable = false;

    //    if (i == -1)
    //    {
    //        selectDiceNumAnimation = UnityEngine.Random.Range(1, 7);
    //        Task task = SendRequest();
    //        task.Start();
    //    }
    //    else
    //        selectDiceNumAnimation = i;

    //    switch (selectDiceNumAnimation)
    //    {
    //        case 1:
    //            dice1_Roll_Animation.SetActive(true);
    //            dice2_Roll_Animation.SetActive(false);
    //            dice3_Roll_Animation.SetActive(false);
    //            dice4_Roll_Animation.SetActive(false);
    //            dice5_Roll_Animation.SetActive(false);
    //            dice6_Roll_Animation.SetActive(false);
    //            break;

    //        case 2:
    //            dice1_Roll_Animation.SetActive(false);
    //            dice2_Roll_Animation.SetActive(true);
    //            dice3_Roll_Animation.SetActive(false);
    //            dice4_Roll_Animation.SetActive(false);
    //            dice5_Roll_Animation.SetActive(false);
    //            dice6_Roll_Animation.SetActive(false);
    //            break;

    //        case 3:
    //            dice1_Roll_Animation.SetActive(false);
    //            dice2_Roll_Animation.SetActive(false);
    //            dice3_Roll_Animation.SetActive(true);
    //            dice4_Roll_Animation.SetActive(false);
    //            dice5_Roll_Animation.SetActive(false);
    //            dice6_Roll_Animation.SetActive(false);
    //            break;

    //        case 4:
    //            dice1_Roll_Animation.SetActive(false);
    //            dice2_Roll_Animation.SetActive(false);
    //            dice3_Roll_Animation.SetActive(false);
    //            dice4_Roll_Animation.SetActive(true);
    //            dice5_Roll_Animation.SetActive(false);
    //            dice6_Roll_Animation.SetActive(false);
    //            break;

    //        case 5:
    //            dice1_Roll_Animation.SetActive(false);
    //            dice2_Roll_Animation.SetActive(false);
    //            dice3_Roll_Animation.SetActive(false);
    //            dice4_Roll_Animation.SetActive(false);
    //            dice5_Roll_Animation.SetActive(true);
    //            dice6_Roll_Animation.SetActive(false);
    //            break;

    //        case 6:
    //            dice1_Roll_Animation.SetActive(false);
    //            dice2_Roll_Animation.SetActive(false);
    //            dice3_Roll_Animation.SetActive(false);
    //            dice4_Roll_Animation.SetActive(false);
    //            dice5_Roll_Animation.SetActive(false);
    //            dice6_Roll_Animation.SetActive(true);
    //            break;
    //    }

    //    gameScript.CallPlayersNotInitialized();
    //}

    #endregion

    string GetColorInitial(string clrStr)
    {
        if (clrStr == "RED")
            return "R";
        else if (clrStr == "BLUE")
            return "B";
        else if (clrStr == "GREEN")
            return "G";
        else//yellow
            return "Y";
    }
}

#region JStruct
[Serializable]
public class PlayerPiece
{
    public bool isOpen { get; set; }
    public bool reachedWinPos { get; set; }
    public int movementBlockIndex { get; set; }
}

[Serializable]
public class RenamedResponse
{
    public RenamedMeta meta;
    public OtherPlayersData data;
}

[Serializable]
public class RenamedMeta
{
    public string msg;
    public bool status;
}

[Serializable]
public class OtherPlayersData
{
    public int remainSeconds;
    public List<RenamedOtherPlayer> OtherPlayer;
}

[Serializable]
public class RenamedOtherPlayer
{
    public string PlayerTeam;
    public int DiceNumber;
    public bool PlayerTurn { get; set; }
    public List<PlayerPiece> Playerpiece;
}


[Serializable]
public class OurPlayerDataSet
{
    public int gameLobbyID { get; set; }
    public string playerID { get; set; }
    public string playerTeam { get; set; }
    public int diceNumber { get; set; }
    public bool playerTurn { get; set; }
    public List<PlayerPiece> PlayerPiece { get; set; }
}
[Serializable]

public class StartGameData
{
    public int lobbyId { get; set; }
    public string playerId { get; set; }
    public bool startGame { get; set; }
}

public class GetPlayers
{
    public int lobbyID { get; set; }
}
public class DataRoot<T>
{
    public string type { get; set; }
    public T data { get; set; }
}

public class LobbyPlayer
{
    public string playerName { get; set; }
    public string playerImageUrl { get; set; }
    public string _id { get; set; }
}

public class LobbyPlayerData
{
    public List<LobbyPlayer> players { get; set; }
}

public class LobbyPlayerDataRoot
{
    public Meta meta { get; set; }
}
public enum PlayerTeam
{
    NONE,
    R,
    B,
    G,
    Y

}
#endregion
public class UnixTimestampConverter
{
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddMilliseconds(unixTimeStamp).ToLocalTime();
    }
}