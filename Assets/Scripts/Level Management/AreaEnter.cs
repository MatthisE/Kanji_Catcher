using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnter : MonoBehaviour
{
    [SerializeField] string transitionAreaName;

    // Start is called before the first frame update
    void Start()
    {
        if(transitionAreaName == Player.instance.transitionAreaName)
        {
            Player.instance.transform.position = transform.position; //make player position the position of this area entry
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
