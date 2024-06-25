using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentListHandler : MonoBehaviour
{
    TournamentsData_JStruct tournamentsData;
    [SerializeField] TournamentListItem tournamentListItem;
    [SerializeField] TournamentCategories[] tournamentCat;
    [SerializeField] Transform tournamentListContent;
    [SerializeField] Transform tournamentCategoriesContent;
    [SerializeField] GameObject loadPnl;

    int winRange1 = 32000;     //0     - 32000
    int winRange2 = 64000;     //32000 - 64000
    int winRange3 = 640000;    //64000 - 640000

    List<TournamentItem_JStruct> tournamentItemsType1, tournamentItemsType2, tournamentItemsType3;

    private void Start()
    {
        tournamentItemsType1 = new List<TournamentItem_JStruct>();
        tournamentItemsType2 = new List<TournamentItem_JStruct>();
        tournamentItemsType3 = new List<TournamentItem_JStruct>();

        tournamentCat[0].GetComponent<Button>().onClick.AddListener(OpenTyep1List);
        tournamentCat[1].GetComponent<Button>().onClick.AddListener(OpenTyep2List);
        tournamentCat[2].GetComponent<Button>().onClick.AddListener(OpenTyep3List);

        LoadTournamentData();
    }
    void LoadTournamentData()
    {
        loadPnl.SetActive(true);
        APIHandler.instance.GetTournamentsData(OnLoadTournamentData);
    }
    void OnLoadTournamentData(bool success, TournamentsData_JStruct tournamentsData_JStruct)
    {
        if (success && tournamentsData_JStruct.meta.status)
        {
            if (tournamentsData == null)
                tournamentsData = new TournamentsData_JStruct();

            tournamentsData = tournamentsData_JStruct;

            StartCoroutine(StartTournamentListInflation());
        }
        loadPnl.SetActive(false);
    }
    IEnumerator StartTournamentListInflation()
    {
        loadPnl.SetActive(true);
        ClearTournamentItemItemContent();
        foreach (TournamentItem_JStruct tournamentItem in tournamentsData.Data)
        {
            float winAmount = tournamentItem.winningAmount;
            if (winAmount <= winRange1)
                tournamentItemsType1.Add(tournamentItem);
            else if (winAmount > winRange1 && winAmount <= winRange2)
                tournamentItemsType2.Add(tournamentItem);
            else if (winAmount > winRange2)
                tournamentItemsType3.Add(tournamentItem);
        }

        tournamentCat[0].gameObject.SetActive(tournamentItemsType1.Count > 0);
        tournamentCat[1].gameObject.SetActive(tournamentItemsType2.Count > 0);
        tournamentCat[2].gameObject.SetActive(tournamentItemsType3.Count > 0);

        if (tournamentItemsType1.Count > 0)
        {
            foreach (TournamentItem_JStruct tournamentItem in tournamentItemsType1)
            {
                TournamentListItem newItem = Instantiate(tournamentListItem, tournamentListContent);
                newItem.Initialize(tournamentItem, OnJoinTournament);
            }
        }
        else if (tournamentItemsType2.Count > 0)
        {
            foreach (TournamentItem_JStruct tournamentItem in tournamentItemsType2)
            {
                TournamentListItem newItem = Instantiate(tournamentListItem, tournamentListContent);
                newItem.Initialize(tournamentItem, OnJoinTournament);
            }
        }
        else if (tournamentItemsType3.Count > 0)
        {
            foreach (TournamentItem_JStruct tournamentItem in tournamentItemsType3)
            {
                TournamentListItem newItem = Instantiate(tournamentListItem, tournamentListContent);
                newItem.Initialize(tournamentItem, OnJoinTournament);
            }
        }
        loadPnl.SetActive(false);
        yield return null;
    }

    void OpenTyep1List()
    {
        ClearTournamentItemItemContent();
        if (tournamentItemsType1.Count > 0)
        {
            foreach (TournamentItem_JStruct tournamentItem in tournamentItemsType1)
            {
                TournamentListItem newItem = Instantiate(tournamentListItem, tournamentListContent);
                newItem.Initialize(tournamentItem, OnJoinTournament);
            }
        }
    }
    void OpenTyep2List()
    {
        ClearTournamentItemItemContent();
        if (tournamentItemsType2.Count > 0)
        {
            foreach (TournamentItem_JStruct tournamentItem in tournamentItemsType2)
            {
                TournamentListItem newItem = Instantiate(tournamentListItem, tournamentListContent);
                newItem.Initialize(tournamentItem, OnJoinTournament);
            }
        }
    }
    void OpenTyep3List()
    {
        ClearTournamentItemItemContent();
        if (tournamentItemsType3.Count > 0)
        {
            foreach (TournamentItem_JStruct tournamentItem in tournamentItemsType3)
            {
                TournamentListItem newItem = Instantiate(tournamentListItem, tournamentListContent);
                newItem.Initialize(tournamentItem, OnJoinTournament);
            }
        }
    }

    void OnJoinTournament(TournamentItem_JStruct tournamentItem)
    {

    }
    void ClearTournamentItemItemContent()
    {
        foreach(Transform obj in tournamentListContent)
        {
            Destroy(obj.gameObject);
        }
    }
}
