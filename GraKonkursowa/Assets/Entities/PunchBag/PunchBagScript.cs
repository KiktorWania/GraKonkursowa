using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBagScript : MonoBehaviour
{
    float time;
    int dmg;
    public TextMesh textDmg;
    private void Update()
    {
        time -= Time.deltaTime;

        if(time <= 0)
        {
            dmg = 0;
            textDmg.text = "DMG: 0";
        }
    }
    public void TakeDamage(int takeDMG)
    {
        dmg += takeDMG;
        time = 5;
        textDmg.text = "DMG: " + dmg.ToString();
        Debug.Log("Kaczka");
        gameObject.GetComponent<Animator>().SetTrigger("Punched");
    }
}
