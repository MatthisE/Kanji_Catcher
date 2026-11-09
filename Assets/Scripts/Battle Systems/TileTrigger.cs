using UnityEngine;

public class TileTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player stepped on the tilemap!");
            // Insert your logic here
        }
    }
}

