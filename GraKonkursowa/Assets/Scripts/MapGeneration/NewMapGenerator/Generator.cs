using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
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

        //fillOutMap(map, wallTile);
        spanninTree();

        for (int i = 0; i < rooms.Values.Count; i++)
        {
            if (rooms[i].connectedIds != null)
            {
                for (int x = 0; x < rooms[i].connectedIds.Count; x++)
                {
                    drawCorridor(rooms[i].getMapCenter(), rooms[rooms[i].connectedIds[x]].getMapCenter());
                }
            }


        }

        fillOutMap(map, wallTile);
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
        if (Application.isPlaying){
            foreach (var room in rooms)
            {
                var roomCenter = room.Value.getMapCenter();

                Gizmos.color = Color.cyan;
                //Gizmos.DrawCube(roomCenter, new Vector3(1, 1, 1));
                Handles.Label(roomCenter, room.Key.ToString());
            }

            



            for (int i = 0; i < rooms.Values.Count; i++)
            {
                if (rooms[i].connectedIds != null)
                {
                    for (int x = 0; x < rooms[i].connectedIds.Count; x++)
                    {
                        Gizmos.DrawLine(rooms[i].getMapCenter(), rooms[rooms[i].connectedIds[x]].getMapCenter());
                    }
                }


            }
        }

    }

    private void spanninTree()
    {

        for (int i = 0; i < rooms.Values.Count; i++)
        {
            float lenght = float.MaxValue;
            int roomId = 0;


            for (int x = 1 + i; x < rooms.Values.Count; x++)
            {
               

                if (rooms[x].getCon() == true)
                {
                    continue;
                }

                float dis = Vector3.Distance(rooms[i].getMapCenter(), rooms[x].getMapCenter());
                if (dis < lenght)
                {
                    roomId = x;
                    lenght = dis;
                }
            }
            
                
                rooms[i].setConected(true);
                rooms[roomId].setConected(true);
                rooms[i].connectedIds.Add(roomId);
            
        }
    }

    public void drawCorridor(Vector3 a, Vector3 b)
    {
        int zasieg = 3;

        Vector3 ab = (a + b) / 2;
        
        float distanceX = Mathf.Abs(a.x - b.x);
        float distanceY = Mathf.Abs(a.y - b.y);
        float test = Mathf.Abs(-4 - -4);

        Debug.Log(distanceX);
        Debug.Log(distanceY);
        Debug.Log(test);

        if (distanceX <= zasieg)
        {
            Vector3Int startPos = new Vector3Int((int)ab.x, (int)a.y, 0);
            Vector3Int curPos = startPos;
            while (curPos.y < b.y)
            {
                map.SetTile(curPos, floorTile);
                curPos.y++;
            }

        }

        if (distanceY <= zasieg )
        {
            Vector3Int startPos = new Vector3Int((int)a.x, (int)ab.y, 0);
            Vector3Int curPos = startPos;
            if (a.x - b.x <= 0)
            {
                //toRight
                while (curPos.x < b.x)
                {
                    map.SetTile(curPos, floorTile);
                    curPos.x++;
                }
            }
        }

        if(distanceX > zasieg || distanceY > zasieg)
        {
            Vector3Int startPos = new Vector3Int((int)a.x, (int)a.y, 0);
            Vector3Int curPos = startPos;
            if (a.x - b.x <= 0)
            {
                //toRight
                while (curPos.x < b.x)
                {
                    map.SetTile(curPos, floorTile);
                    curPos.x++;
                }
            }
            else
            {
                while (curPos.x > b.x)
                {
                    map.SetTile(curPos, floorTile);
                    curPos.x--;
                }
            }

            if (a.y - b.y <= 0)
            {
                //toRight
                while (curPos.y < b.y)
                {
                    map.SetTile(curPos, floorTile);
                    curPos.y++;
                }
            }
            else
            {
                while (curPos.y > b.y)
                {
                    map.SetTile(curPos, floorTile);
                    curPos.y--;
                }
            }
        }
    }

}
