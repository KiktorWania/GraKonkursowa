using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 movement;
    public Rigidbody2D rb;
    public float speeeeed = 5;
    public bool attack;
    public char[] combo;
    public float comboTime = 0;
    public static int direction;
    bool protect;

    public Camera cam;
    float camSpeeeeeed = 8f;
    float distance;
    Vector3 camOffset = new Vector3(0, 0, -10);

    public Animator pAnimation;
    public float attackRange;
    [SerializeField]
    public Transform hitPoint;
    public LayerMask enemiesLayer;

    public Transform cloudPoint;
    public LayerMask cloudLayer;
    public Animator cloudAnimation;
    GameObject[] debugs;
    // Start is called before the first frame update

    public GameObject qAbbility;
    public GameObject eAbbility;
    public GameObject iAbbility;
    public GameObject oAbbility;
    void Awake()
    {
        debugs = GameObject.FindGameObjectsWithTag("DEBUG");
        foreach (GameObject kaczka in debugs)
        {
            kaczka.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TurnDebug();
        Attacks();
        Abbilites();

        //Movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX > 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        else if (moveX < 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        movement = new Vector2(moveX, moveY).normalized;
        //Movement animation
        if (moveX != 0 || moveY != 0)
        {
            pAnimation.SetBool("Running", true);
        }
        else
        {
            pAnimation.SetBool("Running", false);
        }
        
        //Cloud effect
        Collider2D clouds = Physics2D.OverlapCircle(cloudPoint.position, 0.1f, cloudLayer);

        if (clouds != null)
        {
            cam.orthographicSize = 2f;
            cloudAnimation.SetBool("InsideCloud", true);
        }
        else
        {
            cam.orthographicSize = 3f;
            cloudAnimation.SetBool("InsideCloud", false);
        }
    }
    private void FixedUpdate()
    {
        //Camera Algin and Movement velocity
        rb.MovePosition(rb.position + movement * speeeeed * Time.deltaTime);

        Vector3 playerCamPosition = this.gameObject.transform.position + camOffset;
        distance = Vector3.Distance(new Vector3(this.transform.position.x, this.transform.position.y, 0), new Vector3(cam.transform.position.x, cam.transform.position.y, 0));

        cam.transform.position = Vector3.MoveTowards(cam.transform.position, playerCamPosition, distance * camSpeeeeeed * Time.deltaTime);
    }
    private void OnDrawGizmosSelected()
    {
        //Drawing hitbox of attack
        if (hitPoint == null)
            return;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
    }
    void TurnDebug()
    {
        //Debug info
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            if (debugs[0].activeSelf == false)
            {
                foreach (GameObject kaczka in debugs)
                {
                    kaczka.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject kaczka in debugs)
                {
                    kaczka.SetActive(false);
                }
            }
        }
    }
    void Attacks()
    {
        comboTime -= Time.deltaTime;
        if (comboTime <= 0)
        {
            Array.Clear(combo, 0, combo.Length);
        }
        //Attacks
        //Heavy attack
        if (Input.GetKeyDown(KeyCode.J) && attack)
        {
            comboTime = 3;
            if (combo[2] == '\0')
            {
                if (combo[0] == '\0')
                {
                    combo[0] = "J"[0];
                }
                else if (combo[1] == '\0')
                {
                    combo[1] = "J"[0];
                }
                else
                {
                    combo[2] = "J"[0];
                }
            }
            else
            {
                combo[0] = combo[1];
                combo[1] = combo[2];
                combo[2] = "J"[0];
            }
            pAnimation.SetTrigger("Attack");
            StartCoroutine(AttackRecharge(0.3f));

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, enemiesLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                //enemy.GetComponent<PunchBagScript>().TakeDamage(27);
                enemy.GetComponent<EnemyScrpit>().TakeDamage(27);
            }
        }
        //Weak attack
        if (Input.GetKeyDown(KeyCode.K) && attack)
        {
            comboTime = 3;
            if (combo[2] == '\0')
            {
                if (combo[0] == '\0')
                {
                    combo[0] = "K"[0];
                }
                else if (combo[1] == '\0')
                {
                    combo[1] = "K"[0];
                }
                else
                {
                    combo[2] = "K"[0];
                }
            }
            else
            {
                combo[0] = combo[1];
                combo[1] = combo[2];
                combo[2] = "K"[0];
            }

            pAnimation.SetTrigger("Attack");
            StartCoroutine(AttackRecharge(0.1f));

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, enemiesLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                //enemy.GetComponent<PunchBagScript>().TakeDamage(15);
                enemy.GetComponent<EnemyScrpit>().TakeDamage(15);
            }
        }
        //Block
        if (Input.GetKeyDown(KeyCode.L) && attack)
        {
            comboTime = 3;
            if (combo[2] == '\0')
            {
                if (combo[0] == '\0')
                {
                    combo[0] = "L"[0];
                }
                else if (combo[1] == '\0')
                {
                    combo[1] = "L"[0];
                }
                else
                {
                    combo[2] = "L"[0];
                }
            }
            else
            {
                combo[0] = combo[1];
                combo[1] = combo[2];
                combo[2] = "L"[0];
            }
            protect = true;
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            protect = false;
        }
    }
    IEnumerator AttackRecharge(float recharge)
    {
        attack = false;
        yield return new WaitForSecondsRealtime(recharge);
        attack = true;
    }

    void Abbilites()
    {
        if (Input.GetKeyDown(KeyCode.Q) && qAbbility.transform.parent.childCount > 3)
        {
            qAbbility.GetComponent<Abbilitie>().ActiveAbblility();
        }
        if (Input.GetKeyDown(KeyCode.E) && eAbbility.transform.parent.childCount > 3)
        {
            eAbbility.GetComponent<Abbilitie>().ActiveAbblility();
        }
        if (Input.GetKeyDown(KeyCode.I) && iAbbility.transform.parent.childCount > 3)
        {
            iAbbility.GetComponent<Abbilitie>().ActiveAbblility();
        }
        if (Input.GetKeyDown(KeyCode.O) && oAbbility.transform.parent.childCount > 3)
        {
            oAbbility.GetComponent<Abbilitie>().ActiveAbblility();
        }
    }
}
