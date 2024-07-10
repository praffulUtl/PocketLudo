using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlayerItem : MonoBehaviour
{
    [SerializeField] RawImage playerImage;
    [SerializeField] TextMeshProUGUI playerNameTxt;
    [SerializeField] TextMeshProUGUI playerRankTxt;
    [SerializeField] TextMeshProUGUI playerScoreTxt;
    public void Initialize(LeaderboardPlayer_JStruct leaderboardPlayer_JStruct,int index)
    {
        var data = leaderboardPlayer_JStruct;

        if(data.playerImageUrl.Trim() != "")
        APIHandler.instance.DownloadTexture(data.playerImageUrl,PlayerImageDownloadCallback);

        playerNameTxt.text = data.playerName;
        playerRankTxt.text = index.ToString();
        playerScoreTxt.text = data.score.ToString();
    }
    void PlayerImageDownloadCallback(bool success, Texture texture)
    {
        if (success)
            playerImage.texture = texture;
    }
}
