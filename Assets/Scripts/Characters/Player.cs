using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// given to player object, handels its movement
public class Player : MonoBehaviour
{
    public static Player instance; //constant across all projects, makes player functions and vars usable in other scripts

    [SerializeField] int moveSpeed = 1; // used for rigid body velocity
    [SerializeField] Rigidbody2D playerRigidBody;
    [SerializeField] Animator playerAnimator;

    public string transitionAreaName; // given by area exits to give player correct new position (at area entries)

    // limits of tileset scene that player cannot exit
    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    public bool deactivateMovement = false;

    void Start()
    {
        // singelton pattern --> avoid duplicate players in new scenes
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject); // destroy duplicate
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject); //gameObject = player --> does not get destroyed when entering new scene 

         // initialize the player's default look direction to "down"
        playerAnimator.SetFloat("lastX", 0);
        playerAnimator.SetFloat("lastY", -1);
    }

    void Update()
    {
        /*
        float horizontalMovement = Input.GetAxisRaw("Horizontal"); // up, down, W, S keys
        float verticalMovement = Input.GetAxisRaw("Vertical"); // left, right, A, D keys
        */

        float horizontalMovement = Joystick.instance.Horizontal;
        float verticalMovement = Joystick.instance.Vertical;

        if(deactivateMovement) // make player stand still (for example by NPC dialog)
        {
            playerRigidBody.velocity = Vector2.zero; // turn all values to 0
        }
        else
        {
            // Player moves depending on keys pressed an move speed
            playerRigidBody.velocity = new Vector2(horizontalMovement, verticalMovement) * moveSpeed;
        }

        // set values of player Animator depending on object's movement
        playerAnimator.SetFloat("movementX", playerRigidBody.velocity.x);
        playerAnimator.SetFloat("movementY", playerRigidBody.velocity.y);

        // set Animator values of last X and Y movements to make player look in that direction when he stops moving
        if(horizontalMovement != 0 || horizontalMovement != 0 || verticalMovement != 0 || verticalMovement != 0)
        {
            if(!deactivateMovement) // player should not be able to look around when still
            {
                playerAnimator.SetFloat("lastX", horizontalMovement);
                playerAnimator.SetFloat("lastY", verticalMovement);
            }
        }

        // make sure player cannot leave scene
        transform.position = new Vector3(
            // keep player inside min-max position values based on tileset 
            Mathf.Clamp(transform.position.x, bottomLeftEdge.x, topRightEdge.x),
            Mathf.Clamp(transform.position.y, bottomLeftEdge.y, topRightEdge.y),
            Mathf.Clamp(transform.position.z, bottomLeftEdge.z, topRightEdge.z)
        );
    }

    // method to accept the bottomLeftEdge and topRightEdge (called in LevelManager)
    public void SetLimit(Vector3 bottomEdgeToSet, Vector3 topEdgeToSet)
    {
        bottomLeftEdge = bottomEdgeToSet;
        topRightEdge = topEdgeToSet;
    }
}
