using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
	// 1 tick = 0.02s
	private int JUMP_COOLDOWN = 60;
	private int ticks_until_jump = 0;

	private bool isGrounded; // Boolean for checking if we are touching the ground
	private Rigidbody rb; // Character body

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		// Initializations
		isGrounded = true;
		rb = GetComponent<Rigidbody>();
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    new void FixedUpdate()
	{
		if(ticks_until_jump > 0) --ticks_until_jump;
		base.FixedUpdate();
		// Check for key inputs
		float h = Input.GetAxis("Horizontal") * 2;
		float v = Input.GetAxis("Vertical") * 2;
		Vector3 vel = rb.velocity;
		if (Input.GetKeyDown("space") && ticks_until_jump == 0)
		{
			vel.y = 4f;
			ticks_until_jump = JUMP_COOLDOWN;
		}
		vel.x = h;
		vel.z = v;
		rb.velocity = vel;
	}
}
