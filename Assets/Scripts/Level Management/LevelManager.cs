using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// given to level manager object, responsible for determining the edges of the maps
public class LevelManager : MonoBehaviour
{
    [SerializeField] Tilemap tilemap; // background of the scene, player should not be able to leave it
    private Vector3 bottomLeftEdge;
    private Vector3 topRightEdge;

    void Start()
    {
        //limit for player movement based on current tilemap
        bottomLeftEdge = tilemap.localBounds.min + new Vector3(0.5f, 0.75f, 0f); //+small offset
        topRightEdge = tilemap.localBounds.max + new Vector3(-0.5f, -0.75f, 0f); //+small offset

        Player.instance.SetLimit(bottomLeftEdge, topRightEdge);
    }
}
