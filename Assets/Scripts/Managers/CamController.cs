using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// given to virtual camere object
public class CamController : MonoBehaviour
{
    private Player playerTarget; // is set dynamically by script
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField] int musicToPlay;
    private bool musicAlreadyPlaying;

    [SerializeField] private float smoothSpeed = 0.125f; // smooth transition speed between scenes
    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        playerTarget = FindObjectOfType<Player>(); // dynamically find Player
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        virtualCamera.Follow = playerTarget.transform; // asign player position to "Follow" of virtual camera
    }

    void Update()
    {
        // activate background music of scene
        if(!musicAlreadyPlaying)
        {
            musicAlreadyPlaying = true;
            AudioManager.instance.PlayBackgroundMusic(musicToPlay);
        }

        // make camera also follow player in new area
        while(playerTarget == null)
        {
            playerTarget = FindObjectOfType<Player>();
            if(virtualCamera )
            {
                // set the position of the camera's GameObject directly
                GameObject cameraGameObject = virtualCamera.gameObject;
                cameraGameObject.transform.position = playerTarget.transform.position;

                virtualCamera.Follow = playerTarget.transform;
            }
        }

        // smoothly transition camera's position to the player's position when entering new scene
        Vector3 desiredPosition = playerTarget.transform.position;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
