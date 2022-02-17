using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGenerator : MonoBehaviour
{
    protected TileGenerator tileGenerator = null;
    protected Vector2Int startPos = Vector2Int.zero;

    public void GenerateDungeon()
    {
        tileGenerator.Clear();
        Run();
    }

    protected abstract void Run();
}
