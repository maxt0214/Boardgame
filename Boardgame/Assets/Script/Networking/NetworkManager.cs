using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using ExitGames.Client.Photon;
using System;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<NetworkManager>();
            if (!instance)
                throw new Exception("No instance of GameManager in the scene!");
            return instance;
        }
    }

    RoomOptions roomOptions;

    public UnityAction onJoinedRoom;

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

    public void SendNetEvent(Hashtable data, byte code,  bool reliable = false)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(code, data, raiseEventOptions, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code > 199 || photonEvent.Code < 1)
        {
            Debug.LogErrorFormat("EventCode:{0} is prohibited!", photonEvent.Code);
            return;
        }
        EntityManager.Instance.Deserialize((Hashtable)photonEvent.CustomData);
    }
}
