using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;

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
    public Map Map
    {
        get
        {
            return FindObjectOfType<Map>();
        }
    }

    private Die die1;
    public Die Die1
    {
        get
        {
            if (die1 == null)
                die1 = GameObject.Find("Die1").GetComponent<Die>();
            return die1;
        }
    }
    private Die die2;
    public Die Die2
    {
        get
        {
            if (die2 == null)
                die2 = GameObject.Find("Die2").GetComponent<Die>();
            return die2;
        }
    }

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
        Player.transform.position = Map.WayPoints[0].position;
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

    public Dictionary<int, Photon.Realtime.Player> Players
    {
        get
        {
            return PhotonNetwork.CurrentRoom.Players;
        }
    }

    private List<int> playerIds;
    public List<int> PlayerIds
    {
        get
        {
            if(playerIds == null || playerIds.Count != Instance.Players.Count) playerIds = PhotonNetwork.CurrentRoom.Players.Select(kv => kv.Key).ToList();
            return playerIds;
        }
    }

    public int RollDice()
    {
        return Die1.RollAgain() + Die2.RollAgain();
    }
}
