using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    public Dictionary<Vector3Int, int> doors;
    void Start()
    {
        foreach (Vector3Int cellIndex in gameObject.GetComponentInChildren<Tilemap>().cellBounds.allPositionsWithin)
        {
            var tile = gameObject.GetComponentInChildren<Tilemap>().GetTile(cellIndex);
            if (tile != null)
            {
                if (tile.ToString() == "doorTile (UnityEngine.Tilemaps.Tile)")
                {
                    Debug.Log("yes");   
                } 
            }
        }
    }

   
    void Update()
    {
        
    }
}
