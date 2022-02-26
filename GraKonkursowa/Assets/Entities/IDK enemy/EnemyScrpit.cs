using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScrpit : MonoBehaviour
{
    public Transform player;
    public LayerMask playerLayer;
    public GameObject lastSeenPoint;
    public float speed;
    public int health;
    public TextMesh debugInfo;

    public Vector3 randomPlace;
    public Vector3 randNewPlace;

    public int state = 0;
    void Start()
    {
        randomPlace = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
        randNewPlace = transform.position + randomPlace;
        StartCoroutine(randWalk());
    }

    // Update is called once per frame
    void Update()
    {
        debugInfo.text = "Hp: " + health + "\nState: " + state;
        Debug.DrawRay(transform.position, -(transform.position - player.position), Color.white, Vector2.Distance(transform.position, player.position));
        // Does the ray intersect any objects excluding the player layer
        if (Physics2D.Linecast(transform.position, player.position, playerLayer) == false)
        {
            state = 1;
        }
        switch (state)
        {
            case 0:
                //Chill walk around
                transform.position = Vector3.MoveTowards(transform.position, randNewPlace, speed / 2 * Time.deltaTime);
                break;
            case 1:
                //See player and chase him
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                break;
            case 2:
                //Lost player run in the last seen place
                break;
            case 3:
                //Run around last seen place
                break;
        }
    }
    IEnumerator randWalk()
    {
        yield return new WaitForSeconds(3);
        randomPlace = new Vector3(Random.Range(-2,3), Random.Range(-2,3), 0);
        randNewPlace = transform.position + randomPlace;
        StartCoroutine(randWalk());
    }

    void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
