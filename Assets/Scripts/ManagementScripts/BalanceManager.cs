using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class BalanceManager : MonoBehaviour
{
    public TMP_Text balanceText; // Reference to the TextMeshPro text field for balance display
    public TMP_InputField rechargeAmountInputField; // Reference to the TMP InputField for recharge amount
    public Button fetchBalanceButton; // Button to fetch the balance
    public Button rechargeButton; // Button to recharge the balance

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

        if (fetchBalanceButton != null)
        {
            fetchBalanceButton.onClick.AddListener(OnFetchBalanceButtonClicked);
        }

        if (rechargeButton != null)
        {
            rechargeButton.onClick.AddListener(OnRechargeButtonClicked);
        }

        // Automatically fetch balance on start
        StartCoroutine(FetchBalanceRequest());
    }

    private void OnFetchBalanceButtonClicked()
    {
        StartCoroutine(FetchBalanceRequest());
    }

    private void OnRechargeButtonClicked()
    {
        string amountText = rechargeAmountInputField.text;
        if (!string.IsNullOrEmpty(amountText) && float.TryParse(amountText, out float amount))
        {
            StartCoroutine(RechargeBalanceRequest(amount));
        }
        else
        {
            Debug.LogWarning("Invalid recharge amount.");
        }
    }

    private IEnumerator FetchBalanceRequest()
    {
        string url = "https://utlsolutions.com/color-prediction/api/balance?user_id=" + userId; // The endpoint for fetching balance with user ID

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
                BalanceResponse balanceResponse = JsonUtility.FromJson<BalanceResponse>(jsonResponse);
                balanceText.text = "Balance: " + balanceResponse.balance;
                Debug.Log("Balance fetched successfully: " + balanceResponse.balance);
            }
        }
    }

    private IEnumerator RechargeBalanceRequest(float amount)
    {
        string url = "https://utlsolutions.com/color-prediction/api/recharge"; // The endpoint for recharging balance

        var requestBody = new RechargeRequest
        {
            amount = amount,
            user_id = userId
        };

        string jsonString = JsonUtility.ToJson(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + authToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Recharge successful.");
                Debug.Log("Recharge Amount: " + amount); // Add this line for successful recharge debug log
                StartCoroutine(FetchBalanceRequest()); // Update the balance after recharge
            }
        }
    }

    [System.Serializable]
    public class BalanceResponse
    {
        public string balance;
    }

    [System.Serializable]
    public class RechargeRequest
    {
        public float amount;
        public int user_id;
    }
}
