using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Abbilitie : MonoBehaviour
{
    public float timeAblility;
    public float damage;
    public enum AbilityType // your custom enumeration
    {
        None,
        ShadowDash,
        Gun,
        RockKnockBack,
        Berserk,
    };
    public AbilityType dropDown;
    public void Update()
    {
        if (this.gameObject.transform.GetChild(3).GetComponent<Image>().fillAmount != 1)
        {
            this.gameObject.GetComponent<Animator>().SetBool("Comeback", true);
            this.gameObject.transform.GetChild(3).GetComponent<Image>().fillAmount += 1 * Time.deltaTime / timeAblility;
        }
        else
        {
            this.gameObject.GetComponent<Animator>().SetBool("Comeback", false);
        }
    }
    public void ActiveAbblility()
    {
        if (this.gameObject.transform.GetChild(3).GetComponent<Image>().fillAmount == 1)
        {
            this.gameObject.transform.GetChild(3).GetComponent<Image>().fillAmount = 0;
            Debug.Log(this.gameObject.name);
        }
        else
        {

        }
    }
}