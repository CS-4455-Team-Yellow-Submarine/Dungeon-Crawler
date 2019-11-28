using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Add different character types here
public enum CHARACTER_TYPE{Player, Monster, NPC};
// Teams
public enum CHARACTER_TEAM{Friendly, Neutral, Hostile};

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public abstract class Character : MonoBehaviour
{
	public int health = 100;
	public int attackDamage = 10;
	int ticksInvulnerable = 0; // Used to check if character can take damage again or not
	protected bool tookDamage = false;
	protected bool isDead = false;
	protected Rigidbody rb; // Character body
	public GameObject projectile;
	protected int maxHealth;
	public AudioClip hitSound; // Sound clip to play on taking damage
	public string unitName = "Unit Name";

    // Start is called before the first frame update
    protected void Start()
    {
		// Get the rigidbody attached to this
		rb = GetComponent<Rigidbody>();
		if (rb == null)
			Debug.Log("Rigidbody could not be found!");
		// Update the max health
		maxHealth = this.health;

		GetComponent<AudioSource>().playOnAwake = false;
		GetComponent<AudioSource>().clip = hitSound;
    }

	// Update called every tick
    protected void FixedUpdate()
    {
		// Should we handle the dead state?
		if(isDead) HandleDeadState();
		if(ticksInvulnerable > 0) --ticksInvulnerable;
		// Check if we took damage recently
		if(tookDamage) HandleDamageTaken();
    }

	// Function to take damage
	// amount: Amount of damage to take
	// cooldown: ticks until the character can take damage again. Default 5 ticks (0.1 seconds)
	public virtual void TakeDamage(int amount){
		if(ticksInvulnerable > 0 || amount == 0) return;
		// Now we can take damage
		health -= amount;
		tookDamage = true;
		GetComponent<AudioSource>().Play();
		if(!name.Equals("Player")){
			GameObject.Find("EnemyHealthSlider").GetComponent<Slider>().value = (int)(health * 100f / (float)(maxHealth));
			GameObject.Find("Enemy_Health").GetComponent<Text>().text = unitName;
		}
	}

	// Function to handle when damage is taken
	protected virtual void HandleDamageTaken(){
		// Should we die?
		if(health <= 0)
			isDead = true;
		// Any other things to handle
		tookDamage = false;
	}

	// Function to handle when we die
	protected virtual void HandleDeadState(){
		// Check if a dead animation exists or not
		bool hasDeadAnim = false;
		foreach (AnimatorControllerParameter param in GetComponent<Animator>().parameters){
			if (param.name == "isDead")
				hasDeadAnim = true;
		}
		if(hasDeadAnim)
			GetComponent<Animator>().SetBool("isDead", true);
		else{
			OnCharacterDeath();
		}
	}

	// After death animation - add explosion or something
	public virtual void OnCharacterDeath(){
		Globals.MakeExplosion(this.transform.position);
		Destroy(transform.parent.gameObject);
	}

	// Function to handle a heal
	public void HealCharacter(int amount){
		if(amount < 0) return; // Can't heal a negative number
		// Make sure we don't heal to more than max
		health += amount;
		if(health > maxHealth)
			health = maxHealth;
	}

	// Functions to increase max health
	public void ChangeMaxHealthBy(int amount){
		// Maintain the same percentage of health
		float percentHealth = (float)(health) / (float)(maxHealth);
		maxHealth += amount;
		if(maxHealth < 1) maxHealth = 1;
		health = (int)(percentHealth * maxHealth);
		if(health < 1) health = 1;
	}

	public void ChangeMaxHealthTo(int amount){
		// Maintain the same percentage of health
		float percentHealth = (float)(health) / (float)(maxHealth);
		if(amount < 1) amount = 1;
		maxHealth = 1;
		health = (int)(percentHealth * maxHealth);
		if(health < 1) health = 1;
	}

	// Function to handle moving a certain amount
	public void HandleMove(Vector3 movement){
		transform.position = transform.position + movement;
	}

	public Vector3 GetPosition(){ return transform.position; }
	public Quaternion GetRotation(){ return transform.rotation; }
	public void SetRotation(Quaternion q){ transform.rotation = q; }
	public GameObject GetGameObject(){ return gameObject; }
}
