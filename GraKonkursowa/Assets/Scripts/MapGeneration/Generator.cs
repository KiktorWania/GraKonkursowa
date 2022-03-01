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
    private int roomSpaceing = 10;

    public int roomCount = 20; //Liczba pokoi bez głównego pokoju;
    private Dictionary<int, Room> rooms;
    [SerializeField]
    private TileBase floorTile, wallTile;
    /// /////////////////////

    IPoint[] points;
    List<Edge> edges = new List<Edge>();

    Delaunator delaunator;

    /// /////////////////////

    Edge[] connectedRoomids;
    private Room mainRoom;
    Vector2[] roomCenters;
    void Start()
    {


        rooms = new Dictionary<int, Room>();

        mainRoom = new Room(mainRoomPrefab, map, 10); //Stworznie głównego pokoju
        mainRoom.setId(0);
        rooms.Add(0, mainRoom);                       //Dodanie go do słownika z indeksem zerowym

        for (int i = 1; i < roomCount + 1; i++)     //Dodanie reszty pokoi
        {
            Room room = new Room(roomsPrefab[Random.Range(0, 3)], map, roomSpaceing);
            room.setId(i);
            rooms.Add(i, room);
        }


        roomCenters = new Vector2[rooms.Count];

        for (int i = 0; i < roomCenters.Length; i++)
        {
            roomCenters[i] = new Vector2(rooms[i].getMapCenter().x, rooms[i].getMapCenter().y);
        }
        points = roomCenters.ToPoints();

        delaunator = new Delaunator(points);

        //Połącznie pokoi za pomocą tego popsutego algorytmu

        for (int i = 0; i < rooms.Values.Count; i++)    //Wygenerowanie korytarzy
        {
            if (rooms[i].connectedIds != null)
            {
                for (int x = 0; x < rooms[i].connectedIds.Count; x++)
                {
                    //drawCorridor(rooms[i].getMapCenter(), rooms[rooms[i].connectedIds[x]].getMapCenter());
                }
            }


        }
        connectedRoomids = new Edge[rooms.Count];
        createEdges();
        fillOutMap(wallTile);  //wypełnienie reszty mapy

        
        foreach(var edge in connectedRoomids)
        {
            drawCorridor(rooms[edge.Vertex1].getMapCenterInt(), rooms[edge.Vertex2].getMapCenterInt());
        } 

        //drawCorridor(rooms[connectedRoomids[0].Vertex1].getMapCenter(), rooms[connectedRoomids[0].Vertex2].getMapCenter());
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

        float distanceX = Mathf.Abs(startPos.x - endPos.x); //dystans miedzy pokojami w osi X
        float distanceY = Mathf.Abs(startPos.y - endPos.y); //dystans miedzy pokojami w osi Y

        //Reszta magii, nie chce mi sie tego komentowac ¯\_(ツ)_/¯

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
            

            Vector3Int curPos = startPos;
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
            Vector3Int curPos = startPos;
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
                var roomCenter = room.Value.getMapCenter();

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(roomCenter, new Vector3(1, 1, 1));
                Handles.Label(roomCenter, room.Key.ToString());
            }

            showTrianguletion();
        }
    }

    public void showTrianguletion()
    {
        foreach(var edge in connectedRoomids)
        {
            Gizmos.DrawLine(rooms[edge.Vertex1].getMapCenter(), rooms[edge.Vertex2].getMapCenter());
        }
    }
    public void createEdges()
    {
        delaunator.ForEachTriangleEdge(edge =>
        {
            int fromId = 0;
            int toId = 0;

            foreach(var x in rooms)
            {
                var room = x.Value;
                if (room.roomByCords(edge.Q.ToVector2()))
                {
                    fromId = room.getId();
                }
                if (room.roomByCords(edge.P.ToVector2()))
                {
                    toId = room.getId();
                }
            }

            edges.Add(new Edge() { Vertex1 = fromId, Vertex2 = toId, Weight = Vector2.Distance(edge.P.ToVector2(), edge.Q.ToVector2()) });
        });
        Edge[] sortedEdges = sortEdges(edges.ToArray());
        KruskalMST(sortedEdges);
        
    }
    public struct Edge
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }
        public float Weight { get; set; }
    }
    public struct Subset
    {
        public int parent { get; set; }
        public int rank { get; set; }
    }

    public Edge[] sortEdges(Edge[] edgesToSort)
    {
        Array.Sort(edgesToSort, (x, y) => x.Weight.CompareTo(y.Weight));
        return edgesToSort;
    }

    int find(Subset[] subsets, int i)
    {    
        if (subsets[i].parent != i)
            subsets[i].parent = find(subsets, subsets[i].parent);

        return subsets[i].parent;
    }
    void Union(Subset[] subsets, int x, int y)
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


    void KruskalMST(Edge[] edge)
    {
        int V = rooms.Count;
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

            int x = find(subsets, next_edge.Vertex1);
            int y = find(subsets, next_edge.Vertex2);

            if (x != y)
            {
                result[e++] = next_edge;
                Union(subsets, x, y);
            }
        }
        float minimumCost = 0;
        for (i = 0; i < e; ++i)
        {
           // Debug.Log(result[i].Vertex1 + " -- "+ result[i].Vertex2+ " == " + result[i].Weight);
            minimumCost += result[i].Weight;
        }

        connectedRoomids = result;
    }
}
