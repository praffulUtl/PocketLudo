using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    private NetworkVariableULong currentTurn = new NetworkVariableULong(new NetworkVariableSettings()
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    private NetworkVariableInt currentDiceRoll = new NetworkVariableInt(new NetworkVariableSettings()
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    private NetworkVariableBool moveCompleted = new NetworkVariableBool(new NetworkVariableSettings()
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Transform pathContainer;
    [SerializeField] private Transform startPositionContainer;

    // Server-side values for moving / spawning pieces
    private Dictionary<ulong, bool> playerReady = new Dictionary<ulong, bool>();
    private Dictionary<ulong, Piece[]> playerPieces = new Dictionary<ulong, Piece[]>();
    private Dictionary<ulong, bool> playerCompleted = new Dictionary<ulong, bool>();
    private Dictionary<int, int[]> paths;
    private Dictionary<int, Vector3[]> startPosition;
    private LudoTile[] board;
    private int[] finalTiles = new int[4];
    private int rollCountThisTurn;
    private bool canRollAgain;

    // Client-side values
    private Piece[] localPieces = new Piece[4];
    // UI References
    [SerializeField] private Image currentTurnColor;
    [SerializeField] private TextMeshProUGUI diceRollText;
    [SerializeField] private Image playerColor;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Transform winnerContainer;
    [SerializeField] private GameObject winnerPrefab;
    [SerializeField] private GameObject winnerPanel;

    // CONST
    private const int TEAM_COUNT = 4;
    private const int TILE_COUNT = 57;
    private const int TILE_IN_PATH_PRIOR_TO_GOAL = 52;
    private const int TILE_OFFSET_IN_BETWEEN_TEAM = 13;

    // Callbacks
    private void Awake()
    {
        #region Set path values for every team 
        paths = new Dictionary<int, int[]>(TEAM_COUNT);
        for (int teamId = 0; teamId < TILE_OFFSET_IN_BETWEEN_TEAM; teamId++)
        {
            int[] teamPath = new int[TILE_COUNT];
            for (int i = 0; i < TILE_COUNT; i++)
            {
                int offset = (TILE_OFFSET_IN_BETWEEN_TEAM * teamId);
                if (i + ((offset == 0) ? 1 : offset) < TILE_IN_PATH_PRIOR_TO_GOAL)
                    teamPath[i] = i + offset;
                else if (i < TILE_IN_PATH_PRIOR_TO_GOAL - 1)
                    teamPath[i] = i - TILE_IN_PATH_PRIOR_TO_GOAL + offset;
                else
                    teamPath[i] = i + 1 + (teamId * 6);
            }

            paths.Add(teamId, teamPath);
        }
        #endregion

        #region Initiate the board []
        board = new LudoTile[pathContainer.childCount];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new LudoTile();
            board[i].tileTransform = pathContainer.GetChild(i); // for position ref
            board[i].pieces = new Piece[4]; // maximum of 4 on a single tile
        }
        #endregion

        #region Set the start position
        startPosition = new Dictionary<int, Vector3[]>(4);
        for (int i = 0; i < 4; i++)
        {
            Transform c = startPositionContainer.GetChild(i);
            Vector3[] positions = new Vector3[4];
            for (int j = 0; j < 4; j++)
                positions[j] = c.GetChild(j).position;

            startPosition.Add(i, positions);
        }
        #endregion

        #region Final Tiles for victory test
        finalTiles[0] = paths[0][TILE_COUNT-1];
        finalTiles[1] = paths[1][TILE_COUNT-1];
        finalTiles[2] = paths[2][TILE_COUNT-1];
        finalTiles[3] = paths[3][TILE_COUNT-1];
        #endregion
    }
    private void Start()
    {
        RegisterEvents();
        OnPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
    }
    public override void NetworkStart()
    {
        // Is this is the owner, set the movecompleted value so we can begin our first turn
        if (IsOwner)
        { 
            moveCompleted.Value = true;
            moveCompleted.SetDirty(true);
        }

        // Instantiate logic values
        playerReady = new Dictionary<ulong, bool>();
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        { 
            playerReady.Add(nc.Key, false);
            var pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            if (pc.IsOwner)
            {
                // If this is our player, use that to assign the UI
                playerColor.color = Utility.TeamToColor(((Team)Utility.RetrieveTeamId(pc.OwnerClientId)));
                playerName.text = pc.playerName.Value;
            }
        }
    }
    public void OnDestroy()
    {
        UnregisterEvents();
    }
    private void Update()
    {
        // If its not our turn, don't update the raycasts
        if (currentTurn.Value == NetworkManager.Singleton.LocalClientId)
        { 
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 50.0f, LayerMask.GetMask("ActivePiece")))
            {
                if (Input.GetMouseButtonDown(0)) // Left click on a piece
                {
                    MovePiece(hit.collider.GetComponent<Piece>());
                }
            }
        }
    }

    // Piece logic
    private void MovePiece(Piece piece)
    {
        MovePieceServerRpc(Utility.GetPieceIndex(localPieces, piece));
    }
    // Ask the board if its even possible to move this piece based off our diceroll
    private bool CanMovePiece(Piece piece, int diceRoll)
    {
        // 0. Are we in play? if not, did we roll a 6
        if (piece.currentTile == -1 && diceRoll != 6)
        { 
            return false;
        }
        else
        {
            // If theres two or more here
            if (board[paths[(int)piece.currentTeam][0]].PieceCount() > 1) 
            {
                // If its not our team
                if (board[paths[(int)piece.currentTeam][0]].GetFirstPiece().currentTeam != piece.currentTeam) 
                {
                    // A blocker is formed by the enemy team, can't spawn
                    return false; 
                }
            }
        }

        // 1. Find the index in the current team's path
        int matchingIndex = FindIndexInPath(piece);

        // 2. Are we @ the limit
        if (matchingIndex + diceRoll >= paths[(int)piece.currentTeam].Length)
        {
            return false;
        }

        // 3. Check for obstructions
        for (int i = matchingIndex + 1; i <= matchingIndex + diceRoll; i++)
        {
            int targetTile = paths[(int)piece.currentTeam][i];
            int pieceCount = board[targetTile].PieceCount();

            // Initial path, prior to the safe zone
            if (i <= TILE_IN_PATH_PRIOR_TO_GOAL)
            {
                // If no one is here, or a single piece, continue
                if (pieceCount < 2)
                    continue;

                // If theres more than one piece, and its our team
                if (board[targetTile].GetFirstPiece().currentTeam == piece.currentTeam)
                    continue;
                else
                    return false;
            }
            // Pieces of the same team can't stack in the house
            else if (i < 56)
            {
                if (pieceCount > 0)
                    return false;
                else
                    return true;
            }
        }

        // Made it that far? You can do the move!
        return true;
    }
    // Path logic
    private int FindIndexInPath(Piece piece)
    {
        // Find the index of the current location, based off each team's individual path[]
        int matchingIndex = 0;
        for (int j = 0; j < paths[(int)piece.currentTeam].Length; j++)
        {
            if (paths[(int)piece.currentTeam][j] == piece.currentTile)
            {
                matchingIndex = j;
                break;
            }
        }

        return matchingIndex;
    }

    // Server side methods
    private void NextTurn()
    {
        // 0. Save the previous turn value, so we can check which one is next
        var previousTurn = currentTurn.Value;

        // 1. Reset the roll count, used in the 3x 6 in a row roll turn skip
        rollCountThisTurn = 0;

        // 2. Create an array with the client IDs
        ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();

        // 3. Ensure the current player is at the begening of the array, this will swap positions until it is true
        // ex : Blue's turn : [0,2,3,4] -> [4,0,2,3]
        for (int i = 0; i < turnIds.Length; i++)
        {
            // Is it already in order
            if (previousTurn == 0)
                break; 

            // Shift all values by one, and sets the last value as the previous first
            if (turnIds[i] == previousTurn)
            {
                while (turnIds[0] != previousTurn)
                { 
                    ulong prevLead = turnIds[0];
                    for (int j = 1; j < turnIds.Length; j++)
                        turnIds[j - 1] = turnIds[j];
                    turnIds[turnIds.Length - 1] = prevLead;
                }

                break;
            }
        }

        // If the next player's is done with the game, skip him
        for (int i = 1; i < turnIds.Length; i++)
        {
            if (!playerCompleted[turnIds[i]])
            { 
                currentTurn.Value = turnIds[i];
                break;
            }
        }
    }
    private void SpawnAllPlayers()
    {
        // Assign the values (owner side only)
        playerPieces = new Dictionary<ulong, Piece[]>(4);
        playerCompleted = new Dictionary<ulong, bool>(4);

        int playerIndex = 0;
        foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
        {
            Piece[] pieces = new Piece[4];

            // Spawn 4 Networked pieces for every active player
            PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
            for (int i = 0; i < 4; i++)
            {
                GameObject go = GameObject.Instantiate(piecePrefab);
                go.transform.position = startPosition[playerIndex][i];
                go.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
                pieces[i] = go.GetComponent<Piece>();
            }

            playerPieces.Add(nc.Key, pieces);
            playerCompleted.Add(nc.Key, false);

            playerIndex++;
        }
    }
    
    // Client side methods
    private Piece[] FindLocalPieces()
    {
        // This is for players to find which pieces are theirs, and to assign localPiece
        var r = new Piece[4];
        var pieces = FindObjectsOfType<Piece>();
        int pi = 0;
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i].IsOwner)
            {
                r[pi] = pieces[i];
                pi++;
            }
        }

        // Using FindObjectOfType pulls the object from top to bottom
        System.Array.Reverse(r);

        return r;
    }

    // Buttons
    public void DiceRollButton()
    {
        DiceRollServerRpc(NetworkManager.Singleton.LocalClientId);
    }
    public void DiceRollButton(int forceDice)
    {
        DiceRollServerRpc(NetworkManager.Singleton.LocalClientId, forceDice);
    }
    public void EndButton()
    { 
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Lobby");
    }

    // Events
    private void RegisterEvents()
    {
        currentTurn.OnValueChanged += UpdateCurrentTurn;
        currentDiceRoll.OnValueChanged += UpdateDiceUI;
        moveCompleted.OnValueChanged += UpdateMoveCompleted;
    }
    private void UnregisterEvents()
    {
        currentTurn.OnValueChanged -= UpdateCurrentTurn;
        currentDiceRoll.OnValueChanged -= UpdateDiceUI;
        moveCompleted.OnValueChanged -= UpdateMoveCompleted;
    }

    // Called by OnValueChanged for currentTurn
    private void UpdateCurrentTurn(ulong prev, ulong newValue)
    {
        int teamId = Utility.RetrieveTeamId(newValue);
        currentTurnColor.color = Utility.TeamToColor((Team)teamId);
    }
    // Called by OnValueChanged for currentDiceRoll
    private void UpdateDiceUI(int prev, int newValue)
    {
        diceRollText.text = newValue.ToString();
    }
    // Called by OnValueChanged for moveCompleted
    private void UpdateMoveCompleted(bool prev, bool newValue)
    {
        if (newValue)
            diceRollText.text = "Roll!";
    }

    // RPCs
    [ServerRpc(RequireOwnership = false)]
    private void EnterPieceServerRpc(int index ,ulong clientIndex)
    {
        int teamId = Utility.RetrieveTeamId(clientIndex);
        Piece piece = playerPieces[clientIndex][index];
        int startPosition = paths[teamId][0];

        // 1. Move the piece @ the start
        board[startPosition].AddPiece(piece);
        piece.currentTile = startPosition;

        // 2. Are we killing any piece?
        Piece p = board[startPosition].GetFirstPiece();
        if (p != null && p.currentTeam != piece.currentTeam)
        {
            board[startPosition].RemovePiece(p);
            p.currentTile = -1;
            p.PositionClientRpc(-Vector3.one); // start position is set localy
        }

        moveCompleted.Value = true;
        moveCompleted.SetDirty(true);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientIndex }
            }
        };
        DisableInteractionClientRpc(clientRpcParams);
    }
    [ServerRpc(RequireOwnership = false)]
    private void MovePieceServerRpc(int indexPos, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        Piece piece = playerPieces[clientId][indexPos];

        // 0. Are we in play?
        if (piece.currentTile == -1)
        {
            EnterPieceServerRpc(indexPos, clientId);
            return;
        }

        // 1. Find the index in the current team's path
        int matchingIndex = FindIndexInPath(piece);
        int targetTile = paths[(int)piece.currentTeam][matchingIndex + currentDiceRoll.Value];
        int previousPosition = piece.currentTile;

        // 2. Are we killing any piece?
        Piece p = board[targetTile].GetFirstPiece();
        if (p != null && p.currentTeam != piece.currentTeam)
        {
            board[targetTile].RemovePiece(p);
            p.currentTile = -1;
            p.PositionClientRpc(-Vector3.one); // start position is set localy
        }

        // 3. Move the piece there
        piece.currentTile = targetTile;
        board[targetTile].AddPiece(piece);
        board[previousPosition].RemovePiece(piece);

        moveCompleted.Value = true;
        moveCompleted.SetDirty(true);
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
        DisableInteractionClientRpc(clientRpcParams);

        // 4. Are you winning?
        if (finalTiles.Contains(targetTile))
        {
            if (board[targetTile].PieceCount() == 4)
            {
                NetworkManager.Singleton.ConnectedClients.TryGetValue(serverRpcParams.Receive.SenderClientId, out var networkedClient);
                string playerName = networkedClient.PlayerObject.GetComponent<PlayerController>().playerName.Value;
                AddWinnerToTheListClientRpc(playerName);
                playerCompleted[serverRpcParams.Receive.SenderClientId] = true;

                int i = 0;
                foreach(KeyValuePair<ulong, bool> pd in playerCompleted)
                    if (!pd.Value)
                        i++;

                if (i == 1) // If theres only one player left
                { 
                    EndGameClientRpc();
                    return;
                }

                NextTurn();
                return;
            }
        }

        if (!canRollAgain)
            NextTurn();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerReadyServerRpc(ulong clientId)
    {
        // Called when the player is done loading his scene, and his network is initialized
        playerReady[clientId] = true;
        // If all players are ready, spawn all the networked pieces
        if (!playerReady.ContainsValue(false))
            SpawnAllPlayers();
    }
    [ServerRpc(RequireOwnership = false)]
    public void DiceRollServerRpc(ulong clientId, int forceDice = -1)
    {
        // If we're either on the first turn, or we rolled a 6 
        if ((rollCountThisTurn == 0 || canRollAgain) && clientId == currentTurn.Value && moveCompleted.Value)
        {
            // Actually roll
            int rv = (forceDice == -1) ? Random.Range(1, 7) : forceDice;
            rollCountThisTurn++;
            canRollAgain = false;
            if (rv == 6)
            { 
                if (rollCountThisTurn == 3)
                {
                    // If we roll 3x 6 in a row, return
                    NextTurn();
                    return;
                }
                else
                {
                    canRollAgain = true;
                }
            }
            currentDiceRoll.Value = rv;
            currentDiceRoll.SetDirty(true);

            // Can we move any?
            bool[] pieceYouCanMove = new bool[4];
            bool canMove = false;
            for (int i = 0; i < playerPieces[clientId].Length; i++)
            {
                // For all of your pieces, check if we're allowed to move that piece with the current diceroll
                pieceYouCanMove[i] = CanMovePiece(playerPieces[clientId][i], rv);
                if (pieceYouCanMove[i])
                    canMove = true;
            }

            // If i can move atleast one piece, i gotta do it
            if (canMove)
            {
                moveCompleted.Value = false;
                moveCompleted.SetDirty(true);
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                };
                EnableInteractionClientRpc(pieceYouCanMove, clientRpcParams);
            }
            else
            {
                NextTurn();
            }
        }
    }

    [ClientRpc]
    public void AddWinnerToTheListClientRpc(string playerName)
    {
        // Sent to all clients when a player finishes
        string preString = "";
        int cc = winnerContainer.childCount;
        if (cc == 0)
            preString = "1st";
        else if (cc == 1)
            preString = "2nd";
        else if (cc == 2)
            preString = "3rd";
        else
            preString = "4th";

        // Create the piece of UI and fill the name of the winner
        GameObject go = Instantiate(winnerPrefab, winnerContainer);
        var t = go.GetComponent<TextMeshProUGUI>();
        t.text = $"{preString} : {playerName}";
    }
    [ClientRpc]
    public void EndGameClientRpc()
    {
        // Sent to all clients when theres a single player left
        winnerPanel.SetActive(true);
    }
    [ClientRpc]
    public void EnableInteractionClientRpc(bool[] pieceYouCanMove, ClientRpcParams clientRpcParams)
    {
        // Sent to current turn client, and activate the pieces that they can move
        if (localPieces[0] == null)
            localPieces = FindLocalPieces();

        for (int i = 0; i < pieceYouCanMove.Length; i++)
            if (localPieces[i] != null && pieceYouCanMove[i])
                localPieces[i].EnableInteraction();
    }
    [ClientRpc]
    public void DisableInteractionClientRpc(ClientRpcParams clientRpcParams)
    {
        // Sent to current turn client, turn off all pieces
        for (int i = 0; i < localPieces.Length; i++)
            localPieces[i].DisableInteraction();
    }
}
