using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Profile : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    [Header("Profile section")]
    [SerializeField] TextMeshProUGUI nameTxt, nameTxt2;
    [SerializeField] RawImage image;
    [Header("Edit section")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button pickImageBt;
    [SerializeField] Button openEditProfile,updateDataBt;
    PlayerDetails_JStruct playerDetails_JStruct;
    [SerializeField] GameObject editProfilePnl, LoadPnl;

    private void Start()
    {
        playerDetails_JStruct = new PlayerDetails_JStruct();
        Invoke(nameof(LoadProfileData), 1f);
        openEditProfile.onClick.AddListener(OpenEditPnl);
        if(!dummyMode)
        updateDataBt.onClick.AddListener(UpdateProfileData);
    }

    void LoadProfileData()
    {
        Debug.Log("LoadProfileData");
        if(!dummyMode)
        APIHandler.instance.GetUserData(LoadProfileDataCallback);
    }
    void LoadProfileDataCallback(bool success, PlayerDataRoot_JStruct playerDataRoot_JStruct)
    {
        Debug.Log("LoadProfileDataCallback");
        if (success && playerDataRoot_JStruct.meta.status)
        {
            Debug.Log("LoadProfileDataCallback");
            APIHandler.instance.SetPlayerID(playerDataRoot_JStruct.data._id);
            nameTxt.text = playerDataRoot_JStruct.data.playerName;
            nameTxt2.text = playerDataRoot_JStruct.data.playerName;
            nameInput.text = playerDataRoot_JStruct.data.playerName;
            if(playerDataRoot_JStruct.data.playerImageUrl.Trim() != "")
            APIHandler.instance.DownloadTexture(playerDataRoot_JStruct.data.playerImageUrl,LoadImageCallback);
        }
    }
    void LoadImageCallback(bool success, Texture texture)
    {
        image.texture = texture;
    }

    void UpdateProfileData()
    {
        LoadPnl.SetActive(true);
        playerDetails_JStruct.playerName = nameInput.text;
        nameTxt.text = nameInput.text;
        nameTxt2.text = nameInput.text;
        APIHandler.instance.PostUserData(playerDetails_JStruct, UpdateProfileDataCallback);
    }
    void UpdateProfileDataCallback(bool success, Meta meta)
    {
        if(success && meta.status)
        {
            CloseEditPnl();
        }
            LoadPnl.SetActive(false);
    }
    void OpenEditPnl()
    {
        editProfilePnl.SetActive(true);
    }
    void CloseEditPnl()
    {
        editProfilePnl.SetActive(false);
    }
}
