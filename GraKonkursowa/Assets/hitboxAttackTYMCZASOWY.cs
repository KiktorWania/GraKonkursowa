using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitboxAttackTYMCZASOWY : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("a");
            GameObject.Find("PunchBag").GetComponent<PunchBagScript>().TakeDamage();
        }
    }
}
