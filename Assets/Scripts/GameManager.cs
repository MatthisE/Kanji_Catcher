using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] PlayerStats[] playerStats;

    public bool gameMenuOpened, dialogBoxOpened; // considtions for player to stop moving

    // Start is called before the first frame update
    void Start()
    {
        //singelton pattern --> avoid duplicate GameManagers in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        playerStats = FindObjectsOfType<PlayerStats>(); // add all objects that have a PlayerStats script to playerStats Array
    }

    // Update is called once per frame
    void Update()
    {
        // check if Player should stay still
        if(gameMenuOpened || dialogBoxOpened)
        {
            Player.instance.deactivateMovement = true;
        }else
        {
            Player.instance.deactivateMovement = false;
        }
    }
}
