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
