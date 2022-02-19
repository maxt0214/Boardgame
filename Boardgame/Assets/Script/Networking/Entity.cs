using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// All Gameobjects that need to be networked must be inhereted from Entity
/// </summary>
public class Entity : MonoBehaviour
{
    private int tid;
    private int eid;

    private int owner;

    public bool Owned
    {
        get
        {
            return owner == PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

    public void Init(int tid, int eid, int owner)
    {
        this.tid = tid;
        this.eid = eid;
        this.owner = owner;
    }

    /// <summary>
    /// This should return an instance of the specific entity class(ex: character). EACH subclass must derive from this. Note: CreateInstance is only called when received an Entity from remote. DO NOT call CreateInstance! Only Implement.
    /// </summary>
    /// <returns></returns>
    public static Entity CreateInstance(int idx)
    {
        throw new Exception("Entity: CreateInstance should not be called on the base class");
    }

    public virtual void OnAwake()
    {

    }

    public virtual void OnStart()
    {
        
    }

    /// <summary>
    /// Send necessary data in Serialize
    /// </summary>
    public virtual void Serialize(Hashtable h)
    {
        if(Owned)
        {
            h.Add('t', tid);
            h.Add('e', eid);
            h.Add('o', owner);
        }
    }

    /// <summary>
    /// Parse data from remote
    /// </summary>
    public virtual void Deserialize(Hashtable h)
    {
        if(h.TryGetValue('t',out object tid))
        {
            this.tid = (int)tid;
        }

        if (h.TryGetValue('e', out object eid))
        {
            this.eid = (int)eid;
        }

        if (h.TryGetValue('o', out object oid))
        {
            owner = (int)oid;
        }
    }
}
