using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TournamentListItem : MonoBehaviour
{
    TournamentItem_JStruct tournamentItem;
    Action<TournamentItem_JStruct> joinBtAction;
    [SerializeField] TextMeshProUGUI playersCount;
    [SerializeField] TextMeshProUGUI winingAmount;
    [SerializeField] TextMeshProUGUI entryFee;
    [SerializeField] Button joinBt;

    public void Initialize(TournamentItem_JStruct tournamentItem_JStruct, Action<TournamentItem_JStruct> joinBtAction)
    {
        tournamentItem  = tournamentItem_JStruct;
        this.joinBtAction = joinBtAction;

        playersCount.text = tournamentItem.currentParticipants + "/"+tournamentItem.maxParticipants;
        winingAmount.text = tournamentItem.winningAmount.ToString();
        entryFee.text = tournamentItem.entryFee.ToString();

        joinBt.onClick.AddListener(JoinBtAction);
    }
    void JoinBtAction()
    {
        joinBtAction?.Invoke(tournamentItem);
    }
}
