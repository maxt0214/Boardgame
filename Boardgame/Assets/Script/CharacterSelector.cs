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
        clone.SetActive(true);
        clone.name = Characters[idx].name;

        var chara = clone.GetComponent<Character>();
        chara.Init(idx,0,0,0);

        Debug.LogFormat("Character[{0}] has been selected under:{1}!",clone.name,transform.name);
        return chara;
    }

    public Character SelectCharacter(Transform transform, int idx)
    {
        var clone = Instantiate(Characters[idx], transform);
        clone.SetActive(true);
        clone.name = Characters[idx].name;

        var chara = clone.GetComponent<Character>();
        chara.Init(idx, 0, 0, 0);

        Debug.LogFormat("Character[{0}] has been selected under:{1}!", clone.name, transform.name);
        return chara;
    }
}
