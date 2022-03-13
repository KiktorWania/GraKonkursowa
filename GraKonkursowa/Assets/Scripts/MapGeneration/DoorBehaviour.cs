using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    Vector3Int pos;
    Quaternion rotation;

    bool isClosed;

    public Door(Vector3Int pos, Quaternion rotation)
    {
        this.pos = pos;
        this.rotation = rotation;
    }
    void Start()
    {
        Instantiate(null, pos, rotation);
    }

    public void closeDoor()
    {

    }
    public void openDoor()
    {
        
    }
}
