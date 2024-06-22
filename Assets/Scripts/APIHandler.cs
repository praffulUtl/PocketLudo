using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;

public class APIHandler : MonoBehaviour
{
    [SerializeField] DialogBox dialogBox;

    string baseUrl = "https://fqb49r8k-5211.inc1.devtunnels.ms/";
    string endPoint_PostUserEmailReg = "player/register";
    string endPoint_PostUserEmailLogin = "player/login";
    string endPoint_VerifyRegUser = "player/register-verify/otp";
    string endPoint_VerifyLoginUser = "player/login-verify/otp";
    string endPoint_postUserAuth = "";
    string endPoint_postUserData = "";
    string endPoint_postPlayerPieceMove = "";
    string endPoint_getTournaments = "";
    string endPoint_postTournamentJoin = "";
    string endPoint_postJoinGlobalGame = "";
    public static APIHandler instance { get; private set; }

    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    #region User auth/data
    public void PostUserMailPhoneReg(UserMailAndPhone_JStruct data, Action<bool, RegLogUserAuth> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_PostUserEmailReg, jsonString, callback));
    }
    public void PostUserMailPhoneLogin(UserMailAndPhone_JStruct data, Action<bool, RegLogUserAuth> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_PostUserEmailLogin, jsonString, callback));
    }
    public void PostUserRegOTP(UserOTP_JStruct data, Action<bool, RegLogUserAuth> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_VerifyRegUser, jsonString, callback));
    }
    public void PostUserLoginOTP(UserOTP_JStruct data, Action<bool, RegLogUserAuth> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_VerifyLoginUser, jsonString, callback));
    }
    public void PostUserAuthData(UserAuth_JStruct data, Action<bool, UserData_JStruct> callback)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(endPoint_postUserAuth, jsonString,callback));
    }
    public void PostUserData(UpUserData_JStruct data, Action<bool, UserData_JStruct> callback)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(endPoint_postUserData, jsonString, callback));
    }
    #endregion

    #region Global game
    public void PostJoinGlobalGame(GlobalGameJoinData_JStruct data, Action<bool, GameLobbyData_JStruct> callback)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(endPoint_postJoinGlobalGame, jsonString, callback));
    }
    #endregion

    #region tournament
    public void GetTournamentsData(Action<bool, TournamentsData_JStruct> callback)
    {
        StartCoroutine(GetRequest(endPoint_getTournaments, callback));
    }
    public void PostTournamentJoinData(TournamentJoinData_JStruct data,Action<bool, TournamentJoinData_JStruct> callback)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(endPoint_postTournamentJoin, jsonString, callback));
    }
    #endregion

    #region players pieces movement
    public void PostPlayerPieceData(PlayerPieceMoveData_JStruct data,Action<bool, OtherPlayersMoveData_JStruct> callback)
    {
        string jsonString = JsonUtility.ToJson(data);
        StartCoroutine(StartPostRequest(endPoint_postPlayerPieceMove, jsonString, callback));
    }
    #endregion

    #region API Client
    IEnumerator StartPostRequest<T>(string urlEndPoint, string jsonString, Action<bool, T> callBack)
    {
        Debug.Log("StartPostRequest : " + jsonString);
        string url = baseUrl + urlEndPoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, jsonString, "application/json"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                try
                {
                    callBack?.Invoke(false, JsonConvert.DeserializeObject<T>(""));
                }
                catch
                {
                    dialogBox.Show(webRequest.error);
                }
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                Debug.Log("Post response data :" + resData);
                try
                {
                    callBack?.Invoke(true, JsonConvert.DeserializeObject<T>(resData));
                }
                catch
                {
                    dialogBox.Show("Error : " + resData);
                }
            }
        }
    }
    IEnumerator GetRequest<T>(string urlEndPoint, Action<bool,T> callBack)
    {
        string url = baseUrl + urlEndPoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                try
                {
                    callBack?.Invoke(false, JsonConvert.DeserializeObject<T>(""));
                }
                catch
                {
                    dialogBox.Show(webRequest.error);
                }
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                Debug.Log("Get response data :" + resData);
                try
                {
                    callBack?.Invoke(true, JsonConvert.DeserializeObject<T>(resData));
                }
                catch
                {
                    dialogBox.Show("Error : " + resData);
                }
            }
        }
    }
    #endregion
}

#region API Json Struct classes

public class Meta
{
    public string msg { get; set; }
    public bool status { get; set; }
}

public class RegLogUserAuth
{
    public Meta meta { get; set; }
    public Data data { get; set; }
}
public class Data
{
    public bool userExists { get; set; }
}

//User auth & data

public class UserMailAndPhone_JStruct
{
    public string mobile { get; set; }
    public string email { get; set; }
}
public class UserOTP_JStruct
{
    public string mobile { get; set; }
    public string email { get; set; }
    public string otp { get; set; }
}
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
    public bool Status { get; set; }
}

//Tournament
public class TournamentsData_JStruct
{
    public List<TournamentItem_JStruct> TournamentsList { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
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
    public bool Status { get; set; }
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
    public bool Status { get; set; }
}
#endregion