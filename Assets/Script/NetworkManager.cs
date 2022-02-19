using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    RoomOptions roomOptions;

    void Start()
    {
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 0;

    }

    public void ConnecteToServer()
    {
        Debug.Log("Connecting to server");
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager: Connected to the server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("BoardGame", roomOptions, new TypedLobby("Main Lobby",LobbyType.Default));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager: Disconnected from the server in cause of " + cause.ToString());
    }
}
