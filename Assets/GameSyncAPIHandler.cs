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

public class GameSyncAPIHandler : MonoBehaviour
{
    [SerializeField] OnlineGameType DataKeeper;
    [SerializeField] string dummydata = "";
    [SerializeField] GameScriptOnline gameScript;
    [SerializeField] WaitingScreen waitingScreen;
    public OurPlayerDataSetRoot dataToBeSent;
    public string ourPlayerTeam = "RED"; // R B G Y


    private ClientWebSocket webSocket;
    private Uri serverUri = new Uri("wss://3sqlfz6r-8080.inc1.devtunnels.ms/"); // Replace with your server address

    private void Awake()
    {
        dataToBeSent = new OurPlayerDataSetRoot();
        dataToBeSent.meta = new RenamedMeta();
        dataToBeSent.data = new OurPlayerDataSet();
        dataToBeSent.data.Playerpiece = new List<PlayerPiece> { new PlayerPiece(), new PlayerPiece(), new PlayerPiece(), new PlayerPiece() };
    }


    private async void Start()
    {
        DataKeeper = FindAnyObjectByType<OnlineGameType>();
        bool playerTeamFound = false;
        if (DataKeeper.gameType==GameType.TOURNAMENT && DataKeeper.joinTurnamentJoinData.meta != null)
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
        else if (DataKeeper.gameType == GameType.GLOBAL && DataKeeper.globalGameRootData.meta != null)
        {
            foreach (var player in DataKeeper.globalGameRootData.data.PlayersInGame)
            {
                if (!playerTeamFound && player.PlayerID == APIHandler.instance.key_playerId)
                {
                    ourPlayerTeam = player.PlayerTeam;
                    playerTeamFound = true;
                    break;
                }
            }
        }

        gameScript.SetOurPlayerPieceButton(ourPlayerTeam);

        webSocket = new ClientWebSocket();
        await Connect();
        await SendRequest();
        await ReceiveResponse();
   

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
        dataToBeSent.data.PlayerTurn = true;
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
    public void TriggerDummyResponse()
    {
        ProcessResponseData(dummydata);
    }

    void ProcessResponseData(string jsonString)
    {
        var responseObject = JsonConvert.DeserializeObject<RenamedResponse>(jsonString);
        Debug.Log("sec :" + responseObject.data.remainSeconds);
        foreach (var player in responseObject.data.OtherPlayer)
        {
            if (waitingScreen.isOpen)
            {
                if (responseObject.data.remainSeconds > 0)
                {
                    waitingScreen.SetTime(responseObject.data.remainSeconds);
                    if (waitingScreen.InitializeCount < 4)
                        waitingScreen.AddPlayer(player.PlayerTeam);
                }
                else
                    waitingScreen.close();
            }

            if (player.PlayerTurn && dataToBeSent.data.PlayerTurn)
            {
                dataToBeSent.data.PlayerTurn = false;
                // DiceRollButton.enabled = dataToBeSent.data.PlayerTurn;
            }
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
                if (player.Playerpiece.Count >= 4 && player.DiceNumber>=1)
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
                gameScript.DiceRoll(player.DiceNumber);
                return;
            }
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
}

[Serializable]
public class PlayerPiece
{
    public bool IsOpen;
    public bool ReachedWinPos;
    public int MovementBlockIndex;
}

[Serializable]
public class RenamedResponse
{
    public RenamedMeta meta;
    public RenamedData data;
}

[Serializable]
public class RenamedMeta
{
    public string msg;
    public bool status;
}

[Serializable]
public class RenamedData
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
    public string GameLobbyID { get; set; }
    public string PlayerID { get; set; }
    public string PlayerTeam { get; set; }
    public int DiceNumber { get; set; }
    public bool PlayerTurn { get; set; }
    public List<PlayerPiece> Playerpiece { get; set; }
}
[Serializable]
public class OurPlayerDataSetRoot
{
    public RenamedMeta meta { get; set; }
    public OurPlayerDataSet data { get; set; }
}
public enum PlayerTeam
{
    R,
    B,
    G,
    Y

}