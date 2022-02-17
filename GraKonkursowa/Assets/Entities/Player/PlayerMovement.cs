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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;

        if (Input.GetKeyDown(KeyCode.F))
        {
            pAnimation.SetTrigger("Attack");
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speeeeed * Time.deltaTime);

        Vector3 playerCamPosition = this.gameObject.transform.position + camOffset;
        distance = Vector3.Distance(new Vector3(this.transform.position.x, this.transform.position.y, 0), new Vector3(cam.transform.position.x, cam.transform.position.y, 0));

        cam.transform.position = Vector3.MoveTowards(cam.transform.position, playerCamPosition, distance * camSpeeeeeed * Time.deltaTime);
    }
}
