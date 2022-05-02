using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    //Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //Lobby Callbacks
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;
    
    public List<CSteamID> lobbyIDs = new List<CSteamID>();

    //Variables
    public ulong currentLobbyID;
    private const string hostAddressKey = "HostAddress";
    private FPSNetworkManager manager;

    private void Start() {
        if (!SteamManager.Initialized) {
            Debug.Log("<color=yellow>Steam is not open</color>");
            return;
        }

        if (Instance == null) Instance = this;

        manager = GetComponent<FPSNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void GetLobbyList() {
        if(lobbyIDs.Count > 0) {
            lobbyIDs.Clear();
        }

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        if(LobbyListManager.Instance.listOfLobbies.Count > 0) {
            LobbyListManager.Instance.DestroyLobbies();
        }

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbyListManager.Instance.DisplayLobbies(lobbyIDs, result);
    }

    public void HostLobby() {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);


    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby created Successfully");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            hostAddressKey,
            SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
        Debug.Log("Request to join lobby");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        Debug.Log("Lobby Entered");
        currentLobbyID = callback.m_ulSteamIDLobby;

        //Client
        if (NetworkServer.active) return;

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);
        manager.StartClient();
    }

    public void JoinLobby(CSteamID lobbyID) {
        SteamMatchmaking.JoinLobby(lobbyID);
    }
}
