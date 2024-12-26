using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to kanji list object
public class KanjiListManager : MonoBehaviour
{
    public static KanjiListManager instance;
    void Start()
    {
        //singelton pattern --> avoid duplicate Players in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
