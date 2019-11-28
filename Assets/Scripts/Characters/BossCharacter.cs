using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BossCharacter : Character
{
	// Private components
	private Animator anim;
	private bool playerIsInRange = false;
	private bool playerInRoom = false;
	private float lastAttackTime;
	private GameObject thePlayer;
	private Vector3 forwardDirection;
	// Public components
	public int skillDamage = 25;
	public float attackCooldown = 3.5f;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
		anim = GetComponent<Animator>();
		if (anim == null)
			Debug.Log("Animator could not be found");
		thePlayer = GameObject.Find("Player");
    }


	new void FixedUpdate(){
		// Update accordingly
		base.FixedUpdate();
		anim.SetBool("isNearPlayer", playerIsInRange);
		if(Time.time - lastAttackTime > attackCooldown){
			anim.SetBool("isAttacking", true);
		}
		// Rotate towards the player
		forwardDirection = (thePlayer.transform.position - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(forwardDirection);

		if(health <= 0){
			GameObject.Find("EnemyHealthSlider").GetComponent<Slider>().value = 0;
			GameObject.Find("Enemy_Health").GetComponent<Text>().text = "";
		}
	}

	public void DoAttack(){
		lastAttackTime = Time.time;
		// Melee or ranged attack?
		if(playerIsInRange){
			// Launch projectile forward
			GameObject obj = Object.Instantiate(projectile) as GameObject;
			obj.transform.parent = transform;
			Projectile pr = obj.GetComponent<Projectile>();
			// Associate definitions with the projectile
			pr.Define(GetComponent<Rigidbody>(), transform.position + Vector3.up, null);
			// Override the direction of this projectile
			pr.SetMoveDirection(forwardDirection);
			// Set the damage of the projectile
			pr.damage = attackDamage;
		} else{
			Vector3 rightSide = -transform.right;
			//Vector3 rightSide = (Quaternion.Euler(0, 90, 0) * transform.rotation).eulerAngles.normalized;
			for(int i = 0; i < 7; i++){
				GameObject obj = Object.Instantiate(projectile, transform.position + rightSide * (-4.5f + 1.5f * i), Quaternion.identity) as GameObject;
				Projectile pr = obj.GetComponent<Projectile>();
				// Associate definitions with the projectile
				pr.Define(GetComponent<Rigidbody>(), transform.position + rightSide * (-4.5f + 1.5f * i), null, 3f, 15f);
				// Override the direction of this projectile
				pr.SetMoveDirection(transform.rotation * Vector3.forward, 3f, 15f);
				pr.damage = skillDamage;
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.name.Equals("Player")){
			if(!playerInRoom){
				anim.SetBool("playerInRoom", true);
				playerInRoom = true;
				GetComponent<CapsuleCollider>().enabled = false;
				lastAttackTime = Time.time;
				return;
			}
			playerIsInRange = true;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.gameObject.name.Equals("Player")){
			playerIsInRange = false;
		}
	}

	// Used for updating the health info
	new public void TakeDamage(int amount){
		base.TakeDamage(amount);
		GameObject.Find("EnemyHealthSlider").GetComponent<Slider>().value = (int)(health * 100f / (float)(maxHealth));
		GameObject.Find("Enemy_Health").GetComponent<Text>().text = unitName;
	}
}
