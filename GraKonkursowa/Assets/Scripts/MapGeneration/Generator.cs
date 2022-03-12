
//code written by Wiktor Kania

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine;
using System;
using DelaunatorSharp.Unity.Extensions;
using DelaunatorSharp;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    bool debug = false;

    [SerializeField]
    public Tilemap map;
    [SerializeField]
    private GameObject mainRoomPrefab;
    [SerializeField]
    private List<GameObject> roomsPrefab;
    [SerializeField]
    private int roomSpaceing = 10;  //wartosc okreslajaca maksymalne odstepy miedzy pokojami, w tym przypadku od -10 do 10
    [SerializeField]
    public int roomCount = 20;      //Liczba pokoi bez głównego pokoju;

    
    private Room[] rooms;

    [SerializeField]
    private TileBase floorTile, wallTile, doorTile;

    /// /////////////////////

    Delaunator delaunator;  //Algorytm triangulacji
    IPoint[] points;        
    List<Edge> edges = new List<Edge>();

    List<Vector3Int> doorTiles = new List<Vector3Int>();

    /// /////////////////////

    Edge[] connectedRoomids;

    private Room mainRoom;
    Vector2[] roomCenters;
    void Start()
    {
        rooms = new Room[roomCount + 1];
        roomCenters = new Vector2[roomCount + 1];

        mainRoom = new Room(mainRoomPrefab, map, 10, 0);                                //Stworzenie głównego pokoju
        mainRoom.setId(0);

        roomCenters[0] = new Vector2(mainRoom.getMapCenter().x, mainRoom.getMapCenter().y);
        rooms[0] = mainRoom;                                                         //Dodanie go do słownika z indeksem zerowym

        for (int i = 1; i < roomCount + 1; i++)                                         //Dodanie reszty pokoi
        {
            Room room = new Room(roomsPrefab[Random.Range(0, 3)], map, roomSpaceing, i);
            room.setId(i);
            roomCenters[i] = new Vector2(room.getMapCenter().x, room.getMapCenter().y);
            rooms[i] = room;
        }

        roomConnector();        //połączenie pokoi
        fillOutMap(wallTile);   //wypełnienie reszty mapy
        drawDoors();            //wygenerowanie odpowiednich drzwi
        generatePrefabs();      //wygenerowanie prefabow z pokoi
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            debug = !debug;
        }
    }
    void generatePrefabs()
    {
        foreach (var room in rooms)
        {
            var prefab = room.getRoomPrefab();
            var newRoom = Instantiate(room.getRoomPrefab(), room.getOfset(), Quaternion.identity);
            newRoom.GetComponentInChildren<TilemapRenderer>().enabled = false;
        }
    }
    void roomConnector()
    {
        points = roomCenters.ToPoints();
        delaunator = new Delaunator(points);    //triangulacja z centrami pokoi jako punktami

        connectedRoomids = new Edge[rooms.Length];
        createEdges();

        foreach(var edge in connectedRoomids)
        {
            drawCorridor(rooms[edge.from], rooms[edge.to]);

        }
        
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

    public void drawCorridor(Room fromRoom, Room toRoom)
    {
        List<Vector3Int> tilePos = new List<Vector3Int>();

        //w duzym skrócie, jezeli pokoje so wystarczajaca blisko siebie to rysuje to prosta linie miedzy nimi,
        //jesli nie to rysuje korytarz w ksztalcie litery L
        Vector3Int v1 = fromRoom.getMapCenterInt();
        Vector3Int v2 = toRoom.getMapCenterInt();
        Vector3Int midPoint = Vector3ToInt(new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, 0));

        bool inXRange = false, inYRange = false;
        if(midPoint.x > fromRoom.roomXBounds.x && midPoint.x < fromRoom.roomXBounds.y && midPoint.x > toRoom.roomXBounds.x && midPoint.x < toRoom.roomXBounds.y)
        {
            inXRange = true;
        }
        if (midPoint.y > fromRoom.roomYBounds.x && midPoint.y < fromRoom.roomYBounds.y && midPoint.y > toRoom.roomYBounds.x && midPoint.y < toRoom.roomYBounds.y)
        {
            inYRange = true;
        }

        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

        if (inXRange)
        {

            //Rysuje pionowy korytarz
            Vector3Int curPos = new Vector3Int(midPoint.x, v1.y, 0);

            while(curPos.y != v2.y)
            {
                if(curPos.y < v2.y)
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.up;
                }
                else if(curPos.y > v2.y)
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.down;
                }
            }
        }
        if (inYRange)
        {
            //Rysuje poziomy korytarz
            Vector3Int curPos = new Vector3Int(v1.x, midPoint.y, 0);
            while (curPos.x != v2.x)
            {
                if (curPos.x < v2.x)
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.right;
                }
                else if (curPos.x > v2.x)
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.left;
                }
            }
        }

        if(!inYRange && !inXRange)
        {
            Vector3Int curPos = v1;
            while(curPos.x != v2.x)
            {
                if(curPos.x < v2.x)
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.right;
                }else
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.left;
                }
            }

            while(curPos.y != v2.y)
            {
                if(curPos.y < v2.y)
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.up;
                }
                else
                {
                    if (map.GetTile(curPos) == wallTile)
                    {
                        doorTiles.Add(curPos);
                    }
                    map.SetTile(curPos, floorTile);
                    tilePos.Add(curPos);
                    curPos += Vector3Int.down;
                }
            }
        }

        foreach(var pos in tilePos)
        {
            map.SetTransformMatrix(pos, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)));
        }
    }

    public void drawDoors()
    {
        List<Vector3Int> newPos = new List<Vector3Int>();
        foreach (var pos in doorTiles)
        {
            if (checkForDoorTiles(pos))
            {
                newPos.Add(pos);
                map.SetTile(pos, doorTile);
               
            }
        }
        
        doorTiles.Clear();
        doorTiles = newPos;
    }

    public bool checkForDoorTiles(Vector3Int doorPos)
    {
        if(map.GetTile(doorPos + Vector3Int.up) == wallTile && map.GetTile(doorPos + Vector3Int.down) == wallTile)
        {
            return true;
        }
        else if(map.GetTile(doorPos + Vector3Int.right) == wallTile && map.GetTile(doorPos + Vector3Int.left) == wallTile)
        {
            map.SetTransformMatrix(doorPos, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90)));
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        //Gizmos do wyswietlania polaczen miedzy pokojami

        if (Application.isPlaying && debug)
        {
            foreach (var room in rooms)
            {
                var roomCenter = room.getMapCenter();

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(roomCenter, new Vector3(1, 1, 1));
                Handles.Label(roomCenter, room.getId().ToString());
            }

            showTrianguletion();
        }
    }

    public void showTrianguletion()
    {
        foreach(var edge in connectedRoomids)
        {
            Gizmos.DrawLine(rooms[edge.from].getMapCenter(), rooms[edge.to].getMapCenter());
        }
    }
    public void createEdges()   //Tworzy krwawedzie.                                                ....no shit sherlock
    {
        delaunator.ForEachTriangleEdge(edge =>
        {
            int fromId = 0;
            int toId = 0;

            foreach(var room in rooms)
            {
                //W tej pętli porównywane są koordynaty punktów wygenerowanych krawędzi z punktami centrowymi pokoi, dzięki czemu można zidentyfikować,
                //który pokój jest połączony, z którym

                if (room.roomByCords(edge.Q.ToVector2()))
                {
                    fromId = room.getId();
                    
                }
                if (room.roomByCords(edge.P.ToVector2()))
                {
                    toId = room.getId();
                }
            }
            edges.Add(new Edge() { from = fromId, to = toId, length = Vector2.Distance(edge.P.ToVector2(), edge.Q.ToVector2()) });
        }); 
        KruskalAlghoritm(sortEdges(edges.ToArray()));   //sortuje zbiór krawędzi i wywołuje algoryutm Kruskala
        
    }
    public struct Edge  //pojedyńcza krawędź. "from" i "to" przyjmuja int oznaczający id danych pokoi między którymi tworzona jest krawędź o długości "length"
    {
        public int from { get; set; }       //Id pokoju nr. 1
        public int to { get; set; }         //Id pokoju nr. 2          
        public float length { get; set; }   //długość krawędzi
    }
    public struct Subset //podzbiór potrzebny później funkcji find();
    {
        public int parent { get; set; }
        public int rank { get; set; }
    }

    public Edge[] sortEdges(Edge[] edgesToSort) //Sortowanie krawędzi według ich długości. Od najmniejszej
    {
        Array.Sort(edgesToSort, (x, y) => x.length.CompareTo(y.length));
        return edgesToSort;
    }

    int find(Subset[] subsets, int i)   //Funkcja do znajdowania zbioru elementu
    {    
        if (subsets[i].parent != i)
            subsets[i].parent = find(subsets, subsets[i].parent);

        return subsets[i].parent;
    }
    void Union(Subset[] subsets, int x, int y)  //funkcja ktora "łączy" dwa zbiory x i y
    {
        int xroot = find(subsets, x);
        int yroot = find(subsets, y);

        if (subsets[xroot].rank < subsets[yroot].rank)
            subsets[xroot].parent = yroot;
        else if (subsets[xroot].rank > subsets[yroot].rank)
            subsets[yroot].parent = xroot;

        else
        {
            subsets[yroot].parent = xroot;
            subsets[xroot].rank++;
        }
    }
    void KruskalAlghoritm(Edge[] edge)  //Algorytm Kruskala MST. Dużo by pisać, jak cie interesuje to sobie w necie poszukaj
    {
        int V = rooms.Length;
        Edge[] result = new Edge[V];
        int e = 0;
        int i = 0;
        for (i = 0; i < V; ++i)
            result[i] = new Edge();

        Subset[] subsets = new Subset[V];
        for (i = 0; i < V; ++i)
            subsets[i] = new Subset();

        for (int v = 0; v < V; ++v)
        {
            subsets[v].parent = v;
            subsets[v].rank = 0;
        }

        i = 0;

        while (e < V - 1)
        {
            Edge next_edge = new Edge();
            next_edge = edge[i++];

            int x = find(subsets, next_edge.from);
            int y = find(subsets, next_edge.to);

            if (x != y)
            {
                result[e++] = next_edge;
                Union(subsets, x, y);
            }
        }
        connectedRoomids = result;
    }

    public Vector3Int Vector3ToInt(Vector3 toInt)
    {
        Vector3Int center = new Vector3Int(0, 0, 0);
        if(toInt.x - (int) toInt.x != 0)
        {
            if (toInt.x < 0)
            {
                center.x = (int)toInt.x - 1;
            }
            else
            {
                center.x = (int)toInt.x;
            }
        }
        else
        {
            center.x = (int)toInt.x;
        }
        if (toInt.y - (int) toInt.y != 0)
        {
            if(toInt.y < 0)
        {
                center.y = (int)toInt.y - 1;
            }
            else
            {
                center.y = (int)toInt.y;
            }
        }
        else
        {
            center.y = (int)toInt.y;
        }
        return center;
    }

    
}
