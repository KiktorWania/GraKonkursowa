using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public Tilemap map;

    [SerializeField]
    private GameObject mainRoom;
    [SerializeField]
    private GameObject bossRoom;
    [SerializeField]
    private List<GameObject> rooms;

    public int roomCount = 10; // Do not counts main room;

    public TileBase floorTile;

    private List<Vector3Int> doorPoss = new List<Vector3Int>();

    public int corridorLength = 10;
    void Start()
    {
        foreach(Vector3Int cellIndex in mainRoom.gameObject.GetComponentInChildren<Tilemap>().cellBounds.allPositionsWithin)
        {
            var tile = mainRoom.gameObject.GetComponentInChildren<Tilemap>().GetTile(cellIndex);
            if(tile != null)
            { 
                var tilePos = cellIndex;
                if (tile.ToString() == "doorTile (UnityEngine.Tilemaps.Tile)")
                {
                    doorPoss.Add(tilePos);
                }

                if (!map.HasTile(tilePos))
                {
                    map.SetTile(tilePos, tile);
                }
            }
        }
        foreach(Vector3Int doorPos in doorPoss)
        {
            drawCorridor(doorPos);
        }
    }
    void drawCorridor(Vector3Int startPos)    // 1 - UP, 2 - RIGHT, 3 - DOWN, 4 - LEFT
    {
        int direction = Direction(startPos);


        Vector3Int tilePos = new Vector3Int(0, 0, 0);

        for (int i = 0; i < corridorLength; i++)
        {
            switch (direction)
            {
                case 1:
                    tilePos = new Vector3Int(startPos.x, startPos.y + i, 0);
                    //opDirection = 3;
                    break;
                case 2:
                    tilePos = new Vector3Int(startPos.x + i, startPos.y, 0);
                    //opDirection = 4;
                    break;
                case 3:
                    tilePos = new Vector3Int(startPos.x, startPos.y - i, 0);
                    //opDirection = 1;
                    break;
                case 4:
                    tilePos = new Vector3Int(startPos.x - i, startPos.y, 0);
                    //opDirection = 2;
                    break;
                default:
                    map.SetTile(tilePos, floorTile);
                    break;
            }

            if (!map.HasTile(tilePos))
            {
                map.SetTile(tilePos, floorTile);
            }
        }

    }

    int Direction(Vector3Int tilePos)
    {
        Vector3Int basePos = tilePos;
        for (int i = 1; i <= 4; i++)
        {
            tilePos = basePos;
            switch (i)
            {
                case 1:
                    tilePos += Vector3Int.up;
                    break;
                case 2:
                    tilePos += Vector3Int.right;
                    break;
                case 3:
                    tilePos += Vector3Int.down;
                    break;
                case 4:
                    tilePos += Vector3Int.left;
                    break;
            }

            if (!map.HasTile(tilePos))
            {
                return i;
            }
        }
        return 0;
    }
}
