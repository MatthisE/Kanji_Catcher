using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to area entries
public class AreaEnter : MonoBehaviour
{
    [SerializeField] string transitionAreaName;

    void Start()
    {
        if(transitionAreaName == Player.instance.transitionAreaName) // if player is supposed to come out of this entry
        {
            Player.instance.transform.position = transform.position; //make player position the position of this area entry
        }
        
    }
}
