using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<Transform> WayPoints = new List<Transform>();

    private void Awake()
    {
        foreach(var trans in transform.GetComponentsInChildren<Transform>())
        {
            if (trans != transform)
                WayPoints.Add(trans);
        }
    }

    public int ValidateIndex(int idx)
    {
        return (idx > WayPoints.Count) ? idx - WayPoints.Count : idx;
    }

    public Transform GetWayPoint(int idx)
    {
        //Should Validate Index. Will reconstruct in the future
        return WayPoints[idx];
    }
}
