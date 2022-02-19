using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    [HideInInspector]
    public Character character;

    private float rayTimer = 2f;
    private float timeToCast = 0;

    private float attackTimer = 1f;
    private float timeToAttack = 0f;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            GameManager.Instance.Connect();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(attackTimer <= timeToAttack)
            {
                timeToAttack = 0f;
                character.Animate(Character.AnimState.ATTACK);
                Hit();
            }
        }
        timeToAttack += Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W))
            FaceTo(0f);
        if (Input.GetKeyDown(KeyCode.D))
            FaceTo(90f);
        if (Input.GetKeyDown(KeyCode.S))
            FaceTo(180f);
        if (Input.GetKeyDown(KeyCode.A))
            FaceTo(270f);

        //move player to where mouse is clicked
        if (Input.GetButton("Fire1")) 
            CastRay();
        //When agent stopped, set character back to idle
        CheckReachedDest();

        character.SetTransform(transform.position,transform.forward);

        timeToCast += Time.deltaTime;
    }

    private bool startedNavi = false;
    private void CastRay()
    {
        if (rayTimer > timeToCast) return;
        timeToCast = 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.LogFormat("Ray Hit:[{0}]", hit.transform.name);
            if (hit.transform.tag == "Walkable")
            {
                character.Animate(Character.AnimState.WALK);
                agent.SetDestination(hit.transform.position);
                startedNavi = true;
            }
        }
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

    private void FaceTo(float yAngle)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, yAngle, transform.eulerAngles.z);
    }

    private void Hit()
    {
        Vector3 center;
        if(Mathf.Abs(transform.eulerAngles.y - 0f) < 1f)
        {
            center = transform.position + new Vector3(0, 0, 5f);
            Debug.LogFormat("Player[{0}] at{1} attacks {2}", character.name, transform.position, center);
            EntityManager.Instance.HitCharacters(center,character.dmg, character.eid);
        } else if(Mathf.Abs(transform.eulerAngles.y - 90f) < 1f)
        {
            center = transform.position + new Vector3(5f, 0, 0);
            Debug.LogFormat("Player[{0}] at{1} attacks {2}", character.name, transform.position, center);
            EntityManager.Instance.HitCharacters(center, character.dmg, character.eid);
        } else if(Mathf.Abs(transform.eulerAngles.y - 180f) < 1f)
        {
            center = transform.position + new Vector3(0, 0, -5);
            Debug.LogFormat("Player[{0}] at{1} attacks {2}", character.name, transform.position, center);
            EntityManager.Instance.HitCharacters(center, character.dmg, character.eid);
        } else if(Mathf.Abs(transform.eulerAngles.y - 270f) < 1f)
        {
            center = transform.position + new Vector3(-5f, 0, 0);
            Debug.LogFormat("Player[{0}] at{1} attacks {2}", character.name, transform.position, center);
            EntityManager.Instance.HitCharacters(center, character.dmg, character.eid);
        }
    }
}
