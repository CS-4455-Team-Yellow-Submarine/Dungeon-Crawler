using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class KnightController : MonoBehaviour
{
    float speed = 4;
    float rotSpeed = 180;
    float rot = 0f;
    Vector3 moveDir = Vector3.zero;
    CharacterController controller;
    Animator anim;
	Vector3 forward;

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
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.W)){
            if (anim.GetBool("attacking")){
                   return;
			}
			else if (!anim.GetBool("attacking")){
				anim.SetBool("running", true);
				anim.SetInteger("condition", 1);
				float prevY = moveDir.y;
				moveDir = forward * speed;
				moveDir.y = prevY;
			}
        }
		if (Input.GetKeyUp(KeyCode.W)){
			anim.SetBool("running", false);
			anim.SetInteger("condition", 0);
			float prevY = moveDir.y;
			moveDir = Vector3.zero;
			moveDir.y = prevY;
		}
		// move backwards
		if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S)){
			if (anim.GetBool("attacking")){
                    return;
            }
            else if (!anim.GetBool("attacking")){
            	anim.SetBool("running", true);
				anim.SetInteger("condition", 1);
				float prevY = moveDir.y;
				moveDir = -forward * speed;
				moveDir.y = prevY;
			}
		}
		if (Input.GetKeyUp(KeyCode.S)){
			anim.SetBool("running", false);
			anim.SetInteger("condition", 0);
			float prevY = moveDir.y;
			moveDir = Vector3.zero;
			moveDir.y = prevY;
		}
		moveDir.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, rot, 0);
    }

    void GetInput()
    {
		if (Input.GetMouseButtonDown (0)){
			if (anim.GetBool("running")){
				anim.SetBool("running", false);
				anim.SetInteger("condition", 0);
			}
			if (!anim.GetBool("running")){
				Attacking();
			}
        }
    }

    void Attacking()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        anim.SetBool("attacking", true);
        anim.SetInteger("condition", 2);
		//yield return new WaitForSeconds(0.5f);
		GetComponent<PlayerCharacter>().HandleAttack(forward);
        yield return new WaitForSeconds(1f);
        anim.SetInteger("condition", 0);
        anim.SetBool("attacking", false);
    }

	public void SetJumpSpeed(float y){
		moveDir.y = y;
	}
}