using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    public Animator anim;

    public float dmg = 10f;
    public float hp = 100f;

    //These are destination
    private int cid;
    private Vector3 pos;
    private Vector3 dir;

    private void Start()
    {
        
    }

    public void Update()
    {
        
    }

    public void Init(int cid, int tid, int eid, int owner)
    {
        this.cid = cid;
        base.Init(tid, eid, owner);
    }

    public static new Character CreateInstance(int idx)
    {
        var clone = GameManager.Instance.charaSelector.SelectRandom(GameManager.Instance.transform);//non local players are stored under GameManger
        return clone.GetComponent<Character>();
    }

    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        if (Owned)
        {
            h.Add('c', cid);
            h.Add('p', pos);
            h.Add('d', dir);
        }
    }

    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);

        if (h.TryGetValue('c', out object cid))
            this.cid = (int)cid;

        if (h.TryGetValue('p', out object pos))
            this.pos = (Vector3)pos;

        if (h.TryGetValue('d', out object dir))
            this.dir = (Vector3)dir;
    }
}
