using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

// given to virtual camere object
public class CamController : MonoBehaviour
{
    private Player playerTarget; // is set dynamically by script
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField] int musicToPlay;
    private bool musicAlreadyPlaying;

    void Start()
    {
        playerTarget = FindObjectOfType<Player>(); // dynamically find Player
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        //transform.position = playerTarget.transform.position;
        virtualCamera.Follow = playerTarget.transform; // asign player position to "Follow" of virtual camera
    }

    void Update()
    {
        // activate background music of scene
        if(!musicAlreadyPlaying)
        {
            musicAlreadyPlaying = true; // only on load of new scene
            if(musicToPlay != AudioManager.instance.prevMusic){ // only if different from previous music
                AudioManager.instance.PlayBackgroundMusic(musicToPlay);
            }
            AudioManager.instance.prevMusic = musicToPlay; // set prevMusic
        }

        // make camera also follow player in new area
        while(playerTarget == null)
        {
            playerTarget = FindObjectOfType<Player>();
            if(virtualCamera )
            {
                virtualCamera.Follow = playerTarget.transform;
            }
        }
    }
}