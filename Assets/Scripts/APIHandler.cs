using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Net;

public class APIHandler : MonoBehaviour
{
    public bool dummyMode = false;
    [SerializeField] DialogBox dialogBox;


    public string baseUrl => baseUrlPrv;
    string baseUrlPrv = "https://3sqlfz6r-5211.inc1.devtunnels.ms/ludo/v1/";
    string endPoint_PostUserEmailReg = "player/register";
    string endPoint_PostUserEmailLogin = "player/login";
    string endPoint_VerifyRegUser = "player/register-verify/otp";
    string endPoint_VerifyLoginUser = "player/login-verify/otp";
    string endPoint_postUserData = "player/profile";
    string endPoint_GetUserData = "player/profile";
    string endPoint_getTournaments = "tournament";
    string endPoint_postTournamentJoin = "tournament/join";
    string endPoint_postJoinGlobalGame = "online-game/join-lobby";
    string endPoint_lobbyList = "online-game/lobbylist";
    string endPoint_lobbyPlayers = "online-game/players/";
    string endPoint_startGame = "online-game/start-game";
    string endPoint_startGame2 = "online-game/start-game";
    string endPoint_PostLoadLeaderboard = "online-game/leader-list";
    string endPoint_lobbyExpiry = "online-game/lobby-expiration";

    string keyName_playerId = "playerId";
    string keyName_authKey = "authKey";
    string keyName_isRegistered = "isRegistered";

    public string key_playerId { get; private set; }
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

