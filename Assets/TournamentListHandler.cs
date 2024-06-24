using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentListHandler : MonoBehaviour
{
    TournamentsData_JStruct tournamentsData;
    [SerializeField] TournamentListItem tournamentListItem;
    [SerializeField] TournamentCategories tournamentCategories;
    [SerializeField] Transform tournamentListContent;
    [SerializeField] Transform tournamentCategoriesContent;
    [SerializeField] GameObject loadPnl;

    private void Start()
    {
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
        foreach(TournamentItem_JStruct tournamentItem in tournamentsData.Data)
        {
            TournamentListItem newItem = Instantiate(tournamentListItem,tournamentListContent);
            newItem.Initialize(tournamentItem, OnJoinTournament);
        }
        loadPnl.SetActive(false);
        yield return null;
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
