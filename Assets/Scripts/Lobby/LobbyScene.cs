using MLAPI;
using MLAPI.SceneManagement;
using TMPro;
using UnityEngine;

/* TO DO
 * As pointed out in the comment section, using an animator on the canvas is a bad practice
 * as it refreshed the Dirty state of the UI every frame, and it forces the engine to re-draw
 * it, see the following link for more info
 * https://unity3d.com/how-to/unity-ui-optimization-tips
 */

public class LobbyScene : MonoSingleton<LobbyScene>
{
    [SerializeField] private Animator animator;

    // Lobby UI
    [SerializeField] public GameObject playerListItemPrefab;
    [SerializeField] public Transform playerListContainer;
    [SerializeField] public TMP_InputField playerNameInput;

    #region Buttons
    // Main
    public void OnMainHostButton()
    {
        NetworkManager.Singleton.StartHost();
        animator.SetTrigger("Lobby");
    }
    public void OnMainConnectButton()
    {
        NetworkManager.Singleton.StartClient();
        animator.SetTrigger("Lobby");
    }

    // Lobby
    public void OnLobbyBackButton()
    {
        animator.SetTrigger("Main");
        NetworkManager.Singleton.Shutdown();
    }
    public void OnLobbyStartButton()
    {
        NetworkSceneManager.SwitchScene("Game2d");
    }
    public void OnLobbySubmitNameChange()
    {
        string newName = playerNameInput.text;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.ChangeName(newName);
        }
    }
    #endregion
}