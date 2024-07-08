using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListItem : MonoBehaviour
{
    string id = "";
    public bool IsTimer => isTimer;
    bool isTimer = false;
    int playerCount;
    [SerializeField] TextMeshProUGUI playerCountTxt;
    [SerializeField] TextMeshProUGUI entryFeeTxt;
    [SerializeField] Button joinBt;
    Action<string, bool, int> actionOnJoin;
    public void Initialize(Lobbies_JStruct lobbies_JStruct, Action<string,bool,int> actionOnJoin)
    {
        this.actionOnJoin = actionOnJoin;

        this.id = lobbies_JStruct.lobbyId;
        isTimer = lobbies_JStruct.timerMode;
        playerCount = lobbies_JStruct.playersCount;
        playerCountTxt.text = lobbies_JStruct.players.Count.ToString() + "/" + lobbies_JStruct.playersCount.ToString();
        entryFeeTxt.text = "Entry :" + lobbies_JStruct.betAmount.ToString();
        joinBt.onClick.AddListener(JoinBtAction);
    }
    void JoinBtAction()
    {
        actionOnJoin?.Invoke(id,isTimer,playerCount);
    }

}
