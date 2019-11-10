using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class NewKnightController : MonoBehaviour
{
	private const float JUMP_COOLDOWN = 1.1f;
	private float lastJump = -99f;
    float speed = 4;
    float rotSpeed = 180;
    float rot = 0f;
    public Vector3 moveDir = Vector3.zero;
    CharacterController controller;
    Animator anim;
	public Vector3 forward {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
		forward = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        GetInput();
    }

    void Movement()
    {
		rot += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
		forward = Quaternion.Euler(0, Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime, 0) * forward;
		forward = forward.normalized;
		if(Input.GetKeyDown("space") && !anim.GetCurrentAnimatorStateInfo(0).IsTag("attacking")){
			if(Time.time - lastJump > JUMP_COOLDOWN){
				lastJump = Time.time;
				anim.SetBool("isJumping", true);
			}
		}
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W)){
			if (anim.GetBool("isAttacking")){
                   return;
			}
			else if (!anim.GetBool("isAttacking")){
				anim.SetBool("isRunning", true);
				float prevY = moveDir.y;
				moveDir = forward * speed;
				moveDir.y = prevY;
			}
        }
		if (Input.GetKeyUp(KeyCode.W)){
			anim.SetBool("isRunning", false);
			float prevY = moveDir.y;
			moveDir = Vector3.zero;
			moveDir.y = prevY;
		}
		// move backwards
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S)){
			if (anim.GetBool("isAttacking")){
                    return;
            }
			else if (!anim.GetBool("isAttacking")){
				anim.SetBool("isRunning", true);
				float prevY = moveDir.y;
				moveDir = -forward * speed;
				moveDir.y = prevY;
			}
		}
		if (Input.GetKeyUp(KeyCode.S)){
			anim.SetBool("isRunning", false);
			float prevY = moveDir.y;
			moveDir = Vector3.zero;
			moveDir.y = prevY;
		}
		moveDir.y += Physics.gravity.y * Time.deltaTime;
		if(anim.GetBool("isInteracting")){
			moveDir.x = 0;
			moveDir.z = 0;
		}
        controller.Move(moveDir * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, rot, 0);
    }

    void GetInput()
    {
		// On left click: attack
		if (Input.GetMouseButtonDown (0) & !anim.GetCurrentAnimatorStateInfo(0).IsTag("attacking")){
			anim.SetBool("isAttacking", true);
        }
    }

	// Completed attack animation
	public void OnAttackFinish(){
		anim.SetBool("isAttacking", false);
	}

	// Jump animation
	public void OnJumpStart(){
		moveDir.y = Physics.gravity.y * -0.6f;
	}

	public void OnJumpFinish(){
		anim.SetBool("isJumping", false);
	}

	public float getRotation(){ return rot; }
}