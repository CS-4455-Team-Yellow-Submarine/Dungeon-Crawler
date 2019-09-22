﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add different character types here
public enum CHARACTER_TYPE{Player, Monster, NPC};
// Teams
public enum CHARACTER_TEAM{Friendly, Neutral, Hostile};

public class Character : MonoBehaviour
{
	public int health = 100;
	int ticksInvulnerable = 0; // Used to check if character can take damage again or not
	protected bool tookDamage = false;
	protected bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        // Default initializations
    }

	// Update called every tick
    void FixedUpdate()
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
	public void TakeDamage(int amount, int cooldown=5){
		if(ticksInvulnerable > 0 || amount == 0) return;
		// Now we can take damage
		health -= amount;
		ticksInvulnerable = cooldown;
		tookDamage = true;
	}

	// Function to handle when damage is taken
	protected void HandleDamageTaken(){
		// Should we die?
		if(health < 0)
			isDead = true;
		// Any other things to handle
		//Debug.Log("I took damage! Health remaining: " + health);
	}

	// Function to handle when we die
	protected void HandleDeadState(){
		//Debug.Log("I am dead");
		Destroy(this.gameObject);
	}
}
