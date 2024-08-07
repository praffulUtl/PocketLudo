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
    UpdateProfile_JStruct updateProfile_JStruct;
    [SerializeField] GameObject editProfilePnl, LoadPnl;

    private void Start()
    {
        playerDetails_JStruct = new PlayerDetails_JStruct();
        LoadProfileData();
        openEditProfile.onClick.AddListener(OpenEditPnl);
        if(!dummyMode)
        updateDataBt.onClick.AddListener(UpdateProfileData);
        updateProfile_JStruct = new UpdateProfile_JStruct();
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
            if (playerDataRoot_JStruct.data.playerImageUrl.Trim() != "")
                APIHandler.instance.DownloadTexture(playerDataRoot_JStruct.data.playerImageUrl, LoadImageCallback);
        }
    }
    
    void LoadImageCallback(bool success, Texture texture)
    {
        image.texture = texture;
    }

    void UpdateProfileData()
    {
        LoadPnl.SetActive(true);
        updateProfile_JStruct.playerName = nameInput.text;
        updateProfile_JStruct.playerImageUrl = "";
        APIHandler.instance.PutUserData(updateProfile_JStruct, UpdateProfileDataCallback);
    }
    void UpdateProfileDataCallback(bool success, StartGame_JStruct data)
    {
        if(success )
        {
            if(data.meta.status)
            {
                APIHandler.instance.GetUserData(LoadProfileDataCallback);
            }
            CloseEditPnl();
        }
            LoadPnl.SetActive(false);
    }
    void OpenEditPnl()
    {
        //APIHandler.instance.GetUserData(LoadProfileDataCallback);
        editProfilePnl.SetActive(true);
    }
    void CloseEditPnl()
    {
        editProfilePnl.SetActive(false);
    }
}
