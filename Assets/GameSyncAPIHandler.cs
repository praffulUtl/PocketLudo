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

public class WebSocketClient : MonoBehaviour
{
    [SerializeField] GameScript gameScript;

    public OurPlayerDataSetRoot dataToBeSent;
    public string ourPlayerTeam = "RED"; // R B G Y

    public Button DiceRollButton;
    public GameObject dice1_Roll_Animation;
    public GameObject dice2_Roll_Animation;
    public GameObject dice3_Roll_Animation;
    public GameObject dice4_Roll_Animation;
    public GameObject dice5_Roll_Animation;
    public GameObject dice6_Roll_Animation;
    int selectDiceNumAnimation = -1;

    private ClientWebSocket webSocket;
    private Uri serverUri = new Uri("ws://yourserveraddress"); // Replace with your server address
    


    private async void Start()
    {
        webSocket = new ClientWebSocket();
        await Connect();
        await SendRequest();
        await ReceiveResponse();

        dataToBeSent = new OurPlayerDataSetRoot();
        dataToBeSent.meta = new RenamedMeta();
        dataToBeSent.data = new OurPlayerDataSet();
        dataToBeSent.data.Playerpiece = new List<PlayerPiece> { new PlayerPiece(), new PlayerPiece(),new PlayerPiece(), new PlayerPiece() };

        gameScript.SetOurPlayerPieceButton(ourPlayerTeam);

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
        var buffer = new byte[1024];
        WebSocketReceiveResult result;

        try
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log($"Response received: {responseJson}");

            var responseObject = JsonConvert.DeserializeObject<RenamedResponse>(responseJson);
            foreach (var player in responseObject.data.OtherPlayer)
            {
                if (player.PlayerTurn && dataToBeSent.data.PlayerTurn)
                    dataToBeSent.data.PlayerTurn = false;
                if (player.DiceNumber > 0)
                {
                    DiceRoll(player.DiceNumber);
                    return;
                }
            }
            // Handle the response object as needed
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket receive error: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        webSocket?.Dispose();
    }

    #region gameplay
    public void DiceRoll(int i = -1)
    {
        SoundManagerScript.diceAudioSource.Play();
        DiceRollButton.interactable = false;

        if (i == -1)
        {
            selectDiceNumAnimation = UnityEngine.Random.Range(1, 7);
            Task task = SendRequest();
            task.Start();
        }
        else
            selectDiceNumAnimation = i;

        switch (selectDiceNumAnimation)
        {
            case 1:
                dice1_Roll_Animation.SetActive(true);
                dice2_Roll_Animation.SetActive(false);
                dice3_Roll_Animation.SetActive(false);
                dice4_Roll_Animation.SetActive(false);
                dice5_Roll_Animation.SetActive(false);
                dice6_Roll_Animation.SetActive(false);
                break;

            case 2:
                dice1_Roll_Animation.SetActive(false);
                dice2_Roll_Animation.SetActive(true);
                dice3_Roll_Animation.SetActive(false);
                dice4_Roll_Animation.SetActive(false);
                dice5_Roll_Animation.SetActive(false);
                dice6_Roll_Animation.SetActive(false);
                break;

            case 3:
                dice1_Roll_Animation.SetActive(false);
                dice2_Roll_Animation.SetActive(false);
                dice3_Roll_Animation.SetActive(true);
                dice4_Roll_Animation.SetActive(false);
                dice5_Roll_Animation.SetActive(false);
                dice6_Roll_Animation.SetActive(false);
                break;

            case 4:
                dice1_Roll_Animation.SetActive(false);
                dice2_Roll_Animation.SetActive(false);
                dice3_Roll_Animation.SetActive(false);
                dice4_Roll_Animation.SetActive(true);
                dice5_Roll_Animation.SetActive(false);
                dice6_Roll_Animation.SetActive(false);
                break;

            case 5:
                dice1_Roll_Animation.SetActive(false);
                dice2_Roll_Animation.SetActive(false);
                dice3_Roll_Animation.SetActive(false);
                dice4_Roll_Animation.SetActive(false);
                dice5_Roll_Animation.SetActive(true);
                dice6_Roll_Animation.SetActive(false);
                break;

            case 6:
                dice1_Roll_Animation.SetActive(false);
                dice2_Roll_Animation.SetActive(false);
                dice3_Roll_Animation.SetActive(false);
                dice4_Roll_Animation.SetActive(false);
                dice5_Roll_Animation.SetActive(false);
                dice6_Roll_Animation.SetActive(true);
                break;
        }

        gameScript.CallPlayersNotInitialized();
    }

    public void SetPlayerTurn(string turn)
    {
        DiceRollButton.enabled = (turn == ourPlayerTeam);
    }
    #endregion

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



public class OurPlayerDataSet
{
    public string GameLobbyID { get; set; }
    public string PlayerID { get; set; }
    public string PlayerTeam { get; set; }
    public int DiceNumber { get; set; }
    public bool PlayerTurn { get; set; }
    public List<PlayerPiece> Playerpiece { get; set; }
}

public class OurPlayerDataSetRoot
{
    public RenamedMeta meta { get; set; }
    public OurPlayerDataSet data { get; set; }
}