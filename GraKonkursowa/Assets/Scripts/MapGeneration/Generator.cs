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
    private TileBase floorTile, wallTile;
    /// /////////////////////

    Delaunator delaunator;  //Algorytm triangulacji
    IPoint[] points;        
    List<Edge> edges = new List<Edge>();


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
    }
    void roomConnector()
    {
        points = roomCenters.ToPoints();
        delaunator = new Delaunator(points);    //triangulacja z centrami pokoi jako punktami

        connectedRoomids = new Edge[rooms.Length];
        createEdges();

        foreach(var edge in connectedRoomids)
        {
            drawCorridor(rooms[edge.from].getMapCenterInt(), rooms[edge.to].getMapCenterInt());
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

    public void drawCorridor(Vector3Int startPos, Vector3Int endPos)
    {
        //w duzym skrócie, jezeli pokoje so wystarczajaca blisko siebie to rysuje to prosta linie miedzy nimi,
        //jesli nie to rysuje korytarz w ksztalcie litery L

        int zasieg = 5; //zakres ktory pokoje musza przekroczyc aby narysowac korzytarz w ksztalcie L

        Vector3 ab = (startPos + endPos) / 2;
        Vector3Int middle = Vector3ToInt(ab);

        float distanceX = Mathf.Abs(startPos.x - endPos.x); //dystans miedzy pokojami w osi X
        float distanceY = Mathf.Abs(startPos.y - endPos.y); //dystans miedzy pokojami w osi Y

        if (distanceX > zasieg || distanceY > zasieg)
        {
            Vector3Int curPos = startPos;

            //Korytarz zawsze jest generowany od punktu a do punktu b
            //Pierwsze generowany jest korytarz w osi X potem Y

            int direction = 0; // 1 = RIGHT/UP, -1 = LEFT/DOWN
            Debug.Log(startPos);
            Debug.Log(endPos);
            Debug.Log(curPos);
            Debug.Log("--------------");

            while (curPos.x != endPos.x)
            {
                if (curPos.x - endPos.x < 0)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                curPos.x -= direction;

                Debug.Log(curPos + " x");
                map.SetTile(curPos, floorTile);
            }
            while (curPos.y != endPos.y)
            {
                if (curPos.y - endPos.y < 0)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                curPos.y -= direction;

                Debug.Log(curPos + " y");
                map.SetTile(curPos, floorTile);
            }
        }else if(distanceX < zasieg)
        {
            Vector3Int curPos = new Vector3Int(middle.x, startPos.y, 0);
            int direction = 0;
            while (curPos.y != endPos.y)
            {
                if (curPos.y - endPos.y < 0)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                curPos.y -= direction;

                map.SetTile(curPos, floorTile);
            }
        }
        else if (distanceY < zasieg)
        {
            Vector3Int curPos = new Vector3Int(startPos.x, middle.y, 0);
            int direction = 0;
            while (curPos.x != endPos.x)
            {
                if (curPos.x - endPos.x < 0)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                curPos.x -= direction;

                map.SetTile(curPos, floorTile);
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
            Debug.Log(fromId);
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
        Debug.Log(rooms.Length);
        Debug.Log(edge.Length);
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
        if (toInt.x < 0)
        {
            center.x = (int)toInt.x - 1;
        }
        else
        {
            center.x = (int)toInt.x;
        }
        if (toInt.y < 0)
        {
            center.y = (int)toInt.y - 1;
        }
        else
        {
            center.y = (int)toInt.y;
        }

        return center;
    }
}
