using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TournamentCategories : MonoBehaviour
{
    Action<string, string,TournamnetItemType> onClickAction;
    string winingAmountStr;
    string entryFeeStr;
    [SerializeField] TextMeshProUGUI winingAmount;
    [SerializeField] TextMeshProUGUI entryFee;
    [SerializeField] Image background;
    [SerializeField] Sprite backgroundSprite;
    [SerializeField] TournamnetItemType tournamnetItemType = TournamnetItemType.TYPE_1;
    
    public void Initialize(string winingAmountStr,string entryFeeStr,TournamnetItemType tournamnetItemType,Action<string, string, TournamnetItemType> onCLickAction)
    {
        this.winingAmountStr = winingAmountStr;
        this.entryFeeStr = entryFeeStr;
        winingAmount.text = this.winingAmountStr;
        entryFee.text = this.entryFeeStr;
        this.tournamnetItemType = tournamnetItemType;
        this.onClickAction = onCLickAction;
    }
    void OpenCat()
    {
        onClickAction?.Invoke(winingAmountStr, entryFeeStr,tournamnetItemType);
    }

}
public enum TournamnetItemType
{ 
    TYPE_1, 
    TYPE_2, 
    TYPE_3
}

