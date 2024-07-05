using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GameDetails : MonoBehaviour
{
    public TMP_Text entryFeeText;  // Reference to the TMP_Text component

    void Start()
    {
        // Start the coroutine to fetch game details
        StartCoroutine(GetGameDetails());
    }

    IEnumerator GetGameDetails()
    {
        // Get the authkey from PlayerPrefs
        string authKey = PlayerPrefs.GetString("authKey");

        if (string.IsNullOrEmpty(authKey))
        {
            Debug.LogError("Authkey not found in PlayerPrefs");
            yield break;
        }

        // Set up the request
        UnityWebRequest request = new UnityWebRequest("http://localhost:5211/ludo/v1/user/game/details", "GET");
        request.SetRequestHeader("authkey", authKey);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            yield break;
        }

        // Parse the response
        string jsonResponse = request.downloadHandler.text;
        Debug.Log("Response: " + jsonResponse);

        GameDetailsResponse response = JsonUtility.FromJson<GameDetailsResponse>(jsonResponse);

        if (response.meta.status)
        {
            // Update the TMP_Text field with the entryFee value
            entryFeeText.text = response.data.entryFee.ToString();
        }
        else
        {
            Debug.LogError("Failed to retrieve game details");
        }
    }

    // Classes to parse the JSON response within a namespace to avoid conflicts
    [System.Serializable]
    public class GameDetailsResponse
    {
        public Meta meta;
        public Data data;
    }

    [System.Serializable]
    public class Meta
    {
        public string msg;
        public bool status;
    }

    [System.Serializable]
    public class Data
    {
        public string _id;
        public int entryFee;
        public int duration;
        public int firstWinnerPrize;
        public int secondWinnerPrize;
        public long createdAt;
        public long updatedAt;
        public int __v;
    }
}
