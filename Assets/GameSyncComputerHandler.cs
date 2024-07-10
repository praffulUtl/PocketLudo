using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine.UI;

public class GameSyncComputerHandler : MonoBehaviour
{
    [SerializeField] Image redTimerImg, blueTimerImg, greenTimerImg, yellowTimerImg;
    [SerializeField] OnlineGameType DataKeeper;
    [SerializeField] string dummydata = "";
    [SerializeField] GameScriptComputer gameScript;
    [SerializeField] WaitingScreen waitingScreen;
    List<string> playerTeams = new List<string> { "R", "B", "G", "Y" };
    bool checkPlayerNotInGame = false;
    public OurPlayerDataSetRoot2 dataToBeSent;
    public string ourPlayerTeam = "RED"; // R B G Y
    bool diceRolled = false;
    string lastPlayerTurn = "";

    private ClientWebSocket webSocket;
    private Uri serverUri = new Uri("wss://3sqlfz6r-8080.inc1.devtunnels.ms/"); // Replace with your server address

    private void Awake()
    {
        dataToBeSent = new OurPlayerDataSetRoot2();
        dataToBeSent.meta = new RenamedMeta2();
        dataToBeSent.data = new OurPlayerDataSet2();
        dataToBeSent.data.Playerpiece = new List<PlayerPiece2> { new PlayerPiece2(), new PlayerPiece2(), new PlayerPiece2(), new PlayerPiece2() };
    }


    private void Start()
    {
        StartCoroutine(SetupTurns());
        StartCoroutine(checkForPlayerTurnChange());

        gameScript.OnInitializeDiceActiion += SetupPlayerAutoTurn;

        DataKeeper = FindAnyObjectByType<OnlineGameType>();
        bool playerTeamFound = false;
        //if (DataKeeper.gameType == GameType.TOURNAMENT && DataKeeper.joinTurnamentJoinData.meta != null)
        //{
        //    foreach (var player in DataKeeper.joinTurnamentJoinData.data.PlayersInGame)
        //    {
        //        if (!playerTeamFound && player.PlayerID == APIHandler.instance.key_playerId)
        //        {
        //            ourPlayerTeam = player.PlayerTeam;
        //            playerTeamFound = true;
        //            break;
        //        }
        //    }
        //}
        //else if (DataKeeper.gameType == GameType.GLOBAL && DataKeeper.globalGameRootData.meta != null)
        //{
        //    //foreach (var player in DataKeeper.globalGameRootData.data.PlayersInGame)
        //    //{
        //    //    if (!playerTeamFound && player.PlayerID == APIHandler.instance.key_playerId)
        //    //    {
        //    //        ourPlayerTeam = player.PlayerTeam;
        //    //        playerTeamFound = true;
        //    //        break;
        //    //    }
        //    //}
        //}

        dataToBeSent.data.PlayerTeam = GetColorInitial(ourPlayerTeam);

        gameScript.SetOurPlayerPieceButton(ourPlayerTeam);

        //webSocket = new ClientWebSocket();
        //await Connect();
        //await SendRequest();
        //await ReceiveResponse();


    }

