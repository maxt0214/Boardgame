using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void LateUpdate()
    {
        if(player != null)
            transform.position = player.transform.position + new Vector3(0f, 7.5f, -7f);
    }
}
