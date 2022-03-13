using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("test");
        if (collision.tag == "Player")
        {
            
            Debug.Log("chyba dziala");
            foreach (var enemy in enemies)
            {
                //TODO respienie przeciwników
            }
            gameManager.doorStatus(true);

            var collider = this.gameObject.GetComponent<BoxCollider2D>();
            Destroy(collider);
        }
    }
}
