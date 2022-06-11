using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    Rigidbody rb;

    bool hasLanded;
    bool thrown;

    Vector3 initPosition;

    public int diceValue;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }

        if(rb.IsSleeping()&& !hasLanded && thrown)
        {
            hasLanded = true;
            rb.useGravity = false;
        }

        if (rb.IsSleeping() && !hasLanded && diceValue == 0)
        {
            RollAgain();
        }

    }

    void RollDice()
    {
        if(!thrown && !hasLanded)
        {
            thrown = true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
            diceValue = Random.Range(1, 6);
        }
        else if (thrown && hasLanded)
        {
            Reset();
        }
    }

    void Reset()
    {
        transform.position = initPosition;
        thrown = false;
        hasLanded = false;
        rb.useGravity = false;
    }

    public int RollAgain()
    {
        Reset();
        thrown = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        diceValue = Random.Range(1, 6);
        Debug.Log(diceValue);
        return diceValue;
    }

    void SideValueCheck()
    {
        diceValue = 0;
 
    }
}
