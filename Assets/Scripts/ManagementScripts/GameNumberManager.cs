using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameNumberManager : MonoBehaviour
{
    public TMP_Text gameNumberText; // Reference to the TextMeshPro text field for game number display

    private string authToken;
    private int userId;

    private void Start()
    {
        // Retrieve the auth token and user ID from PlayerPrefs
        authToken = PlayerPrefs.GetString("AuthToken", "");
        userId = PlayerPrefs.GetInt("UserId", 0);

        if (string.IsNullOrEmpty(authToken))
        {
            Debug.LogError("Auth token is missing.");
            return;
        }

        if (userId == 0)
        {
            Debug.LogError("User ID is missing.");
            return;
        }

        Debug.Log("User ID: " + userId);

        // Fetch game number at the start
        StartCoroutine(FetchGameNumberRequest());
    }

    private IEnumerator FetchGameNumberRequest()
    {
        string url = "https://utlsolutions.com/color-prediction/api/gameNumber?user_id=" + userId; // The endpoint for fetching game number with user ID

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                GameNumberResponse gameNumberResponse = JsonUtility.FromJson<GameNumberResponse>(jsonResponse);

                if (gameNumberResponse.game_number != null && gameNumberResponse.game_number.Count > 0)
                {
                    gameNumberText.text = gameNumberResponse.game_number[0].ToString();
                    Debug.Log("Game number fetched successfully: " + gameNumberResponse.game_number[0]);
                }
                else
                {
                    Debug.LogError("Game number list is empty or missing.");
                }
            }
        }
    }

    [System.Serializable]
    public class GameNumberResponse
    {
        public int user_id;
        public List<int> game_number; // List to accommodate array of game numbers
    }
}
