using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] Characters;

    public Character SelectRandom(Transform transform)
    {
        int idx = Random.Range(0, Characters.Length-1);
        var clone = Instantiate(Characters[idx],transform);
        return clone.GetComponent<Character>();
    }
}
