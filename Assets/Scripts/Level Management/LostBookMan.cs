using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LostBookMan : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox; 

    // for adjusting NPC's position
    public enum Direction { Up, Down, Left, Right }

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer spriteRenderer;


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
            List<string> sentencesList = new List<string> { "#Visitor", "Some potted plant monsters tackled me and stole the books I was about to borrow!", "They should still be around here somewhere...", "If you help me get my three books back I will give you this weird floating symbol I found." };

            if(QuestManager.instance.questMarkersCompleted[2] == true)
            {
                sentencesList = new List<string> { "#Visitor", "Thank you for your help." };
            }
            else if(Inventory.instance.GetItemsList().Count == 4)
            {
                sentencesList = new List<string> { "#Visitor", "Some potted plant monsters tackled me and stole the books I was about to borrow!", "Wait what? You fought them and got my books back?!", "Amazing! Here have this floating thing I found as a reward." };
                DialogController.instance.ActivateQuestAtEnd("Return Stolen Books", true); // activate quest after dialog
                Inventory.instance.RemoveAllBooks();
            }

            sentences = sentencesList.ToArray();
            DialogController.instance.ActivateDialog(sentences, ""); // open box with first sentence
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
