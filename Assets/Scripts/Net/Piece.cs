using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Piece : NetworkBehaviour
{
    public Team currentTeam;
    public int currentTile = -1;
    public Vector3 startPosition;
    [Header("colorsprites : R,G,Y,B")]
    [SerializeField] Sprite[] colorsprites;
    [SerializeField] SpriteRenderer colorSpriteRenderer;
    public override void NetworkStart()
    {
        // Assign the start position field to be restored when piece is eaten
        startPosition = transform.position;

        int teamId = Utility.RetrieveTeamId(OwnerClientId);
        currentTeam = (Team)teamId;
        GetComponent<MeshRenderer>().material.color = Utility.TeamToColor(currentTeam);
        colorSpriteRenderer.sprite = colorsprites[teamId];
        // Spawn a collider if you're the owner, to allow selection of the pieces
        if (IsOwner)
            gameObject.AddComponent<BoxCollider>();
    }

    // This is used with the scriptable rendering pipeline to add a select effect
    public void EnableInteraction()
    {
        gameObject.layer = LayerMask.NameToLayer("ActivePiece");
    }
    public void DisableInteraction()
    { 
        gameObject.layer = LayerMask.NameToLayer("Piece");
    }

    [ClientRpc]
    public void PositionClientRpc(Vector3 position)
    {
        // If -1, put the piece on its starting position
        if (position == -Vector3.one)
            transform.position = startPosition;
        else
            transform.position = position;
    }
}
