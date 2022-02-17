using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorFirstGeneration : MapGenerator
{
    private int corridorLenght = 14, corridorCount = 5;

    [Range(.1f, 1f)]
    private float roomPercent = 0.8f;

    protected override void Run()
    {
        CoridorFirstGeneration();
        
    }

    private void CoridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions);
        tileGenerator.DrawFloorTiles(floorPositions);
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPosition)
    {
        var curPos = startPos;

        for(int i = 0; i < corridorCount; i++)
        {
            var corridor = RandomWalk.RandomCorridor(curPos, corridorLenght);

            curPos = corridor[corridor.Count - 1];
            floorPosition.UnionWith(corridor);
        }
    }
}
