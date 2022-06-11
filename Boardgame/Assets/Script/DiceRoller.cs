using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DiceValues = new int[4];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int[] DiceValues;
    public int DiceTotals;

    public void RollTheDice()
    {
        DiceTotals = 0;
        for (int i = 0; i < DiceValues.Length; i++)
        {
            DiceValues[i] = Random.Range(0, 2);
            DiceTotals += DiceValues[i];
        }
        Debug.Log("Rolled: " + DiceValues + " (" + DiceTotals + ")");
    }
}
