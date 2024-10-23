using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance; //constant across all projects, makes Player functions and vars usable in other scripts
    [SerializeField] int moveSpeed = 1;
    [SerializeField] Rigidbody2D playerRigidBody;
    [SerializeField] Animator playerAnimator;

    public string transitionAreaName; //given by area exits to give Player correct new position (by area entries)

    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    public bool deactivateMovement = false;

    // Start is called before the first frame update
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
        DontDestroyOnLoad(gameObject); //gameObject == Player --> does not get destroyed when entering new scene 
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        if(deactivateMovement) // make Player stand still (for example by NPC dialog)
        {
            playerRigidBody.velocity = Vector2.zero;
        }
        else
        {
            playerRigidBody.velocity = new Vector2(horizontalMovement, verticalMovement) * moveSpeed;
        }

        playerAnimator.SetFloat("movementX", playerRigidBody.velocity.x);
        playerAnimator.SetFloat("movementY", playerRigidBody.velocity.y);

        if(horizontalMovement == 1 || horizontalMovement == -1 || verticalMovement == 1 || verticalMovement == -1)
        {
            if(!deactivateMovement) // Player should not be able to look around when still
            {
                playerAnimator.SetFloat("lastX", horizontalMovement);
                playerAnimator.SetFloat("lastY", verticalMovement);
            }
        }

        //make sure player cannot leave scene
        transform.position = new Vector3(
            // keep player inside min-max position values based on tileset 
            Mathf.Clamp(transform.position.x, bottomLeftEdge.x, topRightEdge.x),
            Mathf.Clamp(transform.position.y, bottomLeftEdge.y, topRightEdge.y),
            Mathf.Clamp(transform.position.z, bottomLeftEdge.z, topRightEdge.z)
        );
    }

    // Method to accept the bottomLeftEdge and topRightEdge (called in LevelManager)
    public void SetLimit(Vector3 bottomEdgeToSet, Vector3 topEdgeToSet)
    {
        bottomLeftEdge = bottomEdgeToSet;
        topRightEdge = topEdgeToSet;
    }
}
