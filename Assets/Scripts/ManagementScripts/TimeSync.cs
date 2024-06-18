using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class TimerSync : MonoBehaviour
{
    private DateTime serverTime; // The last synchronized server time
    private float clientStartTime; // The local client start time
    private float clientElapsedSeconds; // Elapsed time on the client side

    public string serverTimeUrl = "http://yourlaravelapp/api/current-time"; // URL to fetch server time

    private void Start()
    {
        // Initialize the client start time
        clientStartTime = Time.time;

        // Synchronize with the server time
        StartCoroutine(SynchronizeServerTime());
    }

    private IEnumerator SynchronizeServerTime()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverTimeUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching server time: " + request.error);
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            ServerTimeResponse timeResponse = JsonUtility.FromJson<ServerTimeResponse>(jsonResponse);

            serverTime = DateTime.Parse(timeResponse.currentTime).ToUniversalTime();

            // Calculate the time difference between server and client
            TimeSpan timeDifference = serverTime - DateTime.UtcNow;

            // Adjust the client start time based on the time difference
            clientStartTime -= (float)timeDifference.TotalSeconds;

            // Start the timer
            StartCoroutine(UpdateClientTimer());
        }
    }

    private IEnumerator UpdateClientTimer()
    {
        while (true)
        {
            // Update the elapsed time on the client side
            clientElapsedSeconds = Time.time - clientStartTime;

            // Display or use the client elapsed time as needed
            Debug.Log("Client Elapsed Time: " + clientElapsedSeconds);

            yield return null;
        }
    }

    [Serializable]
    public class ServerTimeResponse
    {
        public string currentTime;
    }
}
