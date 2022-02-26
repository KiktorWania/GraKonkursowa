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
    private List<GameObject> roomsPrefab;

    public int roomCount = 5; // Do not counts main room;
    Dictionary<int, Room> rooms;

    public TileBase floorTile, wallTile;
    

    private List<Vector3Int> doorPoss = new List<Vector3Int>();

    public int corridorLength = 10;

    private Vector3Int offset = new Vector3Int(0, 0, 0);
    /// /////////////////////

    Room room3;

    void Start()
    {
        rooms = new Dictionary<int, Room>();
        
        room3 = new Room(mainRoom, map);
        rooms.Add(0, room3);

        for (int i = 1; i < roomCount + 1; i++)
        {
            Room room = new Room(roomsPrefab[Random.Range(0,3)], map);
            rooms.Add(i, room);
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

    private void OnDrawGizmos()
    {
        foreach(var room in rooms)
        {
            var roomCenter = room.Value.getMapCenter();

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(roomCenter, new Vector3(1, 1, 1));
        }

        spanninTree();

       

        for (int i = 0; i < rooms.Values.Count; i++)
        {
            for (int x = 0; x < rooms[i].connectedIds.Count; x++) {
                Gizmos.DrawLine(rooms[i].getMapCenter(), rooms[rooms[i].connectedIds[x]].getMapCenter());
            }
        }

    }

    private void spanninTree()
    {
        /*
        float lenght = 100000;
        int roomId = 0;
        for(int i = 1; i < rooms.Values.Count; i++)
        {
            float dis = Vector3.Distance(rooms[0].getMapCenter(), rooms[i].getMapCenter());
            if(dis < lenght)
            {
                roomId = i;
                lenght = dis;
            }
        }
        rooms[0].conected = true;
        rooms[0].connectedIds.Add(roomId);
        */
        float lenght = 100000;
        int roomId = 0;

        for (int i = 0; i < rooms.Values.Count; i++)
        {
            

            for (int x = 1 + i; x < rooms.Values.Count; x++)
            {
                float dis = Vector3.Distance(rooms[i].getMapCenter(), rooms[x].getMapCenter());
                if (dis < lenght && rooms[x].getCon() == false)
                {
                    roomId = x;
                    lenght = dis;
                }
            }
            if(rooms[roomId] != null)
            {
                rooms[i].setConected(true);
                rooms[i].connectedIds.Add(roomId);
            }
        }

        
    }


}
