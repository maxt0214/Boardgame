using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class EntityManager
{
    private static EntityManager instance;
    public static EntityManager Instance
    {
        get
        {
            if (instance == null)
                instance = new EntityManager();
            return instance;
        }
    }

    private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
    private Hashtable data = new Hashtable();

    private int unused = 0;

    private float timer = 0f;

    public void AddEntity<T>(T entity) where T : Entity
    {
        int eid = PhotonNetwork.LocalPlayer.ActorNumber * 1000 + unused; //1000 different entity ids for each player
        unused++;

        entity.Init(NetworkConstant.EntityIds[typeof(T)],eid,PhotonNetwork.LocalPlayer.ActorNumber);

        entities.Add(eid,entity);
    }

    public void Serialize(Hashtable h)
    {
        foreach(var kv in entities)
        {
            if(h.TryGetValue(kv.Key,out object table))
            {
                kv.Value.Deserialize((Hashtable)table);
            } else
            {
                h.Add(kv.Key,new Hashtable());
                kv.Value.Deserialize((Hashtable)h[kv.Key]);
            }
        }
    }

    public void Deserialize(Hashtable h)
    {
        foreach(var key in h.Keys)
        {
            if(entities.TryGetValue((int)key,out Entity entity)) //old enetity
            {
                entity.Deserialize((Hashtable)h[key]);
            } else //new entity
            {
                var table = (Hashtable)h[key];
                if(table.TryGetValue('t', out object tid) && table.TryGetValue('c',out object cid))
                {
                    var method = NetworkConstant.Methods[(int)tid];
                    var ent = method.Invoke(null, new object[] { (int)cid }) as Entity;
                    ent.Deserialize(table);
                    ent.OnAwake();
                    ent.OnStart();
                }
            }
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.5f)
        {
            timer = 0f;
            Serialize(data);
        }
    }
}
