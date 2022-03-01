using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Room
{
    Vector3 mapCenter;
    GameObject roomPrefab;
    public Tilemap map, roomMap;

    private int spaccing = 4;
    
    public bool conected;
    public bool connectedByCorridor = false;
    public List<int> connectedIds = new List<int>();

    public bool debug;

    public int id = 0;

    Tilemap roomTilemap;
    Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();
    public Room(GameObject roomPrefab, Tilemap map, int spaccing)
    {
        this.map = map;
        this.roomPrefab = roomPrefab;
        this.roomMap = roomPrefab.GetComponentInChildren<Tilemap>();

        this.roomTilemap = roomPrefab.GetComponentInChildren<Tilemap>();

        this.spaccing = spaccing;

        generate(new Vector3Int(0,0,0));
    }

    public void generate(Vector3Int ofset)
    {
        /*
         W skrócie:
            Zczytuje do s³ownika miejsceKafelka i jego typ(tk. podloga, sciana etc.)
            Sprawdza czy na miejscu jakiego z kafelka znajduje sie juz inny kafelek
            jesli tak to czysci slownik i wywoluje metode ponownie z innym ofsetem
            jesli nie to spisuje na glowna Tilemape pokoj
         */

        foreach (var cellIndex in roomTilemap.cellBounds.allPositionsWithin)
        {
            var tile = roomTilemap.GetTile(cellIndex);
            var tilepos = ofset + cellIndex;
            if (map.HasTile(tilepos)){
                Vector3Int newOfset = ofset + new Vector3Int(Random.Range(-spaccing + 1, spaccing), Random.Range(-spaccing + 1, spaccing), 0);
                mapCenter = newOfset;
                tiles.Clear();

                if (debug)
                {
                    Debug.Log("stary: "+ofset);
                    Debug.Log("nowy: "+newOfset);
                }

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

    public Vector3 getRoomCenter()  //Zwraca surowy srodek pokoju tj. (x, y)
    {
        var map = roomPrefab.GetComponentInChildren<Tilemap>();
        var center = map.cellBounds.center;

        return center;
    }
    public Vector3 getMapCenter() //Zwraca srodek pokoju na mapie tj. (x + ofset, y + ofset)
    {
        return mapCenter + getRoomCenter();
    }
    public Vector3Int getMapCenterInt() //Zwraca srodek pokoju na mapie tj. (x + ofset, y + ofset)
    {
        Vector3Int center = new Vector3Int(0,0,0); 
        Vector3 toInt = mapCenter + getRoomCenter();
        if(toInt.x < 0)
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

    //Gettery i settery - aka. nic ciekawego
    public void setConected(bool con)
    {
        this.conected = con;
    }
    public bool getCon()
    {
        return conected;
    }
    public void setDebug(bool con)
    {
        this.debug = con;
    }

    public bool roomByCords(Vector2 cords)
    {
        if(cords.x == getMapCenter().x && cords.y == getMapCenter().y)
        {
            return true;
        }
        return false;
    }

    public void setId(int id)
    {
        this.id = id;
    }

    public int getId()
    {
        return id;
    }

}
