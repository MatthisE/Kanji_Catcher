using UnityEngine;
using UnityEngine.UI;

// given to NPC
public class DialogHandler : MonoBehaviour
{
    public string[] sentences;
    private bool canActivateBox;

    [SerializeField] private bool shouldActivateQuest;
    [SerializeField] private string questToMark;
    [SerializeField] private bool markAsComplete;

    // for adjusting NPC's position
    public enum Direction { Up, Down, Left, Right }

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // box can only be activated when player is inside object's trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
        if (collision.CompareTag("Player"))
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

        if (canActivateBox && !DialogController.instance.IsDialogBoxActive() && !GameManager.instance.gameMenuOpened) // only call if the box is not already active and menu is not open
        {
            DialogController.instance.ActivateDialog(sentences, ""); // open box with first sentence

            if (shouldActivateQuest)
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
            facingDirection = difference.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            facingDirection = difference.y > 0 ? Direction.Up : Direction.Down;
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
            default:
                break;
        }
    }
}