using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class new_knight_controller : MonoBehaviour
{
    public bool isGrounded;
    public bool isAttacking;

    private float speed = 0.05f;
    private float w_speed = 0.05f;
    private float r_speed = 0.1f;
    public float rotspeed = 0.1f;
    Rigidbody rb;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        float z = Input.GetAxis("Vertical") * speed;
        float y = Input.GetAxis("Horizontal") * rotspeed;


        transform.Translate(0, 0, z);
        transform.Rotate(0, y, 0);

        if (!isAttacking)
        {

            if (Input.GetKey(KeyCode.Space))
            {

                anim.SetTrigger("isJumping");
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = r_speed;
                if (Input.GetKey(KeyCode.W))
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isRunning", true);
                    anim.SetBool("isIdle", false);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isRunning", true);
                    anim.SetBool("isIdle", false);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isIdle", true);
                }
            }
            else
            {
                speed = w_speed;
                if (Input.GetKey(KeyCode.W))
                {
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isIdle", false);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isIdle", false);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isIdle", true);
                }
            }
        }

        if (Input.GetKey(KeyCode.J))
        {

            anim.SetTrigger("isAttacking");
        }
    }
}
