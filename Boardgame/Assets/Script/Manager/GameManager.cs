using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    public CharacterSelector charaSelector;
    public PlayerController Player;

    public bool gameStarted { get; private set; } = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        NetworkManager.Instance.onJoinedRoom += StartGame;

        NetworkConstant.Init();

        var character = charaSelector.SelectRandom(Player.transform);
        Player.character = character;
        DontDestroyOnLoad(Player.gameObject);
        DontDestroyOnLoad(charaSelector.gameObject);
    }

    private void OnDestroy()
    {
        if(NetworkManager.Instance != null)
            NetworkManager.Instance.onJoinedRoom -= StartGame;
    }

    public void Connect()
    {
        var floatingE = GameObject.Find("E");
        if ((Player.transform.position - floatingE.transform.position).magnitude > 4.3) return;

        if (!gameStarted && !NetworkManager.Instance.connecting)
            NetworkManager.Instance.ConnecteToServer();
    }

    private void StartGame()
    {
        Debug.Log("Joined Room: Game Started!");
        PhotonNetwork.LoadLevel(1);
        gameStarted = true;
        EntityManager.Instance.AddEntity(Player.character);
    }

    void Update()
    {
        if (gameStarted) //When game started(joined room), update EntityManager
            EntityManager.Instance.Update();
    }
}
