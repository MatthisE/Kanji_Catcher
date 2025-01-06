using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// given to NPC
public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; 

    [SerializeField] bool shouldActivateQuest;
    [SerializeField] string questToMark;
    [SerializeField] bool markAsComplete;

    // for adjusting NPC's position
    public enum Direction { Up, Down, Left, Right }

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // box can only be activated when player is inside object's trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = true;
            ActionButton.instance.SetActiveState(true);
            ActionButton.instance.SetActionText("Talk");

            ActionButton.instance.GetComponent<Button>().onClick.RemoveAllListeners();
            ActionButton.instance.GetComponent<Button>().onClick.AddListener(OpenDialogBox);
            ActionButton.instance.GetComponent<Button>().onClick.AddListener(FacePlayer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canActivateBox = false;
            if (ActionButton.instance != null)
            {
                ActionButton.instance.SetActiveState(false);
                ActionButton.instance.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    private void OpenDialogBox()
    {
        ActionButton.instance.SetActiveState(false);

        if(canActivateBox && !DialogController.instance.IsDialogBoxActive() && GameManager.instance.gameMenuOpened != true) // only call if the box is not already active and menu is not open
        {
            DialogController.instance.ActivateDialog(sentences, ""); // open box with first sentence

            if(shouldActivateQuest)
            {
                DialogController.instance.ActivateQuestAtEnd(questToMark, markAsComplete); // activate quest after dialog
            }
        }
    }
    
    // make the NPC face the player
    public void FacePlayer()
    {
        Transform player = GameObject.Find("Player").transform;
        Vector2 playerPosition = player.position;
        Vector2 npcPosition = transform.position;
        Vector2 difference = playerPosition - npcPosition;

        Direction facingDirection;

        // determine the facing direction based on the player's position
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            if (difference.x > 0)
                facingDirection = Direction.Right;
            else
                facingDirection = Direction.Left;
        }
        else
        {
            if (difference.y > 0)
                facingDirection = Direction.Up;
            else
                facingDirection = Direction.Down;
        }

        UpdateSprite(facingDirection);
    }

    // update the NPC's sprite based on the calculated direction
    private void UpdateSprite(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                spriteRenderer.sprite = upSprite;
                break;
            case Direction.Down:
                spriteRenderer.sprite = downSprite;
                break;
            case Direction.Left:
                spriteRenderer.sprite = leftSprite;
                break;
            case Direction.Right:
                spriteRenderer.sprite = rightSprite;
                break;
        }
    }
}
