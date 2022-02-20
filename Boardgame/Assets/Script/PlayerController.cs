using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    [HideInInspector]
    public Character character;

    private float attackTimer = 1f;
    private float timeToAttack = 0f;

    private int playerIdx = 0;
    private bool TurnStarted = false;
    private bool HasMoved = false;
    private bool MyTurn = false;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.gameStarted)
            GameManager.Instance.Connect();

        if(PhotonNetwork.IsMasterClient && !TurnStarted && GameManager.Instance.gameStarted)
        {
            TurnStarted = true;
            NotifyNext();
        }

        if(Input.GetKeyDown(KeyCode.R) && MyTurn)
        {
            if(!HasMoved)
            {
                HasMoved = true;
                character.mid = GameManager.Instance.Map.ValidateIndex(character.mid + GameManager.Instance.RollDice()/2);
                var dest = GameManager.Instance.Map.GetWayPoint(character.mid);
                MoveTo(dest);
            } else
            {
                HasMoved = false;
                Attack(character.mid, GameManager.Instance.RollDice() * 10);
                MyTurn = false;
                //This client done
                if (PhotonNetwork.IsMasterClient)
                {
                    NotifyNext();
                } else
                {
                    //Send back to host when done
                    NetworkManager.Instance.SendNetEvent(PhotonNetwork.LocalPlayer.ActorNumber,NetworkConstant.finishedTurn);
                }
            }
        }

        timeToAttack += Time.deltaTime;
    }

    private void Attack(int index, int damage)
    {
        if (attackTimer <= timeToAttack)
        {
            timeToAttack = 0f;
            character.Animate(Character.AnimState.ATTACK);
            Hit(index, damage);
        }
    }

    private void LateUpdate()
    {
        //When agent stopped, set character back to idle
        CheckReachedDest();

        character.SetTransform(transform.position,transform.forward);
    }

    private bool startedNavi = false;
    public void MoveTo(Transform dest)
    {
        character.Animate(Character.AnimState.WALK);
        agent.SetDestination(dest.position);
        startedNavi = true;
    }

    private void CheckReachedDest()
    {
        if (!startedNavi) return;
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    character.Animate(Character.AnimState.IDLE);
                    startedNavi = false;
                }
            }
        }
    }

    private void Hit(int mid, int dmg)
    {
        EntityManager.Instance.HitCharacters(mid, dmg, character.eid);
    }
    //only called by the host
    public void NotifyNext()
    {
        if(playerIdx >= GameManager.Instance.PlayerIds.Count)
        {
            playerIdx = 0;
            TurnStarted = false;
            return;
        }

        var aid = GameManager.Instance.PlayerIds[playerIdx++];
        if (PhotonNetwork.LocalPlayer.ActorNumber == aid)
            MyTurn = true;
        else
            NetworkManager.Instance.SendNetEvent(aid,NetworkConstant.startTurn,true);
    }
    //Called when received this client get to start
    public void StartTurn(int aid)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == aid)
            MyTurn = true;
    }

    public void ClientFinishedTurn(int aid)
    {
        if(playerIdx < GameManager.Instance.PlayerIds.Count)
            NotifyNext();
        else
        {
            playerIdx = 0;
            TurnStarted = false;
        }
    }
}
