using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTower : MonoBehaviour
{
    public LayerMask enemyLayer;
    public GameObject lookingEnemy;
    Vector3 startPos;
    GameObject shiled;
    private void Start()
    {
        startPos = transform.position;
        gameObject.GetComponent<LineRenderer>().SetPosition(0, startPos);
    }
    void Update()
    {
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        float dist = 0;
        foreach (GameObject eTransform in enemy)
        {
            float a = Vector3.Distance(eTransform.transform.position, transform.position);
            if (dist < a)
            {
                lookingEnemy = eTransform;
                dist = a;
            }
        }
        if (lookingEnemy == null) return;
        Physics2D.Linecast(transform.position, lookingEnemy.transform.position, enemyLayer);

        gameObject.GetComponent<LineRenderer>().SetPosition(1, lookingEnemy.transform.position);

        //lookingEnemy.GetComponent<EnemyScrpit>().ActiveShield(shiled);
    }
}
