using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Room
{
    Vector3 roomCenter;
    Vector3 mapCenter;
    Vector3 ofset2;
    GameObject roomPrefab;
    public Tilemap map, roomMap;
    int id = 0;
    public bool conected;
    public List<int> connectedIds = new List<int>();
    public int connectedId;

    public Room(GameObject roomPrefab, Tilemap map)
    {
        this.map = map;
        this.roomPrefab = roomPrefab;
        this.roomMap = roomPrefab.GetComponentInChildren<Tilemap>();
        this.roomCenter = getRoomCenter();
        
        

        generate(new Vector3Int(1, 0, 0));
        id++;
    }

    public void generate(Vector3Int ofset)
    {
        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();
        var roomTilemap = roomPrefab.GetComponentInChildren<Tilemap>();

        foreach (var cellIndex in roomTilemap.cellBounds.allPositionsWithin)
        {
            var tile = roomTilemap.GetTile(cellIndex);
            var tilepos = ofset + cellIndex;
            if (map.HasTile(tilepos)){
                Vector3Int newOfset = ofset + new Vector3Int(Random.Range(-10, 10), Random.Range(-10, 10), 0);
                mapCenter = newOfset;
                tiles.Clear();
                generate(newOfset);
                break;
            }
            else
            {
                tiles.Add(tilepos, tile);
            }
        }

        foreach (var tile in tiles)
        {
            map.SetTile(tile.Key, tile.Value);
        }

        

    }

    public Vector3 getRoomCenter()
    {
        var map = roomPrefab.GetComponentInChildren<Tilemap>();
        var center = map.cellBounds.center;



        return center;
    }
    public Vector3 getMapCenter()
    {
        return mapCenter + getRoomCenter();
    }

    public int getId()
    {
        Debug.Log("Id: "+id);
        return id;
    }

    public void setConected(bool con)
    {
        this.conected = con;
    }
    public bool getCon()
    {
        return conected;
    }



    /*
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

        Debug.Log(tilemap.cellBounds.center + ofset);

        
    }

    */


}
