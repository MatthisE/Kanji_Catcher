using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public static MenuButton instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this; // assign this as the singleton instance
        }
    }

    // make all scripts be able to set menu button's active state
    public void SetActiveState(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
