using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomWalk
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPos, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPos);
        var previousPos = startPos;

        for(int i = 0; i < walkLength; i++)
        {
            var newPos = previousPos + Direction2D.getDirection();
            path.Add(newPos);
            previousPos = newPos;
        }

        return path;
    }

    public static List<Vector2Int> RandomCorridor(Vector2Int startPos, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.getDirection();
        var currentPos = startPos;
        corridor.Add(currentPos);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPos += direction;
            corridor.Add(currentPos);
        }

        return corridor;
    }

    public static class Direction2D {
        public static List<Vector2Int> directionList = new List<Vector2Int>
        {
            new Vector2Int(0, 1),  // UP
            new Vector2Int(1, 0),   // RIGHT
            new Vector2Int(0, -1), // DOWN
            new Vector2Int(-1, 0)  // LEFT
        };

        public static Vector2Int getDirection()
        {
            return directionList[Random.Range(0, directionList.Count)];
        }
    }

    
}
