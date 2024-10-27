using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamController : MonoBehaviour
{
    private Player playerTarget; //is set dynamically by script
    private CinemachineVirtualCamera virtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = FindObjectOfType<Player>(); //dynamically find Player
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        virtualCamera.Follow = playerTarget.transform; //asign it to "Follow" of virtual camera
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
