using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager Instance;

    //Lobby List Vars
    public GameObject lobbyMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public GameObject lobbiesButton, hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void GetListOfLobbies() {
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);
        
        lobbyMenu.SetActive(true);

        SteamLobby.Instance.GetLobbyList();
    }

    public void BackToMenu() {
        lobbiesButton.SetActive(true);
        hostButton.SetActive(true);
        
        lobbyMenu.SetActive(false);
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result) {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby) {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;

                createdItem.GetComponent<LobbyDataEntry>().lobbyName = 
                    SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");

                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies() {
        foreach(GameObject lobbyItem in listOfLobbies) {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }
}
