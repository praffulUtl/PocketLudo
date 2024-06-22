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

    private void Start()
    {
        LoadTournamentData();
    }
    void LoadTournamentData()
    {
        APIHandler.instance.GetTournamentsData(OnLoadTournamentData);
    }
    void OnLoadTournamentData(bool success, TournamentsData_JStruct tournamentsData_JStruct)
    {
        if (success && tournamentsData_JStruct.Status)
        {
            if (tournamentsData == null)
                tournamentsData = new TournamentsData_JStruct();

            tournamentsData = tournamentsData_JStruct;

            StartCoroutine(StartTournamentListInflation());
        }
    }
    IEnumerator StartTournamentListInflation() 
    {
        ClearTournamentItemItemContent();
        foreach(TournamentItem_JStruct tournamentItem in tournamentsData.TournamentsList)
        {
            TournamentListItem newItem = Instantiate(tournamentListItem,tournamentListContent);
            newItem.Initialize(tournamentItem, OnJoinTournament);
        }
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
