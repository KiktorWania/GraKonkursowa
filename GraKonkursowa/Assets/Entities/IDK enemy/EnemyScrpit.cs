using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScrpit : MonoBehaviour
{
    public Transform player;
    public LayerMask playerLayer;
    public float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -(transform.position - player.position), Color.white, Vector2.Distance(transform.position, player.position));
        // Does the ray intersect any objects excluding the player layer
        if (Physics2D.Linecast(transform.position, player.position, playerLayer) == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
}
