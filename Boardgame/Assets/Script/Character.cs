using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    public Animator anim;
    public enum AnimState
    {
        IDLE,
        WALK,
        ATTACK,
        DIE,
        HIT
    }

    public float dmg = 10f;
    public float hp = 100f;
    private AnimState state = AnimState.IDLE;

    //These are destination
    private int cid;
    public Vector3 pos { get; private set; } = Vector3.zero;
    public Vector3 dir { get; private set; } = Vector3.zero;

    public float speed = 3.5f;

    public void Init(int cid, int tid, int eid, int owner)
    {
        this.cid = cid;
        Init(tid, eid, owner);
    }

    public override void OnAwake()
    {

    }

    public override void OnStart()
    {
        transform.position = pos;
        transform.forward = dir;
    }

    public override void OnUpdate()
    {
        if (!Owned)
        {
            var dis = pos - transform.position;
            if (dis.magnitude > 1)
            {
                transform.position += dis.normalized * speed * Time.deltaTime;
            }
            else
            {
                transform.position = pos;
                transform.forward = dir;
            }
        }
    }

    public static new Character CreateInstance(int idx)
    {
        var clone = GameManager.Instance.charaSelector.SelectCharacter(GameManager.Instance.transform,idx);//non local players are stored under GameManger
        return clone.GetComponent<Character>();
    }

    public override void Serialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Serialize(h);

        if (Owned)
        {
            h[(byte)'c'] = cid;
            h[(byte)'p'] = new float[3] { pos.x, pos.y, pos.z };
            h[(byte)'d'] = new float[3] { dir.x, dir.y, dir.z };
            h[(byte)'a'] = (byte)state;
        }
    }

    public override void Deserialize(ExitGames.Client.Photon.Hashtable h)
    {
        base.Deserialize(h);
        
        if (h.TryGetValue((byte)'c', out object cid))
        {
            this.cid = (int)cid;
        }

        if (h.TryGetValue((byte)'p', out object apos))
        {
            var arr = (float[])apos;
            pos = new Vector3(arr[0], arr[1], arr[2]);
        }

        if (h.TryGetValue((byte)'d', out object adir))
        {
            var arr = (float[])adir;
            dir = new Vector3(arr[0], arr[1], arr[2]);
        }

        if (h.TryGetValue((byte)'a', out object state))
        {
            if(this.state != (AnimState)(byte)state)
            {
                this.state = (AnimState)(byte)state;
                Animate(this.state);
            }
        }

        Debug.LogFormat("Received: Character:[{0}] Position:[{1}] Direction:[{2}] Animation:[{3}]", GameManager.Instance.charaSelector.Characters[this.cid].name, pos, dir, this.state);
    }

    public void SetTransform(Vector3 pos, Vector3 frwd)
    {
        this.pos = pos;
        dir = frwd;
        Debug.LogFormat("SetTransform: pos: {0}  dir: {1}", pos, dir);
    }

    public void Animate(AnimState state)
    {
        switch(state)
        {
            case AnimState.IDLE:
                this.state = AnimState.IDLE;
                anim.SetBool("Walk", false);
                break;
            case AnimState.WALK:
                this.state = AnimState.WALK;
                anim.SetBool("Walk", true);
                break;
            case AnimState.ATTACK:
                this.state = AnimState.ATTACK;
                anim.SetTrigger("Attack");
                break;
            case AnimState.DIE:
                this.state = AnimState.DIE;
                anim.SetTrigger("Dead");
                break;
            case AnimState.HIT:
                this.state = AnimState.HIT;
                anim.SetTrigger("Hit");
                break;
        }
    }

    public void DealDamage(int damage)
    {
        hp -= damage / 10.0f;
        if(hp < 0f)
        {
            Animate(AnimState.DIE);

        }
        Animate(AnimState.HIT);
    }
}
