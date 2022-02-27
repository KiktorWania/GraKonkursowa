using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Room
{
    Vector3 mapCenter;
    GameObject roomPrefab;
    public Tilemap map, roomMap;

    private int range = 4;
    
    public bool conected;
    public bool connectedByCorridor = false;
    public List<int> connectedIds = new List<int>();

    public bool debug;

    Tilemap roomTilemap;
    Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();
    public Room(GameObject roomPrefab, Tilemap map)
    {
        this.map = map;
        this.roomPrefab = roomPrefab;
        this.roomMap = roomPrefab.GetComponentInChildren<Tilemap>();

        this.roomTilemap = roomPrefab.GetComponentInChildren<Tilemap>();

        generate(new Vector3Int(0,0,0));
    }

    public void generate(Vector3Int ofset)
    {
        /*
         W skr�cie:
            Zczytuje do s�ownika miejsceKafelka i jego typ(tk. podloga, sciana etc.)
            Sprawdza czy na miejscu jakiego z kafelka znajduje sie juz inny kafelek
            jesli tak to czysci slownik i wywoluje metode ponownie z innym ofsetem
            jesli nie to spisuje na glowna Tilemape pokoj
         */

        foreach (var cellIndex in roomTilemap.cellBounds.allPositionsWithin)
        {
            var tile = roomTilemap.GetTile(cellIndex);
            var tilepos = ofset + cellIndex;
            if (map.HasTile(tilepos)){
                Vector3Int newOfset = ofset + new Vector3Int(Random.Range(-range+1, range), Random.Range(-range+1, range), 0);
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

}
