using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Definitions for a projectile
public enum ELEMENT{None = 0, Fire = 1, Water = 2};

public class Projectile : MonoBehaviour
{
	//protected Rigidbody rb;
	protected Rigidbody who; // Who attacked
	protected Vector3 source; // Attack from position
	protected GameObject target; // Attack target
	protected float moveSpeed; // How fast this thing moves
	protected int moveTime;
	protected Vector3 moveVec = new Vector3(0, 1, 0);
	public ELEMENT element = ELEMENT.None;
	public int damage = 10; // How much damage can we inflict
	protected float attackRange; // How far the projectile can fly, only relevant for melee attacks

    // Start is called before the first frame update
    void Start()
    {
		// Get the rigidbody attached to this
		//rb = GetComponent<Rigidbody>();
		//if (rb == null)
		//	Debug.Log("Rigidbody could not be found!");
    }

	// Instantiations for the projectile
	public void Define(Rigidbody rigidbody, Vector3 src, GameObject dst, float spd = 0f, float range = 1.2f){
		who = rigidbody;
		transform.position = src;
		source = src;
		target = dst;
		moveSpeed = spd;
		attackRange = range;
		// Melee or ranged projectile?
		if(moveSpeed < 0.01f){
			moveTime = 40; // Melee, just have it hit target after 0.8 seconds
		}
		else{ // Otherwise, calculate how long it'll take to reach target (for destruction)
			if(target != null)
				moveTime = (int)((target.transform.position - source).magnitude / moveSpeed * 50f);
			else
				moveTime = (int) (50f * attackRange / moveSpeed);
		}
		if(target != null)
			moveVec = (target.transform.position - source) / moveTime;
		if(moveSpeed < 0.01f){
			moveVec = moveVec.normalized * attackRange;
		}
		// Ignore collisions between this projectile and the source
		//Physics.IgnoreCollision(who.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		transform.position = transform.position + (moveVec / 40f);
		--moveTime;
		// Are we done moving?
		if(moveTime == 0)
			HandleDead();
    }

	/*/ Determine if anything can be damaged here
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
	}*/

	void OnTriggerEnter(Collider col){
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check to make sure it's not the source
		if(col.attachedRigidbody == who) return;
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
		if(c is BasicEnemyCharacter){
			(c as BasicEnemyCharacter).TakeDamage(damage);
		} 
		else if(c is PlayerCharacter){
			(c as PlayerCharacter).TakeDamage(damage);
		}
		else{
			c.TakeDamage(damage);
		}
		HandleDead();
	}

	// Override the projectile direction
	public void SetMoveDirection(Vector3 dir, float spd = 0f, float range = 1.2f){
		if(spd < 0.01f)
			moveVec = dir;
		else{
			moveVec = dir.normalized * 0.5f * range / spd;
		}
	}

	// Any optional things to do when this projectile should die
	protected void HandleDead(){
		Destroy(gameObject);
	}
}