    private async Task Connect()
    {
        try
        {
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("WebSocket connected!");
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
        string jsonRequest = JsonConvert.SerializeObject(dataToBeSent);
        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonRequest);

        try
        {
            await webSocket.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, CancellationToken.None);
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
        WebSocketReceiveResult result;

        try
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log($"Response received: {responseJson}");
            ProcessResponseData(responseJson);
            // Handle the response object as needed
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket receive error: {e.Message}");
        }
    }

    public void SendData()
    {
        //Task task = SendRequest();

        //task.Start();

        RenamedResponse renamedResponse = JsonConvert.DeserializeObject<RenamedResponse>(dummydata);

    }
    [ContextMenu("TriggerDummyResponse")]
    public void TriggerDummyResponse(string dummydata)
    {
        ProcessResponseData(dummydata);
    }

    void ProcessResponseData(string jsonString)
    {
        dataToBeSent.data.PlayerTurn = true;
        var responseObject = JsonConvert.DeserializeObject<RenamedResponse2>(jsonString);
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
            
                waitingScreen.close();

                if (player.PlayerTurn && dataToBeSent.data.PlayerTurn)
                {
                    dataToBeSent.data.PlayerTurn = false;
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
                        if (player.Playerpiece[0].MovementBlockIndex > 0)
                            gameScript.redPlayerI_UI(1);
                        else if (player.Playerpiece[1].MovementBlockIndex > 0)
                            gameScript.redPlayerII_UI(1);
                        else if (player.Playerpiece[2].MovementBlockIndex > 0)
                            gameScript.redPlayerIII_UI(1);
                        else if (player.Playerpiece[3].MovementBlockIndex > 0)
                            gameScript.redPlayerIV_UI(1);
                    }
                }
                else if (player.PlayerTeam == PlayerTeam.B.ToString())
                {
                    if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                    {
                        if (player.Playerpiece[0].MovementBlockIndex > 0)
                            gameScript.bluePlayerI_UI(1);
                        else if (player.Playerpiece[1].MovementBlockIndex > 0)
                            gameScript.bluePlayerII_UI(1);
                        else if (player.Playerpiece[2].MovementBlockIndex > 0)
                            gameScript.bluePlayerIII_UI(1);
                        else if (player.Playerpiece[3].MovementBlockIndex > 0)
                            gameScript.bluePlayerIV_UI(1);
                    }
                }
                else if (player.PlayerTeam == PlayerTeam.G.ToString())
                {
                    if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                    {
                        if (player.Playerpiece[0].MovementBlockIndex > 0)
                            gameScript.greenPlayerI_UI(1);
                        else if (player.Playerpiece[1].MovementBlockIndex > 0)
                            gameScript.greenPlayerII_UI(1);
                        else if (player.Playerpiece[2].MovementBlockIndex > 0)
                            gameScript.greenPlayerIII_UI(1);
                        else if (player.Playerpiece[3].MovementBlockIndex > 0)
                            gameScript.greenPlayerIV_UI(1);
                    }
                }
                else if (player.PlayerTeam == PlayerTeam.Y.ToString())
                {
                    if (player.Playerpiece.Count >= 4 && player.DiceNumber >= 1)
                    {
                        if (player.Playerpiece[0].MovementBlockIndex > 0)
                            gameScript.yellowPlayerI_UI(1);
                        else if (player.Playerpiece[1].MovementBlockIndex > 0)
                            gameScript.yellowPlayerII_UI(1);
                        else if (player.Playerpiece[2].MovementBlockIndex > 0)
                            gameScript.yellowPlayerIII_UI(1);
                        else if (player.Playerpiece[3].MovementBlockIndex > 0)
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

    IEnumerator SetupTurns()
    {
        Debug.Log("SetupTurns");
        yield return new WaitForSeconds(1f);
        int i = 0;
        while (i < 4)
        {
            gameScript.DiceRoll(0);
            i++;
        }
    }

    IEnumerator checkForPlayerTurnChange()
    {
        var waitSec = new WaitForSeconds(0.25f);
        while (true)
        {
            if (diceRolled && lastPlayerTurn != gameScript.PlayerTurn)
            {
                lastPlayerTurn = gameScript.PlayerTurn;
                diceRolled = false;
            }

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
        {
            StartOtherPlayerTimerCoroutine = StartCoroutine(StartOtherPlayerTimer());
        }
    }

    public void ResetDataToBeSent()
    {
        foreach (var piece in dataToBeSent.data.Playerpiece)
        {
            piece.MovementBlockIndex = -1;
        }
    }

    Coroutine StartOurPlayerTimerCoroutine;
    IEnumerator StartOurPlayerTimer()
    {
        var waitsec = new WaitForSeconds(1f);
        int countSec = 15;
        while (countSec > 0)
        {
            countSec--;
            Debug.Log("Our turn :" + countSec);
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
        float countSec = 30f;
        while (countSec > 0)
        {
            redTimerImg.fillAmount = 0;
            blueTimerImg.fillAmount = 0;
            greenTimerImg.fillAmount = 0;
            yellowTimerImg.fillAmount = 0;

            float fill = countSec / 30f;
            Debug.Log(countSec + " : " + fill);
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
            Debug.Log("others turn :" + countSec);
            yield return waitsec;
        }
        Debug.Log("other Player turn over");
        StopOtherPlayerTimer();
    }
    void StopOtherPlayerTimer()
    {
        Debug.Log("sTOP : " + gameScript.PlayerTurn);
        StopCoroutine(StartOtherPlayerTimerCoroutine);
        StartOtherPlayerTimerCoroutine = null;

        if (gameScript.selectDiceNumAnimation > 0)
        {
            Debug.Log("Blue player turn :" + gameScript.selectDiceNumAnimation);
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
        webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        webSocket?.Dispose();
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


    [Serializable]
    public class PlayerPiece2
    {
        public bool IsOpen;
        public bool ReachedWinPos;
        public int MovementBlockIndex;
    }

    [Serializable]
    public class RenamedResponse2
    {
        public RenamedMeta2 meta;
        public RenamedData2 data;
    }

    [Serializable]
    public class RenamedMeta2
    {
        public string msg;
        public bool status;
    }

    [Serializable]
    public class RenamedData2
    {
        public int remainSeconds;
        public List<RenamedOtherPlayer2> OtherPlayer;
    }

    [Serializable]
    public class RenamedOtherPlayer2
    {
        public string PlayerTeam;
        public int DiceNumber;
        public bool PlayerTurn { get; set; }
        public List<PlayerPiece2> Playerpiece;
    }


    [Serializable]
    public class OurPlayerDataSet2
    {
        public string GameLobbyID { get; set; }
        public string PlayerID { get; set; }
        public string PlayerTeam { get; set; }
        public int DiceNumber { get; set; }
        public bool PlayerTurn { get; set; }
        public List<PlayerPiece2> Playerpiece { get; set; }
    }
    [Serializable]
    public class OurPlayerDataSetRoot2
    {
        public RenamedMeta2 meta { get; set; }
        public OurPlayerDataSet2 data { get; set; }
    }
}

