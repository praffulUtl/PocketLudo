using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCategories : MonoBehaviour
{
    string winingAmountStr;
    string entryFeeStr;
    [SerializeField] TextMeshProUGUI winingAmount;
    [SerializeField] TextMeshProUGUI entryFee;
    
    public void Initialize(string winingAmountStr,string entryFeeStr)
    {
        this.winingAmountStr = winingAmountStr;
        this.entryFeeStr = entryFeeStr;
        winingAmount.text = this.winingAmountStr;
        entryFee.text = this.entryFeeStr;
    }
}

