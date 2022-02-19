using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if (!instance)
                throw new Exception("No instance of GameManager in the scene!");
            return instance;
        }
    }

    public CharacterSelector charaSelector;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        NetworkManager.Instance.onJoinedRoom += StartGame;
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.onJoinedRoom -= StartGame;
    }

    private void StartGame()
    {

    }

    void Update()
    {
        
    }
}
