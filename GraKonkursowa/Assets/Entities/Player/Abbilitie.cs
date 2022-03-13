using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Abbilitie : MonoBehaviour
{
    public float timeAblility;
    public float damage;
    public float distance;
    GameObject player;

    public GameObject lookingEnemy;
    public enum AbilityType // your custom enumeration
    {
        None,
        ShadowDash,
        Gun,
        RockKnockBack,
        Berserk,
    };
    public AbilityType dropDown;
    public LayerMask lM;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
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

            switch (dropDown.ToString())
            {
                case "ShadowDash":
                    if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0), distance, lM);
                        if (hit == false)
                        {
                            player.transform.position += new Vector3(1 * Input.GetAxisRaw("Horizontal") * distance, 1 * Input.GetAxisRaw("Vertical") * distance, 0);
                        }
                        else
                        {
                            float moreCorection = 1;
                            if (Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0)
                                moreCorection = 0.5f;
                            if ((hit.distance - 1) < 0)
                                this.gameObject.transform.GetChild(3).GetComponent<Image>().fillAmount = 1;
                            else
                                player.transform.position += new Vector3(1 * Input.GetAxisRaw("Horizontal") * (hit.distance - 1 - moreCorection), 1 * Input.GetAxisRaw("Vertical") * (hit.distance - 1), 0);
                            Debug.Log("Short dash: " + hit.collider);
                        }                    
                    }
                    else
                    {
                        this.gameObject.transform.GetChild(3).GetComponent<Image>().fillAmount = 1;
                    }
                    break;

                case "Gun":
                    GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
                    float dist = 0;
                    foreach (GameObject eTransform in enemy)
                    {
                        float a = Vector3.Distance(eTransform.transform.position, player.transform.position);
                        if (dist < a)
                        {
                            lookingEnemy = eTransform;
                            dist = a;
                        }
                    }
                    if (lookingEnemy == null) return;
                    Physics2D.Linecast(transform.position, lookingEnemy.transform.position, lM);
                    lookingEnemy.GetComponent<EnemyScrpit>().TakeDamage(20);
                    break;
            }
        }
        else
        {

        }
    }
}