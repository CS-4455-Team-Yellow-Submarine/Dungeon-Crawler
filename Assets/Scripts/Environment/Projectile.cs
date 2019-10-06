using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definitions for a projectile
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
	protected Rigidbody rb;
	protected Rigidbody who; // Who attacked
	protected Vector3 source, target; // Attack from, to positions
	protected float moveSpeed; // How fast this thing moves
	protected int moveTime;
	protected Vector3 moveVec;
	protected int damage = 10; // How much damage can we inflict

    // Start is called before the first frame update
    void Start()
    {
		// Get the rigidbody attached to this
		rb = GetComponent<Rigidbody>();
		if (rb == null)
			Debug.Log("Rigidbody could not be found!");
    }

	// Instantiations for the projectile
	public void Define(Rigidbody rigidbody, Vector3 src, Vector3 dst, int dmg = 10, float spd = 0f){
		who = rigidbody;
		transform.position = src;
		source = src;
		target = dst;
		moveSpeed = spd;
		damage = dmg;
		// Melee or ranged projectile?
		if(moveSpeed < 0.001f)
			moveTime = 25; // Melee, just have it hit target after 0.5 seconds
		else // Otherwise, calculate how long it'll take to reach target (for destruction)
			moveTime = (int)((target - source).magnitude / moveSpeed * 50f);
		moveVec = (target - source) / moveTime;
		// Ignore collisions between this projectile and the source
		Physics.IgnoreCollision(who.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		transform.position = transform.position + moveVec;
		--moveTime;
		// Are we done moving?
		if(moveTime == 0)
			HandleDead();
    }

	// Determine if anything can be damaged here
	void OnCollisionEnter(Collision collision){
		Collider col = collision.collider;
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's something that can be damaged
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.tag.Equals("Character")) return;
		// Damage it if possible
		Component[] comps = obj.GetComponents(typeof(Component)) as Component[];
		Character c = null;
		// Need to cast types since the thing being damaged is a derived class
		foreach(Component comp in comps)
			if(comp is Character)
				c = comp as Character;
		if(c == null) return;
		c.TakeDamage(damage);
		HandleDead();
	}

	// Any optional things to do when this projectile should die
	protected void HandleDead(){
		Destroy(gameObject);
	}
}
