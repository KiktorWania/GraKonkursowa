using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : AbstractMapGenerator
{
    public Vector2Int startPos = Vector2Int.zero;

    private int corridorLenght = 14, corridorCount = 5;

    [Range(.1f, 1f)]
    private float roomPercent = 0.8f;




    public int iterations = 10;

    public int walkLength = 10;
   
    public bool startRandomlyEachIteration = true;

    public TileGenerator tileGenerator;

    protected override void Run()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();
        tileGenerator.Clear();
        tileGenerator.DrawFloorTiles(floorPositions);
    }

    public HashSet<Vector2Int> RunRandomWalk()
    {
        var currentPos = startPos;
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();

        for(int i = 0; i < iterations; i++)
        {
            var path = RandomWalk.SimpleRandomWalk(currentPos, walkLength);
            floorPos.UnionWith(path);
            if (startRandomlyEachIteration)
            {
                currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
            }
        }

        return floorPos;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPosition)
    {
        var curPos = startPos;

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = RandomWalk.RandomCorridor(curPos, corridorLenght);

            curPos = corridor[corridor.Count - 1];
            floorPosition.UnionWith(corridor);
        }
    }
}
