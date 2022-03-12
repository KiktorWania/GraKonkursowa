using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            foreach (var enemy in enemies)
            {
                //TODO respienie przeciwników
            }
            //zamykanie drzwi

            var collider = gameObject.GetComponent<BoxCollider2D>();
            Destroy(collider);
        }
        
    }
}
