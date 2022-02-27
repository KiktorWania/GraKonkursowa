using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    public Tilemap map;
    [SerializeField]
    private GameObject mainRoomPrefab;
    [SerializeField]
    private List<GameObject> roomsPrefab;

    public int roomCount = 20; //Liczba pokoi bez głównego pokoju;
    private Dictionary<int, Room> rooms;
    [SerializeField]
    private TileBase floorTile, wallTile;
    
    /// /////////////////////

    private Room mainRoom;

    void Start()
    {
        rooms = new Dictionary<int, Room>();

        mainRoom = new Room(mainRoomPrefab, map);   //Stworznie głównego pokoju
        rooms.Add(0, mainRoom);                     //Dodanie go do słownika z indeksem zerowym

        for (int i = 1; i < roomCount + 1; i++)     //Dodanie reszty pokoi
        {
            Room room = new Room(roomsPrefab[Random.Range(0,3)], map);
            rooms.Add(i, room);
        }

        spanninTree();  //Połącznie pokoi za pomocą tego popsutego algorytmu

        for (int i = 0; i < rooms.Values.Count; i++)    //Wygenerowanie korytarzy
        {
            if (rooms[i].connectedIds != null)
            {
                for (int x = 0; x < rooms[i].connectedIds.Count; x++)
                {
                    drawCorridor(rooms[i].getMapCenter(), rooms[rooms[i].connectedIds[x]].getMapCenter());
                }
            }


        }

        fillOutMap(wallTile);  //wypełnienie reszty mapy
    }

    public void fillOutMap(TileBase fillTile)
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

    private void spanninTree()
    {

        for (int i = 0; i < rooms.Values.Count; i++)
        {
            float lenght = float.MaxValue;
            float maxLenght = 50f;
            int roomId = 0;


            for (int x = 1 + i; x < rooms.Values.Count; x++)    //szukanie najkrótszej drogi z pokoju o indeksie i do pokoju o indeksie x 
            {
                
                float dis = Vector3.Distance(rooms[i].getMapCenter(), rooms[x].getMapCenter()); 
                if (dis < lenght )
                {
                    roomId = x;
                    lenght = dis;
                }
            }
            
                
            if(roomId != 0)
            {
                rooms[i].setConected(true);
                rooms[roomId].setConected(true);
                rooms[i].connectedIds.Add(roomId);
            }
            
        }
    }

    public void drawCorridor(Vector3 a, Vector3 b)  
    {
        //w duzym skrócie, jezeli pokoje so wystarczajaca blisko siebie to rysuje to prosta linie miedzy nimi,
        //jesli nie to rysuje korytarz w ksztalcie litery L
  
        int zasieg = 5; //zakres ktory pokoje musza przekroczyc aby narysowac korzytarz w ksztalcie L

        Vector3 ab = (a + b) / 2;
        
        float distanceX = Mathf.Abs(a.x - b.x); //dystans miedzy pokojami w osi X
        float distanceY = Mathf.Abs(a.y - b.y); //dystans miedzy pokojami w osi Y

        //Reszta magii, nie chce mi sie tego komentowac ¯\_(ツ)_/¯

        if(distanceX > zasieg || distanceY > zasieg)
        {
            Vector3Int startPos = new Vector3Int((int)a.x, (int)a.y, 0);
            Vector3Int curPos = startPos;
            if (a.x - b.x <= 0)
            {
               
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
        else if (distanceX <= zasieg)
        {
            Vector3Int startPos = new Vector3Int((int)ab.x, (int)a.y, 0);
            Vector3Int curPos = startPos;
            while (curPos.y < b.y)
            {
                map.SetTile(curPos, floorTile);
                curPos.y++;
            }

        }

        else if (distanceY <= zasieg)
        {
            Vector3Int startPos = new Vector3Int((int)a.x, (int)ab.y, 0);
            Vector3Int curPos = startPos;
            if (a.x - b.x <= 0)
            {

                while (curPos.x < b.x)
                {
                    map.SetTile(curPos, floorTile);
                    curPos.x++;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        bool DebugMode = true;
        //Gizmos do wyswietlania polaczen miedzy pokojami

        if (Application.isPlaying && DebugMode)
        {
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

}
