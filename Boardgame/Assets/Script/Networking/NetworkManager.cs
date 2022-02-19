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
            return instance;
        }
    }

    RoomOptions roomOptions;

    public UnityAction onJoinedRoom;
    public bool connecting { get; private set; } = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 0;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
    }

    public void ConnecteToServer()
    {
        Debug.Log("Connecting to server");
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
        connecting = true;
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
        connecting = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogFormat("Joined Room Failed with returncode[{0}] and errmsg:{1}",returnCode,message);
        connecting = false;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager: Disconnected from the server in cause of " + cause.ToString());
        connecting = false;
    }

    public void SendNetEvent(Hashtable data, byte code, bool reliable = false)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        if(PhotonNetwork.InRoom)
            PhotonNetwork.RaiseEvent(code, data, raiseEventOptions, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
    }

    public void SendNetEvent(int[] param, byte code, bool reliable = false)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(code, param, raiseEventOptions, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == NetworkConstant.senddata)
            EntityManager.Instance.Deserialize((Hashtable)photonEvent.CustomData);
        if (photonEvent.Code == NetworkConstant.senddmg)
            EntityManager.Instance.DoHitCharacters((int[])photonEvent.CustomData);
    }
}
