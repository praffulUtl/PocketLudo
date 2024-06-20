using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class APIHandler : MonoBehaviour
{
    [SerializeField] string urlPath = "";
    string PostUserAuthEndPoint = "";
    string PostUserDataEndPoint = "";
    string PostPlayerPieceMoveEndPoint = "";
    string GetTournamentEndPoint = "";
    string PostTournamentJoinEndPoint = "";

    #region User auth/data
    public void PostUserAuthData(UserAuth_JStruct data)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(PostUserAuthEndPoint, jsonString,PostUserAuthDataCallback));
    }
    public void PostUserData(UpUserData_JStruct data)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(PostUserDataEndPoint, jsonString, PostUserAuthDataCallback));
    }
    private void PostUserAuthDataCallback(string dataString)
    {
        UserData_JStruct userData_JStruct = JsonUtility.FromJson<UserData_JStruct>(dataString);
    }
    #endregion

    #region players pieces movement
    public void PostPlayerPieceData(PlayerPieceMoveData_JStruct data)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(PostPlayerPieceMoveEndPoint, jsonString, PostPlayerPieceDataCallback));
    }
    private void PostPlayerPieceDataCallback(string dataString)
    {
        OtherPlayersMoveData_JStruct otherPlayersMoveData_JStruct = JsonUtility.FromJson<OtherPlayersMoveData_JStruct>(dataString);
    }
    #endregion

    #region tournament
    public void GetTournamentsData()
    {
        StartCoroutine(GetRequest(GetTournamentEndPoint, GetTournamentsDataCallback));
    }
    private void GetTournamentsDataCallback(string dataString)
    {
        TournamentsData_JStruct tournamentsData_JStruct = JsonUtility.FromJson<TournamentsData_JStruct>(dataString);
    }
    public void PostTournamentJoinData(TournamentJoinData_JStruct data)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(PostTournamentJoinEndPoint, jsonString, PostTournamentJoinCallback));
    }
    private void PostTournamentJoinCallback(string dataString)
    {
        TournamentJoinData_JStruct tournamentJoinData_JStruct = JsonUtility.FromJson<TournamentJoinData_JStruct>(dataString);
    }
    #endregion

    #region API Client
    IEnumerator StartPostRequest(string urlEndPoint, string jsonString, Action<string> callBack)
    {
        string url = urlPath + urlEndPoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, jsonString, "application/json"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                callBack?.Invoke(resData);
            }
        }
    }
    IEnumerator GetRequest(string urlEndPoint, Action<string> callBack)
    {
        string url = urlPath + urlEndPoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                callBack?.Invoke(resData);
            }
        }
    }
    #endregion
}

#region API Json Struct classes

//User auth & data
public class UserAuth_JStruct
{
    public string VerificationID { get; set; }
    public string PhoneNumber { get; set; }
}
public class UpUserData_JStruct
{
    public string PlayerID { get; set; }
    public string PlayerName { get; set; }
    public string PlayerImage { get; set; }
}
public class UserData_JStruct
{
    public string PlayerID { get; set; }
    public string PlayerName { get; set; }
    public string PlayerImageUrl { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
}

//Tournament
public class TournamentsData_JStruct
{
    public List<TournamentItem_JStruct> TournamentsList { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
}
public class TournamentItem_JStruct
{
    public string TournamentID { get; set; }
    public string WinningAmount { get; set; }
    public string EntryFee { get; set; }
    public string PlayersCount { get; set; }
}
public class TournamentJoinData_JStruct
{
    public string TournamentID { get; set; }
    public string PlayerID { get; set; }
    public string PaymentConfirmationID { get; set; }
}

public class GlobalGameJoinData_JStruct
{
    public string PlayerID { get; set; }
    public string GameMode { get; set; }
}

public class GameLobbyData_JStruct
{
    public string GameLobbyId { get; set; }
    public List<PlayerItem_JStruct> PlayersInGame { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
}
public class PlayerItem_JStruct
{
    public string PlayerID { get; set; }
    public string PlayerName { get; set; }
    public string PlayerImageUrl { get; set; }
    public string PlayerTeam { get; set; }
}

public class Playerpiece_JStruct
{
    public string IsOpen { get; set; }
    public string ReachedWinPos { get; set; }
    public string XPosOnBoard { get; set; }
    public string YPosOnBoard { get; set; }
}
public class PlayerPieceMoveData_JStruct
{
    public string GameLobbyID { get; set; }
    public string PlayerID { get; set; }
    public string PlayerTeam { get; set; }
    public List<Playerpiece_JStruct> Playerpiece { get; set; }
}
public class OtherPlayer_JStruct
{
    public string PlayerTeam { get; set; }
    public List<Playerpiece_JStruct> Playerpiece { get; set; }
}
public class OtherPlayersMoveData_JStruct
{
    public List<OtherPlayer_JStruct> OtherPlayers { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
}
#endregion