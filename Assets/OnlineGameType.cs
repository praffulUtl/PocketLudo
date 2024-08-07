using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineGameType : MonoBehaviour
{
    public JoinedTournamentDataRoot_JStruct joinTurnamentJoinData;
    public OnlineGameJoinDataRoot_JStruct globalGameRootData;

    public int lobbyId ;
    public bool isTimer = false;

    public GameType gameType = GameType.LOCAL;
}
public enum GameType
{
    LOCAL,
    GLOBAL,
    TOURNAMENT    
}
