using UnityEngine;

// given to area entries
public class AreaEnter : MonoBehaviour
{
    [SerializeField] private string transitionAreaName;

    private void Start()
    {
        if (transitionAreaName == Player.instance.transitionAreaName) // if player is supposed to come out of this entry
        {
            GameManager.instance.goThroughExit = false; // make player be able to move again
            Player.instance.transform.position = transform.position; // make player position the position of this area entry
        }

    }
}