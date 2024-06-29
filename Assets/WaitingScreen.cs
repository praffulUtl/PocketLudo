using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class WaitingScreen : MonoBehaviour
{
    float time = 0;
    [SerializeField] TextMeshProUGUI remainTimeText;
    [SerializeField] WaitScreenPlayerItem[] waitScreenPlayerItems;
    [SerializeField] Color red, blue, green, yellow;
    [NonSerialized] public int InitializeCount = 0;
    [NonSerialized]public bool isOpen = true;
    public void SetTime(int sec)
    {
        if (processTimerCoroutine != null)
        {
            StopCoroutine(processTimerCoroutine);
            processTimerCoroutine = null;
        }
        processTimerCoroutine = StartCoroutine(processTimer(sec));
    }
    Coroutine processTimerCoroutine;
    IEnumerator processTimer(float sec)
    {
        var waitSec = new WaitForSeconds(1);
        time = sec;
        while (true)
        {
            time -= 1;
            remainTimeText.text = $"Remaining Time : {time} s";
            yield return waitSec;   
        }
    }
    public void AddPlayer(string playerTeam)
    {
        PlayerTeam playerTeam1 = Enum.Parse<PlayerTeam>(playerTeam);
        foreach (var player in waitScreenPlayerItems) 
        { 
            if(player != null && !player.isInitialized) 
            {
                player.Initialize(GetColor(playerTeam1), "Player " + GetPlayerColorName(playerTeam1));
                InitializeCount++;
            }
        }
    }
    Color GetColor(PlayerTeam playerTeam)
    {
        if(playerTeam == PlayerTeam.R)
            return red;
        else if (playerTeam == PlayerTeam.B)
            return blue;
        else if (playerTeam == PlayerTeam.G)
            return green;
        else
            return yellow;

    }
    string GetPlayerColorName(PlayerTeam playerTeam)
    {
        if (playerTeam == PlayerTeam.R)
            return "red";
        else if (playerTeam == PlayerTeam.B)
            return "blue";
        else if (playerTeam == PlayerTeam.G)
            return "green";
        else
            return "yellow";
    }
    public void close()
    {
        isOpen = false;
        gameObject.SetActive(false);
        StopCoroutine(processTimerCoroutine);
    }
}
