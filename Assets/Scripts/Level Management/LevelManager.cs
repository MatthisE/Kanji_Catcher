using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// responsible for determining the edges of our level
public class LevelManager : MonoBehaviour
{
    [SerializeField] Tilemap tilemap; //the background one the should not leave
    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    // Start is called before the first frame update
    void Start()
    {
        //limits for player movement based on current tilemap
        bottomLeftEdge = tilemap.localBounds.min + new Vector3(0.5f, 0.75f, 0f); //+small offset
        topRightEdge = tilemap.localBounds.max + new Vector3(-0.5f, -0.75f, 0f); //+small offset

        Player.instance.SetLimit(bottomLeftEdge, topRightEdge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
