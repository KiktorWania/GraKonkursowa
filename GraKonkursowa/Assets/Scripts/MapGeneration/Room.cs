using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Room
{
    Vector3 mapCenter;
    GameObject roomPrefab;
    

    public Vector2 roomXBounds;
    public Vector2 roomYBounds;

    public Tilemap map, roomMap;

    private int spaccing = 4;
    
    public bool conected;
    public bool connectedByCorridor = false;
    public List<int> connectedIds = new List<int>();

    public int id = 0;

    Tilemap roomTilemap;
    Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();
    public Room(GameObject roomPrefab, Tilemap map, int spaccing, int id)
    {
        this.map = map;
        this.roomPrefab = roomPrefab;
        this.roomMap = roomPrefab.GetComponentInChildren<Tilemap>();

        this.roomTilemap = roomPrefab.GetComponentInChildren<Tilemap>();

        this.spaccing = spaccing;
        this.id = id;

        generate(new Vector3Int(0,0,0));
        getRoomSize();
    }

    public void generate(Vector3Int ofset)
    {
        /*
         W skrócie:
            Zczytuje do s³ownika miejsceKafelka i jego typ(tk. podloga, sciana etc.)
            Sprawdza czy na miejscu jakiegos z kafelkow znajduje sie juz inny kafelek
            jesli tak, to czysci slownik i wywoluje metode ponownie z innym ofsetem
            jesli nie, to spisuje na glowna Tilemape pokoj
         */

        

        foreach (var cellIndex in roomTilemap.cellBounds.allPositionsWithin)
        {
            var tile = roomTilemap.GetTile(cellIndex);
            var tilepos = ofset + cellIndex;
            if (map.HasTile(tilepos)){
                Vector3Int newOfset = ofset + new Vector3Int(Random.Range(-spaccing + 1, spaccing), Random.Range(-spaccing + 1, spaccing), 0);
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

    //Gettery i settery
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
    public Vector3Int getMapCenterInt() //Zwraca srodek pokoju na mapie ale z koordynatami stricte do tilemapy
    {
        /*
         Dlaczego?
            poniewa¿ samo skrócenie Vectora3 do Vectora3Int, a na takim wlasnie dzialaja tilemapy, wiaze ze soba wiele utrudnien
            glownie chodzi o to ze wartosci dodadnie i ujemne zostaja skrocone
            przyklad:
                Vector3 = (-1.5, 2.5, 0);   - przed
                Vector3Int = (-1, 2, 0);    - po
            
            problem polega na tym ze kafelki na ujemnych koordynatach zostaja przesuniete w prawo, poniewaz wartosc sie zwieksza (jest blizej 0), a te na dodatnich w lewo
            Chcemy aby obie strony przesuwaly sie w JEDNA strone tak aby zapobiec bledom w przyszlosci
            
            przykla tej funkcji
                Vector3 = (-1.5, 2.5, 0);   - przed
                Vector3Int = (-2, 2, 0);    - po
         */


        Vector3Int center = new Vector3Int(0,0,0); 
        Vector3 toInt = mapCenter + getRoomCenter();
        if (toInt.x - (int)toInt.x != 0)
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
        if (toInt.y - (int)toInt.y != 0)
        {
            if (toInt.y < 0)
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
    public GameObject getRoomPrefab()
    {
        return roomPrefab;
    }
    public Vector3 getOfset()
    {
        return mapCenter;
    }
    public void getRoomSize()
    {
        int minX = 0, minY = 0, maxX = 0, maxY = 0;

        foreach (var cellIndex in roomTilemap.cellBounds.allPositionsWithin)
        {
            var tilepos = cellIndex;
            if (roomMap.HasTile(tilepos))
            {
                if (tilepos.x < minX)
                {
                    minX = tilepos.x;
                }
                else if (tilepos.x > maxX)
                {
                    maxX = tilepos.x;
                }

                if (tilepos.y < minY)
                {
                    minY = tilepos.y;
                }
                else if (tilepos.y > maxY)
                {
                    maxY = tilepos.y;
                }
            }
        }
        roomXBounds = new Vector2(minX + mapCenter.x, maxX + mapCenter.x);
        roomYBounds = new Vector2(minY + mapCenter.y, maxY + mapCenter.y);
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
