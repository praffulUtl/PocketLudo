using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableItems : MonoBehaviour
{
    [SerializeField] private GameObject baseLayer;
    [SerializeField] private GameObject friendOverlay;
    [SerializeField] private GameObject onlineOverlay;
    [SerializeField] private GameObject onComputer;
    [SerializeField] private GameObject onPassNPlay;
    [SerializeField] private GameObject onProfile;
    [SerializeField] private GameObject onTournment;
    public void onClickFriend()
    {
        baseLayer.SetActive(false);
        friendOverlay.SetActive(true);
    }
    public void onClickOnline()
    {
        baseLayer.SetActive(false);
        onlineOverlay.SetActive(true);
    }
    public void onClickComputer()
    {
        baseLayer.SetActive(false);
        onComputer.SetActive(true);
    }
    public void onClickPlayNPass()
    {
        baseLayer.SetActive(false);
        onPassNPlay.SetActive(true);
    }
    public void onClickProfile()
    {
        baseLayer.SetActive(false);
        onProfile.SetActive(true);
    }
    public void onClickTournment()
    {
        baseLayer.SetActive(false);
        onTournment.SetActive(true);
    }
    public void onClickBackButton()
    {
        baseLayer.SetActive(true);
        friendOverlay.SetActive(false);
        onlineOverlay.SetActive(false);
        onComputer.SetActive(false);
        onPassNPlay.SetActive(false);
        onProfile.SetActive(false);
        onTournment.SetActive(false);
    }
}
