using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardHandler : MonoBehaviour
{
    [SerializeField] bool dummyMode = false;
    [SerializeField] string dummyData = "";
    [SerializeField] LeaderboardPlayerItem Prefab_LeaderboardPlayerItem;
    [SerializeField] Transform content;
    [SerializeField] int currentPageNo = 1;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] GameObject loadingObj;
    LoadLeaderboardData_JStruct loadLeaderboardData_JStruct;
    bool isLoading = false;
    private void Start()
    {
        loadLeaderboardData_JStruct = new LoadLeaderboardData_JStruct();
        scrollbar.onValueChanged.AddListener(CheckScrollOver);
        Invoke(nameof(LoadLeaderboard),1f);
    }
    void CheckScrollOver(float f)
    {
        if (scrollbar.value < 0 && !isLoading)
            LoadLeaderboard();
    }
    void LoadLeaderboard()
    {
        Debug.Log("Loading leaderboard");
        isLoading = true;
        loadingObj.SetActive(true);
        loadLeaderboardData_JStruct.playerId = APIHandler.instance.key_playerId;
        loadLeaderboardData_JStruct.pageNo = currentPageNo;
        if (!dummyMode)
        {
            APIHandler.instance.PostLoadLeaderboard(loadLeaderboardData_JStruct, LoadLeaderboardCallback);
            currentPageNo++;
        }
        else
        {
            Debug.Log("LoadDummyData");
            StartCoroutine(LoadDummyData());
        }
    }
    void LoadLeaderboardCallback(bool success, LeaderboardDataRoot_JStruct leaderboardDataRoot_JStruct)
    {
        if (success && leaderboardDataRoot_JStruct.meta.status)
        {
            var players = leaderboardDataRoot_JStruct.data.players;
            foreach (var item in players)
            {
                GameObject newPlayerItem = Instantiate(Prefab_LeaderboardPlayerItem.gameObject, content);
                LeaderboardPlayerItem newObj = newPlayerItem.GetComponent<LeaderboardPlayerItem>();
                newObj.Initialize(item);
            }
        }
        isLoading = false;
        loadingObj.SetActive(false);
    }
    IEnumerator LoadDummyData()
    {
        while (isLoading)
        {
            Debug.Log("LoadDummyData");
            yield return new WaitForSeconds(1f);
            LeaderboardDataRoot_JStruct leaderboardDataRoot_JStruct = JsonConvert.DeserializeObject<LeaderboardDataRoot_JStruct>(dummyData);
            LoadLeaderboardCallback(true, leaderboardDataRoot_JStruct);
            loadingObj.SetActive(false);
            isLoading = false;
        }
    }
}
