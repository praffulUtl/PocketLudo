using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class LudoGame : MonoBehaviour
{
    public enum Team
    {
        R,
        B,
        G,
        Y
    }

    [System.Serializable]
    public class PlayerPiece
    {
        public bool IsOpen;
        public bool ReachedWinPos;
        public int MovementBlockIndex;
    }

    [System.Serializable]
    public class OtherPlayer
    {
        public Team PlayerTeam = Team.R;
        public int DiceNumber;
        public bool PlayerTurn;
        public List<PlayerPiece> Playerpiece;
    }

    [System.Serializable]
    public class Data
    {
        public List<OtherPlayer> OtherPlayer;
    }

    [System.Serializable]
    public class Meta
    {
        public string msg;
        public bool status;
    }

    [System.Serializable]
    public class GameState
    {
        public Meta meta;
        public Data data;
    }

    public Team selectedTeam;
    private GameState gameState;
    private const int TargetPosition = 54;

    void Start()
    {
        InitializeGameState();
        StartCoroutine(UpdateGameStateCoroutine());
    }

    void InitializeGameState()
    {
        gameState = new GameState
        {
            meta = new Meta { msg = "", status = true },
            data = new Data
            {
                OtherPlayer = new List<OtherPlayer>
                {
                    new OtherPlayer { PlayerTeam = Team.R, DiceNumber = 0, PlayerTurn = false, Playerpiece = CreatePlayerPieces() },
                    new OtherPlayer { PlayerTeam = Team.B, DiceNumber = 0, PlayerTurn = true, Playerpiece = CreatePlayerPieces() },
                    new OtherPlayer { PlayerTeam = Team.G, DiceNumber = 0, PlayerTurn = false, Playerpiece = CreatePlayerPieces() },
                    new OtherPlayer { PlayerTeam = Team.Y, DiceNumber = 0, PlayerTurn = false, Playerpiece = CreatePlayerPieces() }
                }
            }
        };
    }

    List<PlayerPiece> CreatePlayerPieces()
    {
        return new List<PlayerPiece>
        {
            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 },
            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 },
            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 },
            new PlayerPiece { IsOpen = false, ReachedWinPos = false, MovementBlockIndex = -1 }
        };
    }

    IEnumerator UpdateGameStateCoroutine()
    {
        while (!AllPlayersFinished())
        {
            foreach (var player in gameState.data.OtherPlayer)
            {
                UpdatePlayerState(player);
                var filteredGameState = FilterGameState(gameState, selectedTeam);
                string json = JsonConvert.SerializeObject(filteredGameState, Formatting.Indented);
                Debug.Log(json);
                yield return new WaitForSeconds(1); // Simulate time between turns
            }
        }

        Debug.Log("All players have finished!");
    }

    GameState FilterGameState(GameState state, Team excludedTeam)
    {
        var filteredState = new GameState
        {
            meta = state.meta,
            data = new Data
            {
                OtherPlayer = state.data.OtherPlayer.FindAll(player => player.PlayerTeam != excludedTeam)
            }
        };
        return filteredState;
    }

    bool AllPlayersFinished()
    {
        foreach (var player in gameState.data.OtherPlayer)
        {
            foreach (var piece in player.Playerpiece)
            {
                if (!piece.ReachedWinPos)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void UpdatePlayerState(OtherPlayer player)
    {
        player.DiceNumber = Random.Range(1, 7);
        if (player.DiceNumber == 6)
        {
            foreach (var piece in player.Playerpiece)
            {
                if (!piece.IsOpen)
                {
                    piece.IsOpen = true;
                    player.DiceNumber = 0;
                    break;
                }
            }
        }
        if (player.DiceNumber > 0)
        {
            foreach (var piece in player.Playerpiece)
            {
                if (piece.IsOpen && !piece.ReachedWinPos)
                {
                    piece.MovementBlockIndex += player.DiceNumber;
                    if (piece.MovementBlockIndex >= TargetPosition)
                    {
                        piece.MovementBlockIndex = TargetPosition;
                        piece.ReachedWinPos = true;
                    }
                    break;
                }
            }
        }
    }
}
