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
        var character = GameManager.Instance.Player.character;
        if (h.TryGetValue(character.eid, out object table))
        {
            character.Serialize((Hashtable)table);
        }
        else
        {
            h.Add(character.eid, new Hashtable());
            character.Serialize((Hashtable)h[character.eid]);
        }
    }

    public void Deserialize(Hashtable h)
    {
        foreach(var key in h.Keys)
        {
            if(entities.TryGetValue((int)key,out Entity entity)) //old enetity
            {
                if(!entity.Owned)
                    entity.Deserialize((Hashtable)h[key]);
            } else //new entity
            {
                var table = (Hashtable)h[key];
                if(table.TryGetValue((byte)'t', out object tid) && table.TryGetValue((byte)'c',out object cid))
                {
                    var method = NetworkConstant.Methods[(int)tid];
                    var ent = method.Invoke(null, new object[] { (int)cid }) as Entity;
                    ent.Deserialize(table);
                    ent.OnAwake();
                    ent.OnStart();
                    entities.Add((int)key,ent);
                }
            }
        }
    }

    public void HitCharacters(int mid, int dmg, int attacker)
    {
        int[] data = new int[3];
        foreach(var kv in entities)
        {
            var chara = kv.Value as Character;
            if (chara.mid == mid && chara.eid != attacker)
            {
                data[0] = kv.Key;
                data[1] = dmg * 10;
                data[2] = attacker;
                NetworkManager.Instance.SendNetEvent(data, NetworkConstant.senddmg, true);
            }
        }
    }

    public void DoHitCharacters(int[] dmginfo)
    {
        if(dmginfo.Length != 3)
        {
            Debug.LogError("Unknown DamageInfor Received!");
            return;
        }

        if(entities.TryGetValue(dmginfo[2], out Entity et))
        {
            var attacker = et as Character;
            attacker.Animate(Character.AnimState.ATTACK);
        }

        if(entities.TryGetValue(dmginfo[0],out Entity entity))
        {
            var character = entity as Character;
            character.DealDamage(dmginfo[1]);
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.5f)
        {
            timer = 0f;
            Serialize(data);
            NetworkManager.Instance.SendNetEvent(data,NetworkConstant.senddata);
        }

        foreach(var entity in entities.Values)
        {
            entity.OnUpdate();
        }
    }
}
