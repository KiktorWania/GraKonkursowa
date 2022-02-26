using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 movement;
    public Rigidbody2D rb;
    public float speeeeed;

    public Camera cam;
    public float camSpeeeeeed;
    public float distance;
    public Vector3 camOffset;

    public Animator pAnimation;
    public float attackRange;
    public Transform hitPoint;
    public LayerMask enemiesLayer;

    public Transform cloudPoint;
    public LayerMask cloudLayer;
    public Animator cloudAnimation;
    GameObject[] debugs;
    // Start is called before the first frame update
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
        //Attacks
        //Heavy attack
        if (Input.GetKeyDown(KeyCode.J))
        {
            pAnimation.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, enemiesLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<PunchBagScript>().TakeDamage(27);
            }
        }
        //Weak attack
        if (Input.GetKeyDown(KeyCode.K))
        {
            pAnimation.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, enemiesLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<PunchBagScript>().TakeDamage(15);
            }
        }

        //Cloud effect
        Collider2D clouds = Physics2D.OverlapCircle(cloudPoint.position, 0.1f, cloudLayer);

        if (clouds != null)
        {
            cam.orthographicSize = 3f;
            cloudAnimation.SetBool("InsideCloud", true);
        }
        else
        {
            cam.orthographicSize = 5f;
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
            if (debugs[0].active == false)
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
}
