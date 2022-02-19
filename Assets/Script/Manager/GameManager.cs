using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private GameManager instance;
    public GameManager Instance
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

    private Dictionary<int, Entity> entities;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }
}
