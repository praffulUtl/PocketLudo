using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitingScreen : MonoBehaviour
{
    [SerializeField] WaitScreenPlayerItem[] waitScreenPlayerItems;
    [SerializeField] Color red, blue, green, yellow;
    public int InitializeCount = 0;
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
}
