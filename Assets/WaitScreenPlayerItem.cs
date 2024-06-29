using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitScreenPlayerItem : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI playerNameTxt;
    [SerializeField] GameObject activeContent;
    [SerializeField] GameObject waitingContent;
    public bool isInitialized = false;

    public void Initialize(Color color, string playerName)
    {
        image.color = color;
        playerNameTxt.text = playerName;
        activeContent.SetActive(true);
        waitingContent.SetActive(false);
        isInitialized = true;
    }
}
