using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BalanceManager : MonoBehaviour
{
    public GameObject transactionDetailPrefab; // Reference to the Transaction Detail prefab
    public Transform transactionListParent; // Parent GameObject to contain the instantiated transaction details

    public TMP_Text balanceText; //  to the TextMeshPro text field for balance display
    public TMP_Text[] balanceText2; // Reference to the TextMeshPro text field for balance display
    public TMP_Text winningBalanceText; // Reference to the TextMeshPro text field for winning balance display
    public TMP_InputField rechargeAmountInputField; // Reference to the TMP InputField for recharge amount
    public Button fetchBalanceButton; // Button to fetch the balance
    public Button rechargeButton; // Button to recharge the balance
    public Button fetchWinningBalanceButton; // Button to fetch the winning balance
    public Button[] amountButtons; // Array to hold references to the amount buttons
    private string balance;
    public bool rechargeScreen = true;

    public string authKey;

    private string baseURL;
    [SerializeField]
    private APIHandler apiHandler;

    private void Start()
    {
        baseURL = apiHandler.baseUrl;
        // Retrieve the auth token and user ID from PlayerPrefs

        //authKey = PlayerPrefs.GetString("authKey", ""); // Assuming this is where the authkey is stored
        authKey = APIHandler.instance.key_authKey;
        
        if (rechargeScreen)
        {
            StartCoroutine(FetchRechargeListRequest());
        }

        if (fetchBalanceButton != null)
        {
            fetchBalanceButton.onClick.AddListener(OnFetchBalanceButtonClicked);
        }

        if (rechargeButton != null)
        {
            rechargeButton.onClick.AddListener(OnRechargeButtonClicked);
        }

        if (fetchWinningBalanceButton != null)
        {
            fetchWinningBalanceButton.onClick.AddListener(OnFetchWinningBalanceButtonClicked);
        }

        // Assign click listeners to all the amount buttons
        foreach (Button button in amountButtons)
        {
            button.onClick.AddListener(() => OnAmountButtonClicked(button));
        }

        // Automatically fetch balance on start
        StartCoroutine(FetchBalanceRequest());
        StartCoroutine(FetchWinningBalanceRequest());
    }

    private void OnFetchBalanceButtonClicked()
    {
        StartCoroutine(FetchBalanceRequest());
    }

    private void OnFetchWinningBalanceButtonClicked()
    {
        StartCoroutine(FetchWinningBalanceRequest());
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

    // Method to handle amount button clicks
    private void OnAmountButtonClicked(Button button)
    {
        // Get the text component of the button and parse the value
        int amount = int.Parse(button.GetComponentInChildren<TMP_Text>().text);
        // Update the input field with the selected amount
        rechargeAmountInputField.text = amount.ToString();
    }

    private IEnumerator FetchBalanceRequest()
    {
        string url = baseURL + "player/profile"; // The endpoint for fetching balance with user ID

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("authkey", authKey);
            Debug.Log("authKey :" + authKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ProfileResponse profileResponse = JsonUtility.FromJson<ProfileResponse>(jsonResponse);
                balanceText.text = profileResponse.data.topUpBalance.ToString();

                foreach(var obj in balanceText2)
                    obj.text = profileResponse.data.topUpBalance.ToString();

                balance = profileResponse.data.topUpBalance.ToString();
                PlayerPrefs.SetString("balance", balance);
                Debug.Log("Balance fetched successfully: " + profileResponse.data.topUpBalance);
            }
        }
    }

    private IEnumerator FetchWinningBalanceRequest()
    {
        string url = baseURL + "player/profile"; // The endpoint for fetching winning balance

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("authkey", authKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ProfileResponse profileResponse = JsonUtility.FromJson<ProfileResponse>(jsonResponse);
                if(winningBalanceText!=null)
                winningBalanceText.text = profileResponse.data.winningBalance.ToString();
                Debug.Log("Winning balance fetched successfully: " + profileResponse.data.winningBalance);
            }
        }
    }

    private IEnumerator FetchRechargeListRequest()
    {
        string url = baseURL + "recharge/list"; // The endpoint for fetching the recharge list

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("authkey", authKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("jsonResponse: " + jsonResponse);
                RechargeListResponse rechargeListResponse = JsonUtility.FromJson<RechargeListResponse>(jsonResponse);

                if (rechargeListResponse.meta.status && rechargeListResponse.data.Length > 0)
                {
                    // Clear existing transaction details
                    foreach (Transform child in transactionListParent)
                    {
                        Destroy(child.gameObject);
                    }

                    // Instantiate and update the UI with the last 10 transaction details
                    int transactionCount = Mathf.Min(rechargeListResponse.data.Length, 10);

                    for (int i = 0; i < transactionCount; i++)
                    {
                        var transaction = rechargeListResponse.data[i];
                        GameObject transactionDetail = Instantiate(transactionDetailPrefab, transactionListParent);
                        TMP_Text[] textFields = transactionDetail.GetComponentsInChildren<TMP_Text>();

                        textFields[0].text = "Success";
                        textFields[1].text = transaction.amount;
                        textFields[2].text = transaction.createdAt.ToString();
                        textFields[3].text = transaction.transactionId;
                    }
                }
                else
                {
                    Debug.LogWarning("No recharge transactions found.");
                }
            }
        }
    }

    private IEnumerator RechargeBalanceRequest(float amount)
    {
        string url = baseURL + "recharge/add"; // The endpoint for recharging balance

        // Generate a random transaction ID
        string transactionId = System.Guid.NewGuid().ToString();

        var requestBody = new RechargeRequest
        {
            amount = amount,
            transactionId = transactionId
        };

        string jsonString = JsonUtility.ToJson(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("authkey", authKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                SceneManager.LoadScene("OTP");

            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                RechargeResponse response = JsonUtility.FromJson<RechargeResponse>(jsonResponse);

                if (response.meta.status)
                {
                    Debug.Log("Recharge successful.");
                    Debug.Log("Recharge Amount: " + amount);
                    Debug.Log("Transaction ID: " + transactionId);
                    // Update the transaction list after recharge

                    // Disable the recharge button and amount buttons after successful recharge
                    //rechargeButton.interactable = false;
                    StartCoroutine(FetchRechargeListRequest());
                    StartCoroutine(FetchBalanceRequest());

                }
                else
                {
                    Debug.LogError("Recharge failed: " + response.meta.msg);
                }
            }

        }
    }

    [System.Serializable]
    public class ProfileResponse
    {
        public Meta meta;
        public ProfileData data;

        [System.Serializable]
        public class Meta
        {
            public string msg;
            public bool status;
        }

        [System.Serializable]
        public class ProfileData
        {
            public float winningBalance;
            public float topUpBalance;
        }
    }

    [System.Serializable]
    public class RechargeRequest
    {
        public float amount;
        public string transactionId;
    }

    [System.Serializable]
    public class RechargeListResponse
    {
        public Meta meta;
        public RechargeData[] data;

        [System.Serializable]
        public class Meta
        {
            public string msg;
            public bool status;
        }

        [System.Serializable]
        public class RechargeData
        {
            public string amount;
            public string transactionId ;
            public long createdAt;
        }
    }

    [System.Serializable]
    public class RechargeResponse
    {
        public Meta meta;

        [System.Serializable]
        public class Meta
        {
            public string msg;
            public bool status;
        }
    }
}
