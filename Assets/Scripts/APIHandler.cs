using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;

public class APIHandler : MonoBehaviour
{
    public bool dummyMode = false;
    [SerializeField] DialogBox dialogBox;

    string baseUrl = "https://3sqlfz6r-5211.inc1.devtunnels.ms/v1/";
    string endPoint_PostUserEmailReg = "player/register";
    string endPoint_PostUserEmailLogin = "player/login";
    string endPoint_VerifyRegUser = "player/register-verify/otp";
    string endPoint_VerifyLoginUser = "player/login-verify/otp";
    string endPoint_postUserAuth = "";
    string endPoint_postUserData = "player/profile";
    string endPoint_GetUserData = "player/profile";
    string endPoint_postPlayerPieceMove = "";
    string endPoint_getTournaments = "tournament";
    string endPoint_postTournamentJoin = "";
    string endPoint_postJoinGlobalGame = "";
    string keyName_authKey = "authKey";
    string keyName_isRegistered = "isRegistered";
    public string key_authKey { get; private set; }
    public bool key_isRegistered { get; private set; }
    public static APIHandler instance { get; private set; }

    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        if (PlayerPrefs.HasKey(keyName_isRegistered))
            this.key_isRegistered = (PlayerPrefs.GetInt(keyName_isRegistered) == 1);

        if (PlayerPrefs.HasKey(keyName_authKey))
            this.key_authKey = PlayerPrefs.GetString(keyName_authKey);
    }
    public void SetAuthKey(string key)
    {
        this.key_authKey = key;
        PlayerPrefs.SetString(keyName_authKey, this.key_authKey);        
    }
    public void SetUseRegistered(bool reg)
    {
        this.key_isRegistered = reg;
        PlayerPrefs.SetInt(keyName_isRegistered, reg ? 1 : 0);
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
    public void PostUserRegOTP(UserOTP_JStruct data, Action<bool, VerifyOTPRes_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_VerifyRegUser, jsonString, callback));
    }
    public void PostUserLoginOTP(UserOTP_JStruct data, Action<bool, VerifyOTPRes_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_VerifyLoginUser, jsonString, callback));
    }
    public void GetUserData(UserOTP_JStruct data, Action<bool, VerifyOTPRes_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_VerifyLoginUser, jsonString, callback));
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
                Debug.Log("Unable to hit api : "+url);
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
                Debug.Log("Post response data :" + url + "\n" + resData);
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
            if (keyName_authKey.Trim() != "" && key_authKey.Trim() != "")
                webRequest.SetRequestHeader(keyName_authKey, key_authKey);
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
                Debug.Log("Post response data :" + url + "\n" + resData);
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

public class VerifyOTPRes_JStruct
{
    public Meta meta { get; set; }
    public string token { get; set; }
}

public class PlayerDetails_JStruct
{
    public string playerName { get; set; }
    public string playerImageUrl { get; set; }
    public string _id { get; set; }
}

public class PlayerData_JStruct
{
    public Meta meta { get; set; }
    public PlayerDetails_JStruct data { get; set; }
}

//Tournament
public class TournamentsData_JStruct
{
    public Meta meta { get; set; }
    public List<TournamentItem_JStruct> Data { get; set; }
}
public class TournamentItem_JStruct
{
    public string _id { get; set; }
    public string name { get; set; }
    public int winningAmount { get; set; }
    public int entryFee { get; set; }
    public int currentParticipants { get; set; }
    public int maxParticipants { get; set; }
    public string status { get; set; }
    public string type { get; set; }
    public long createdAt { get; set; }
    public long updatedAt { get; set; }
    public int __v { get; set; }
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