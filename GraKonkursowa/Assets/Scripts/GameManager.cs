using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Generator mapGen;
    public List<Door> doors = new List<Door>();

    void Start()
    { 

    }
    

    
    void Update()
    {
      
    }

    public void doorStatus(bool closed)
    {
        if (closed)
        {
            foreach(var door in doors)
            {
                door.closeDoor();
            }
        }
        else
        {
            foreach (var door in doors)
            {
                door.openDoor();
            }
        }
    }
}
