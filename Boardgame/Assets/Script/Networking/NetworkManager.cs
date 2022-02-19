using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    RoomOptions roomOptions;

    public UnityEvent onJoinedRoom;

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

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.LogFormat("Joined Room");
        onJoinedRoom?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogFormat("Joined Room Failed with returncode[{0}] and errmsg:{1}",returnCode,message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager: Disconnected from the server in cause of " + cause.ToString());
    }
}
