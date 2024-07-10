using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListItem : MonoBehaviour
{
    double time = 0;
    string id = "";
    public bool IsTimer => isTimer;
    bool isTimer = false;
    int playerCount;
    [SerializeField] TextMeshProUGUI playerCountTxt;
    [SerializeField] TextMeshProUGUI entryFeeTxt;
    [SerializeField] Button joinBt;
    Action<string, bool, int> actionOnJoin;
    [SerializeField] TextMeshProUGUI gameStartInTime;
    public void Initialize(Lobbies_JStruct lobbies_JStruct, Action<string,bool,int> actionOnJoin)
    {
        this.actionOnJoin = actionOnJoin;

        this.id = lobbies_JStruct.lobbyId;
        isTimer = lobbies_JStruct.timerMode;
        playerCount = lobbies_JStruct.playersCount;
        playerCountTxt.text = lobbies_JStruct.players.Count.ToString() + "/" + lobbies_JStruct.playersCount.ToString();
        entryFeeTxt.text = "Entry :" + lobbies_JStruct.betAmount.ToString();
        joinBt.onClick.AddListener(JoinBtAction);


        DateTime now = UnixTimestampConverter.UnixTimeStampToDateTime(lobbies_JStruct.createdAt);
        Debug.Log("lobby time : "+ now);
        DateTime end = UnixTimestampConverter.UnixTimeStampToDateTime(lobbies_JStruct.createdAt + 60000);
        double sec = (end - now).TotalSeconds;
        gameStartInTime.text = $"{sec}s";
        //StartCoroutine(processTimer(sec));
    }
    void JoinBtAction()
    {
        actionOnJoin?.Invoke(id,isTimer,playerCount);
    }

    IEnumerator processTimer(double sec)
    {
        var waitSec = new WaitForSeconds(1);
        time = sec;
        while (time > 0)
        {
            time -= 1;
            gameStartInTime.text = $"{time}s";
            yield return waitSec;
        }

            Destroy(gameObject);
    }
}