        if (PlayerPrefs.HasKey(keyName_playerId))
            this.key_playerId = PlayerPrefs.GetString(keyName_playerId);
    }
    public void SetAuthKey(string key)
    {
        this.key_authKey = key;
        PlayerPrefs.SetString(keyName_authKey, this.key_authKey);        
    }
    public void SetPlayerID(string playerId)
    {
        this.key_playerId = playerId;
        PlayerPrefs.SetString(keyName_playerId, this.key_playerId);
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
    public void GetUserData(Action<bool, PlayerDataRoot_JStruct> callback)
    {
        Debug.Log("GetUserData");
        StartCoroutine(GetRequest(endPoint_GetUserData, callback));
    }
    public void PutUserData(UpdateProfile_JStruct data, Action<bool, StartGame_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPutRequest(endPoint_postUserData, jsonString, callback));
    }
    #endregion

    #region Global game
    public void PostJoinGlobalGame(GlobalGameJoinData_JStruct data, Action<bool, OnlineGameJoinDataRoot_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_postJoinGlobalGame, jsonString, callback));
    }
    public void GetLobbyList(Action<bool, LobbiesData_JStruct> callback) 
    {
        StartCoroutine(GetRequest(endPoint_lobbyList,callback));
    }

    public void GetLobbyPlayers(int lobbyId, Action<bool, LobbyPlayers> callback)
    {
        StartCoroutine(GetRequest(endPoint_lobbyPlayers+lobbyId, callback));
    }

    public void PostStartGlobalGame(TriggerStartGame_JStruct data, Action<bool, StartGame_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPostRequest(endPoint_startGame2, jsonString, callback));
    }

    public void PutLobbyExpiry(LobbyExpiration_JStruct data, Action<bool, StartGame_JStruct> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        StartCoroutine(StartPutRequest(endPoint_lobbyExpiry, jsonString, callback));
    }

    #endregion

    #region tournament
    public void GetTournamentsData(Action<bool, TournamentsData_JStruct> callback)
    {
        StartCoroutine(GetRequest(endPoint_getTournaments, callback));
    }
    public void PostTournamentJoinData(TournamentJoinData_JStruct data,Action<bool, Meta> callback)
    {
        string jsonString = JsonConvert.SerializeObject(data);
        Debug.Log("StartPostRequest : " + jsonString);
        StartCoroutine(StartPostRequest(endPoint_postTournamentJoin, jsonString, callback));
    }
    #endregion

    #region Leaderboard
    public void GetLoadLeaderboard(Action<bool, LeaderboardDataRoot_JStruct> callback)
    {
        StartCoroutine(GetRequest(endPoint_PostLoadLeaderboard, callback));
    }
    #endregion

    #region texture fetching
    public void DownloadTexture(string url, Action<bool,Texture> callback)
    {
        StartCoroutine(GetTexture(url,callback));
    }
    #endregion



    #region API Client

    IEnumerator StartPutRequest<T>(string urlEndPoint, string jsonString, Action<bool, T> callBack)
    {
        Debug.Log("StartPutRequest : " + jsonString);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonString);
        string url = baseUrlPrv + urlEndPoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, data))
        {
            if (key_authKey != null && key_authKey.Trim() != "")
                webRequest.SetRequestHeader(keyName_authKey, key_authKey);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Unable to hit api : " + url + " " + webRequest.GetRequestHeader(keyName_authKey));
                try
                {
                    dialogBox.Show(webRequest.error);
                    callBack?.Invoke(false, JsonConvert.DeserializeObject<T>(""));
                }
                catch
                {

                }
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                Debug.Log("Post response data :" + url + "\n" + resData);
                try
                {
                    callBack?.Invoke(true, JsonConvert.DeserializeObject<T>(resData));
                Debug.Log("Upload complete!");
                }
                catch
                {
                    dialogBox.Show("Error : " + resData);
                }
            }
        }
    }

    IEnumerator StartPostRequest<T>(string urlEndPoint, string jsonString, Action<bool, T> callBack)
    {
        Debug.Log("StartPostRequest : " + jsonString);
        string url = baseUrlPrv + urlEndPoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, jsonString, "application/json"))
        {
            if (key_authKey != null && key_authKey.Trim() != "")
                webRequest.SetRequestHeader(keyName_authKey, key_authKey);
            //webRequest.SetRequestHeader("authKey", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfaWQiOiI2NjdmOWY0OWQxNzUzZjY0ZTIwZTM0ZTIiLCJlbWFpbCI6InByYWZmdWwuYmhhcnRpM0BnbWFpbC5jb20iLCJpYXQiOjE3MjA1Mzk3ODUsImV4cCI6MTc1MjA5NzM4NX0.FLyQmoFd1G9JVlLOX3mnAJraaLiV6FPNZl2scnfy08I");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Unable to hit api : "+url+" "+jsonString);
                try
                {
                    dialogBox.Show(webRequest.error);
                    callBack?.Invoke(false, JsonConvert.DeserializeObject<T>(""));
                }
                catch
                {

                }
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                Debug.Log("Post response data :" + url + "\n" + resData);
                try
                {
                    callBack?.Invoke(true, JsonConvert.DeserializeObject<T>(resData));
                    Debug.Log("Post response data deserialize success");
                }
                catch
                {
                    dialogBox.Show("Error : " + resData);
                }
            }
        }
    }

    IEnumerator Upload(string endpoint,string data)
    {
        var url = baseUrlPrv+ endpoint;
        using (UnityWebRequest www = UnityWebRequest.Post(url,data, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
    IEnumerator GetRequest<T>(string urlEndPoint, Action<bool, T> callBack)
    {
        string url = baseUrlPrv + urlEndPoint;
        Debug.Log("get req: " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            if (key_authKey != null && key_authKey.Trim() != "")
                webRequest.SetRequestHeader(keyName_authKey, key_authKey);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
                try
                {
                    dialogBox.Show(webRequest.error.ToString());
                    callBack?.Invoke(false, JsonConvert.DeserializeObject<T>(""));
                }
                catch
                {
                }
            }
            else
            {
                string resData = webRequest.downloadHandler.text;
                Debug.Log("Get response data :" + url + "\n" + resData);
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

    IEnumerator GetTexture(String url,Action<bool, Texture> callback)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            callback?.Invoke(false,null);
            //dialogBox.Show(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            callback?.Invoke(true, myTexture);
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
    public string playerId { get; set; }
    public string token { get; set; }
}

public class PlayerDetails_JStruct
{
    public string _id { get; set; }
    public string playerId { get; set; }
    public string mobile { get; set; }
    public string email { get; set; }
    public string status { get; set; }
    public string roleType { get; set; }
    public int topUpBalance { get; set; }
    public double winningBalance { get; set; }
    public int looseBalance { get; set; }
    public long createdAt { get; set; }
    public long updatedAt { get; set; }
    public int __v { get; set; }
    public string playerImageUrl { get; set; }
    public string playerName { get; set; }
    public int score { get; set; }
    //public string playerName { get; set; }
    //public string playerImageUrl { get; set; }
}

public class PlayerDataRoot_JStruct
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
[Serializable] 
public class TournamentJoinData_JStruct
{
    public string tournamentId { get; set; }
    public string playerId { get; set; }
    //public string paymentConfirmationID { get; set; }
}

public class GlobalGameJoinData_JStruct
{
    public string PlayerID { get; set; }
    public bool TimerMode { get; set; }
    public int PlayerCount { get; set; }
    public int LobbyId { get; set; }
}

[Serializable]
public class OnlineGameJoinDataRoot_JStruct
{
    public Meta meta { get; set; }
    public OnlineGameJoinData_JStruct data { get; set; }
}
[Serializable]
public class OnlineGameJoinData_JStruct
{
    public int lobbyId;
}

[Serializable]
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

[Serializable]
public class JoinedTournamentDataRoot_JStruct
{
    public Meta meta { get; set; }
    public JoinedTournamentData_JStruct data { get; set; }
}
[Serializable]
public class JoinedTournamentData_JStruct
{
    public string TournamentID { get; set; }
    public string PlayerID { get; set; }
    public string PaymentConfirmationID { get; set; }
    public string GameLobbyId { get; set; }
    public List<PlayersInGame_JStruct> PlayersInGame { get; set; }
}
[Serializable]
public class PlayersInGame_JStruct
{
    public string PlayerID { get; set; }
    public string PlayerName { get; set; }
    public string PlayerImageUrl { get; set; }
    public string PlayerTeam { get; set; }
}

//Leaderboard
public class LoadLeaderboardData_JStruct
{
    public string playerId { get; set; }
    public int pageNo { get; set; }
}
public class LeaderboardData_JStruct
{
    public LeaderboardPlayer_JStruct user { get; set; }
    public int pageNo { get; set; }
    public List<LeaderboardPlayer_JStruct> players { get; set; }
}


public class LeaderboardPlayer_JStruct
{
    public string playerName { get; set; }
    public string playerImageUrl { get; set; }
    public int score { get; set; }
    public string _id { get; set; }
}

public class LeaderboardDataRoot_JStruct
{
    public Meta meta { get; set; }
    public List<LeaderboardPlayer_JStruct> data { get; set; }
}

public class Lobbies_JStruct
{
    public string _id { get; set; }
    public int playersCount { get; set; }
    public int lobbyId { get; set; }
    public int betAmount { get; set; }
    public bool timerMode { get; set; }
    public string gameType { get; set; }
    public List<LobbyListItemPlayer_JStruct> players { get; set; }
    public object existPlayerCount { get; set; }
    public object startTime { get; set; }
    public object endTime { get; set; }
    public bool lobbyIdExpiration { get; set; }
    public bool isLobbyFull { get; set; }
    public long createdAt { get; set; }
    public long updatedAt { get; set; }
    public int __v { get; set; }
}

public class LobbyListItemPlayer_JStruct
{
    public string playerId { get; set; }
    public string playerTeam { get; set; }
    public int winningAmount { get; set; }
    public string winningStatus { get; set; }
    public object winningNumber { get; set; }
    public object winDate { get; set; }
    public object looseDate { get; set; }
    public string _id { get; set; }
}

public class LobbiesData_JStruct
{
    public Meta meta { get; set; }
    public List<Lobbies_JStruct> data { get; set; }
}
public class LobbyExpiration_JStruct
{
    public int lobbyId { get; set; }
    public bool lobbyIdExpiration { get; set; }
}
public class LobbyPlayers
{
    public Meta meta { get; set; }
    public LobbyPlayers_JStruct data { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class LobbyPlayers_JStruct
{
    public string _id { get; set; }
    public int lobbyId { get; set; }
    public long createdAt { get; set; }
    public long updatedAt { get; set; }
    public int betAmount { get; set; }
    public bool timerMode { get; set; }
    public string gameType { get; set; }
    public bool lobbyIdExpiration { get; set; }
    public int RemainSeconds { get; set; }
    public int PlayerTurnSeconds { get; set; }
    public List<LobbyPlayer_JStruct> players { get; set; }
}


public class LobbyPlayer_JStruct
{
    public string playerTeam { get; set; }
    public string PlayerId { get; set; }
}

public class TriggerStartGame_JStruct
{
    public int lobbyId { get; set; }
    public bool startGame { get; set; }
}

public class StartGame_JStruct
{ 
    public Meta meta { get; set; }
}
public class UpdateProfile_JStruct
{
    public string playerName { get; set; }
    public string playerImageUrl { get; set; }
}
#endregion