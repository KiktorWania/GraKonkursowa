using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public static class Room
{
    

    public static void generateRoom(Tilemap map, GameObject room, Vector3Int ofset)
    {
        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();
        var tilemap = room.GetComponentInChildren<Tilemap>();

        foreach (var cellIndex in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(cellIndex);
            var tilepos = ofset + cellIndex;
            if (!map.HasTile(tilepos))
            {
                tiles.Add(tilepos, tile);
            }
            else if (map.HasTile(tilepos))
            {
                tiles.Clear();
                generateRoom(map, room, ofset + new Vector3Int(Random.Range(-10, 10), Random.Range(-10, 10), 0));
                break;
            }
        }

        foreach(var tile in tiles)
        {
            map.SetTile(tile.Key, tile.Value);
        }
    }

    public static void fillOutMap(Tilemap map, TileBase fillTile)
    {
        foreach (var cellIndex in map.cellBounds.allPositionsWithin)
        {
            var tilepos = cellIndex;
            if (!map.HasTile(tilepos))
            {
                map.SetTile(cellIndex, fillTile);
            }
        }
    }

    
}
