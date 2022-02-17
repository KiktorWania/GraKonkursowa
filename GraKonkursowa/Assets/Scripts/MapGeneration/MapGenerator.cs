using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    Vector2Int startPos = Vector2Int.zero;

    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    public int walkLength = 10;
    [SerializeField]
    public bool startRandomlyEachIteration = true;

    public void Run()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk();

        foreach(var position in floorPositions)
        {
            Debug.Log(position);
        }
    }

    protected HashSet<Vector2Int> RunRandomWalk()
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
}
