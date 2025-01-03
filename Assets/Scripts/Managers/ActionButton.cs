using UnityEngine;
using TMPro;

public class ActionButton : MonoBehaviour
{
    public static ActionButton instance;
    private TextMeshProUGUI actionText;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // assign this as the singleton instance

        actionText = GetComponentInChildren<TextMeshProUGUI>();
        SetActiveState(false);
    }

    // make all scripts be able to set action button's active state
    public void SetActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    // make all scripts be able to set action button's text
    public void SetActionText(string newText)
    {
        actionText.text = newText;
    }
}
