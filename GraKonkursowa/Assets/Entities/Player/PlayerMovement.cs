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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX > 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        else if (moveX < 0)
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        if (moveX != 0)
        {
            pAnimation.SetBool("Running", true);
        }
        else
        {
            pAnimation.SetBool("Running", false);
        }

        movement = new Vector2(moveX, moveY).normalized;
        if (Input.GetKeyDown(KeyCode.F))
        {
            pAnimation.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, enemiesLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<PunchBagScript>().TakeDamage();
            }
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speeeeed * Time.deltaTime);

        Vector3 playerCamPosition = this.gameObject.transform.position + camOffset;
        distance = Vector3.Distance(new Vector3(this.transform.position.x, this.transform.position.y, 0), new Vector3(cam.transform.position.x, cam.transform.position.y, 0));

        cam.transform.position = Vector3.MoveTowards(cam.transform.position, playerCamPosition, distance * camSpeeeeeed * Time.deltaTime);
    }
    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null)
            return;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
    }
}
