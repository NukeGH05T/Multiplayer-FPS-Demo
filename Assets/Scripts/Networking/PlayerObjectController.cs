using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIDNumber;
    [SyncVar] public ulong playerSteamID;

    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool isReady;

    //Cosmetics
    [SyncVar(hook = nameof(SendPlayerColor))] public int PlayerColor;

    private void PlayerReadyUpdate(bool oldReady, bool newReady)
    {
        if (isServer) {
            this.isReady = newReady;
        }
        if (isClient) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady() {
        this.PlayerReadyUpdate(this.isReady, !this.isReady);
    }

    public void ChangeReady() {
        if (hasAuthority) CmdSetPlayerReady();
    }

    private FPSNetworkManager manager;

    private FPSNetworkManager Manager {
        get
        {
            if (manager != null) {
                return manager;
            }

            return manager = FPSNetworkManager.singleton as FPSNetworkManager;
        }
    }

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.gamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.gamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName) {
        this.PlayerNameUpdate(this.playerName, playerName);
    }

    private void PlayerNameUpdate(string oldValue, string newValue) {
        if (isServer) {
            this.playerName = newValue;
        } 

        if (isClient) {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string sceneName) {
        if (hasAuthority) {
            CmdCanStartGame(sceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string sceneName) {
        manager.StartGame(sceneName);
    }

    //Cosmetics
    [Command]
    private void CmdUpdatePlayerColor(int newValue)
    {
        SendPlayerColor(PlayerColor, newValue);
    }

    private void SendPlayerColor(int oldValue, int newValue)
    {
        if (isServer) {
            PlayerColor = newValue;
        } 

        if (isClient && (oldValue != newValue)) {
            UpdateColor(newValue);
        }
    }

    private void UpdateColor(int message) {
        PlayerColor = message;
    }
}
